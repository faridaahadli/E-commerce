using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Courier
{
    public class OrderDeliveryByRegionResponse
    {
        public string TenantId { get; set; }
        public string StoreName { get; set; }
        public int DeliveryPricingId { get; set; }
        public decimal DeliveryPrice { get; set; }
    }
}