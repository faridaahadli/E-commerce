using CRMHalalBackEnd.App_Code;
using CRMHalalBackEnd.Models;
using CRMHalalBackEnd.Models.NewCompany;
using CRMHalalBackEnd.Models.Role;
using CRMHalalBackEnd.Repository;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Security.Claims;
using System.Web.Http;
using CRMHalalBackEnd.Filters;
using CRMHalalBackEnd.Helpers;
using CRMHalalBackEnd.Models.Order;
using WebApi.Jwt.Filters;

namespace CRMHalalBackEnd.Controllers.Role
{
    public class RoleController : ApiController
    {
        private readonly UtilsClass _controllerActions = new UtilsClass();
        private readonly RoleRepository _repository = new RoleRepository();
        private readonly LanguagesRepository _langRepository = new LanguagesRepository();
        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "57")]
        public IHttpActionResult InsertRole(RoleInsDto role)
        {
            var userId = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            Response<RoleResponse> response;
            try
            {
                var roleId = _repository.InsertRole(role, tenantId, int.Parse(userId));
                var lang = Request.GetLangFromHeader();
                var langId = _langRepository.GetLangId(lang);
                response = new Response<RoleResponse>
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Data = _repository.GetRoleById(langId, roleId, tenantId)
                };

            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<RoleResponse>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<RoleResponse>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception e)
            {
                response = new Response<RoleResponse>
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = e.Message,
                    Data = null
                };
            }

            return Ok(response);
        }
        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "58")]
        public IHttpActionResult UpdateRole(RoleUpdDto role)
        {
            var userid = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            Response<string> response;
            try
            {

                response = new Response<string>
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Data = null,
                    Message = _repository.UpdateRole(role, tenantId, int.Parse(userid))
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
            catch (Exception e)
            {
                response = new Response<string>
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = e.Message,
                    Data = null
                };
            }

            return Ok(response);
        }
        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "59")]
        public IHttpActionResult DeleteRole(int roleId)
        {
            var userid = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            Response<string> response;
            try
            {

                response = new Response<string>
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = null,
                    Message = _repository.DeleteRole(roleId, tenantId, int.Parse(userid))
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
            catch (Exception e)
            {
                response = new Response<string>
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "56")]
        public IHttpActionResult GetAllRole()
        {
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            var userId = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            Response<List<RoleResponse>> response;
            try
            {
                var lang = Request.GetLangFromHeader();
                var langId = _langRepository.GetLangId(lang);
                response = new Response<List<RoleResponse>>
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Data = _repository.GetAllRole(langId, tenantId, int.Parse(userId))
                };

            }
            catch (Exception e)
            {
                response = new Response<List<RoleResponse>>
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "57")]
        public IHttpActionResult CategoryAndPermissionGet()
        {
            Response<RoleAddSendingData> response;

            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            var userId = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);

            RoleAddSendingData roleData;
            try
            {
                var roleRepo = new RoleRepository();
                var lang = Request.GetLangFromHeader();
                var langId = _langRepository.GetLangId(lang);

                roleData = roleRepo.GetCategoryAndPermission(langId, tenantId, int.Parse(userId));
                response = new Response<RoleAddSendingData>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Category and Permissions gonderildi",
                    Success = true,
                    Data = roleData
                };
            }
            catch (Exception ex)
            {
                response = new Response<RoleAddSendingData>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = ex.Message,
                    Success = false,
                    Data = null
                };
            }
            return Ok(response);
        }

        [HttpGet]
        [JwtRoleAuthentication(Actor = "Company", Permission = "70")]
        public IHttpActionResult SearchByEmailOrCode(string email = "", string code = "")
        {
            Response<UserResponseModel> response;
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            var userId = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            UserResponseModel user;
            try
            {
                var userRepo = new UserRepository();
                user = userRepo.GetUserByCodeOrEmail(int.Parse(userId), tenantId, email, code);
                response = new Response<UserResponseModel>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "İstifadəçi mövcuddur.",
                    Success = true,
                    Data = user
                };
            }
            catch (Exception ex)
            {
                response = new Response<UserResponseModel>()
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Message = ex.Message,
                    Success = false,
                    Data = null
                };
            }
            return Ok(response);
        }


        [HttpGet]
        [JwtRoleAuthentication(Actor = "Company", Permission = "73")]
        public IHttpActionResult GetCompanyEmployeeRoleAddData()
        {
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = int.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));

            Response<CompanyRoleAddSendingData> response;
            CompanyRoleAddSendingData data;
            try
            {
                var roleRepo = new RoleRepository();
                var  lang = Request.GetLangFromHeader();
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);

                data = roleRepo.GetRoleAndPermission(langNumber, tenantId, userId);

                response = new Response<CompanyRoleAddSendingData>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Data ugurla gonderildi.",
                    Success = true,
                    Data = data
                };
            }
            catch (Exception ex)
            {
                response = new Response<CompanyRoleAddSendingData>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = ex.Message,
                    Success = false,
                    Data = null
                };
            }
            return Ok(response);
        }

        [HttpGet]
        [JwtRoleAuthentication(Actor = "Company", Permission = "73")]
        public IHttpActionResult GetCompanyEmployeeUpdateData(int employeeId)
        {
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = int.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            Response<CompanyEmployeeUpdatePackage> response;

            CompanyEmployeeUpdatePackage data;
            try
            {
                var roleRepo = new RoleRepository();
                var lang = Request.GetLangFromHeader();
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);

                data = roleRepo.GetRoleAndPermissionForUpdate(langNumber, employeeId, tenantId, userId);

                response = new Response<CompanyEmployeeUpdatePackage>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Data ugurla gonderildi.",
                    Success = true,
                    Data = data
                };
            }
            catch (Exception ex)
            {
                response = new Response<CompanyEmployeeUpdatePackage>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = ex.Message,
                    Success = false,
                    Data = null
                };
            }
            return Ok(response);
        }

    }
}
