using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Courier
{
    public class DeliveryDto
    {
        public int DeliveryTypeId { get; set; }
        public decimal?  DeliveryPrice { get; set; }
        public int DeliveryPricingId { get; set; }
    }
}