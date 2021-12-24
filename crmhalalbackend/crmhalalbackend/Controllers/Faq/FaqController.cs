using CRMHalalBackEnd.App_Code;
using CRMHalalBackEnd.Models;
using CRMHalalBackEnd.Models.Faq;
using CRMHalalBackEnd.repository;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Web.Http;
using CRMHalalBackEnd.Filters;
using System.Data.SqlClient;
using CRMHalalBackEnd.Helpers;
using CRMHalalBackEnd.Repository;

namespace CRMHalalBackEnd.Controllers.Faq
{
    public class FaqController : ApiController
    {
        private readonly UtilsClass _controllerActions = new UtilsClass();
        private readonly LanguagesRepository _langRepository = new LanguagesRepository();
        private readonly FaqRepository _faqRepository = new FaqRepository();
        private readonly StoreRepository _storeRepository = new StoreRepository();

        [HttpGet]
        [JwtRoleAuthentication(Actor = "Company", Permission = "34")]//+
        public IHttpActionResult GetAllFaq()
        {
            Response<List<AllFaq>> response;
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            var userId = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            try
            {
                var lang = Request.GetLangFromHeader();
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);

                response = new Response<List<AllFaq>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _faqRepository.AllFaq(langNumber, tenantId, int.Parse(userId))
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<AllFaq>>()
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "34")]//+
        public IHttpActionResult GetFaqById(int faqId)
        {
            Response<FaqDto> response;
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            try
            { 
                response = new Response<FaqDto>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _faqRepository.GetFaqById(faqId, tenantId)
                };
            }
            catch (Exception ex)
            {
                response = new Response<FaqDto>()
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
        [Route("note/api/Faq/GetAllFaqForStore/{domain}")]
        [AllowAnonymous]
        public IHttpActionResult GetAllFaqForStore(string domain)
        {
            Response<List<AllFaq>> response;
            try
            { 
                var lang = Request.GetLangFromHeader();

                domain = domain.Replace('_', '.');

                var tenantId = _storeRepository.GetTenantIdByStoreName(domain);
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);

                response = new Response<List<AllFaq>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _faqRepository.AllFaqForStore(langNumber, domain)
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<AllFaq>>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }

            return Ok(response);
        }

        //Actor="Company", Permission="36"
        [HttpGet]
        [JwtRoleAuthentication(Actor = "Company", Permission = "36")]
        public IHttpActionResult GetModList()
        {
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            var userId = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            Response<List<GetModForInsert>> response;

            try
            { 
                response = new Response<List<GetModForInsert>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _faqRepository.GetModList(tenantId, int.Parse(userId))
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<GetModForInsert>>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }

            return Ok(response);
        }

        //Actor="Company", Permission="36"
        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "36")]
        public IHttpActionResult InsertFaq(FaqDto faq)
        {
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            var userId = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            Response<int> response;

            try
            {
                var newFaq = _faqRepository.Insert(faq, tenantId, int.Parse(userId));
                response = new Response<int>
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Data = newFaq,
                    Message = "FAQ uğurla yaradıldı!"
                };
                return Ok(response);

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

        //Actor="Company", Permission="35"
        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "35")]
        public IHttpActionResult UpdateFaq(UpdateFaq faq)
        {
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            var userId = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            Response<int> response;
            
            try
            {
                var id = _faqRepository.FaqUpdate(tenantId, faq, int.Parse(userId));
                response = new Response<int>
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Data = id
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

        //Actor="Company", Permission="35"
        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "35")]
        public IHttpActionResult UpdateDragDrop(List<DragDrop> faq)
        {
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            var userId = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            Response<string> response;

            try
            {
                _faqRepository.FaqDargDropUpdate(tenantId, int.Parse(userId), faq);
                response = new Response<string>
                {
                    Code = (int)HttpStatusCode.Created,
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

        //Actor="Company", Permission="38"
        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "38")]
        public IHttpActionResult FaqDelete(int id)
        {
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            var userid = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            Response<object> response;

            try
            {
                _faqRepository.Delete(id, int.Parse(userid), tenantId);
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

    }
}
