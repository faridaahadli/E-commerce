using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRMHalalBackEnd.Models.File;

namespace CRMHalalBackEnd.Models.Promotion
{
    public class PromoProductInsDto
    {
        public int PromoProductId { get; set; }
        public string ProductGuid { get; set; }
        public decimal Quantity { get; set; }
        public bool IsPromoProduct { get; set; }
        public override string ToString()
        {
            return $"{nameof(PromoProductId)}: {PromoProductId}, {nameof(ProductGuid)}: {ProductGuid}, {nameof(Quantity)}: {Quantity}, {nameof(IsPromoProduct)}: {IsPromoProduct}";
        }
    }
}