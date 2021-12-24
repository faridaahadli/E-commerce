using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Payment
{
    public class RefundResponse
    {
        public int RefundId { get; set; }
        public int OrderId { get; set; }
        public bool IsCourierCostInclude { get; set; }
        public decimal  Amount { get; set; }
        public DateTime  CreateDate { get; set; }
        public string  Reason { get; set; }
        public bool? IsConfirm { get; set; } = null;
        public List<RefundedItem>  RefundedItems { get; set; }

    }
}