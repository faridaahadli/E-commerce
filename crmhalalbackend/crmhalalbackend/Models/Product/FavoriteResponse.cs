using CRMHalalBackEnd.Models.Category;
using CRMHalalBackEnd.Models.File;
using CRMHalalBackEnd.Models.Store;
using System;

namespace CRMHalalBackEnd.Models.Product
{
    public class FavoriteResponse
    {
        public int ProductId { get; set; }
        public string ProductGuid { get; set; } =String.Empty;
        public CategoryGetDto Category { get; set; } = new CategoryGetDto();
        public ProductUnit.ProductUnit MeasureType { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Description { get; set; }
        public FileDto Image { get; set; } = new FileDto();
        public decimal Price { get; set; }
        public string Slug { get; set; }
        public decimal Discount { get; set; }
        public StoreResponse Store { get; set; }
        public override string ToString()
        {
            return $"{nameof(ProductGuid)}: {ProductGuid},  {nameof(Name)}: {Name}, {nameof(Image)}: {Image}, {nameof(Price)}: {Price},{nameof(Discount)}: {Discount}";
        }

    }
}