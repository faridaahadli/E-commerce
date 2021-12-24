using CRM_Halal.App_Code;
using CRMHalalBackEnd.App_Code;
using CRMHalalBackEnd.Helpers;
using CRMHalalBackEnd.Models;
using CRMHalalBackEnd.Models.MyUser;
using CRMHalalBackEnd.Models.ViewModels;
using CRMHalalBackEnd.Repository;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using WebApi.Jwt;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using CRMHalalBackEnd.Interfaces;

namespace CRMHalalBackEnd.Controllers.Account
{
    [AllowAnonymous]
    public class AccountController : ApiController
    {
        private readonly UtilsClass _controllerActions = new UtilsClass();

        [HttpPost]
        [Route("note/api/account/{domain}/Register")]
        public async Task<IHttpActionResult> Register(string domain, NewUser user)
        {
            var userId = 0;
            Response<UserResponseModel> response;
            NewUser newUser = null;
            try
            {

                var userRepo = new UserRepository();
                var smsRepo = new SmsVerificationRepository();
                var userByEmail = userRepo.GetUserByEmail(user.Email);
                user.Password = PasswordClass.HashPassword(user.Password);
                if (userByEmail != null)
                {
                    if (userByEmail.SocialToken != "")
                        throw new Exception("Siz artiq " + userByEmail.SocialProvider + " vasitesi ile qeydiyyatdan kecmisiniz.");

                    //bunlara ehtiyac varmi yoxla
                }

                if (!Regex.IsMatch(user.Contact.Text,@"^\+[1-9]{1}[0-9]{3,14}$"))
                {
                    throw new Exception("Nömrənin formatı düzgün deyil!");
                }

                newUser = userRepo.Upsert(domain.Replace('_', '.'), user, userId);
                UserResponseModel userResponse = new UserResponseModel()
                {

                    guid = newUser.UserGuid,
                    IsVerifyNeeded = newUser.IsVerifyNeeded
                };

                string html = HtmlFileSend.HtmlFileSender("~/HtmlFiles/User qeydiyyati/index.html");

                await EmailSend.SendEmailAsync(newUser.Email, "Note Qeydiyyat", html);

                response = new Response<UserResponseModel>
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Data = userResponse
                };
            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<UserResponseModel>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = await new StringReader(ex.Message).ReadLineAsync()
                };
                }
                else
                {
                    response = new Response<UserResponseModel>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception ex)
            {
                response = new Response<UserResponseModel>
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Data = null,
                    Message = ex.Message
                };
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("note/api/account/{domain}/Login")]
        public IHttpActionResult Login(string domain, LoginViewModel model)
        {
            Response<UserResponseModel> response;
            var repo = new UserRepository();
            try
            {
                UserResponseModel user = null;
                var userByEmail = repo.GetUserByEmail(model.Email);
                if (userByEmail == null)
                    throw new Exception("Bele bir istifadeci movcud deyldir");
                if (PasswordClass.ValidatePassword(model.Password, userByEmail.Password) //2933020292
                    && userByEmail.IsActive)
                {

                    user = JwtManager.NewGenerateToken(repo.GetUserById(userByEmail.UserId));
                    response = new Response<UserResponseModel>
                    {
                        Code = (int)HttpStatusCode.OK,
                        Success = true,
                        Data = user
                    };
                    userByEmail.LastLoginDate = DateTime.UtcNow.ToLocalTime();
                    userByEmail.LastLoginIp = model.LastLoginIp;
                    userByEmail.LastFailedRetries = 0;
                    //repo.LastFailedRetryUpdate(userByEmail.UserId,0);
                    repo.Upsert(domain.Replace('_', '.'), userByEmail, userByEmail.UserId);
                }
                else
                {
                    userByEmail.LastFailedRetries += 1;
                    repo.LastFailedRetryUpdate(userByEmail.UserId, userByEmail.LastFailedRetries);
                    response = new Response<UserResponseModel>
                    {
                        Code = (int)HttpStatusCode.NotFound,
                        Success = false,
                        Message = "İstifadəçi adı və ya şifrə yanlışdır."
                    };
                }

            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<UserResponseModel>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<UserResponseModel>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception ex)
            {
                response = new Response<UserResponseModel>
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Data = null,
                    Message = ex.Message
                };
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("note/api/account/{domain}/SocialLogin")]
        public async Task<IHttpActionResult> SocialLogin(string domain, NewUser user)
        {
            var userId = 0;
            NewUser newUser = null;
            Response<UserResponseModel> response;
            var repo = new UserRepository();
            try
            {
                UserResponseModel responseUser = null;
                var userByEmail = repo.GetUserByEmail(user.Email);

                IProvider provider = FactoryProvider.Build(user.SocialProvider);

                if (provider is null)
                    throw new Exception("Provider duzgun deyldir.");

                if (!await provider.TokenValidate(user.SocialToken))
                    throw new Exception("Invalid token");



                if (userByEmail == null)
                {
                    user.LastLoginDate = DateTime.Now.ToLocalTime();
                    newUser = repo.Upsert(domain.Replace('_', '.'), user, userId);
                    userId = repo.GetUserByGuid(newUser.UserGuid).UserId;
                    repo.ChangeUserActivation(userId, 1);
                    newUser.IsActive = true;
                    UserVerificationData data = new UserVerificationData()
                    {
                        UserId = userId,
                        IsActive = true
                    };
                    repo.UserVerificationUpdate(data);
                }
                else
                {

                    if (userByEmail.SocialProvider != user.SocialProvider
                         && userByEmail.SocialProvider != String.Empty)
                        throw new Exception("Uğursuz cəhd");
                    newUser = repo.GetUserById(userByEmail.UserId);

                }


                responseUser = JwtManager.NewGenerateToken(newUser);
                response = new Response<UserResponseModel>
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = responseUser,
                };

            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<UserResponseModel>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<UserResponseModel>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception ex)
            {
                response = new Response<UserResponseModel>
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Data = null,
                    Message = ex.Message
                };
            }
            return Ok(response);
        }



        [HttpPost]
        public async Task<IHttpActionResult> SendPasswordToEmail(string email)
        {
            Response<UserResponseModel> response;
            try
            {
                UserRepository userRepo = new UserRepository();

                var userByEmail = userRepo.GetUserByEmail(email);
                if (userByEmail == null)
                    throw new Exception("Bu adda istifadeci movcud deyldir.");

                string newPass = RandGen.Generate(6, 6);

                //string pathToFile = System.Web.Hosting.HostingEnvironment.MapPath("~/EmailTemplate.html");
                //var builder = new BodyBuilder();

                //using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
                //{

                //    builder.HtmlBody = SourceReader.ReadToEnd();

                //}

                //var last =
                //    "</td></ tr ></ table ></ td ></ tr ></ table ></ td ></ tr ></ table ></ td ></ tr ></ table ></ body ></ html > ";

                string html = HtmlFileSend.HtmlFileSender("~/HtmlFiles/EmailTemplate.html");

                await EmailSend.SendEmailAsync(email, "Daxil olmaq üçün yeni kodunuz", html.Replace("{0}", newPass));

                string hashPass = PasswordClass.HashPassword(newPass);
                userRepo.UpdateUserPassword(email, hashPass);

                response = new Response<UserResponseModel>
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = null,
                    Message = "Yeni şifrəniz e-poçt ünvanına göndərildi!"
                };

            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<UserResponseModel>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<UserResponseModel>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception ex)
            {
                response = new Response<UserResponseModel>
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Data = null,
                    Message = ex.Message
                };
            }
            return Ok(response);
        }


        [HttpPost]
        public async Task<IHttpActionResult> TestSendHtml(string email)
        {


                string html = HtmlFileSend.HtmlFileSender("~/HtmlFiles/Domain/index.html");

                await EmailSend.SendEmailAsync(email, "Daxil olmaq üçün yeni kodunuz", html);

        
            return Ok();
        }
    }
}
