using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Web.Http;
using System.Data.SqlClient;

using CRMHalalBackEnd.Filters;
using CRMHalalBackEnd.Helpers;
using CRMHalalBackEnd.App_Code;
using CRMHalalBackEnd.Models;
using CRMHalalBackEnd.Models.Basket;
using CRMHalalBackEnd.Repository;

namespace CRMHalalBackEnd.Controllers.Basket
{
    [JwtRoleAuthentication(Actor = "User")]
    public class NewBasketController : ApiController
    {
        private readonly UtilsClass _controllerActions = new UtilsClass();
        private readonly LanguagesRepository _langRepository = new LanguagesRepository();
        private readonly StoreRepository _storeRepository = new StoreRepository();
        private readonly BasketRepository _basketRepository = new BasketRepository();

        [HttpGet]
        [Route("note/api/NewBasket/Get/{domain}")]
        public IHttpActionResult Get(string domain)
        {
            Response<IEnumerable<NewBasketResponse>> response;
            int userId = int.Parse(_controllerActions.getUserId((ClaimsIdentity)User.Identity));
            IEnumerable<NewBasketResponse> baskets;
            try
            {
                var lang = Request.GetLangFromHeader();

                var tenantId = _storeRepository.GetTenantIdByStoreName(domain.Replace('_','.'));
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);

                baskets = _basketRepository.GetAllBasketElements(langNumber, lang, userId);
                response = new Response<IEnumerable<NewBasketResponse>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = baskets
                };
            }
            catch (Exception ex)
            {
                response = new Response<IEnumerable<NewBasketResponse>>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = ex.Message,
                    Success = false
                };
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("note/api/NewBasket/Post/{domain}")]
        public IHttpActionResult Post(string domain, NewBasket basket)
        {
            Response<IEnumerable<NewBasketResponse>> response;
            int userId = int.Parse(_controllerActions.getUserId((ClaimsIdentity)User.Identity));
            IEnumerable<NewBasketResponse> baskets;
            try
            {
                var lang = Request.GetLangFromHeader();

                var tenantId = _storeRepository.GetTenantIdByStoreName(domain.Replace('_', '.'));
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);


                baskets = _basketRepository.Insert(basket, langNumber, lang, userId);
                response = new Response<IEnumerable<NewBasketResponse>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Səbətə əlavə edildi",
                    Success = true,
                    Data = baskets
                };
            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<IEnumerable<NewBasketResponse>>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<IEnumerable<NewBasketResponse>>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception ex)
            {
                response = new Response<IEnumerable<NewBasketResponse>>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = ex.Message,
                    Success = false
                };
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("note/api/NewBasket/PostBasketProducts/{domain}")]
        public IHttpActionResult PostBasketProducts(string domain, List<NewBasket> products)
        {
            Response<IEnumerable<NewBasketResponse>> response;

            int userId = int.Parse(_controllerActions.getUserId((ClaimsIdentity)User.Identity));
            IEnumerable<NewBasketResponse> baskets = new List<NewBasketResponse>();
            try
            {
                var lang = Request.GetLangFromHeader();

                var tenantId = _storeRepository.GetTenantIdByStoreName(domain.Replace('_', '.'));
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);

                foreach (var product in products)
                {
                    baskets = _basketRepository.Insert(product, langNumber, lang, userId);
                }

                response = new Response<IEnumerable<NewBasketResponse>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Səbətə əlavə edildi",
                    Success = true,
                    Data = baskets
                };
            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<IEnumerable<NewBasketResponse>>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<IEnumerable<NewBasketResponse>>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception ex)
            {
                response = new Response<IEnumerable<NewBasketResponse>>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = ex.Message,
                    Success = false
                };
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("note/api/NewBasket/PostWithoutLogin/{domain}")]
        [AllowAnonymous]
        public IHttpActionResult PostWithoutLogin(string domain, NewBasket basket)
        {
            Response<IEnumerable<NewBasketResponse>> response;
            IEnumerable<NewBasketResponse> baskets;
            try
            {
                var lang = Request.GetLangFromHeader();

                var tenantId = _storeRepository.GetTenantIdByStoreName(domain.Replace('_', '.'));
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);

                baskets = _basketRepository.Insert(basket, langNumber, lang, 0);
                response = new Response<IEnumerable<NewBasketResponse>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Səbətə əlavə edildi",
                    Success = true,
                    Data = baskets
                };
            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<IEnumerable<NewBasketResponse>>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<IEnumerable<NewBasketResponse>>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception ex)
            {
                response = new Response<IEnumerable<NewBasketResponse>>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = ex.Message,
                    Success = false
                };
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("note/api/NewBasket/PromotionBasketPost/{domain}")]
        public IHttpActionResult PromotionBasketPost(string domain, PromotionBasketInsDto basket)
        {
            Response<IEnumerable<NewBasketResponse>> response;
            int userId = int.Parse(_controllerActions.getUserId((ClaimsIdentity)User.Identity));
            IEnumerable<NewBasketResponse> baskets;
            try
            {
                var lang = Request.GetLangFromHeader();

                var tenantId = _storeRepository.GetTenantIdByStoreName(domain.Replace('_', '.'));
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);

                baskets = _basketRepository.Insert(basket, langNumber, lang, userId);
                response = new Response<IEnumerable<NewBasketResponse>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Səbətə əlavə edildi",
                    Success = true,
                    Data = baskets
                };
            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<IEnumerable<NewBasketResponse>>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<IEnumerable<NewBasketResponse>>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }

            catch (Exception ex)
            {
                response = new Response<IEnumerable<NewBasketResponse>>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = ex.Message,
                    Success = false
                };
            }
            return Ok(response);
        }

        //Get promotions inside of a basket without login
        [HttpPost]
        [Route("note/api/NewBasket/PromotionsBasketGet/{domain}")]
        [AllowAnonymous]
        public IHttpActionResult PromotionsBasketGet(string domain, List<PromotionBasketInsDto> promotions)
        {
            Response<IEnumerable<NewBasketResponse>> response;

            IEnumerable<NewBasketResponse> baskets;
            try
            {
                var lang = Request.GetLangFromHeader();

                var tenantId = _storeRepository.GetTenantIdByStoreName(domain.Replace('_', '.'));
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);

                baskets = _basketRepository.GetBasketPromotions(langNumber, lang, promotions);
                response = new Response<IEnumerable<NewBasketResponse>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Səbətə əlavə edildi",
                    Success = true,
                    Data = baskets
                };
            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<IEnumerable<NewBasketResponse>>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<IEnumerable<NewBasketResponse>>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }

            catch (Exception ex)
            {
                response = new Response<IEnumerable<NewBasketResponse>>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = ex.Message,
                    Success = false
                };
            }
            return Ok(response);
        }

        //Get products inside of a basket without login
        [HttpPost]
        [Route("note/api/NewBasket/ProductsBasketGet/{domain}")]
        [AllowAnonymous]
        public IHttpActionResult ProductsBasketGet(string domain, List<NewBasket> products)
        {
            Response<IEnumerable<NewBasketResponse>> response;

            IEnumerable<NewBasketResponse> baskets;
            try
            {
                var lang = Request.GetLangFromHeader();

                var tenantId = _storeRepository.GetTenantIdByStoreName(domain.Replace('_', '.'));
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);

                baskets = _basketRepository.GetBasketProducts(langNumber, lang, products);
                response = new Response<IEnumerable<NewBasketResponse>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = baskets
                };
            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<IEnumerable<NewBasketResponse>>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<IEnumerable<NewBasketResponse>>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }

            catch (Exception ex)
            {
                response = new Response<IEnumerable<NewBasketResponse>>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = ex.Message,
                    Success = false
                };
            }
            return Ok(response);
        } //+

        [HttpPost]
        [Route("note/api/NewBasket/PromotionsBasketPost/{domain}")]
        public IHttpActionResult PromotionsBasketPost(string domain, List<PromotionBasketInsDto> promotions)
        {
            Response<IEnumerable<NewBasketResponse>> response;
            int userId = int.Parse(_controllerActions.getUserId((ClaimsIdentity)User.Identity));
            IEnumerable<NewBasketResponse> baskets = new List<NewBasketResponse>();

            try
            {
                var lang = Request.GetLangFromHeader();

                var tenantId = _storeRepository.GetTenantIdByStoreName(domain.Replace('_', '.'));
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);

                foreach (var promotion in promotions)
                {
                    baskets = _basketRepository.Insert(promotion, langNumber, lang, userId);
                }

                response = new Response<IEnumerable<NewBasketResponse>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Səbətə əlavə edildi",
                    Success = true,
                    Data = baskets
                };
            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<IEnumerable<NewBasketResponse>>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<IEnumerable<NewBasketResponse>>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }

            catch (Exception ex)
            {
                response = new Response<IEnumerable<NewBasketResponse>>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = ex.Message,
                    Success = false
                };
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("note/api/NewBasket/PromotionBasketPostWithoutLogin/{domain}")]
        [AllowAnonymous]
        public IHttpActionResult PromotionBasketPostWithoutLogin(string domain, PromotionBasketInsDto basket)
        {
            Response<IEnumerable<NewBasketResponse>> response;

            IEnumerable<NewBasketResponse> baskets;
            try
            {
                var lang = Request.GetLangFromHeader();

                var tenantId = _storeRepository.GetTenantIdByStoreName(domain.Replace('_', '.'));
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);

                baskets = _basketRepository.Insert(basket, langNumber, lang, 0);
                response = new Response<IEnumerable<NewBasketResponse>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Səbətə əlavə edildi",
                    Success = true,
                    Data = baskets
                };
            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<IEnumerable<NewBasketResponse>>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<IEnumerable<NewBasketResponse>>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }

            catch (Exception ex)
            {
                response = new Response<IEnumerable<NewBasketResponse>>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = ex.Message,
                    Success = false
                };
            }
            return Ok(response);
        }

        [HttpPost]
        [Route("note/api/NewBasket/BasketUpdate/{domain}")]
        public IHttpActionResult BasketUpdate(string domain, NewBasketUpdDto basket)
        {
            Response<IEnumerable<NewBasketResponse>> response;
            IEnumerable<NewBasketResponse> baskets;
            int userId = int.Parse(_controllerActions.getUserId((ClaimsIdentity)User.Identity));
            try
            {
                var lang = Request.GetLangFromHeader();

                var tenantId = _storeRepository.GetTenantIdByStoreName(domain.Replace('_', '.'));
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);

                if (basket.Quantity == 0)
                {
                    baskets = _basketRepository.Delete(basket.BasketGuid, langNumber, lang, userId);
                }
                else
                {
                    baskets = _basketRepository.Update(basket, langNumber, lang, userId);
                }

                response = new Response<IEnumerable<NewBasketResponse>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Uğurla tamamlandı.",
                    Success = true,
                    Data = baskets
                };
            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<IEnumerable<NewBasketResponse>>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<IEnumerable<NewBasketResponse>>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception ex)
            {
                response = new Response<IEnumerable<NewBasketResponse>>()
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
        [Route("note/api/NewBasket/BasketDelete/{domain}")]
        public IHttpActionResult BasketDelete(string domain, string basketGuid)
        {
            Response<IEnumerable<NewBasketResponse>> response;
            IEnumerable<NewBasketResponse> baskets;
            int userId = int.Parse(_controllerActions.getUserId((ClaimsIdentity)User.Identity));
            try
            {
                var lang = Request.GetLangFromHeader();

                var tenantId = _storeRepository.GetTenantIdByStoreName(domain.Replace('_', '.'));
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);

                baskets = _basketRepository.Delete(basketGuid, langNumber, lang, userId);
                response = new Response<IEnumerable<NewBasketResponse>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Uğurla tamamlandı.",
                    Success = true,
                    Data = baskets
                };
            }
            catch (SqlException ex)
            {

                if (ex.Class.Equals("51000"))
                {
                    response = new Response<IEnumerable<NewBasketResponse>>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<IEnumerable<NewBasketResponse>>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception ex)
            {
                response = new Response<IEnumerable<NewBasketResponse>>()
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
        [Route("note/api/NewBasket/BasketDelete/{domain}")]
        public IHttpActionResult BasketDelete(string domain, List<string> basketGuids)
        {
            Response<IEnumerable<NewBasketResponse>> response;
            IEnumerable<NewBasketResponse> baskets = new List<NewBasketResponse>();
            int userId = int.Parse(_controllerActions.getUserId((ClaimsIdentity)User.Identity));
            try
            {
                var lang = Request.GetLangFromHeader();

                var tenantId = _storeRepository.GetTenantIdByStoreName(domain.Replace('_', '.'));
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);

                foreach (string basketGuid in basketGuids)
                {
                    baskets = _basketRepository.Delete(basketGuid, langNumber, lang, userId);
                }

                response = new Response<IEnumerable<NewBasketResponse>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Uğurla tamamlandı.",
                    Success = true,
                    Data = baskets
                };
            }
            catch (SqlException ex)
            {

                if (ex.Class.Equals("51000"))
                {
                    response = new Response<IEnumerable<NewBasketResponse>>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<IEnumerable<NewBasketResponse>>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception ex)
            {
                response = new Response<IEnumerable<NewBasketResponse>>()
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
