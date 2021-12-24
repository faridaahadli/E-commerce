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
using CRMHalalBackEnd.Models.SpecialOffer;
using CRMHalalBackEnd.Repository;
using WebApi.Jwt.Filters;

namespace CRMHalalBackEnd.Controllers.Store
{
    [JwtCompanyAuthentication]
    public class SpecialOfferController : ApiController
    {
        private readonly UtilsClass _controllerActions = new UtilsClass();
        private readonly LanguagesRepository _langRepository = new LanguagesRepository();
        private readonly StoreRepository _storeRepository = new StoreRepository();
        private readonly SpecialOfferRepository _repository = new SpecialOfferRepository();

        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company",Permission = "26")]
        public IHttpActionResult Insert(SpecialOfferInsDto insDto)
        {
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = Int32.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            Response<List<SpecialOfferDto>> response;

            try
            {
                var lang = Request.GetLangFromHeader();
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);

                List<SpecialOfferDto> specialOfferDto = _repository.InsertSpecialOffer(insDto, langNumber, lang, tenantId, userId);
                response = new Response<List<SpecialOfferDto>>()
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Data = specialOfferDto
                };
            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<List<SpecialOfferDto>>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<List<SpecialOfferDto>>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception ex)
            {
                response = new Response<List<SpecialOfferDto>>()
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "27")]
        public IHttpActionResult DeleteSpecialOffer(int specialOfferId)
        {
            Response<List<SpecialOfferDto>> response;
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = int.Parse(_controllerActions.getActiveUserId((ClaimsIdentity) User.Identity));
            try
            {
                _repository.DeleteSpecialOffer(specialOfferId,tenantId, userId);
                response = new Response<List<SpecialOfferDto>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Message = "Silinme Ugurla Tamamlandi",
                    Data = null
                };
            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<List<SpecialOfferDto>>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<List<SpecialOfferDto>>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception ex)
            {
                response = new Response<List<SpecialOfferDto>>()
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "25")]
        public IHttpActionResult GetSpecialOffer(bool isDailyOffer)
        {
            Response<List<SpecialOfferDto>> response;
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            var userId = _controllerActions.getActiveUserId((ClaimsIdentity) User.Identity);
            try
            {
                var lang = Request.GetLangFromHeader();
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);

                List<SpecialOfferDto> specialOffer = _repository.GetAllSpecialOffer(langNumber, lang, isDailyOffer, tenantId,int.Parse(userId));
                response = new Response<List<SpecialOfferDto>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = specialOffer
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<SpecialOfferDto>>()
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "25")]
        public IHttpActionResult GetSpecialOfferByBeginAndEndDate(bool dailyOffer,DateTime beginDate,DateTime? endDate=null)
        {
            Response<List<SpecialOfferDto>> response;
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            string userId = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            string sBeginDate = beginDate.ToString("yyyy-MM-dd");
            string sEndDate = endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd"):"";
            try
            {
                var lang = Request.GetLangFromHeader();
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);

                List<SpecialOfferDto> specialOffer = _repository.GetAllSpecialOfferByBeginAndEndDate(langNumber, lang, tenantId, int.Parse(userId),dailyOffer, sBeginDate, sEndDate);
                response = new Response<List<SpecialOfferDto>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = specialOffer
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<SpecialOfferDto>>()
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
