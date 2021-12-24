using CRMHalalBackEnd.App_Code;
using CRMHalalBackEnd.Filters;
using CRMHalalBackEnd.Models;
using CRMHalalBackEnd.Models.UserDesign;
using CRMHalalBackEnd.Repository;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Security.Claims;
using System.Web.Http;

namespace CRMHalalBackEnd.Controllers.UserInterface
{
    public class UserInterfaceController : ApiController
    {
        private readonly UtilsClass _controllerActions = new UtilsClass();

        //Actor="Company", Permission="17"
        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "17")]
        public IHttpActionResult CreateUserDesign(DesignDto design)
        {
            
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = Int32.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));

            Response<List<DesignResponse>> response;
            try
            {
                var repository = new UserDesignRepository();
                var userDesign = repository.Insert(design, tenantId,userId);

                response = new Response<List<DesignResponse>>
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Data = userDesign

                };
            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<List<DesignResponse>>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<List<DesignResponse>>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception e)
            {
                response = new Response<List<DesignResponse>>
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = e.Message,
                    Data = null
                };
            }

            return Ok(response);
        }

        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult GetUserDesign(string domain)
        {
            //var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            Response<List<UserDesignGet>> response;

            try
            {
                var repository = new UserDesignRepository();
                response = new Response<List<UserDesignGet>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = repository.GetDesign(domain)
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<UserDesignGet>>()
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "12")]
        public IHttpActionResult GetUserDesignAdmin()
        {
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            Response<List<UserDesignGet>> response;

            try
            {
                var repository = new UserDesignRepository();
                response = new Response<List<UserDesignGet>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = repository.GetDesignAdmin(tenantId)
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<UserDesignGet>>()
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "68")]
        public IHttpActionResult DeleteDesign(int designId)
        {
            Response<int> response;
            int userId = Int32.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            DesignDto design = null;
            try
            {
                var designRepo = new UserDesignRepository();
                int returndesignId = designRepo.DeleteDesign(designId, tenantId,userId);
                response = new Response<int>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Dizayn ugurla slindi.",
                    Success = true,
                    Data = returndesignId
                };
            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<int>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<int>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception ex)
            {
                response = new Response<int>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = ex.Message,
                    Success = false
                };
            }
            return Ok(response);
        }
    }
}
