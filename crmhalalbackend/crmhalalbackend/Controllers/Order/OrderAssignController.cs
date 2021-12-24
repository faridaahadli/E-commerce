using CRMHalalBackEnd.App_Code;
using CRMHalalBackEnd.Filters;
using CRMHalalBackEnd.Models;
using CRMHalalBackEnd.Models.Courier.OrderStatus;
using CRMHalalBackEnd.Models.Order;
using CRMHalalBackEnd.Repository;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using CRMHalalBackEnd.Helpers;

namespace CRMHalalBackEnd.Controllers.Order
{
    public class OrderAssignController : ApiController
    {
        private readonly UtilsClass _controllerActions = new UtilsClass();
        private readonly LanguagesRepository _langRepository = new LanguagesRepository();

        #region OrderPage

        [HttpGet]
        [JwtRoleAuthentication(Actor = "Company", Permission = "106")]
        public IHttpActionResult GetAllOrderForAdmin()
        {
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = int.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            Response<List<AllOrderForAdmin>> response;
            try
            {
                var repository = new OrderAssignRepository();
                var lang = Request.GetLangFromHeader();
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);
                response = new Response<List<AllOrderForAdmin>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = repository.GetAllOrderForAdmin(langNumber, tenantId, userId)
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<AllOrderForAdmin>>()
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "106")]
        public IHttpActionResult GetAllOrderLineForAdmin(int orderId)
        {
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = int.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            Response<List<OrderLineInfo>> response;
            try
            {
                var repository = new OrderAssignRepository();
                var lang = Request.GetLangFromHeader();
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);
                response = new Response<List<OrderLineInfo>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = repository.GetAllOrderLineForAdmin(langNumber, tenantId, userId, orderId)
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<OrderLineInfo>>()
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "106")]
        public IHttpActionResult GetStatusForOrder()
        {
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = int.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            Response<List<OrderStatusInsert>> response;
            try
            {
                var repository = new OrderAssignRepository();
                var lang = Request.GetLangFromHeader();
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);
                response = new Response<List<OrderStatusInsert>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = repository.GetAllStatusForOrder(langNumber,tenantId, userId)
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<OrderStatusInsert>>()
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "106")]
        public IHttpActionResult UpdateOrderStatus(OrderStatusUpdate status)
        {

            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            var userId = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            Response<int> response;
            tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            try
            {
                var orderAssignRepo = new OrderAssignRepository();
                var model = orderAssignRepo.OrderStatusUpdate(status, tenantId, int.Parse(userId));
                response = new Response<int>
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Data = model

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
                response = new Response<int>
                {
                    Code = (int)HttpStatusCode.Conflict,
                    Success = false,
                    Message = ex.Message,
                    Data = 0
                };
            }

            return Ok(response);
        }


        [HttpGet]
        [JwtRoleAuthentication(Actor = "Company", Permission = "106")]
        public IHttpActionResult OrderFilter(int page, int persize = 10, int? statusId = null, int? customerId = null,
            string beginDate = null, string deliveryTime = null, int? filterId = null)
        {
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = int.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));

            Response<List<AllOrderForAdmin>> response;
            try
            {
                var repository = new OrderAssignRepository();
                var lang = Request.GetLangFromHeader();
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);
                response = new Response<List<AllOrderForAdmin>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = repository.OrderFilter(langNumber, statusId, customerId, tenantId, userId, beginDate, deliveryTime, filterId, page, persize)
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<AllOrderForAdmin>>()
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "106")]
        public IHttpActionResult GetOrdersPageSize(int? statusId = null, int? customerId = null, string beginDate = null, string deliveryTime = null, int perSize = 10)
        {
            Response<int> response;
            int userId = int.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            try
            {
                var repository = new OrderAssignRepository();
                response = new Response<int>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = repository.GetOrderSize(statusId, customerId, tenantId, userId, beginDate, deliveryTime, perSize)
                };
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "106")]
        public IHttpActionResult GetCustomers()
        {
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = int.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            Response<List<Customers>> response;
            try
            {
                var repository = new OrderAssignRepository();
                response = new Response<List<Customers>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = repository.GetAllCustomer(tenantId, userId)
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<Customers>>()
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

        #region StatusPage

        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "104")]
        public IHttpActionResult OrderStatusInsert(OrderStatusInsert status)
        {
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = int.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            var orderAssignRepo = new OrderAssignRepository();
            string data;
            Response<string> response;
            try
            {
                data = orderAssignRepo.OrderStatusInsert(status, tenantId, userId);
                response = new Response<string>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Data ugurla gonderildi.",
                    Success = true,
                    Data = data
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "104")]
        public IHttpActionResult GetOrderStatus()
        {
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = int.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            Response<List<OrderStatusInsert>> response;
            try
            {
                var repository = new OrderAssignRepository();
                response = new Response<List<OrderStatusInsert>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = repository.GetAllStatus(tenantId, userId)
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<OrderStatusInsert>>()
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "104")]
        public IHttpActionResult DeleteOrderStatus(int id)
        {
            Response<int> response;
            int userId = Int32.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            try
            {
                var orderAssignRepo = new OrderAssignRepository();
                int returnId = orderAssignRepo.DeleteOrderStatus(id, tenantId, userId);
                response = new Response<int>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Status ugurla silindi.",
                    Success = true,
                    Data = returnId
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

        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "104")]
        public IHttpActionResult UpdateStatusPayment(int statusId)
        {
            Response<string> response;
            int userId = Int32.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            try
            {
                var orderAssignRepo = new OrderAssignRepository();
                orderAssignRepo.UpdateStatusPayment(statusId, tenantId, userId);
                response = new Response<string>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Status uğurla yeniləndi.",
                    Success = true
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
                response = new Response<string>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = ex.Message,
                    Success = false
                };
            }
            return Ok(response);
        }

        [HttpGet]
        [JwtRoleAuthentication(Actor = "Company", Permission = "106")]
        public IHttpActionResult GetOrderPagination(int page, int perSize = 10)
        {
            Response<List<AllOrderForAdmin>> response;
            int userId = Int32.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);

            try
            {
                var repository = new OrderAssignRepository();
                response = new Response<List<AllOrderForAdmin>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = repository.OrderPagination(userId, tenantId, page, perSize)
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<AllOrderForAdmin>>()
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
