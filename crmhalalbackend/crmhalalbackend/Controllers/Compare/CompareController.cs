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
using CRMHalalBackEnd.Repository;

namespace CRMHalalBackEnd.Controllers.Compare
{
    public class CompareController : ApiController
    {

        private readonly UtilsClass _controllerActions = new UtilsClass();
        private readonly LanguagesRepository _langRepository =  new LanguagesRepository();
        private readonly StoreRepository _storeRepository = new StoreRepository();
        private readonly CompareRepository _compareRepository = new CompareRepository();

        [HttpPost]
        [Route("note/api/Compare/CompareInsert/{domain}")]
        [JwtRoleAuthentication(Actor = "User")]
        public IHttpActionResult CompareInsert(string domain,string productGuid)
        {
            string tenantId = _storeRepository.GetTenantIdByStoreName(domain.Replace('_', '.'));
            int userId = Int32.Parse(_controllerActions.getUserId((ClaimsIdentity)User.Identity));

            Response<string> response;
            try
            {
                var compares = _compareRepository.Insert(productGuid, tenantId, userId);

                response = new Response<string>
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Data = "Ugurla yaradildi"

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
        [Route("note/api/Compare/GetAllCompare/{domain}")]
        [JwtRoleAuthentication(Actor = "User")]
        public IHttpActionResult GetAllCompare(string domain)
        {
            string tenantId = _storeRepository.GetTenantIdByStoreName(domain.Replace('_','.'));
            int userId = int.Parse(_controllerActions.getUserId((ClaimsIdentity)User.Identity));


            Response<List<Models.CompareProduct.Compare>> response;

            try
            {
                var lang = Request.GetLangFromHeader();
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);

                response = new Response<List<Models.CompareProduct.Compare>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _compareRepository.GetAllCompare(langNumber, lang, tenantId, userId)
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<Models.CompareProduct.Compare>>()
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
        [Route("note/api/Compare/GetCompareProductById/{domain}")]
        [AllowAnonymous]
        public IHttpActionResult GetCompareProductById(string domain, string guid)
        {
            Response<Models.CompareProduct.Compare> response;

            try
            {
                var lang = Request.GetLangFromHeader();

                var tenantId = _storeRepository.GetTenantIdByStoreName(domain.Replace('_', '.'));
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);

                response = new Response<Models.CompareProduct.Compare>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _compareRepository.GetProductCompareById(langNumber, lang, guid)
                };
            }
            catch (Exception ex)
            {
                response = new Response<Models.CompareProduct.Compare>()
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
        [Route("note/api/Compare/SearchProduct/{domain}")]
        [AllowAnonymous]
        public IHttpActionResult SearchProduct(string domain, string key)
        {

            string tenantId = _storeRepository.GetTenantIdByStoreName(domain.Replace('_', '.'));
            //int userId = Int32.Parse(_controllerActions.getUserId((ClaimsIdentity)User.Identity));
            Response<List<Models.CompareProduct.Compare>> response;
            try
            {
                var lang = Request.GetLangFromHeader();
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);

                response = new Response<List<Models.CompareProduct.Compare>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _compareRepository.SearchProduct(langNumber, key, tenantId)
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<Models.CompareProduct.Compare>>()
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
        [JwtRoleAuthentication(Actor = "User")]
        public IHttpActionResult DeleteCompare(string compareGuid, string domain)
        {
            var domainFinal = domain.Replace('_', '.');
            Response<int> response;
            int userId = Int32.Parse(_controllerActions.getUserId((ClaimsIdentity)User.Identity));
            var storeRepo = new StoreRepository();
            string tenantId = storeRepo.GetTenantIdByStoreName(domainFinal);
            try
            {
                var compareRepo = new CompareRepository();
                int returnId = compareRepo.DeleteCompare(compareGuid, tenantId, userId);
                response = new Response<int>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Müqayisə məhsulu ugurla silindi.",
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
                    Success = false,
                    Message = ex.Message
                };
            }
            return Ok(response);
        }

    }
}
