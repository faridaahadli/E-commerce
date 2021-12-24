using CRMHalalBackEnd.App_Code;
using CRMHalalBackEnd.Filters;
using CRMHalalBackEnd.Helpers;
using CRMHalalBackEnd.Models;
using CRMHalalBackEnd.Models.Order;
using CRMHalalBackEnd.Models.Payment;
using CRMHalalBackEnd.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Http;

namespace CRMHalalBackEnd.Controllers.OrderRefund
{
    public class RefundController : ApiController
    {
        private readonly RefundRepository _repository = new RefundRepository();
        private readonly LanguagesRepository _langRepository = new LanguagesRepository();
        private readonly StoreRepository _storeRepository = new StoreRepository();
        private readonly UtilsClass _controllerActions = new UtilsClass();


        //private const string MERCHANT_HANDLER = "https://testecomm.pashabank.az:18443/ecomm2/MerchantHandler?";
        //// PKCS#12 keystore with the Merchant's signed certificate
        //private readonly string X509_CERTIFICATE_FILE = System.Web.Hosting.HostingEnvironment.MapPath("~/certificate/certificate.p12");
        //// Keystore password
        //private const string CERTIFICATE_PASSWORD = "P@ssword";

        [HttpPost]
        [JwtRoleAuthentication(Actor = "User")]
        public IHttpActionResult RefundInsert(Refund refund)
        {
            int userId = int.Parse(_controllerActions.getDefaultUserId((ClaimsIdentity)User.Identity));
          
            Response<int> response = null;
            try
            {
                response = new Response<int>()
                {
                    Data = _repository.InsertRefund(userId, refund),
                    Code = (int)HttpStatusCode.OK,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                response = new Response<int>()
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Success = false,
                    Message = ex.Message
                };
            }
            return Ok(response);
        }




        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company",Permission = "117")]
        public IHttpActionResult RefundInsertByAdmin(Refund refund)
        {
            int userId = int.Parse(_controllerActions.getDefaultUserId((ClaimsIdentity)User.Identity));
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            Response<int> response = null;
            try
            {
                response = new Response<int>()
                {
                    Data = _repository.InsertRefundByAdmin(userId,tenantId,refund),
                    Code = (int)HttpStatusCode.OK,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                response = new Response<int>()
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Success = false,
                    Message = ex.Message
                };
            }
            return Ok(response);
        }



        [HttpGet]
        [JwtRoleAuthentication(Actor = "Company", Permission = "106")]
        public IHttpActionResult GetRefund(int orderId)
        {
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = int.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            Response<List<OrderLineInfo>> response;
            try
            {
                var lang = Request.GetLangFromHeader();
                var langId = _langRepository.GetLangId(lang);

                response = new Response<List<OrderLineInfo>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _repository.GetRefund(tenantId, userId, orderId,langId)
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<OrderLineInfo>>()
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "112")]
        public IHttpActionResult GetRefundForAdmin(int orderId)
        {
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = int.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            Response<OrderLineInfo> response;
            try
            {
                var lang = Request.GetLangFromHeader();
                var langId = _langRepository.GetLangId(lang);

                response = new Response<OrderLineInfo>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _repository.GetRefundForAdmin(tenantId, userId, orderId,langId)
                };
            }
            catch (Exception ex)
            {
                response = new Response<OrderLineInfo>()
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "112")]
        public IHttpActionResult GetRefundListForAdmin(int currentPage,int perPage)
        {
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = int.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            Response<List<RefundList>> response;
            try
            {

                response = new Response<List<RefundList>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _repository.GetRefundListForAdmin(tenantId,currentPage,perPage)
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<RefundList>>()
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
        [JwtRoleAuthentication(Actor = "Company", Permission = "112")]
        public IHttpActionResult GetRefundSize(int perPage)
        {
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = int.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            Response<int> response;
            try
            {

                response = new Response<int>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _repository.GetRefundSize(tenantId,perPage)
                };
            }
            catch (Exception ex)
            {
                response = new Response<int>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = 0
                };
            }

            return Ok(response);
        }



        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "117")]
        public IHttpActionResult ConfirmRefund(RefundConfirm refund)
        {
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            int userId = int.Parse(_controllerActions.getActiveUserId((ClaimsIdentity)User.Identity));
            Response<int> response;
            try
            {
     
                response = new Response<int>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _repository.ConfirmRefund(tenantId, userId,refund)
                };
            }
            catch (Exception ex)
            {
                response = new Response<int>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = 0
                };
            }

            return Ok(response);
        }


        #region


        //private string readResponse(Stream stream)
        //{
        //    string response = null;
        //    using (StreamReader reader = new StreamReader(stream))
        //    {
        //        do
        //        {
        //            response += Convert.ToChar(reader.Read());
        //        } while (reader.Peek() >= 0);
        //    }
        //    return response;
        //}

        //[HttpGet]
        //public IHttpActionResult Get(string transId)
        //{
        //    StringBuilder sb = new StringBuilder(MERCHANT_HANDLER)
        //        .Append("command=k")
        //        .Append("&trans_id=").Append(HttpUtility.UrlEncode(transId));


        //    // Calling the ECOMM module
        //    X509Certificate2Collection certificate = new X509Certificate2Collection();
        //    certificate.Import(X509_CERTIFICATE_FILE, CERTIFICATE_PASSWORD,
        //        X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);

        //    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        //    ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;
        //    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(sb.ToString());
        //    req.AllowAutoRedirect = true;
        //    req.ClientCertificates = certificate;
        //    req.Method = "POST";
        //    req.ContentType = "application/x-www-form-urlencoded";
        //    Stream postStream = req.GetRequestStream();
        //    HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

        //    Stream stream = resp.GetResponseStream();

        //    string response = readResponse(stream);
        //    stream.Close();

        //    return Ok();
        //}

        #endregion
    }
}
