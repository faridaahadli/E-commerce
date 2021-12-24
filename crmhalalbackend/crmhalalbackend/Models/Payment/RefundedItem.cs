using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Payment
{
    public class RefundedItem
    {
        public int ItemId { get; set; }
        public decimal Quantity { get; set; }
        public bool? IsConfirm { get; set; } = null;
        public string Reason { get; set; }
        public string RejectReason { get; set; }
        public decimal Amount { get; set; }
        public bool IsProduct { get; set; }

    }
}