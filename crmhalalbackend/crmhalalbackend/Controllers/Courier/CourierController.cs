using CRMHalalBackEnd.App_Code;
using CRMHalalBackEnd.Filters;
using CRMHalalBackEnd.Models;
using CRMHalalBackEnd.Models.Courier;
using CRMHalalBackEnd.Repository;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Security.Claims;
using System.Web.Http;
using System.Web.WebPages.Scope;
using CRMHalalBackEnd.Helpers;

namespace CRMHalalBackEnd.Controllers.Courier
{
    public class CourierController : ApiController
    {

        private readonly UtilsClass _controllerActions = new UtilsClass();
        private readonly StoreRepository _storeRepository = new StoreRepository();
        private  readonly LanguagesRepository _langRepository = new LanguagesRepository();
        private readonly CourierRepository _courierRepository = new CourierRepository();

        #region Courier
        [HttpGet]
        [JwtRoleAuthentication(Actor = "Company", Permission = "96")]
        public IHttpActionResult GetRegionDelivery()
        {
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = int.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));


            Response<List<RegionDeliveryResponse>> response;
            try
            {
                var lang = Request.GetLangFromHeader();
                var langId = _langRepository.GetLangId(lang);

                var data = _courierRepository.GetRegionDelivery(langId, tenantId, userId);
                response = new Response<List<RegionDeliveryResponse>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Data ugurla gonderildi.",
                    Success = true,
                    Data = data
                };
            }

            catch (Exception ex)
            {
                response = new Response<List<RegionDeliveryResponse>>()
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
        [Route("note/api/Courier/GetRegionDeliveryForStore/{domain}")]
        [AllowAnonymous]
        public IHttpActionResult GetRegionDeliveryForStore(string domain)
        {
            string tenantId = _storeRepository.GetTenantIdByStoreName(domain.Replace('_', '.'));
            Response<List<RegionDeliveryResponse>> response;
            try
            {
                var lang = Request.GetLangFromHeader();

                var langId = _langRepository.GetLangId(lang);
                var langNumber = _langRepository.GetLangNumberForStore(lang, tenantId);

                var data = _courierRepository.GetRegionDeliveryForStore(langId,langNumber, tenantId, 0);
                response = new Response<List<RegionDeliveryResponse>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = data
                };
            }

            catch (Exception ex)
            {
                response = new Response<List<RegionDeliveryResponse>>()
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
        public IHttpActionResult GetRegions()
        {

            Response<List<RegionResponse>> response;
            try
            {
                var lang = Request.GetLangFromHeader();
                var langId = _langRepository.GetLangId(lang);

                response = new Response<List<RegionResponse>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _courierRepository.GetRegions(langId)
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<RegionResponse>>()
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "96")]
        public IHttpActionResult PostRegionDelivery(CourierInsDto delivery)
        {
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = int.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));

            Response<string> response;
            try
            {
                var data = _courierRepository.RegionDeliveryInsert(delivery, tenantId, userId);
                response = new Response<string>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Data ugurla gonderildi.",
                    Success = true,
                    Data = data
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
                response = new Response<string>()
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }

            return Ok(response);
        }

        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "96")]
        public IHttpActionResult PostStoreDeliveryInfo(CourierInsDto delivery)
        {
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = int.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));

            Response<string> response;
            try
            {
                string data = _courierRepository.StoreDeliveryInfoInsUpd(delivery, tenantId, userId);
                response = new Response<string>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Data ugurla gonderildi.",
                    Success = true,
                    Data = data
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
                response = new Response<string>()
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "77")]
        public IHttpActionResult GetAllRegions()
        {
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = Int32.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));

            Response<List<AllRegions>> response;
            try
            {
                var lang = Request.GetLangFromHeader();
                var langId = _langRepository.GetLangId(lang);

                response = new Response<List<AllRegions>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _courierRepository.GetAllRegions(langId, tenantId, userId)
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<AllRegions>>()
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "78")]
        public IHttpActionResult DeleteDelivery(int id)
        {
            int userId = Int32.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);

            Response<int> response;
            try
            {
                _courierRepository.DeleteDelivery(id, tenantId, userId);
                response = new Response<int>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Delivery ugurla slindi.",
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "96")]
        public IHttpActionResult DeleteDeliveryPricing(int id)
        {
            int userId = Int32.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);

            Response<int> response;
            try
            {
                int returnId = _courierRepository.DeleteDeliveryPricing(id, tenantId, userId);
                response = new Response<int>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Region qiymeti ugurla silindi.",
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
                    Code = (int)HttpStatusCode.NoContent,
                    Message = ex.Message,
                    Success = false
                };
            }
            return Ok(response);
        }

        #endregion

        [HttpPost]
        [JwtRoleAuthentication(Actor = "User")]
        public IHttpActionResult PostDeliveryPriceByStore(OrderDeliveryByRegion delivery)
        {
            Response<IEnumerable<OrderDeliveryByRegionResponse>> response;
            IEnumerable<OrderDeliveryByRegionResponse> datas = new List<OrderDeliveryByRegionResponse>();

            try
            {
                datas = _courierRepository.GetDeliveryPriceByStore(delivery);
                response = new Response<IEnumerable<OrderDeliveryByRegionResponse>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Qiymətlər uğurla gündərildi",
                    Success = true,
                    Data = datas
                };
            }
            catch (Exception ex)
            {
                response = new Response<IEnumerable<OrderDeliveryByRegionResponse>>()
                {
                    Code = (int)HttpStatusCode.NoContent,
                    Message = ex.Message,
                    Success = false,
                    Data = null
                };
            }
            return Ok(response);
        }
    }
}
