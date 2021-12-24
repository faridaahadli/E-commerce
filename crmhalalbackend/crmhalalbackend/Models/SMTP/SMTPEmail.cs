using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.SMTP
{
    public class SMTPEmail
    {
        public string StoreEmail { get; set; }
        public string Password { get; set; }
        public string UserEmail { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public int SmtpTypeId { get; set; }
    }
    public class SmtpType
    {
        public int Port { get; set; }
        public string Host { get; set; }
    }

}