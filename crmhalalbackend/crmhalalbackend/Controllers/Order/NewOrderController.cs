using CRMHalalBackEnd.App_Code;
using CRMHalalBackEnd.Filters;
using CRMHalalBackEnd.Models;
using CRMHalalBackEnd.Models.Order;
using CRMHalalBackEnd.Repository;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Web.Http;
using System.Data.SqlClient;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using Castle.Core.Internal;
using CRMHalalBackEnd.Helpers;
using CRMHalalBackEnd.Models.Order.Buyer_Order;
using CRMHalalBackEnd.Models.Payment;
using CRMHalalBackEnd.Models.Store;

namespace CRMHalalBackEnd.Controllers.Order
{

    public class NewOrderController : ApiController
    {
        private readonly UtilsClass _controllerActions = new UtilsClass();
        private readonly OrderRepository _orderRepository = new OrderRepository();
        private readonly LanguagesRepository _langRepository= new LanguagesRepository();
        private readonly StoreRepository _storeRepository = new StoreRepository();

        //[HttpGet]
        //[JwtRoleAuthentication(Actor = "User")]
        //public IHttpActionResult GetAllOrderByUserId()
        //{
        //    int userId = int.Parse(_controllerActions.getDefaultUserId((ClaimsIdentity)User.Identity));
        //    Response<IEnumerable<BuyerOrder>> response;

        //    try
        //    {
        //        response = new Response<IEnumerable<BuyerOrder>>()
        //        {
        //            Code = (int)HttpStatusCode.Created,
        //            //Data = _repository.GetAllOrderForUser(userId),
        //            Success = true
        //        };

        //    }
        //    catch(SqlException ex)
        //    {
        //        if (ex.Number == 51000)
        //        {

        //            response = new Response<IEnumerable<BuyerOrder>>()
        //            {
        //                Code = (int)HttpStatusCode.BadRequest,
        //                Success = false,
        //                Message = ex.Message

        //            };

        //        }
        //        else
        //        {
        //            response = new Response<IEnumerable<BuyerOrder>>()
        //            {
        //                Code = (int)HttpStatusCode.InternalServerError,
        //                Success = false,
        //                Message = ex.Message

        //            };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response = new Response<IEnumerable<BuyerOrder>>()
        //        {
        //            Code = (int)HttpStatusCode.InternalServerError,
        //            Success = false,
        //            Message = ex.Message

        //        };
        //    }

        //    return Ok(response);
        //}

        [HttpPost]
        [JwtRoleAuthentication(Actor = "User")]
        public IHttpActionResult Post(OrderInsDto order)
        {
            Response<int> response;
            var userId = int.Parse(_controllerActions.getUserId((ClaimsIdentity)User.Identity));
            try
            {
                if (!order.Phone.IsNullOrEmpty())
                {
                    if (!Regex.IsMatch(order.Phone, @"^\+[1-9]{1}[0-9]{3,14}$"))
                    {
                        throw new Exception("Nömrənin formatı düzgün deyil!");
                    }
                }

                var result = _orderRepository.Insert(order, userId);
                response = new Response<int>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Sifariş qəbul edildi.",
                    Success = true,
                    Data = result
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
        [AllowAnonymous]
        public IHttpActionResult GetPaymentMethod(List<string> tenantId)
        {
            Response<IEnumerable<StorePaymentDto>> response;
            try
            {
                var lang = Request.GetLangFromHeader();
                var langId = _langRepository.GetLangId(lang);

                var methods = _orderRepository.GetPaymentMethod(langId, tenantId);
                response = new Response<IEnumerable<StorePaymentDto>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Ödəniş üsulları uğurla göndərildi.",
                    Success = true,
                    Data = methods
                };
            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<IEnumerable<StorePaymentDto>>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message,
                        Data = null
                    };
                }
                else
                {
                    response = new Response<IEnumerable<StorePaymentDto>>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message,
                        Data = null
                    };
                }

            }
            catch (Exception ex)
            {
                response = new Response<IEnumerable<StorePaymentDto>>()
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
        [JwtRoleAuthentication(Actor = "Company")]
        public IHttpActionResult GetAllPaymentMethod()
        {
            Response<IEnumerable<PaymentMethodDto>> response;
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity) User.Identity);
            try
            {
                var lang = Request.GetLangFromHeader();
                var langId = _langRepository.GetLangId(lang);

                response = new Response<IEnumerable<PaymentMethodDto>>()
                {
                    Code = (int)HttpStatusCode.Created,
                    Data = _orderRepository.GetAllPaymentMethod(langId, tenantId),
                    Success = true
                };

            }
            catch (Exception ex)
            {
                response = new Response<IEnumerable<PaymentMethodDto>>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message

                };
            }

            return Ok(response);
        }

        [HttpGet]
        [JwtRoleAuthentication(Actor = "User")]
        public IHttpActionResult GetAllOrderForUser()
        {
            int userId = int.Parse(_controllerActions.getDefaultUserId((ClaimsIdentity)User.Identity));

            Response<List<BuyerOrder>> response;

            try
            {
                var lang = Request.GetLangFromHeader();
                response = new Response<List<BuyerOrder>>()
                {
                    Code = (int)HttpStatusCode.Created,
                    Data = _orderRepository.AllUserOrder(userId,lang),
                    Success = true
                };

            }
            catch (SqlException ex)
            {
                if (ex.Number == 51000)
                {

                    response = new Response<List<BuyerOrder>>()
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message

                    };

                }
                else
                {
                    response = new Response<List<BuyerOrder>>()
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message

                    };
                }
            }
            catch (Exception ex)
            {
                response = new Response<List<BuyerOrder>>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message

                };
            }

            return Ok(response);
        }

        [HttpGet]
        [JwtRoleAuthentication(Actor = "User")]
        public IHttpActionResult GetAllOrderLineForUser(int orderId)
        {
            int userId = int.Parse(_controllerActions.getDefaultUserId((ClaimsIdentity)User.Identity));
            Response<UserOrderLineFront> response;

            try
            {
                var lang = Request.GetLangFromHeader();
                var langId = _langRepository.GetLangId(lang);

                response = new Response<UserOrderLineFront>()
                {
                    Code = (int)HttpStatusCode.Created,
                    Data = _orderRepository.AllOrderLineForUser(orderId, userId,langId),
                    Success = true
                };

            }
            catch (SqlException ex)
            {
                if (ex.Number == 51000)
                {

                    response = new Response<UserOrderLineFront>()
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };

                }
                else
                {
                    response = new Response<UserOrderLineFront>()
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message

                    };
                }
            }
            catch (Exception ex)
            {
                response = new Response<UserOrderLineFront>()
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
