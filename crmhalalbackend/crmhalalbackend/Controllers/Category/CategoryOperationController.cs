using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Security.Claims;
using System.Web.Http;
using System.Web.UI.WebControls;
using CRMHalalBackEnd.App_Code;
using CRMHalalBackEnd.Models;
using CRMHalalBackEnd.Models.Category;
using CRMHalalBackEnd.Repository;
using CRMHalalBackEnd.Models.Variation;
using CRMHalalBackEnd.Filters;
using CRMHalalBackEnd.Helpers;

namespace CRMHalalBackEnd.Controllers.Category
{
    public class CategoryOperationController : ApiController
    {
        private readonly UtilsClass _controllerActions = new UtilsClass();
        private readonly LanguagesRepository _langRepository = new LanguagesRepository();
        private readonly StoreRepository _storeRepository = new StoreRepository();
        private readonly CategoryRepository _categoryRepository = new CategoryRepository();

        //Actor="Company", Permission="9,98"
        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission  =  "9,98")]
        public IHttpActionResult CreateCategory(CategoryDto category)
        {
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = Int32.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            Response<int> response;
            try
            {
                var newCategory = _categoryRepository.Insert(category, userId, tenantId);
                response = new Response<int>
                {
                    Code = (int) HttpStatusCode.Created,
                    Success = true,
                    Data = newCategory,
                    Message = "Kateqoriya uğurla yaradıldı!"
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
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message

                };
            }

            return Ok(response);
        }

        //Actor="Company", Permission="8,9"
        [HttpGet]
        [JwtRoleAuthentication(Actor = "Company", Permission  = "8,9")]
        public IHttpActionResult GetParentCategory()
        {
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = Int32.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            Response<List<AllCategoriesForShop>> response;

            try
            {
                response = new Response<List<AllCategoriesForShop>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _categoryRepository.GetParentCategory(tenantId, userId)
                };
            }
          
            catch (Exception ex)
            {
                response = new Response<List<AllCategoriesForShop>>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }

            return Ok(response);
        }

        //Actor="Company", Permission="5"
        [HttpGet]
        [JwtRoleAuthentication(Actor = "Company", Permission = "5")]
        public IHttpActionResult GetAllCategories()
        {
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = Int32.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));

            Response<List<AllCategories>> response;
            try
            {
                var lang = Request.GetLangFromHeader();

                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);
                response = new Response<List<AllCategories>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _categoryRepository.AllCategories(langNumber, tenantId, userId)
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<AllCategories>>()
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "5")]
        public IHttpActionResult GetCategoriesById(int id)
        {
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = Int32.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));

            Response<CategoryDtoResponse> response;

            try
            {
                response = new Response<CategoryDtoResponse>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _categoryRepository.GetCategoryById(id, userId, tenantId)
                };
            }
            catch (Exception ex)
            {
                response = new Response<CategoryDtoResponse>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }

            return Ok(response);
        }

        //Actor="Company", Permission="8"
        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "8")]
        public IHttpActionResult UpdateCategory(CategoryDto category)
        {
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = Int32.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            Response<string> response;
            try
            {
                _categoryRepository.UpdateCategory(tenantId, userId, category);
                response = new Response<string>
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Message = "Dəyişiklik uğurla tamamlandı."

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
                    Message = ex.Message
                };
            }
            return Ok(response);
        }

        //Actor="Company", Permission="10"
        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "10")]
        public IHttpActionResult DeleteCategory(int categoryId)
        {
            Response<int> response;

            int userId = Int32.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);

            try
            {
                var returnCategoryId = _categoryRepository.DeleteCategory(categoryId, userId, tenantId);
                response = new Response<int>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Kateqoriya uğurla silindi.",
                    Success = true,
                    Data = returnCategoryId
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
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message

                };
            }

            return Ok(response);


        }

        //Actor="Company", Permission="11,13,16,21"
        [HttpGet]
        [JwtRoleAuthentication(Actor = "Company", Permission = "13")]
        public IHttpActionResult GetCategoryForSelectShowProduct()
        {
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = Int32.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));

            Response<List<CategoryForProductPageDto>> response;

            try
            {
                var lang = Request.GetLangFromHeader();
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);

                var category = _categoryRepository.GetCategoryTreeShowProduct(langNumber, tenantId, userId);
                response = new Response<List<CategoryForProductPageDto>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = category
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<CategoryForProductPageDto>>()
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "21")]
        public IHttpActionResult GetCategoryForSelectInsertProduct()
        {
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = Int32.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));

            Response<List<CategoryForProductPageDto>> response;
            try
            {
                var category = _categoryRepository.GetCategoryTreeInsertProduct(tenantId, userId);
                response = new Response<List<CategoryForProductPageDto>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = category
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<CategoryForProductPageDto>>()
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
        [Route("note/api/CategoryOperation/GetCategoryFilterData/{domain}")]
        [AllowAnonymous]
        public IHttpActionResult GetCategoryFilterData(string domain, int categoryId)
        {

            Response<CategorySideBar> response;
            domain = domain.Replace('_', '.');
            try
            {
                var lang = Request.GetLangFromHeader();
                
                var tenantId = _storeRepository.GetTenantIdByStoreName(domain);
                var langNUmber = _langRepository.GetLangNumberForStore(lang, tenantId);

                var filterList = _categoryRepository.GetCategorySideBarData(langNUmber, lang, domain, categoryId);
                response = new Response<CategorySideBar>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = filterList
                };
            }
            catch (Exception ex)
            {
                response = new Response<CategorySideBar>()
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
        [Route("note/api/CategoryOperation/GetLikeProducts/{domain}")]
        [AllowAnonymous]
        public IHttpActionResult GetLikeProducts(string domain, string productGuid)
        {
            Response<List<VariationMainPageDto>> response;
            try
            {
                var lang = Request.GetLangFromHeader();

                var tenantId = _storeRepository.GetTenantIdByStoreName(domain.Replace('_', '.'));
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);
                response = new Response<List<VariationMainPageDto>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _categoryRepository.LikeProducts(langNumber, lang, productGuid, 0)
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
        [Route("note/api/CategoryOperation/GetLikeProductsForUser/{domain}")]
        [JwtRoleAuthentication(Actor ="User")]
        public IHttpActionResult GetLikeProductsForUser(string domain, string productGuid)
        {
            int userId = int.Parse(_controllerActions.getUserId((ClaimsIdentity)User.Identity));

            Response<List<VariationMainPageDto>> response;
            try
            {
                var lang = Request.GetLangFromHeader();

                var tenantId = _storeRepository.GetTenantIdByStoreName(domain.Replace('_', '.'));
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);
                response = new Response<List<VariationMainPageDto>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _categoryRepository.LikeProducts(langNumber, lang, productGuid, userId)
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
    }
}