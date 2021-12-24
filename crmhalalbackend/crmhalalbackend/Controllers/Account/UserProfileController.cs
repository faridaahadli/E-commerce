using CRM_Halal.App_Code;
using CRMHalalBackEnd.App_Code;
using CRMHalalBackEnd.Models;
using CRMHalalBackEnd.Models.MyUser;
using CRMHalalBackEnd.Repository;
using System;
using System.Net;
using System.Security.Claims;
using System.Web.Http;
using Castle.Core.Internal;
using CRMHalalBackEnd.Filters;
using CRMHalalBackEnd.SmsService;
using CRMHalalBackEnd.Models.SmsVerification;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace CRMHalalBackEnd.Controllers.Account
{
    [JwtRoleAuthentication(Actor = "User")]
    public class UserProfileController : ApiController
    {
        UtilsClass controllerActions = new UtilsClass();
        // GET: api/UserProfile
        [HttpGet]
        [JwtRoleAuthentication(Actor = "User")]
        public IHttpActionResult Get(string lang)
        {
            var userId = int.Parse(controllerActions.getUserId((ClaimsIdentity)User.Identity));
            Response<UserResponseModel> response;
            try
            {
                var userRepo = new UserRepository();
                var roleRepo = new RoleRepository();
                var newUser = userRepo.GetUserById(userId);
                UserResponseModel userResponse = new UserResponseModel();
                userResponse.name = newUser.FirstName;
                userResponse.surname = newUser.LastName;
                userResponse.birthdate = newUser.BirthDate;
                userResponse.email = newUser.Email;
                userResponse.IsVerifyNeeded = newUser.IsVerifyNeeded;
                userResponse.Code = newUser.Code;
                var role = roleRepo.GetRoleByUserId(userId);
                if (newUser.Contact != null)
                {
                    userResponse.contact = newUser.Contact.Text;

                }
                if (newUser.Address != null)
                {
                    userResponse.address = newUser.Address.Address;

                }
                //userResponse.social_token = newUser.SocialToken;
                userResponse.guid = newUser.UserGuid;
                userResponse.Code = newUser.Code;
                userResponse.role_name = role;
                userResponse.IsNotificate = newUser.IsNotificate;
                response = new Response<UserResponseModel>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Data = userResponse,
                    Success = true
                };

            }
         
            catch (Exception ex)
            {
                response = new Response<UserResponseModel>()
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Data = null,
                    Success = false,
                    Message = "Belə bir istifadeçi mövcut deyil!"
                };
            }

            return Ok(response);
        }

        [HttpPost]
        
        public IHttpActionResult Update(UserUpdDto user)
        {
            var userId = int.Parse(controllerActions.getUserId((ClaimsIdentity)User.Identity));
            Response<UserResponseModel> response;
            try
            {
                var userRepo = new UserRepository();
                var roleRepo = new RoleRepository();
                if (!user.Password.Equals(user.RepeatPassword))
                    throw new Exception("Şifrələr uyğunlaşmır");

                if (!user.Password.IsNullOrEmpty()  &&  !user.RepeatPassword.IsNullOrEmpty())
                    user.Password = PasswordClass.HashPassword(user.Password);

                var newUser = userRepo.Update(user,userId);

                UserResponseModel userResponse = new UserResponseModel();
                userResponse.name = newUser.FirstName;
                userResponse.surname = newUser.LastName;
                userResponse.birthdate = newUser.BirthDate;
                userResponse.email = newUser.Email;
                var role = roleRepo.GetRoleByUserId(userId);
                if (newUser.Contact != null)
                {
                    userResponse.contact = newUser.Contact.Text;

                }
                if (newUser.Address != null)
                {
                    userResponse.address = newUser.Address.Address;

                }
                //userResponse.social_token = newUser.SocialToken;
                userResponse.guid = newUser.UserGuid;
                userResponse.role_name = role;
                response = new Response<UserResponseModel>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Data = userResponse,
                    Success = true
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
                response = new Response<UserResponseModel>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Data = null,
                    Success = false,
                    Message = ex.Message
                };
            }
            return Ok(response);
        }

      


        [HttpPost]
        public IHttpActionResult SetContact(string number,string userGuid)
        {
            Response<string> response;
            var userRepo = new UserRepository();
            var smsRepo = new SmsVerificationRepository();
            if (!Regex.IsMatch(number, @"^\+[1-9]{1}[0-9]{3,14}$"))
            {
                throw new Exception("Nömrənin formatı düzgün deyil!");
            }
            try
            {
                var result = userRepo.SetContact(number,userGuid);

                response = new Response<string>
                {
                    Code = (int)HttpStatusCode.Created,
                    Message="Xeyirli olsun.",
                    Success = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                response = new Response<string>
                {
                    Code = (int)HttpStatusCode.NoContent,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            return Ok(response);
        }

        [HttpPost]
        public IHttpActionResult DeleteUser()
        {
            return Ok(new Response<bool>()
            {
                Code = 200,
                Data = true,
                Message = "User Deleted",
                Success = true
            });
        }
    }
}
