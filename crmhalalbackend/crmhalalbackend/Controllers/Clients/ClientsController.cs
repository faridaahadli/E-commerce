using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using CRMHalalBackEnd.App_Code;
using CRMHalalBackEnd.Filters;
using CRMHalalBackEnd.Helpers;
using CRMHalalBackEnd.Models;
using CRMHalalBackEnd.Models.Clients;
using CRMHalalBackEnd.Models.Notification;
using CRMHalalBackEnd.Repository;

namespace CRMHalalBackEnd.Controllers.Clients
{
    public class ClientsController : ApiController
    {
        private readonly UtilsClass _controllerActions = new UtilsClass();
        private readonly ClientsRepository _repository = new ClientsRepository();

        [HttpGet]
        [JwtRoleAuthentication(Actor = "Company", Permission = "100")]
        public IHttpActionResult GetAllClient()
        {
            Response<List<ClientResponse>> response;
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            var userId = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            try
            {
                List<ClientResponse> specialOffer = _repository.GetAllClient(tenantId,int.Parse(userId));
                response = new Response<List<ClientResponse>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = specialOffer
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<ClientResponse>>()
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "100")]
        public IHttpActionResult GetAllClientByData(string data)
        {
            Response<List<ClientResponse>> response;
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            var userId = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            try
            {
                List<ClientResponse> specialOffer = _repository.GetAllClientByData(tenantId, int.Parse(userId),data);
                response = new Response<List<ClientResponse>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = specialOffer
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<ClientResponse>>()
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "108")]
        public async Task<IHttpActionResult> SendClientNotification(NotificationDto notification)
        {

            notification.NotificationStatus = new NotificationStatus(){NotificationStatusId = 1};
            notification.NotificationData.NotificationType =new NotificationType(){NotificationTypeId = 2};
            Response<string> response;
            try
            {
                await NotificationProcess.SendNotification(notification, Request);
                response = new Response<string>()
                {
                    Code = (int) HttpStatusCode.OK,
                    Message = "Bildiriş Göndərildi!",
                    Success = true
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

    }
}
