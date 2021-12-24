using CRMHalalBackEnd.App_Code;
using CRMHalalBackEnd.Filters;
using CRMHalalBackEnd.Models;
using CRMHalalBackEnd.Models.BlogCategory;
using CRMHalalBackEnd.Repository;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace CRMHalalBackEnd.Controllers.BlogCategory
{
    public class BlogCategoryController : ApiController
    {
        private readonly UtilsClass _controllerActions = new UtilsClass();
        private readonly BlogCategoryRepository _repository = new BlogCategoryRepository();
        

        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company")]
        public IHttpActionResult Post(BlogCategoryInsert category)
        {
            Response<int> response;
            int userId = int.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            try
            {
                var categoryId = _repository.Insert(category,userId,tenantId);
                response = new Response<int>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Kateqoriya yaradıldı",
                    Success = true,
                    Data = categoryId
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
        [JwtRoleAuthentication(Actor = "Company")]
        public IHttpActionResult Update(BlogCategoryUpd category)
        {
            Response<int> response;
            int userId = int.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            try
            {
                var categoryId = _repository.Update(category, userId, tenantId);
                response = new Response<int>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Kateqoriya duzeldildi",
                    Success = true,
                    Data = categoryId
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
        [JwtRoleAuthentication(Actor = "Company")]
        public IHttpActionResult BlogCategoryDelete(int categoryId)
        {
            Response<int> response;
            int userId = int.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            try
            {
              _repository.Delete(categoryId, userId, tenantId);
                response = new Response<int>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Kateqoriya silindi",
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
                    Success = false,
                    Data = 0
                };
            }
            return Ok(response);
        }


        [HttpGet]
        [JwtRoleAuthentication(Actor = "Company")]
        public IHttpActionResult BlogCategoriesGet()
        {
            Response<IEnumerable<BlogCategoryResponse>> response;
            IEnumerable<BlogCategoryResponse> categories = new List<BlogCategoryResponse>();
           
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            try
            {
                categories = _repository.GetBlogCategories(tenantId);
                response = new Response<IEnumerable<BlogCategoryResponse>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Kateqoriyalar gonderildi",
                    Success = true,
                    Data = categories
                };
            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<IEnumerable<BlogCategoryResponse>>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<IEnumerable<BlogCategoryResponse>>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception ex)
            {
                response = new Response<IEnumerable<BlogCategoryResponse>>()
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
        public IHttpActionResult BlogCategoryGet(int categoryId)
        {
            Response<BlogCategoryResponse> response;
            BlogCategoryResponse category = new BlogCategoryResponse();

            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            try
            {
                category = _repository.GetBlogCategory(tenantId,categoryId);
                response = new Response<BlogCategoryResponse>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Kateqoriyalar gonderildi",
                    Success = true,
                    Data = category
                };
            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<BlogCategoryResponse>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<BlogCategoryResponse>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception ex)
            {
                response = new Response<BlogCategoryResponse>()
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
