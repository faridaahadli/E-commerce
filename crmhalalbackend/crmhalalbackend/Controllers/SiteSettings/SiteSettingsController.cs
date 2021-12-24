using CRMHalalBackEnd.App_Code;
using CRMHalalBackEnd.Filters;
using CRMHalalBackEnd.Models;
using CRMHalalBackEnd.Models.SiteSettings;
using CRMHalalBackEnd.Repository;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using CRMHalalBackEnd.Helpers;
using WebApi.Jwt.Filters;

namespace CRMHalalBackEnd.Controllers.SiteSettings
{
    public class SiteSettingsController : ApiController
    {
        private readonly UtilsClass _controllerActions = new UtilsClass();
        private readonly SiteSettingsRepository _siteSettingsRepository = new SiteSettingsRepository();
        private readonly LanguagesRepository _languagesRepository = new LanguagesRepository();
        private readonly StoreRepository _storeRepository = new StoreRepository();

        #region PrivacyPolicy

        //Actor="Company", Permission="31"
        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "31")]
        public IHttpActionResult InsertPrivacyPolicy(PrivacyPolicyAndAbout privacyPolicy)
        {
            var userid = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);

            Response<PrivacyPolicyAndAbout> response;
            try
            {

                var result = _siteSettingsRepository.InsertPrivacyPolicy(privacyPolicy, tenantId, userid);
                response = new Response<PrivacyPolicyAndAbout>
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Message = "Dəyişiklik uğurla tamamlandı",
                    Data = result
                };

            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<PrivacyPolicyAndAbout>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<PrivacyPolicyAndAbout>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception ex)
            {
                response = new Response<PrivacyPolicyAndAbout>
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            return Ok(response);
        }

        //Actor="Company", Permission="28"
        [HttpGet]
        [JwtRoleAuthentication(Actor = "Company", Permission = "28")]
        public IHttpActionResult GetPrivacyPolicy()
        {
            Response<PrivacyPolicyResponse> response;
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = Int32.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            try
            {
                response = new Response<PrivacyPolicyResponse>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _siteSettingsRepository.GetPrivacyPolicy(tenantId, userId)
                };
            }
            catch (Exception ex)
            {
                response = new Response<PrivacyPolicyResponse>()
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
        [Route("note/api/SiteSettings/GetPrivacyPolicyForShop/{domain}")]
        [AllowAnonymous]
        public IHttpActionResult GetPrivacyPolicyForShop(string domain)
        {
            Response<PrivacyPolicyResponse> response;
           
            try
            {
                var lang = Request.GetLangFromHeader();

                var tenantId = _storeRepository.GetTenantIdByStoreName(domain.Replace('_', '.'));
                var langNumber = _languagesRepository.GetLangNumberForStore(lang, tenantId);

                response = new Response<PrivacyPolicyResponse>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _siteSettingsRepository.GetPrivacyPolicyForShop(langNumber, domain.Replace('_', '.'))
                };
            }
            catch (Exception ex)
            {
                response = new Response<PrivacyPolicyResponse>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }

            return Ok(response);
        }
        #endregion PrivacyPolicy

        #region About

        //Actor="Company", Permission="33"
        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "33")]
        public IHttpActionResult InsertAbout(PrivacyPolicyAndAbout about)
        {
            var userid = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            Response<PrivacyPolicyAndAbout> response;
            try
            {
                var result = _siteSettingsRepository.InsertAbout(about, tenantId, userid);
                response = new Response<PrivacyPolicyAndAbout>
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Message = "Dəyişiklik uğurla tamamlandı",
                    Data = result
                };

            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<PrivacyPolicyAndAbout>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<PrivacyPolicyAndAbout>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception ex)
            {
                response = new Response<PrivacyPolicyAndAbout>
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            return Ok(response);
        }

        //Actor="Company", Permission="32"
        [HttpGet]
        [JwtRoleAuthentication(Actor = "Company", Permission = "32")]
        public IHttpActionResult GetAbout()
        {
            Response<PrivacyPolicyResponse> response;
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = Int32.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));

            try
            {
                response = new Response<PrivacyPolicyResponse>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _siteSettingsRepository.GetAbout(tenantId, userId)
                };
            }
            catch (Exception ex)
            {
                response = new Response<PrivacyPolicyResponse>()
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
        [Route("note/api/SiteSettings/GetAboutForShop/{domain}")]
        [AllowAnonymous]
        public IHttpActionResult GetAboutForShop(string domain)
        {
            Response<PrivacyPolicyResponse> response;
         
            try
            {
                var lang = Request.GetLangFromHeader();

                var tenantId = _storeRepository.GetTenantIdByStoreName(domain.Replace('_', '.'));
                var langNumber = _languagesRepository.GetLangNumberForStore(lang, tenantId);

                response = new Response<PrivacyPolicyResponse>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _siteSettingsRepository.GetAboutForShop(langNumber, domain.Replace('_', '.'))
                };
            }
            catch (Exception ex)
            {
                response = new Response<PrivacyPolicyResponse>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }

            return Ok(response);
        }
        #endregion About

        #region SosialMedia

        //Actor="Company", Permission="46"
        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "46")]
        public IHttpActionResult InsertSocialMedia(SosialMedia model)
        {
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            var userid = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            Response<List<SosialMediaResponse>> response;

            try
            {
                var newMedia = _siteSettingsRepository.InsertSosialMedia(model, tenantId, userid);
                response = new Response<List<SosialMediaResponse>>
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Data = newMedia,
                    Message = "Sosial Media uğurla yaradıldı!"
                };
            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<List<SosialMediaResponse>>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<List<SosialMediaResponse>>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception e)
            {
                response = new Response<List<SosialMediaResponse>>
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = "Sosial Media yaradila bilmedi!"

                };
            }

            return Ok(response);
        }

        //Actor="Company", Permission="47"
        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "47")]
        public IHttpActionResult DeleteSosialMedia(int id)
        {
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            var userid = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            Response<object> response;
            try
            {
                _siteSettingsRepository.DeleteSosialMedia(id, tenantId, int.Parse(userid));
                response = new Response<object>()
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                };
            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<object>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<object>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception e)
            {
                response = new Response<object>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = e.Message,
                    Data = null
                };
            }

            return Ok(response);
        }

        //Actor="Company", Permission="44"
        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "44")]
        public IHttpActionResult UpdateSosialMedia(UpdateSosialMedia sosialMedia)
        {
            var userid = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            Response<List<SosialMediaResponse>> response;
            tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            try
            {
                var model = _siteSettingsRepository.UpdateSosialMedia(sosialMedia, tenantId, int.Parse(userid));
                response = new Response<List<SosialMediaResponse>>
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Data = model

                };
            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<List<SosialMediaResponse>>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<List<SosialMediaResponse>>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception ex)
            {
                response = new Response<List<SosialMediaResponse>>
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }

            return Ok(response);
        }


        //Actor="Company", Permission="41"
        [HttpGet]
        [JwtRoleAuthentication(Actor = "Company", Permission = "41")]
        public IHttpActionResult GetAllSosialMedia()
        {
            Response<List<SosialMediaResponse>> response;
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = Int32.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));

            try
            {
                response = new Response<List<SosialMediaResponse>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _siteSettingsRepository.GetSosialMedia(tenantId,userId)
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<SosialMediaResponse>>()
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
        public IHttpActionResult GetSosialMediaForShop(string domain)
        {
            Response<List<SosialMediaResponse>> response;
           
            try
            {
                response = new Response<List<SosialMediaResponse>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _siteSettingsRepository.GetSosialMediaForShop(domain)
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<SosialMediaResponse>>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }

            return Ok(response);
        }
        #endregion SosialMedia

        #region Footer

        //Actor="Company", Permission="49"
        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "49")]
        public IHttpActionResult InsertFooter(FooterSettings footer)
        {
            var userid = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);

            Response<List<FooterSettingsResponse>> response;
            try
            {

                var result = _siteSettingsRepository.InsertFooter(footer, tenantId, userid);
                response = new Response<List<FooterSettingsResponse>>
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Message = "Dəyişiklik uğurla tamamlandı",
                    Data = result
                };

            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<List<FooterSettingsResponse>>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<List<FooterSettingsResponse>>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception ex)
            {
                response = new Response<List<FooterSettingsResponse>>
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            return Ok(response);
        }

        //Actor="Company", Permission="45"
        [HttpGet]
        [JwtRoleAuthentication(Actor = "Company", Permission = "45")]
        public IHttpActionResult GetAllFooter()
        {
            Response<List<FooterSettingsResponse>> response;
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = Int32.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));

            try
            {
                response = new Response<List<FooterSettingsResponse>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _siteSettingsRepository.GetFooter(tenantId,userId)
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<FooterSettingsResponse>>()
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
        [Route("note/api/SiteSettings/GetFooterForShop/{domain}")]
        [AllowAnonymous]
        public IHttpActionResult GetFooterForShop(string domain)
        {
            Response<List<FooterResponseForShop>> response;
           
            try
            {
                var lang = Request.GetLangFromHeader();

                var tenantId = _storeRepository.GetTenantIdByStoreName(domain.Replace('_', '.'));
                var langNumber = _languagesRepository.GetLangNumberForStore(lang, tenantId);

                response = new Response<List<FooterResponseForShop>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _siteSettingsRepository.GetFooterForShop(langNumber, domain.Replace('_', '.'))
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<FooterResponseForShop>>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }

            return Ok(response);
        }

        //Actor="Company", Permission="48"
        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "48")]
        public IHttpActionResult UpdateFooter(UpdateFooter footer)
        {
            var userid = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            Response<List<FooterSettingsResponse>> response;
            tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            try
            {
                var model = _siteSettingsRepository.UpdateFooter(footer, tenantId, int.Parse(userid));
                response = new Response<List<FooterSettingsResponse>>
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Data = model

                };
            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<List<FooterSettingsResponse>>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<List<FooterSettingsResponse>>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception ex)
            {
                response = new Response<List<FooterSettingsResponse>>
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }

            return Ok(response);
        }

        //Actor="Company", Permission="45"
        [HttpGet]
        [JwtRoleAuthentication(Actor = "Company", Permission = "45")]
        public IHttpActionResult GetParentFooter()
        {
            Response<List<ParentFoooter>> response;
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = Int32.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));

            try
            {
                response = new Response<List<ParentFoooter>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _siteSettingsRepository.GetParentFooter(tenantId,userId)
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<ParentFoooter>>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }

            return Ok(response);
        }

        //Actor="Company", Permission="50"
        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "50")]
        public IHttpActionResult DeleteFooter(int id)
        {
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            var userid = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            Response<object> response;
            try
            {
                _siteSettingsRepository.DeleteFooter(id, tenantId, int.Parse(userid));
                response = new Response<object>()
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                };
            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<object>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<object>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception e)
            {
                response = new Response<object>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = e.Message,
                    Data = null
                };
            }

            return Ok(response);
        }

        #endregion Footer

        #region Contact

        //Actor="Company", Permission="53"
        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "53")]
        public IHttpActionResult InsertContact(HeaderInsert header)
        {
            var userid = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            Response<List<HeaderRespoonse>> response;

            
            try
            {
                var newcontact = _siteSettingsRepository.InsertHeader(header, tenantId, int.Parse(userid));
                response = new Response<List<HeaderRespoonse>>
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Data = newcontact,
                    Message = "Bizimlə əlaqə uğurla yaradıldı!"
                };
            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<List<HeaderRespoonse>>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<List<HeaderRespoonse>>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception e)
            {
                response = new Response<List<HeaderRespoonse>>
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = "Bizimlə əlaqə yaradila bilmedi!"

                };
            }

            return Ok(response);
        }

        //Actor="Company", Permission="51"
        [HttpGet]
        [JwtRoleAuthentication(Actor = "Company", Permission = "51")]
        public IHttpActionResult GetContact()
        {
            Response<List<HeaderRespoonse>> response;
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = Int32.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));

            try
            {
                response = new Response<List<HeaderRespoonse>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _siteSettingsRepository.GetContactByTeanantId(tenantId,userId)
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<HeaderRespoonse>>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }

            return Ok(response);
        }

        //Actor="Company", Permission="51"
        [HttpGet]
        [JwtRoleAuthentication(Actor = "Company", Permission = "51")]
        public IHttpActionResult GetAllContact()
        {
            Response<List<GetAllHeader>> response;
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = Int32.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));

            try
            {
                response = new Response<List<GetAllHeader>>()
                {
                    Code = (int) HttpStatusCode.OK,
                    Success = true,
                    Data = _siteSettingsRepository.GetAllContactByTenantId(tenantId,userId)
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<GetAllHeader>>()
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
        public IHttpActionResult GetAllContactForShop(string domain)
        {
            Response<List<GetContactsForShop>> response;
            try
            {
                response = new Response<List<GetContactsForShop>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _siteSettingsRepository.GetAllContactForShop(domain)
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<GetContactsForShop>>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }

            return Ok(response);
        }

        //Actor="Company", Permission="54"
        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "54")]
        public IHttpActionResult DeleteContact(int id)
        {
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            var userid = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            Response<object> response;
            try
            {
                _siteSettingsRepository.DeleteContact(id, tenantId, int.Parse(userid));
                response = new Response<object>()
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                };
            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<object>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<object>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception e)
            {
                response = new Response<object>()
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
        [AllowAnonymous]
        public IHttpActionResult GetContactForShop(string domain)
        {
            Response<List<ContactResponse>> response;
            
            try
            {
                response = new Response<List<ContactResponse>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _siteSettingsRepository.GetContactForShop(domain)
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<ContactResponse>>()
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "53")]
        public IHttpActionResult UpdateWhatsapp(int contactId)
        {
            var userid = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            Response<int> response;
                     try
            {
                var model = _siteSettingsRepository.UpdateWhatsapp(int.Parse(userid), tenantId,contactId);
                response = new Response<int>
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Data = model

                };
            }
            catch (Exception ex)
            {
                response = new Response<int>
                {
                    Code = (int)HttpStatusCode.Conflict,
                    Success = false,
                    Message = ex.Message,
                    Data = 0
                };
            }

            return Ok(response);
        }
        #endregion Contact

        #region TermsAndConditions

        //Actor="Company", Permission="64"
        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "64")]
        public IHttpActionResult InsertTermsConditions(PrivacyPolicyAndAbout termsCond)
        {
            var userid = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);

            Response<int> response;
            try
            {

                var result = _siteSettingsRepository.InsertTermsConditions(termsCond, tenantId, userid);
                response = new Response<int>
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Message = "Dəyişiklik uğurla tamamlandı",
                    Data = result
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


        //Actor="Company", Permission="63"
        [HttpGet]
        [JwtRoleAuthentication(Actor = "Company", Permission = "63")]
        public IHttpActionResult GetTermsConditions()
        {
            Response<PrivacyPolicyResponse> response;
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = Int32.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));

            try
            {
                response = new Response<PrivacyPolicyResponse>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _siteSettingsRepository.GetTermsAndConditions(tenantId,userId)
                };
            }
            catch (Exception ex)
            {
                response = new Response<PrivacyPolicyResponse>()
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
        [Route("note/api/SiteSettings/GetTermsForShop/{domain}")]
        [AllowAnonymous]
        public IHttpActionResult GetTermsForShop(string domain)
        {
            Response<PrivacyPolicyResponse> response;
            try
            {
                var lang = Request.GetLangFromHeader();

                var tenantId = _storeRepository.GetTenantIdByStoreName(domain.Replace('_', '.'));
                var langNumber = _languagesRepository.GetLangNumberForStore(lang, tenantId);

                response = new Response<PrivacyPolicyResponse>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _siteSettingsRepository.GetTermsForShop(langNumber, domain.Replace('_', '.'))
                };
            }
            catch (Exception ex)
            {
                response = new Response<PrivacyPolicyResponse>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }

            return Ok(response);
        }
        #endregion TermsAndConditions

    }
}
