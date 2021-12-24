using CRMHalalBackEnd.App_Code;
using CRMHalalBackEnd.Models;
using CRMHalalBackEnd.Models.Product;
using CRMHalalBackEnd.Repository;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Web.Http;
using CRMHalalBackEnd.Filters;
using System.Data.SqlClient;
using CRMHalalBackEnd.Helpers;

namespace CRMHalalBackEnd.Controllers.Product
{
    [JwtRoleAuthentication(Actor = "User")]
    public class ProductsController : ApiController
    {
        private readonly UtilsClass _controllerActions = new UtilsClass();
        private readonly LanguagesRepository _langRepository = new LanguagesRepository();
        private readonly StoreRepository _storeRepository = new StoreRepository();
        private readonly ProductRepository _productRepository = new ProductRepository();

        [HttpPost]
        public IHttpActionResult ProductComment(ProductComment productComment)
        {
            int userId = Int32.Parse(_controllerActions.getUserId((ClaimsIdentity)User.Identity));
            Response<ProductComment> response;
            try
            {
                var comment = _productRepository.ProductComment(productComment, userId);

                response = new Response<ProductComment>
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Data = comment

                };
            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<ProductComment>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<ProductComment>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception e)
            {
                response = new Response<ProductComment>
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
        public IHttpActionResult GetComment(string groupId)
        {
            int userId = Int32.Parse(_controllerActions.getUserId((ClaimsIdentity)User.Identity));
            Response<List<CommentResponse>> response;

            try
            {
                response = new Response<List<CommentResponse>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _productRepository.GetProductComment(userId,groupId)
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<CommentResponse>>()
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
        public IHttpActionResult AddFavorite(FavoriteDto favorite)
        {
            int userId = Int32.Parse(_controllerActions.getUserId((ClaimsIdentity)User.Identity));
            Response<FavoriteDto> response;
            try
            {
                var comment = _productRepository.AddFavorite(favorite, userId);

                response = new Response<FavoriteDto>
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Data = favorite

                };
            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<FavoriteDto>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<FavoriteDto>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception e)
            {
                response = new Response<FavoriteDto>
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
        public IHttpActionResult DeleteFavorite(string productGuid)
        {
            Response<int> response;
            int userId = Int32.Parse(_controllerActions.getUserId((ClaimsIdentity)User.Identity));
            FavoriteDto favorite = null;
            try
            {
                _productRepository.DeleteFavoriteProduct(productGuid, userId);
                response = new Response<int>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Məhsul seçilmişlər siyahısından çıxarıldı.",
                    Success = true,
                    Data=0
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
                    Code = (int)HttpStatusCode.NoContent,
                    Message = ex.Message,
                    Success = false
                };
            }
            return Ok(response);
        }
        [HttpPost]
        public IHttpActionResult DeleteAllFavorite()
        {
            Response<int> response;
            int userId = Int32.Parse(_controllerActions.getUserId((ClaimsIdentity)User.Identity));
            FavoriteDto favorite = null;
            try
            {
                _productRepository.DeleteAllFavoriteProduct(userId);
                response = new Response<int>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Məhsul seçilmişlər siyahısından çıxarıldı.",
                    Success = true,
                    Data = 0
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
        [Route("note/api/Products/GetFavoriteProduct/{domain}")]
        public IHttpActionResult GetFavoriteProduct(string domain)
        {
            int userId = Int32.Parse(_controllerActions.getUserId((ClaimsIdentity)User.Identity));
            Response<List<FavoriteResponse>> response;

            try
            {
                var lang = Request.GetLangFromHeader();

                var tenantId = _storeRepository.GetTenantIdByStoreName(domain.Replace('_','.'));
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);

                response = new Response<List<FavoriteResponse>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _productRepository.GetFavoriteProduct(langNumber, lang, userId)
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<FavoriteResponse>>()
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
