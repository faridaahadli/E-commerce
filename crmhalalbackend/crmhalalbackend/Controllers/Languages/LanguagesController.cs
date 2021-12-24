using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Security.Claims;
using System.Web.Http;
using CRMHalalBackEnd.App_Code;
using CRMHalalBackEnd.Filters;
using CRMHalalBackEnd.Models;
using CRMHalalBackEnd.Models.Languages;
using CRMHalalBackEnd.Repository;

namespace CRMHalalBackEnd.Controllers.Languages
{
    public class LanguagesController : ApiController
    {

        private readonly UtilsClass _controllerActions = new UtilsClass();
        private readonly LanguagesRepository _repository = new LanguagesRepository();
        private readonly StoreRepository _repoStore = new StoreRepository();

        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult GetLanguages()
        {
            Response<IEnumerable<LanguagesDto>> response = null;
            try
            {
                IEnumerable<LanguagesDto> languages = _repository.GetAllLanguage();
                response = new Response<IEnumerable<LanguagesDto>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = languages
                };
            }
            catch (Exception ex)
            {
                response = new Response<IEnumerable<LanguagesDto>>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message
                };
            }

            return Ok(response);
        }


        [HttpGet]
        [JwtRoleAuthentication(Actor = "Company",Permission = "110")]
        public IHttpActionResult GetLanguagesByStore()
        {
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity) User.Identity);
            int userId = int.Parse(_controllerActions.getActiveUserId((ClaimsIdentity) User.Identity));
            Response<IEnumerable<StoreLanguageDto>> response = null;
            try
            {
                IEnumerable<StoreLanguageDto> languageIds = _repository.GetLanguageByTenant(tenantId,userId);
                response = new Response<IEnumerable<StoreLanguageDto>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = languageIds
                };
            }
            catch (Exception ex)
            {
                response = new Response<IEnumerable<StoreLanguageDto>>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message
                };
            }

            return Ok(response);
        }

        [HttpGet]
        [Route("note/api/Languages/GetLanguagesShopPage/{domain}")]
        [AllowAnonymous]
        public IHttpActionResult GetLanguagesShopPage(string domain)
        {
            string tenantId = _repoStore.GetTenantIdByStoreName(domain.Replace('_', '.'));
            Response<IEnumerable<StoreLanguageDto>> response = null;
            try
            {
                IEnumerable<StoreLanguageDto> languageIds = _repository.GetLanguageByTenant(tenantId, 0);
                response = new Response<IEnumerable<StoreLanguageDto>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = languageIds
                };
            }
            catch (Exception ex)
            {
                response = new Response<IEnumerable<StoreLanguageDto>>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message
                };
            }

            return Ok(response);
        }


        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "108")]
        public IHttpActionResult AddLanguage(int languageId,int transNumId, int line)
        {
            var userId = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            Response<int> response;
            try
            {
                var storeLangId = _repository.InsertLanguage(languageId, transNumId, line, tenantId, int.Parse(userId));

                response = new Response<int>
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Data = storeLangId
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
            catch (Exception e)
            {
                response = new Response<int>
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = e.Message
                };
            }

            return Ok(response);
        }

        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "109")]
        public IHttpActionResult DeleteLanguage(int storeLanguageId, int defaultLanguageId = 0)
        {
            var userId = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            Response<string> response;
            try
            {
                _repository.LanguageDelete(storeLanguageId, defaultLanguageId, tenantId, int.Parse(userId));

                response = new Response<string>
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Dil uğurla silindi",
                    Success = true
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
                    Message = e.Message
                };
            }

            return Ok(response);
        }

        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "108")]
        public IHttpActionResult LanguageStatusChange(int storeLanguageId)
        {
            var userId = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            Response<string> response;
            try
            {
                _repository.LanguageStatusChange(storeLanguageId, tenantId, int.Parse(userId));

                response = new Response<string>
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Əsas dil uğurla dəyişdirildi!",
                    Success = true
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
                    Message = e.Message
                };
            }

            return Ok(response);
        }


    }
}
