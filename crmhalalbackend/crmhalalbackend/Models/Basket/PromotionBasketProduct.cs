using CRMHalalBackEnd.Models.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Basket
{
    public class PromotionBasketProduct
    {
        public int ProductId { get; set; }
        public string ProductGuid { get; set; }
        public string ProductName { get; set; } = String.Empty;
        public FileDto Image { get; set; }
        public ProductUnit.ProductUnit MeasureType { get; set; }
        public int StockQuantity { get; set; }
        public bool IsPromoProduct { get; set; }
    }
}