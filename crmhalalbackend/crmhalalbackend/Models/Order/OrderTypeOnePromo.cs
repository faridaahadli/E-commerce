using CRMHalalBackEnd.Models.Basket;
using CRMHalalBackEnd.Models.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Order
{
    public class OrderTypeOnePromo
    {
        public List<PromotionBasketResponse> Promotions { get; set; }
        public StoreResponse Store { get; set; }
    }
}