using CRMHalalBackEnd.Helpers;
using CRMHalalBackEnd.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace CRMHalalBackEnd.Controllers.Payment
{
    public class MessagePacketPaymentController : ApiController
    {
        [HttpPost]
        [AllowAnonymous]
        public IHttpActionResult CompleteForSmsPacket()
        {
            string transId = HttpContext.Current.Request.Params["trans_id"];

            IPaymentProvider provider = FactoryPaymentProvider.Build("smspacket");
            var redirectUrl = provider.Complete(transId);



            return Redirect(redirectUrl);
        }

    }
}
