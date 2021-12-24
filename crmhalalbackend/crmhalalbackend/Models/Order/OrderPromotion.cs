using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Order
{
    public class OrderPromotion
    {
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public int PromotionId { get; set; }
        public decimal Amount { get; set; }
    }
}