using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.SmsVerification
{
    public class SmsVerificationInfo
    {
        public string UserGuid { get; set; }
        public string VerificationCode { get; set; }
        public bool IsVerified { get; set; } = false;
        public DateTime VerifiedDate { get; set; }
        public DateTime SendDate { get; set; }
        public DateTime ExpiredDate { get; set; }
    }
}