using CRMHalalBackEnd.App_Code;
using CRMHalalBackEnd.Filters;
using CRMHalalBackEnd.Models;
using CRMHalalBackEnd.Models.Promotion;
using CRMHalalBackEnd.Models.Store;
using CRMHalalBackEnd.Repository;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using CRMHalalBackEnd.Helpers;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Castle.Core.Internal;

namespace CRMHalalBackEnd.Controllers.Store
{
    public class StoreController : ApiController
    {
        private readonly UtilsClass _controllerActions = new UtilsClass();
        [HttpGet]
        [Route("note/api/store/get/{slug}")]
        public IHttpActionResult Get(string slug)
        {
            Response<StoreResponse> response;
            int userId = int.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            StoreResponse storeRes = null;
            try
            {
                var storeRepo = new StoreRepository();
                storeRes = storeRepo.GetStoreByName(slug);
                response = new Response<StoreResponse>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "",
                    Success = true,
                    Data = storeRes
                };
            }
            catch (Exception ex)
            {
                response = new Response<StoreResponse>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = "Yenidən cəhd edin",
                    Success = false,
                    Data = null
                };
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("note/api/Store/GetAllStore")]
        [AllowAnonymous]
        public IHttpActionResult GetStoresForNote()
        {
            var storeRepo = new StoreRepository();

            Response<List<StoreResponse>> response;
            try
            {
                response = new Response<List<StoreResponse>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "",
                    Success = true,
                    Data = storeRepo.GetStoresForNote()
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<StoreResponse>>()
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
        [Route("note/api/Store/GetStoreAllData/{domain}")]
        [AllowAnonymous]
        public IHttpActionResult GetStoreAllData(string domain, DateTime? date = null)
        {
            var storeRepo = new StoreRepository();
            var productRepo = new ProductRepository();
            var specialRepo = new SpecialOfferRepository();
            var categoryRepo = new CategoryRepository();
            var promoRepo = new PromotionRepository();
            var langRepo = new LanguagesRepository();

            date = date ?? DateTime.Now.ToUniversalTime();
            Response<StorePageDto> response;
            try
            {
                string tenantId = storeRepo.GetTenantIdByStoreName(domain);
                var lang = Request.GetLangFromHeader();
                var langNumber = langRepo.GetLangNumberForStore(lang, tenantId);

                StorePageDto storePageDto = new StorePageDto()
                {
                    Store = storeRepo.GetStoreByDomain(domain),
                    Category = categoryRepo.ShopCategory(langNumber, lang, tenantId),
                    Slider = storeRepo.GetStoreSlidersByTenantIdWithoutUserId(tenantId),
                    DailySpecialOffer = specialRepo.GetAllSpecialOfferByStorePage(langNumber, lang, tenantId, true, date.Value.ToString("yyyy-MM-dd"), 0),
                    SpecialOffer = specialRepo.GetAllSpecialOfferByStorePage(langNumber, lang, tenantId, false, date.Value.ToString("yyyy-MM-dd"), 0),
                    ProductMainPages = productRepo.GetAllVariationOnMainPageWithUser(langNumber, lang, tenantId, 0),
                    Promotions = (List<PromotionResponse>)promoRepo.GetPromotionsWithoutUser(langNumber, lang, tenantId, date.Value.ToString("yyyy-MM-dd"))
                };
                response = new Response<StorePageDto>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "",
                    Success = true,
                    Data = storePageDto
                };
            }
            catch (Exception ex)
            {
                response = new Response<StorePageDto>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = "Yenidən cəhd edin",
                    Success = false,
                    Data = null
                };
            }
            return Ok(response);
        }

        [HttpGet]
        [Route("note/api/Store/GetStoreStatus/{domain}")]
        [AllowAnonymous]
        public IHttpActionResult GetStoreStatus(string domain)
        {
            Response<bool?> response;

            bool status = false;
            try
            {
                var storeRepo = new StoreRepository();
                status = storeRepo.GetStoreStatusByDomain(domain.Replace("_","."));
                response = new Response<bool?>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "",
                    Success = true,
                    Data = status
                };
            }
            catch (Exception ex)
            {
                response = new Response<bool?>()
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
        [JwtRoleAuthentication(Actor = "User")]
        public IHttpActionResult GetStoreAllDataForUser(string domain, DateTime? date = null)
        {
            var storeRepo = new StoreRepository();
            var productRepo = new ProductRepository();
            var specialRepo = new SpecialOfferRepository();
            var categoryRepo = new CategoryRepository();
            var promoRepo = new PromotionRepository();
            var langRepo = new LanguagesRepository();

            date = date ?? DateTime.Now.ToUniversalTime();
            Response<StorePageDto> response;
            try
            {
                int userId = int.Parse(_controllerActions.getUserId((ClaimsIdentity)User.Identity));
                string tenantId = storeRepo.GetTenantIdByStoreName(domain);
                var lang = Request.GetLangFromHeader();
                var langNumber = langRepo.GetLangNumberForStore(lang, tenantId);

                StorePageDto storePageDto = new StorePageDto()
                {
                    Store = storeRepo.GetStoreByDomain(domain),
                    Category = categoryRepo.ShopCategory(langNumber, lang, tenantId),
                    Slider = storeRepo.GetStoreSlidersByTenantIdWithoutUserId(tenantId),
                    IsFollowing = storeRepo.GetFollowingData(tenantId, userId),
                    DailySpecialOffer = specialRepo.GetAllSpecialOfferByStorePage(langNumber, lang, tenantId, true, date.Value.ToString("yyyy-MM-dd"), userId),
                    SpecialOffer = specialRepo.GetAllSpecialOfferByStorePage(langNumber, lang, tenantId, false, date.Value.ToString("yyyy-MM-dd"), userId),
                    ProductMainPages = productRepo.GetAllVariationOnMainPageWithUser(langNumber, lang, tenantId, userId),
                    Promotions = (List<PromotionResponse>)promoRepo.GetPromotionsWithoutUser(langNumber, lang, tenantId, date.Value.ToString("yyyy-MM-dd"))
                };
                response = new Response<StorePageDto>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "",
                    Success = true,
                    Data = storePageDto
                };
            }
            catch (Exception ex)
            {
                response = new Response<StorePageDto>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = "Yenidən cəhd edin",
                    Success = false,
                    Data = null
                };
            }
            return Ok(response);
        }

        [JwtRoleAuthentication(Actor = "Company", Permission = "2")]
        public IHttpActionResult Get()
        {
            Response<StoreResponse> response;
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            string userId = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            StoreResponse storeRes = null;
            try
            {
                var storeRepo = new StoreRepository();
                storeRes = storeRepo.GetStoreByTenant(tenantId, int.Parse(userId));
                response = new Response<StoreResponse>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "",
                    Success = true,
                    Data = storeRes
                };
            }
            catch (Exception ex)
            {
                response = new Response<StoreResponse>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = "Yenidən cəhd edin",
                    Success = false,
                    Data = null
                };
            }
            return Ok(response);
        }

        [HttpGet]
        [JwtRoleAuthentication(Actor = "Company")]
        public IHttpActionResult GetDomain()
        {
            Response<string> response;
            var repository = new StoreRepository();
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity) User.Identity);
            try
            {
                response = new Response<string>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = repository.GetStoreDomain(tenantId)
                };
            }
            catch (Exception e)
            {
                response = new Response<string>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Data = null
                };
            }
            return Ok(response);
        }

        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult CheckDomain(string domain)
        {
            Response<bool?> response;

            bool exist = false;
            try
            {
                var storeRepo = new StoreRepository();
                exist = storeRepo.CheckDomain(domain);
                response = new Response<bool?>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "",
                    Success = true,
                    Data = exist
                };
            }
            catch (Exception ex)
            {
                response = new Response<bool?>()
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "4")]
        public async Task<IHttpActionResult> Post(StoreInsDto store)
        {
            Response<StoreResponse> response;
            int userId = int.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            StoreResponse storeRes = null;
            try
            {
                var storeRepo = new StoreRepository();
                if (!Regex.IsMatch(store.Contacts.Where(k => k.ContactTypeId == 2).Select(k => k.Text).First(), @"^\+[1-9]{1}[0-9]{3,14}$"))
                {
                    throw new Exception("Nömrənin formatı düzgün deyil!");
                }

                if (store.Domain.Contains(".note.az"))
                {
                    string subDomain = store.Domain.Substring(0, store.Domain.LastIndexOf(".note.az", StringComparison.CurrentCultureIgnoreCase));
                    var firstSubDomain = subDomain.Split('.')[0];
                    store.Domain = firstSubDomain + ".note.az";
                }

                storeRes = storeRepo.Insert(store, userId, tenantId);

                string htmlSubDomain = HtmlFileSend.HtmlFileSender("~/HtmlFiles/Subdomain/index.html");
                string htmlDomain = HtmlFileSend.HtmlFileSender("~/HtmlFiles/Domain/index.html");

                store.Contacts.ForEach(k =>
                {
                    Task.Run(async () =>
                    {
                        if (k.ContactTypeId == 1)
                        {
                            await EmailSend.SendEmailAsync(k.Text, "Note - Mağaza qeydiyyatı", store.Domain.Contains(".note.az")?htmlSubDomain:htmlDomain);
                          
                        }
                        
                    });
                });

                

               

                response = new Response<StoreResponse>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Mağaza uğurla yaradıldı",
                    Success = true,
                    Data = storeRes
                };
            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<StoreResponse>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<StoreResponse>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception ex)
            {
                response = new Response<StoreResponse>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Message = ex.Message,
                    Success = false,
                    Data = null
                };
            }
            return Ok(response);
        }

        #region MainPageSettings

        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "20")]
        public IHttpActionResult InsertSlider(SliderDto slider)
        {
            Response<int> response;
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = Int32.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            try
            {
                var storeRepo = new StoreRepository();
                response = new Response<int>()
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Data = storeRepo.InsertSliderForStore(slider, tenantId, userId),
                    Message = "Slider uğurla yaradıldı!"
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
                    Success = false,
                    Data = 0
                };
            }
            return Ok(response);
        }

        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "22")]
        public IHttpActionResult DeleteSlider(int sliderId)
        {
            Response<int> response;
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = Int32.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            StoreResponse storeRes = null;
            try
            {
                var storeRepo = new StoreRepository();
                int returnSliderId = storeRepo.DeleteStoreSliderById(sliderId, tenantId, userId);
                response = new Response<int>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Slider ugurla slindi.",
                    Success = true,
                    Data = returnSliderId
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

        [HttpGet]
        [JwtRoleAuthentication(Actor = "Company", Permission = "19")]
        public IHttpActionResult GetSliders()
        {
            Response<List<SliderDto>> response;
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            string userId = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            try
            {
                var storeRepo = new StoreRepository();
                List<SliderDto> sliderDto = storeRepo.GetStoreSlidersByTenantId(tenantId, int.Parse(userId));
                response = new Response<List<SliderDto>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = sliderDto
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<SliderDto>>()
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "17")]
        public IHttpActionResult UpdateShowSpecial(bool showSpecial)
        {
            Response<string> response;
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = Int32.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            try
            {
                var storeRepo = new StoreRepository();
                storeRepo.UpdateStoreShowSpecial(tenantId, userId, showSpecial);
                response = new Response<string>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Deyishiklik Ugurla Tamamlandi",
                    Success = true,
                    Data = null
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
                    Success = false,
                    Data = null
                };
            }
            return Ok(response);
        }

        #endregion


        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "3")]
        public IHttpActionResult UpdateStore(StoreUpd store)
        {
            Response<string> response;
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = Int32.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            try
            {
                if (!Regex.IsMatch(store.Contacts.Where(k => k.ContactTypeId == 2).Select(k => k.Text).First(), @"^\+[1-9]{1}[0-9]{3,14}$"))
                {
                    throw new Exception("Nömrənin formatı düzgün deyil!");
                }
                var storeRepo = new StoreRepository();
                storeRepo.UpdateStore(tenantId, userId, store);
                response = new Response<string>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Deyishiklik Ugurla Tamamlandi",
                    Success = true,
                    Data = null
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
                    Success = false,
                    Data = null
                };
            }
            return Ok(response);
        }

        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "3")]
        public IHttpActionResult UpdateStoreSettings(StoreSettingsUpd storeSettings)
        {
            Response<string> response;
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = Int32.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            try
            {
                var storeRepo = new StoreRepository();

                StringBuilder builder = new StringBuilder();
                if (!storeSettings.CheckedPaymentType.IsNullOrEmpty())
                {
                    if (storeSettings.CheckedPaymentType.Count != 3)
                        throw new Exception("Ödəniş üsulları düzgün daxil edilməyib!");
                    storeSettings.CheckedPaymentType.Reverse();
                    storeSettings.CheckedPaymentType.ForEach(x => { builder.Append(x ? "1" : "0"); });
                    storeSettings.PaymentTypeBinary = Convert.ToInt32(builder.ToString(), 2);
                }
                storeRepo.UpdateStoreSettings(tenantId, userId, storeSettings);
                response = new Response<string>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Deyishiklik Ugurla Tamamlandi",
                    Success = true,
                    Data = null
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
                    Success = false,
                    Data = null
                };
            }
            return Ok(response);
        }


    }
}
