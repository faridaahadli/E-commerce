using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using CRMHalalBackEnd.App_Code;
using CRMHalalBackEnd.Filters;
using CRMHalalBackEnd.Helpers;
using CRMHalalBackEnd.Models;
using CRMHalalBackEnd.Models.NewCompany;
using CRMHalalBackEnd.Models.Notification;
using CRMHalalBackEnd.Repository;

namespace CRMHalalBackEnd.Controllers.Company
{

    public class CompanyOperationController : ApiController
    {
        private readonly UtilsClass _controllerActions = new UtilsClass();
        private readonly CompanyRepository _repository = new CompanyRepository();

        [JwtRoleAuthentication(Actor = "Company", Permission = "1")]
        public IHttpActionResult UpdateCompany(CompanyRegDto companyRegDto)
        {
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            var userId = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            Response<CompanyDto> response;
            try
            {
                if (!Regex.IsMatch(companyRegDto.Contacts.Where(k => k.ContactTypeId == 2).Select(k => k.Text).First(),@"^\+[1-9]{1}[0-9]{3,14}$"))
                {
                    throw new Exception("Nömrənin formatı düzgün deyil!");
                }
                response = new Response<CompanyDto>
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _repository.Update(companyRegDto, tenantId, userId)
                };
            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<CompanyDto>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<CompanyDto>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception ex)
            {
                response = new Response<CompanyDto>
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message
                };
            }
            return Ok(response);
        }
        [JwtRoleAuthentication(Actor = "Company", Permission = "61")]
        public IHttpActionResult GetCompanyData()
        {
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            var userId = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            Response<CompanyDto> response;
            try
            {
                response = new Response<CompanyDto>
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _repository.GetCompanyByTenant(tenantId, userId)
                };
            }
            catch (Exception ex)
            {
                response = new Response<CompanyDto>
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "70")]
        public async Task<IHttpActionResult> EmployeeInsert(CompanyEmployeeInsDto companyEmployee)
        {
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = int.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            string result;
            Response<string> response;
            try
            {
                result = _repository.CompanyEmployeeInsert(companyEmployee, userId, tenantId);
                response = new Response<string>
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Data = result
                };

                NotificationDto notification = new NotificationDto()
                {
                    ToUserGuids = new[] { companyEmployee.UserGuid }.ToList(),
                    NotificationStatus = new NotificationStatus() { NotificationStatusId = 1 },
                    NotificationData = new NotificationData()
                    {
                        Title = "İş üçün dəvətnamə",
                        Text = "Sizi vezifeye teyin etdik",
                        NotificationType = new NotificationType() { NotificationTypeId = 1 }
                    }
                };
                await NotificationProcess.SendNotification(notification, Request);

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
        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "71")]
        public async Task<IHttpActionResult> EmployeeUpdate(CompanyEmployeeUpdateDto companyEmployee)
        {
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = int.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            string result;
            Response<string> response;
            try
            {
                result = _repository.CompanyEmployeeUpdate(companyEmployee, tenantId, userId);
                response = new Response<string>
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Data = result
                };

                NotificationDto notification = new NotificationDto()
                {
                    ToUserGuids = new[] { companyEmployee.UserGuid }.ToList(),
                    NotificationStatus = new NotificationStatus() { NotificationStatusId = 1 },
                    NotificationData = new NotificationData()
                    {
                        Title = "Vəzifə dəyişikliyi",
                        Text = "Sizin vəzifələrinizdə dəyişiklik edildi.",
                        NotificationType = new NotificationType() { NotificationTypeId = 1 }
                    }
                };
                await NotificationProcess.SendNotification(notification, Request);
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
        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "72")]
        public IHttpActionResult EmployeeDelete(int employeeId)
        {
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = int.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            string result;
            Response<string> response;
            try
            {
                result = _repository.CompanyEmployeeDelete(employeeId, tenantId, userId);
                response = new Response<string>
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "73")]
        public IHttpActionResult GetAllEmployee(int currentPage, int perPage, int? roleId)
        {
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = int.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            ICollection<CompanyEmployeeResponse> result;
            Response<ICollection<CompanyEmployeeResponse>> response;
            try
            {
                result = _repository.GetAllEmployee(tenantId, currentPage, perPage, userId, roleId);
                response = new Response<ICollection<CompanyEmployeeResponse>>
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = result
                };


            }
            catch (Exception ex)
            {
                response = new Response<ICollection<CompanyEmployeeResponse>>
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "73")]
        public IHttpActionResult GetTotalPage(int perPage, int? roleId)
        {
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int result;
            Response<int> response;
            try
            {
                result = _repository.GetTotalPage(tenantId, perPage, roleId);
                response = new Response<int>
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = result
                };

            }
            catch (Exception ex)
            {
                response = new Response<int>
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = -1
                };
            }
            return Ok(response);
        }
    }
}
