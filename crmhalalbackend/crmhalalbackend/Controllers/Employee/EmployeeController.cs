using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using CRMHalalBackEnd.App_Code;
using CRMHalalBackEnd.Filters;
using CRMHalalBackEnd.Helpers;
using CRMHalalBackEnd.Models;
using CRMHalalBackEnd.Repository;

namespace CRMHalalBackEnd.Controllers.Employee
{
    public class EmployeeController : ApiController
    {
        private readonly UtilsClass _controllerActions = new UtilsClass();
        private readonly EmployeeRepository _repositoryEmployee = new EmployeeRepository();
        private readonly StoreRepository _repositoryStore = new StoreRepository();

        [HttpPost]
        [JwtRoleAuthentication(Actor = "User")]
        public async Task<IHttpActionResult> ConfirmEmployee(int notificationId,int companyId, bool confirm)
        {
            var userId = _controllerActions.getUserId((ClaimsIdentity) User.Identity);
            Response<int> response = null;
            try
            {
                int result = _repositoryEmployee.ConfirmEmployee(int.Parse(userId), companyId, confirm);
                if (result == 1)
                {
                    response = new Response<int>()
                    {
                        Code = (int)HttpStatusCode.OK,
                        Success = true,
                        Data = result,
                        Message = "İstək təsdiqləndi!"
                    };
                    await NotificationProcess.ConfirmNotification(notificationId, true, Request);
                }else if (result == 2)
                {
                    response = new Response<int>()
                    {
                        Code = (int)HttpStatusCode.OK,
                        Success = true,
                        Data = result,
                        Message = "İstək ləğv edildi!"
                    };
                     await  NotificationProcess.ConfirmNotification(notificationId, false, Request);
                }

               
            }
            catch (Exception ex)
            {
                response = new Response<int>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = 0
                };
            }

            return Ok(response);
        }

        [HttpGet]
        [JwtRoleAuthentication(Actor = "Company")]
        [Route("note/api/Store/CheckEmployee/{domain}")]
        public IHttpActionResult CheckEmployee(string domain)
        {
            Response<bool> response;
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity) User.Identity);
            try
            {
                string sendTenantId = _repositoryStore.GetTenantIdByStoreName(domain.Replace('_','.'));
                response = new Response<bool>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = tenantId == sendTenantId
                };
            }
            catch (Exception ex)
            {
                response = new Response<bool>()
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
