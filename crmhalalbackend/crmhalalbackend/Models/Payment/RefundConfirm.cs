using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Payment
{
    public class RefundConfirm
    {
        public int RefundId { get; set; }
        public int PaymentTypeId { get; set; }
        public decimal TotalAmount { get; set; }
        public string ReasonOfReject { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsCourierCostInclude { get; set; }
        public DateTime ConfirmDate { get; set; }
        public List<RejectRefund> RejectedRefunds { get; set; }
    }
}