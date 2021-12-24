using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Basket
{
    public class PromotionBasketInsDto
    {
        public int PromotionId { get; set; }
        public decimal Quantity { get; set; }
        public bool  IsLoggedIn { get; set; }
    }
}