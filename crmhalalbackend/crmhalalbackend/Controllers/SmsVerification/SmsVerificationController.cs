using CRMHalalBackEnd.App_Code;
using CRMHalalBackEnd.Models;
using CRMHalalBackEnd.Models.MyUser;
using CRMHalalBackEnd.Models.SmsVerification;
using CRMHalalBackEnd.Repository;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;
using WebApi.Jwt;

namespace CRMHalalBackEnd.Controllers
{
    [AllowAnonymous]
    public class SmsVerificationController : ApiController
    {
        private readonly UtilsClass _controllerActions = new UtilsClass();

        public IHttpActionResult Post(SmsData smsResponse)
        {
            Response<string> response;
            var userId = 0;
            var smsRepo = new SmsVerificationRepository();
            SmsVerificationInfo smsInfo = null;
            var userRepo = new UserRepository();
            try
            {
                smsInfo = smsRepo.GetSmsInfoByUserGuid(smsResponse.SendingUserGuid);
                //Check code is expired or not
                if (smsResponse.RequestDate > smsInfo.ExpiredDate)
                {
                    response = new Response<string>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = "Kodun vaxtı keçmişdir."
                    };
                    return Ok(response);
                }
                   
                if (smsResponse.VerificationCode != smsInfo.VerificationCode)
                {
                    response = new Response<string>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = "Kod yanlış daxil edilmişdir."
                    };
                    return Ok(response);
                }    
                    
              
                userId = userRepo.GetUserByGuid(smsResponse.SendingUserGuid).UserId;

            }
            catch(Exception ex)
            {
                response = new Response<string>
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message
                };
            }
            try
            {
                //Update user and SmsVerification datas
                UserVerificationData data = new UserVerificationData()
                {
                    UserId=userId,
                    IsActive = true,
                    IsVerified = true,
                    VerifiedDate = DateTime.UtcNow
                };
                userRepo.UserVerificationUpdate(data);
                smsInfo.IsVerified = true;
                smsInfo.VerifiedDate = data.VerifiedDate;
                smsRepo.Update(smsInfo);
                response = new Response<string>
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Message= "Uğurla tamamlandi"
                };

            }
            catch (Exception ex){
                response = new Response<string>
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success =false,
                    Message = ex.Message
                };
            }
        
            return Ok(response);
        }

        [HttpPost]
        public IHttpActionResult PostRegister(SmsData smsResponse)
        {
            Response<UserResponseModel> response;
            var userId = 0;
            var smsRepo = new SmsVerificationRepository();
            SmsVerificationInfo smsInfo = null;
            var userRepo = new UserRepository();
            try
            {
                smsInfo = smsRepo.GetSmsInfoByUserGuid(smsResponse.SendingUserGuid);
                //Check code is expired or not
                if (smsResponse.RequestDate > smsInfo.ExpiredDate)
                {
                    response = new Response<UserResponseModel>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = "Kodun vaxtı keçmişdir."
                    };
                    return Ok(response);
                }

                if (smsResponse.VerificationCode != smsInfo.VerificationCode)
                {
                    response = new Response<UserResponseModel>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = "Kod yalnış daxil edilmişdir."
                    };
                    return Ok(response);
                }

                userId = userRepo.GetUserByGuid(smsResponse.SendingUserGuid).UserId;

            }
            catch (Exception ex)
            {
                response = new Response<UserResponseModel>
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message
                };
            }
            try
            {
                //Update user and SmsVerification datas
                UserVerificationData data = new UserVerificationData()
                {
                    UserId = userId,
                    IsActive = true,
                    IsVerified = true,
                    VerifiedDate = DateTime.UtcNow
                };
                userRepo.UserVerificationUpdate(data);
                smsInfo.IsVerified = true;
                smsInfo.VerifiedDate = data.VerifiedDate;
                smsRepo.Update(smsInfo);

            
                UserResponseModel user = null;
                user = JwtManager.NewGenerateToken(userRepo.GetUserById(userId));


                var userById = userRepo.GetUserByGuid(smsResponse.SendingUserGuid);
                userById.LastLoginDate = DateTime.UtcNow.ToLocalTime();
                userById.LastLoginIp = smsResponse.LastLoginIp;
                userById.LastFailedRetries = 0;
                //repo.LastFailedRetryUpdate(userByEmail.UserId,0);
                userRepo.Upsert(String.Empty, userById, userId);

                response = new Response<UserResponseModel>
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = user
                };

            }
            catch (Exception ex)
            {
                response = new Response<UserResponseModel>
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message
                };
            }

            return Ok(response);
        }



        [HttpPost]
        public IHttpActionResult SendSms(string number,string userGuid)
        {
            Response<bool> response;
            Random rnd = new Random();
            string code = _controllerActions.addNulls(rnd.Next(1, 999999));
            SmsVerificationRepository smsRepo = new SmsVerificationRepository();
            try
            {
                if (!Regex.IsMatch(number,@"^\+[1-9]{1}[0-9]{3,14}$"))
                {
                    throw new Exception("Nömrənin formatı düzgün deyil!");
                }
                smsRepo.SmsSend(code, number,userGuid);
                SmsVerificationInfo smsInfo = new SmsVerificationInfo();
                smsInfo.UserGuid =userGuid;
                smsInfo.VerificationCode = code.ToString();
                smsInfo.SendDate = DateTime.UtcNow.ToLocalTime();
                smsInfo.ExpiredDate = smsInfo.SendDate.AddMinutes(3);
                smsRepo.Insert(smsInfo);
                response = new Response<bool>
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Message = "Nomre gul kimidi"
                };
            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<bool>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<bool>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception ex)
            {
                response = new Response<bool>
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message
                };
            }
            return Ok(response);
        }
    }
}
