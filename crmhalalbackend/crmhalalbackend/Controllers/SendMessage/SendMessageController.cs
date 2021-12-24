using CRMHalalBackEnd.App_Code;
using CRMHalalBackEnd.Filters;
using CRMHalalBackEnd.Helpers;
using CRMHalalBackEnd.Interfaces;
using CRMHalalBackEnd.Models;
using CRMHalalBackEnd.Models.Message;
using CRMHalalBackEnd.Models.Message.Package;
using CRMHalalBackEnd.Repository;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace CRMHalalBackEnd.Controllers.SendMessage
{
    public class SendMessageController : ApiController
    {
        private readonly UtilsClass _controllerActions = new UtilsClass();
        SendMessageRepository repo = new SendMessageRepository();

        #region Common 
        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "108")]
        public IHttpActionResult CreateMessage(InsertMessage message)
        {
            var userid = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);

            Response<string> response;
            try
            {


                var result = repo.InsertMessage(message, tenantId, userid);

                response = new Response<string>
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Message = "Dəyişiklik uğurla tamamlandı",
                    Data = result
                };

            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<string>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<string>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception ex)
            {
                response = new Response<string>
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            return Ok(response);

        }

        [HttpGet]
        [JwtRoleAuthentication(Actor = "Company", Permission = "108")]
        public IHttpActionResult GetUsers()
        {
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            var userId = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            Response<List<GetUsers>> response = null;
            try
            {
                var result = repo.GetUsers(tenantId, int.Parse(userId));

                response = new Response<List<GetUsers>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = result,
                    Message = "userler getirildi!"
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<GetUsers>>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }

            return Ok(response);
        }
        #endregion Common


        #region Email 
        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "108")]
        public IHttpActionResult CreateMailInfo(UserMailInfo info)
        {
            var userid = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);

            Response<string> response;
            try
            {


                var result = repo.InsertMailInfo(info, tenantId, userid);
                response = new Response<string>
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Message = "Dəyişiklik uğurla tamamlandı",
                    Data = result
                };

            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<string>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<string>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception ex)
            {
                response = new Response<string>
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            return Ok(response);

        }

        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult GetEmailProviders()
        {
            Response<List<ProviderTypes>> response;

            try
            {
                var repository = new SendMessageRepository();
                response = new Response<List<ProviderTypes>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = repository.GetEmailProviderName()
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<ProviderTypes>>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            return Ok(response);
        }

        [HttpGet]
        [JwtRoleAuthentication(Actor = "Company", Permission = "108")]
        public IHttpActionResult GetStoreEmail()
        {
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            var userId = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            Response<string> response;

            try
            {
                var repository = new SendMessageRepository();
                response = new Response<string>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = repository.GetStoreEmail(int.Parse(userId), tenantId)
                };
            }
            catch (Exception ex)
            {
                response = new Response<string>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            return Ok(response);
        }

        [HttpGet]
        [JwtRoleAuthentication(Actor = "Company", Permission = "108")]
        public IHttpActionResult GetAllEmail()
        {
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            var userId = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            Response<List<AllEmailFront>> response;

            try
            {
                var repository = new SendMessageRepository();
                response = new Response<List<AllEmailFront>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = repository.GetAllEmail(int.Parse(userId), tenantId)
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<AllEmailFront>>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            return Ok(response);
        }
        #endregion Email


        #region SMS
        [HttpGet]
        [JwtRoleAuthentication(Actor = "Company", Permission = "108")]
        public async Task<IHttpActionResult> GetAllMessages()
        {
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            Response<List<AllMessageFront>> response = null;
            try
            {
                var result = repo.GetAllMessages(tenantId);

                response = new Response<List<AllMessageFront>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = await result,
                    Message = "Mesaj datalari getirildi"
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<AllMessageFront>>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }

            return Ok(response);
        }

        #endregion SMS


        #region Package
        [HttpGet]
        [JwtRoleAuthentication(Actor = "Company", Permission = "110")]
        public IHttpActionResult GetPackageById(int? packageId = null)
        {

            Response<List<AllPackages>> response;

            try
            {
                var repository = new SendMessageRepository();
                response = new Response<List<AllPackages>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = repository.GetPackageById(packageId)
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<AllPackages>>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            return Ok(response);
        }
        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "110")]
        public IHttpActionResult InsertPackageForCompany(InsertPackage package)
        {

            var userId = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
         
            Response<string> response;
            try
            {
                var paymentId = repo.InsertPackageForCompany(package, tenantId, userId);
                var lang = Request.GetLangFromHeader();
                IPaymentProvider provider = FactoryPaymentProvider.Build("smspacket");

                string returnUrl = string.Empty;

                if (paymentId != 0)
                {
                    returnUrl = provider.GetPaymentPageUrl(lang, paymentId, int.Parse(userId), tenantId);
                }

                response = new Response<string>
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Message = "Dəyişiklik uğurla tamamlandı",
                    Data = returnUrl
                };

            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<string>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<string>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception ex)
            {
                response = new Response<string>
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message
                };
            }
            return Ok(response);

        }
        [HttpGet]
        [JwtRoleAuthentication(Actor = "Company", Permission = "110")]  //permissionlara bax
        public IHttpActionResult GetPackageHistory(bool? isNote=null, int? filterId=null)
        {
            // filterId tarixe gore siralanma ucun teleb olunur. Eger 1dirse  create_date asc, eks halda desc olur 
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            var userId = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            Response<PackageHistory> response;

            try
            {
                var repository = new SendMessageRepository();
                response = new Response<PackageHistory>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = repository.GetPackageHistory(int.Parse(userId), tenantId, isNote,filterId)
                };
            }
            catch (Exception ex)
            {
                response = new Response<PackageHistory>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            return Ok(response);
        }

        #endregion



    }
}
