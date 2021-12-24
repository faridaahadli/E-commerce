using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Payment
{
    public class Refund
    {
        public int OrderId { get; set; }
        public string  TenantId { get; set; }
        public int PaymentTypeId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DeliveryPrice { get; set; }
        public bool? IsCourierCostInclude { get; set; }
        public bool RefundByAdmin { get; set; }
        public decimal Amount { get; set; }
        public DateTime ConfirmDate { get; set; }
        public List<RefundedItem>  RefundedItems { get; set; }
       
    }
}