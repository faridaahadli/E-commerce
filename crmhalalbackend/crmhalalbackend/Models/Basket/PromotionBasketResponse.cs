using CRMHalalBackEnd.Models.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Basket
{
    public class PromotionBasketResponse
    {
        public int PromotionId { get; set; }
        public FileDto Image { get; set; }
        public string Description { get; set; }
        public List<PromotionBasketProduct> PromoProducts { get; set; }
    }
}