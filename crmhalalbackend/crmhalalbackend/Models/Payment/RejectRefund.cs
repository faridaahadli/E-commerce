using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Payment
{
    public class RejectRefund
    {
        public int RefundDataId { get; set; }
        public string ReasonOfReject { get; set; }
        public DateTime RejectDate { get; set; }
    }
}