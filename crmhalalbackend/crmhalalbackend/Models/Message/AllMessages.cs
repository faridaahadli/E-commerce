
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Message
{
    public class AllMessages
    {
        public int MessageId { get; set; }
        public string Message { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime SendDate { get; set; }
        public string UserNumber { get; set; }
        public string UserName { get; set; }
        public string Status { get; set; }
        public string MessageApiId { get; set; }
    }
}