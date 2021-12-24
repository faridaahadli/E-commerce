using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Security.Claims;
using System.Web.Http;
using CRMHalalBackEnd.App_Code;
using CRMHalalBackEnd.Filters;
using CRMHalalBackEnd.Helpers;
using CRMHalalBackEnd.Models;
using CRMHalalBackEnd.Models.Product;
using CRMHalalBackEnd.Repository;

namespace CRMHalalBackEnd.Controllers.Category
{
    public class CategoryProductController : ApiController
    {
        private readonly UtilsClass _controllerActions = new UtilsClass();
        private readonly LanguagesRepository _langRepository = new LanguagesRepository();
        private readonly ProductRepository _productRepository = new ProductRepository();

        [HttpGet]
        [JwtRoleAuthentication(Actor = "Company",Permission = "6")]
        public IHttpActionResult GetCategoryProduct(int categoryId)
        {

            Response<List<VariationDto>> response;
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            var userId = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            try
            {
                var lang = Request.GetLangFromHeader();
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);

                response = new Response<List<VariationDto>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _productRepository.GetAllCategoryProduct(langNumber, tenantId,  categoryId,int.Parse(userId))
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
        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company",Permission = "7")]
        public IHttpActionResult DeleteCategoryProduct(params string[] categoryIds)
        {

            Response<List<VariationDto>> response;
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            var userId = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            try
            {
                var repository = new ProductRepository();
                repository.DeleteCategoryProduct(tenantId, int.Parse(userId), categoryIds);
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
    }
}
