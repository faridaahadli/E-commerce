using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Basket
{
    public class NewBasket
    {
        public string Id { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public decimal Discount { get; set; }
        public string TenantId { get; set; }
        public bool IsBasket { get; set; }
        public bool IsLoggedIn { get; set; }

    }
}