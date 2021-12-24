using System;
using System.Collections.Generic;
using CRMHalalBackEnd.Models.Category;
using CRMHalalBackEnd.Models.File;
using CRMHalalBackEnd.Models.Store;

namespace CRMHalalBackEnd.Models.Variation
{
    public class VariationMainPageDto
    {
        public int ProductId { get; set; }
        public string ProductGuid { get; set; } = String.Empty;
        public StoreResponse Store { get; set; } = new StoreResponse();
        public AllCategoriesForShop Category { get; set; } = new AllCategoriesForShop();
        public bool IsFavorite { get; set; }
        public string Name { get; set; } = string.Empty;
        public int  Index { get; set; }
        public int StockQuantity { get; set; }
        public ProductUnit.ProductUnit MeasureType { get; set; }
        public string Description { get; set; }
        public decimal Rating { get; set; }
        public decimal Price { get; set; }
        public decimal Discount{ get; set; }
        public string Slug { get; set; } = string.Empty;
        public FileDto Image { get; set; } = new FileDto();
        public override string ToString()
        {
            return $"{nameof(ProductGuid)}: {ProductGuid}, {nameof(Store)}: {Store}, {nameof(Category)}: {Category}, {nameof(Name)}: {Name}, {nameof(Price)}: {Price}, {nameof(Discount)}: {Discount}, {nameof(Slug)}: {Slug}, {nameof(Image)}: {Image}, {nameof(IsFavorite)}: {IsFavorite}";
        }
    }
}