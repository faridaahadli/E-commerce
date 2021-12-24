using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Message
{
    public class CheckMessageApiResponse
    {
        public int StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public List<Result> Result { get; set; }

    }
    public class Result
    {
        public string MessageId { get; set; }
        public string Receiver { get; set; }
        public int SmsStatus { get; set; }
        public string SmsStatusDescription { get; set; }
        public string IsFinalStatus { get; set; }
        public string StatusTime { get; set; }
        public string SmsCharge { get; set; }

    }
}