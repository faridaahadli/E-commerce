using System;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using CRMHalalBackEnd.App_Code;
using CRMHalalBackEnd.Filters;
using CRMHalalBackEnd.Helpers;
using CRMHalalBackEnd.Interfaces;
using CRMHalalBackEnd.Models;
using CRMHalalBackEnd.Models.Payment;
using CRMHalalBackEnd.Repository;

namespace CRMHalalBackEnd.Controllers.Payment
{
    public class PaymentController : ApiController
    {
        private readonly PaymentRepository _repository = new PaymentRepository();
        private readonly StoreRepository _storeRepository = new StoreRepository();
        private readonly UtilsClass _controllerActions = new UtilsClass();
        private readonly LanguagesRepository _languagesRepository = new LanguagesRepository();


        [HttpPost]
        [JwtRoleAuthentication]
        [Route("note/api/Payment/{domain}/Post")]
        public IHttpActionResult Post(string domain, int orderId)
        {

            Response<string> response;
            int userId = int.Parse(_controllerActions.getDefaultUserId((ClaimsIdentity)User.Identity));

            try
            {
                domain = domain.Replace('_', '.');

                string tenantId = _storeRepository.GetTenantIdByStoreName(domain);
                int paymentId = _repository.InsertPayment(domain, userId, orderId);
                var lang = Request.GetLangFromHeader();
                IPaymentProvider provider = FactoryPaymentProvider.Build("pashabank");
                string returnUrl;
                if (paymentId != 0)
                {
                    returnUrl = provider.GetPaymentPageUrl(lang, paymentId, userId, tenantId);
                }
                else
                {
                    returnUrl = "https://"+domain + "/payment-status?orderId=" + orderId;
                }
                response = new Response<string>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = returnUrl
                };
            }
            catch (Exception ex)
            {
                response = new Response<string>()
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
        [AllowAnonymous]
        public IHttpActionResult Complete()
        {
            string trans_id = HttpContext.Current.Request.Params["trans_id"];

            IPaymentProvider provider = FactoryPaymentProvider.Build("pashabank");
            string redirectUrl = provider.Complete(trans_id);



            return Redirect(redirectUrl);
        }

        [HttpGet]
        [JwtRoleAuthentication]
        public IHttpActionResult PaymentDetails(int orderId)
        {

            int userId = int.Parse(_controllerActions.getDefaultUserId((ClaimsIdentity)User.Identity));
            Response<PaymentResponse> response = null;
            try
            {
                var lang = Request.GetLangFromHeader();
                var langId = _languagesRepository.GetLangId(lang);
                response = new Response<PaymentResponse>()
                {
                    Data = _repository.GetPaymentDetailsByOrderId(langId,orderId),
                    Code = (int)HttpStatusCode.OK,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                response = new Response<PaymentResponse>()
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Success = false,
                    Message = ex.Message
                };
            }
            return Ok(response);
        }

        //[HttpPost]
        //[JwtRoleAuthentication(Actor = "Company")]
        //[Route("note/api/Payment/{domain}/PostCash")]
        //public IHttpActionResult PostCash(string domain, int orderId, int paymentTypeId)
        //{

        //    int userId = int.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
        //    string tenantId = _controllerActions.getTenantId((ClaimsIdentity) User.Identity);
        //    Response<int> response = null;
        //    try
        //    {
        //        response = new Response<int>()
        //        {
        //            Data = _repository.InsertPaymentCash(tenantId,userId,orderId,paymentTypeId),
        //            Code = (int)HttpStatusCode.OK,
        //            Success = true
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        response = new Response<int>()
        //        {
        //            Code = (int)HttpStatusCode.NotFound,
        //            Success = false,
        //            Message = ex.Message
        //        };
        //    }
        //    return Ok(response);

        //}

    }
}
