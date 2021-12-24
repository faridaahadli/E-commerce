using CRMHalalBackEnd.Models.Category;
using CRMHalalBackEnd.Models.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Promotion
{
    public class PromoProductResponse
    {
        public int PromoProductId { get; set; }
        public string ProductGuid { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public AllCategoriesForShop Category { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public FileDto Image { get; set; }
        public decimal Quantity { get; set; }
        public bool IsPromoProduct { get; set; }
    }
}