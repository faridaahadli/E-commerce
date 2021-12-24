using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using Castle.Core.Internal;
using CRMHalalBackEnd.App_Code;
using CRMHalalBackEnd.Filters;
using CRMHalalBackEnd.Helpers;
using CRMHalalBackEnd.Models;
using CRMHalalBackEnd.Models.Category;
using CRMHalalBackEnd.Models.Product;
using CRMHalalBackEnd.Models.ProductUnit;
using CRMHalalBackEnd.Models.Variation;
using CRMHalalBackEnd.Repository;
using WebApi.Jwt.Filters;

namespace CRMHalalBackEnd.Controllers.Product
{
    [JwtCompanyAuthentication]
    public class ProductOperationController : ApiController
    {
        private readonly UtilsClass _controllerActions = new UtilsClass();
        private readonly LanguagesRepository _langRepository = new LanguagesRepository();
        private readonly StoreRepository _storeRepository = new StoreRepository();

        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "21")]
        public IHttpActionResult Insert(Models.Product.Product product)
        {
            var userid = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);

            Response<ProductDto> response;
            try
            {
                var repository = new ProductRepository();
                repository.Insert(product, tenantId, int.Parse(userid));

                response = new Response<ProductDto>
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Message = "Added Product"
                };

            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<ProductDto>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<ProductDto>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception e)
            {
                response = new Response<ProductDto>
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "18")]
        public IHttpActionResult Update(VariationUpd variationUpd)
        {
            var userid = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            Response<string> response;
            try
            {
                var repository = new ProductRepository();
                var newProduct = repository.Update(variationUpd, tenantId, int.Parse(userid));

                response = new Response<string>
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Data = "Dəyişiklik uğurla tamamlandı"
                };

            }
            catch (SqlException ex)
            {

                if (ex. Number == 51000)
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "30")]
        public IHttpActionResult UpdateMainPageProduct(bool showOnMain, string id)
        {
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            var userid = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            Response<List<VariationMainPageDto>> response;
            try
            {
                var repository = new ProductRepository();
                var lang = Request.GetLangFromHeader();
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);
                repository.UpdateProductShowOnMainPage(id, showOnMain, tenantId, int.Parse(userid));

                response = new Response<List<VariationMainPageDto>>
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Data = repository.GetAllVariationOnMainPageWithAdmin(langNumber, tenantId, int.Parse(userid))
                };

            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<List<VariationMainPageDto>>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<List<VariationMainPageDto>>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception e)
            {
                response = new Response<List<VariationMainPageDto>>
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "16")]
        public IHttpActionResult UpdateModel(Models.Product.Product product)
        {
            var userid = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            Response<ProductDto> response;
            try
            {
                var repository = new ProductRepository();
                var newProduct = repository.UpdateModel(product, tenantId, int.Parse(userid));

                response = new Response<ProductDto>
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Message = "Dəyişiklik uğurla tamamlandı"
                    //Data = repository.GetProductModelByGroupId(product.GroupId, tenantId)
                };

            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<ProductDto>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<ProductDto>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception e)
            {
                response = new Response<ProductDto>
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "24")]
        public IHttpActionResult Delete(string id)
        {
            var userid = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            Response<string> response;
            try
            {
                var repository = new ProductRepository();
                var newProduct = repository.Delete(id, tenantId, int.Parse(userid));

                response = new Response<string>
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = "Uğurla Silindi"
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
                    Code = (int)HttpStatusCode.NoContent,
                    Success = false,
                    Message = e.Message,
                    Data = null
                };
            }

            return Ok(response);
        }

        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "24")]
        public IHttpActionResult DeleteVariations(params string[] productsId)
        {

            Response<List<VariationDto>> response;
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            var userId = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            try
            {
                var repository = new ProductRepository();
                repository.DeleteCategoryProduct(tenantId, int.Parse(userId), productsId);
                response = new Response<List<VariationDto>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Message = "Products Delete"
                };
            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<List<VariationDto>>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<List<VariationDto>>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception ex)
            {
                response = new Response<List<VariationDto>>()
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "23")]
        public IHttpActionResult DeleteProduct(string groupId)
        {
            var userid = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            Response<string> response;
            try
            {
                var repository = new ProductRepository();
                repository.DeleteProductModel(groupId, tenantId, int.Parse(userid));

                response = new Response<string>
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = "Uğurla Silindi"
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "21")]
        public IHttpActionResult GetBuyGroup()
        {
            Response<string> response;

            try
            {
                var repository = new ProductRepository();
                response = new Response<string>()
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Data = repository.GetGroupId(7, 8)
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
        [HttpGet]
        [JwtRoleAuthentication(Actor = "Company", Permission = "13,21")]
        public IHttpActionResult GetAllManufacturer()
        {
            Response<List<string>> response;
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            var userId = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            try
            {
                var repository = new ProductRepository();
                response = new Response<List<string>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = repository.GetAllManufacturer(tenantId, int.Parse(userId))
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<string>>()
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
        [JwtRoleAuthentication(Actor = "Company")]
        public IHttpActionResult GetAllMeasureType()
        {
            Response<List<Dictionary<string, dynamic>>> response;
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity) User.Identity);
            try
            {
                var repository = new ProductRepository();
                response = new Response<List<Dictionary<string, dynamic>>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = repository.GetAllMeasureType(tenantId)
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<Dictionary<string, dynamic>>>()
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
        [Obsolete]
        // Admin Sehifede tablede productlari getirmek ucun api. is_visible true ve false olanlari gelirir.
        public IHttpActionResult GetAllProductOld(int page, int perSize = 10)
        {
            Response<List<VariationDto>> response;
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            try
            {
                var repository = new ProductRepository();
                response = new Response<List<VariationDto>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = repository.GetAllProductOld(tenantId, page, perSize)
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<VariationDto>>()
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            return Ok(response);
        }

        [HttpGet]
        [JwtRoleAuthentication(Actor = "Company", Permission = "13")]
        public IHttpActionResult GetAllProduct(int page = 0, int pageSize = 0, string guid = "", string brent = "", int categoryId = 0,
            int minPrice = 0, int maxPrice = int.MaxValue, string sort = "date_desc")
        {
            Response<List<VariationDto>> response;
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            var userId = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            
            try
            {
                var repository = new ProductRepository();
                var lang = Request.GetLangFromHeader();
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);
                response = new Response<List<VariationDto>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = repository.GetAllProduct(langNumber, tenantId, page, pageSize, guid, brent, categoryId, minPrice, maxPrice, sort, int.Parse(userId))
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<VariationDto>>()
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "23,26,30,40")]
        // Home page settingsde istifade olunan api. Ancag is_visiable true olanlari gondeririy.
        public IHttpActionResult SearchAllProductAdmin()
        {
            Response<List<VariationMainPageDto>> response;
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            try
            {
                var repository = new ProductRepository();
                var lang = Request.GetLangFromHeader();
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);

                response = new Response<List<VariationMainPageDto>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = repository.SearchAllVariationForShop(langNumber, lang, true, tenantId)
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<VariationMainPageDto>>()
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
        [Obsolete]
        // Admin sehifede filter ucun Productlarin adini tam formada geliren Api. is_visibale true ve false olanalri getirir.
        public IHttpActionResult SearchAllProductForFilter(string domain, string data = "")
        {
            Response<List<VariationMainPageDto>> response;
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            try
            {
                var repository = new ProductRepository();
                var lang = Request.GetLangFromHeader();
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);

                response = new Response<List<VariationMainPageDto>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = repository.SearchAllVariationForShop(langNumber, lang,false, tenantId, data)
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<VariationMainPageDto>>()
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "13")]
        // Admin sehifede Update ucun modali dolduran api.
        public IHttpActionResult GetProductById(string id)
        {
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)(User.Identity));
            var userId = _controllerActions.getActiveUserId((ClaimsIdentity)(User.Identity));
            Response<VariationUpdDto> response;
            try
            {
                var repository = new ProductRepository();
                var lang = Request.GetLangFromHeader();
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);

                response = new Response<VariationUpdDto>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = repository.GetVariationById(langNumber, id, tenantId, int.Parse(userId))
                };
            }
            catch (Exception ex)
            {
                response = new Response<VariationUpdDto>()
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "11")]
        // Admin Sehifede Product modeli update elemek ucun yazilan api.
        public IHttpActionResult GetProductModelByGroupId(string groupId)
        {
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            var userId = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            Response<ProductDto> response;
            try
            {
                var repository = new ProductRepository();
                response = new Response<ProductDto>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = repository.GetProductModelByGroupId(groupId, tenantId, int.Parse(userId))
                };
            }
            catch (Exception ex)
            {
                response = new Response<ProductDto>()
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
        [AllowAnonymous]
        [Route("note/api/ProductOperation/GetProductModelByGuid/{domain}")]
        // Productun alish sehifesinde varasialar ile birlikde getiren Api.
        public IHttpActionResult GetProductModelByGuid(string domain,int id)
        {
            Response<ProductShopPageDto> response;
            try
            {
                var repository = new ProductRepository();
                var lang = Request.GetLangFromHeader();

                var tenantId = _storeRepository.GetTenantIdByStoreName(domain.Replace('_','.'));
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);

                response = new Response<ProductShopPageDto>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = repository.GetProductModelByProductGuid(langNumber, lang, domain.Replace('_','.'),id)
                };
            }
            catch (Exception ex)
            {
                response = new Response<ProductShopPageDto>()
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
        [Obsolete]
        // Admin sehifede tablenin Pagelerinin sayini hesablayan api.
        public IHttpActionResult GetAllProductPageSizeOld(int perSize = 10)
        {
            Response<int> response;
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            try
            {
                var repository = new ProductRepository();
                response = new Response<int>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = repository.GetAllProductPageSizeOld(perSize, tenantId)
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "13")]
        // Admin sehifede tablenin Pagelerinin sayini hesablayan api.
        public IHttpActionResult GetAllProductPageSize(int pageSize = 0, string guid = "", string brent = "", int categoryId = 0,
            int minPrice = 0, int maxPrice = int.MaxValue)
        {
            Response<int> response;
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            var userId = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            try
            {
                var repository = new ProductRepository();
                response = new Response<int>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = repository.GetAllProductPageSize(tenantId, pageSize, guid, brent, categoryId, minPrice, maxPrice, int.Parse(userId))
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "29")]
        // admin sehifede mehsullar hissesinde load olunanda productlari gonderen api.
        public IHttpActionResult GetAllProductOnMainPage()
        {
            Response<List<VariationMainPageDto>> response;
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            var userId = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            try
            {
                var repository = new ProductRepository();
                var lang = Request.GetLangFromHeader();
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);

                response = new Response<List<VariationMainPageDto>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = repository.GetAllVariationOnMainPageWithAdmin(langNumber, tenantId, int.Parse(userId))
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<VariationMainPageDto>>()
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
        [Route("note/api/ProductOperation/GetAllProductForShop/{domain}")]
        [AllowAnonymous]
        // Bu Store sehifesinde ve Note sehifesinde productlari gonderir.
        public IHttpActionResult GetAllProductForShop(string domain)
        {
            StoreRepository repo = new StoreRepository();
            Response<List<VariationMainPageDto>> response;
            try
            {
                var repository = new ProductRepository();
                var lang = Request.GetLangFromHeader();

                var tenantId = _storeRepository.GetTenantIdByStoreName(domain.Replace('_','.'));
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);

                response = new Response<List<VariationMainPageDto>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = repository.SearchAllVariationFilter(langNumber, true, tenantId)
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<VariationMainPageDto>>()
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
        [Route("note/api/ProductOperation/SearchAllProductShopPage/{domain}")]
        [AllowAnonymous]
        public IHttpActionResult SearchAllProductShopPage(string domain, string data = "")
        {
            Response<List<VariationMainPageDto>> response;
            try
            {
                var repository = new ProductRepository();
                var lang = Request.GetLangFromHeader();

                var tenantId = _storeRepository.GetTenantIdByStoreName(domain.Replace('_','.'));
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);

                response = new Response<List<VariationMainPageDto>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = repository.SearchAllVariationForShop(langNumber, lang, true, tenantId, data,domain.Replace('_','.'))
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<VariationMainPageDto>>()
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
        [AllowAnonymous]
        [Obsolete]
        // categoriya Seo
        public IHttpActionResult SearchProductByCategoryIdOrAttributeNameOld()
        {
            var queryString = Request.RequestUri.Query.Remove(0, 1);
            var queryData = queryString.Split('&');
            var categoryId = string.Empty;
            int minPrice = 0;
            int maxPrice = Int32.MaxValue;
            bool isDiscount = false;
            int differenceAttributeCount = 0;
            string val = String.Empty;
            string type = String.Empty;
            var attributes = new List<KeyValuePair<string, string>>();

            foreach (var query in queryData)
            {
                var keyVal = query.Split('=');
                if (keyVal[0].Equals("categoryId"))
                {
                    categoryId = keyVal[1];
                    continue;
                }
                if (keyVal[0].Equals("minPrice"))
                {
                    minPrice = int.Parse(keyVal[1]);
                    continue;
                }
                if (keyVal[0].Equals("maxPrice"))
                {
                    maxPrice = int.Parse(keyVal[1]);
                    continue;
                }
                if (keyVal[0].Equals("isDiscount"))
                {
                    isDiscount = Boolean.Parse(keyVal[1]);
                    continue;
                }
                if (keyVal[0].Equals("differenceAttributeCount"))
                {
                    differenceAttributeCount = int.Parse(keyVal[1]);
                    continue;
                }
                if (keyVal[0].Equals("val"))
                {
                    val = keyVal[1];
                    continue;
                }
                if (keyVal[0].Equals("type"))
                {
                    type = keyVal[1];
                    continue;
                }
                attributes.Add(new KeyValuePair<string, string>(HttpUtility.UrlDecode(keyVal[0]), HttpUtility.UrlDecode(keyVal[1])));

            }

            Response<List<VariationMainPageDto>> response;
            try
            {
                var repository = new ProductRepository();
                var data = repository.ProductShopFilter(categoryId, minPrice, maxPrice, differenceAttributeCount,
                    isDiscount, attributes);
                if (val.Equals("name"))
                {
                    if (type.Equals("a"))
                    {
                        data.Sort((a, b) => String.Compare(a.Name, b.Name, StringComparison.Ordinal));
                    }
                    else
                    {
                        data.Sort((a, b) => String.Compare(b.Name, a.Name, StringComparison.Ordinal));
                    }
                }
                else if (val.Equals("price"))
                {
                    if (type.Equals("a"))
                    {
                        data.Sort((a, b) => a.Discount.CompareTo(b.Discount));
                    }
                    else
                    {
                        data.Sort((a, b) => b.Discount.CompareTo(a.Discount));
                    }
                }
                else
                {
                    if (type.Equals("d"))
                    {
                        data.Reverse();
                    }
                }

                response = new Response<List<VariationMainPageDto>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = data
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<VariationMainPageDto>>()
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
        [Route("note/api/ProductOperation/SearchProductByCategoryIdOrAttributeName/{domain}")]
        [AllowAnonymous]
        public IHttpActionResult SearchProductByCategoryIdOrAttributeName(string domain)
        {
            var queryString = !Request.RequestUri.Query.IsNullOrEmpty()? Request.RequestUri.Query.Remove(0, 1):"";
            var queryData = queryString.Split('&');
            var filterData = new DataForFilter();
            foreach (var query in queryData)
            {
                var keyVal = query.Split('=');
                if (keyVal[0].Equals("categoryId"))
                {
                    filterData.CategoryId = int.Parse(keyVal[1]);
                    continue;
                }
                if (keyVal[0].Equals("minPrice"))
                {
                    filterData.MinPrice = decimal.Parse(keyVal[1]);
                    continue;
                }
                if (keyVal[0].Equals("maxPrice"))
                {
                    filterData.MaxPrice = decimal.Parse(keyVal[1]);
                    continue;
                }
                if (keyVal[0].Equals("isDiscount"))
                {
                    filterData.IsDiscount = Boolean.Parse(keyVal[1]);
                    continue;
                }
                if (keyVal[0].Equals("differenceAttributeCount"))
                {
                    continue;
                }
                if (keyVal[0].Equals("val"))
                {
                    filterData.OrderBy = keyVal[1];
                    continue;
                }
                if (keyVal[0].Equals("type"))
                {
                    filterData.OrderType = keyVal[1];
                    continue;
                }
                if(!keyVal[0].IsNullOrEmpty())
                    filterData.Attributes.Add(new KeyValuePair<string, string>(HttpUtility.UrlDecode(keyVal[0]), HttpUtility.UrlDecode(keyVal[1])));
            }
            Response<List<VariationMainPageDto>> response;
            try
            {
                var repository = new ProductRepository();
                var lang = Request.GetLangFromHeader();

                var tenantId = _storeRepository.GetTenantIdByStoreName(domain.Replace('_','.'));
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);
                response = new Response<List<VariationMainPageDto>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = repository.ProductShopFilterWithProcedure(filterData, langNumber, lang, domain.Replace('_','.'))
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<VariationMainPageDto>>()
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "13")]
        public IHttpActionResult GetProductCount()
        {
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)(User.Identity));
            var userId = _controllerActions.getActiveUserId((ClaimsIdentity)(User.Identity));
            Response<int> response;
            try
            {
                var repository = new ProductRepository();
                response = new Response<int>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = false,
                    Data = repository.GetProductCount(tenantId, int.Parse(userId))
                };

            }
            catch (Exception ex)
            {
                response = new Response<int>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Data = 0,
                    Message = ex.Message
                };
            }
            return Ok(response);
        }

        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "30")]
        public IHttpActionResult MainPageProductSort(IList<string> productGuids,bool isRandom)
        {
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)(User.Identity));
            var userId = _controllerActions.getActiveUserId((ClaimsIdentity)(User.Identity));

            Response<string> response;
            try
            {
                var repository = new ProductRepository();
                if (isRandom)
                {
                    productGuids.Shuffle();
                }
                repository.UpdateProductWeight(productGuids, tenantId, int.Parse(userId));
                response = new Response<string>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = false,
                    Data = "Dəyişiklik uğurla tamamlandı."
                };

            }
            catch (Exception ex)
            {
                response = new Response<string>()
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
