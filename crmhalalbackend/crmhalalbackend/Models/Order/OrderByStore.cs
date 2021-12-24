using CRMHalalBackEnd.Models.Courier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Order
{
    public class OrderByStore
    {

        public string TenantId { get; set; }
        public decimal StorePrice{ get; set; }
        public int PaymentTypeId { get; set; }
        public IEnumerable<OrderProduct> OrderProducts { get; set; }
        public IEnumerable<OrderPromotion> OrderPromotions { get; set; }
        public DeliveryDto DeliveryData { get; set; }

    }
}