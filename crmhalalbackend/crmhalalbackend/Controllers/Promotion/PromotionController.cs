using CRMHalalBackEnd.App_Code;
using CRMHalalBackEnd.Models;
using CRMHalalBackEnd.Models.Promotion;
using CRMHalalBackEnd.Repository;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Web.Http;
using CRMHalalBackEnd.Filters;
using CRMHalalBackEnd.Models.Order;
using System.Data.SqlClient;
using CRMHalalBackEnd.Helpers;

namespace CRMHalalBackEnd.Controllers.Promotion
{
    public class PromotionController : ApiController
    {
        private readonly UtilsClass _controllerActions = new UtilsClass();
        private readonly LanguagesRepository _langRepository = new LanguagesRepository();
        private readonly StoreRepository _storeRepository = new StoreRepository();
        private readonly PromotionRepository _promotionRepository = new PromotionRepository();


        [HttpGet]
        [JwtRoleAuthentication(Actor = "Company", Permission = "25")]
        public IHttpActionResult Get()
        {
            Response<IEnumerable<PromotionResponse>> response;
            int userId = int.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            try
            {
                var promotions = _promotionRepository.GetPromotions(tenantId, userId);
                response = new Response<IEnumerable<PromotionResponse>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = promotions
                };
            }
            catch (Exception ex)
            {
                response = new Response<IEnumerable<PromotionResponse>>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = ex.Message,
                    Success = false,
                    Data = null
                };
            }
            return Ok(response);
        }

        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "26")]
        public IHttpActionResult Post(PromotionInsDto promo)
        {
            Response<IEnumerable<PromotionResponse>> response;
            int userId = int.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            try
            {
                var promotion = _promotionRepository.Insert(promo, tenantId, userId);
                response = new Response<IEnumerable<PromotionResponse>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Promotion yaradıldı",
                    Success = true,
                    Data = promotion
                };
            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<IEnumerable<PromotionResponse>>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<IEnumerable<PromotionResponse>>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception ex)
            {
                response = new Response<IEnumerable<PromotionResponse>>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = ex.Message,
                    Success = false,
                    Data = null
                };
            }
            return Ok(response);
        }

        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "27")]
        public IHttpActionResult PromotionDelete(int promoId)
        {
            Response<PromotionResponse> response;
            int userId = int.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            try
            {
                _promotionRepository.Delete(promoId, tenantId, userId);
                response = new Response<PromotionResponse>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Promotion silindi",
                    Success = true
                };
            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<PromotionResponse>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<PromotionResponse>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception ex)
            {
                response = new Response<PromotionResponse>()
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
        [Route("note/api/Promotion/SinglePromotionGet/{domain}")]
        [AllowAnonymous]
        public IHttpActionResult SinglePromotionGet(string domain, int promotionId)
        {
            Response<PromotionResponse> response;

            try
            {
                var lang = Request.GetLangFromHeader();

                var tenantId = _storeRepository.GetTenantIdByStoreName(domain.Replace('_','.'));
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);

                var promotion = _promotionRepository.GetSinglePromotion(langNumber, lang, promotionId, domain.Replace('_', '.'));
                response = new Response<PromotionResponse>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Single promotion success",
                    Success = true,
                    Data = promotion
                };
            }
            catch (Exception ex)
            {
                response = new Response<PromotionResponse>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = ex.Message,
                    Success = false,
                    Data = null
                };
            }
            return Ok(response);
        }

        [HttpPost]
        [JwtRoleAuthentication(Actor = "User")]
        public IHttpActionResult CheckPromotion(OrderInsDto basket)
        {
            Response<IEnumerable<OrderTypeOnePromo>> response;
            int userId = int.Parse(_controllerActions.getUserId((ClaimsIdentity)User.Identity));
            new List<OrderTypeOnePromo>();
            try
            {
                var result = _promotionRepository.CheckTypeOnePromotion(basket, userId);
                response = new Response<IEnumerable<OrderTypeOnePromo>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Aksiya Yoxlanıldı!",
                    Success = true,
                    Data = result
                };
            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<IEnumerable<OrderTypeOnePromo>>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<IEnumerable<OrderTypeOnePromo>>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception ex)
            {
                response = new Response<IEnumerable<OrderTypeOnePromo>>()
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
