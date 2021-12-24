using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.SmsVerification
{
    public class SmsData
    {
        public string SendingUserGuid { get; set; }
        public string VerificationCode { get; set; }
        public string LastLoginIp { get; set; }
        public DateTime RequestDate { get; set; }
    }
}