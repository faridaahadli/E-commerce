using System;
using CRMHalalBackEnd.Models.Category;
using CRMHalalBackEnd.Models.File;
using CRMHalalBackEnd.Models.Store;

namespace CRMHalalBackEnd.Models.SpecialOffer
{
    public class SpecialOfferDto
    {
        public int ProductId { get; set; }
        public int Id { get; set; }
        public bool IsFavorite { get; set; }
        public StoreResponse Store { get; set; } = new StoreResponse();
        public string ProductGuid { get; set; } = String.Empty;
        public AllCategoriesForShop Category { get; set; }
        public ProductUnit.ProductUnit MeasureType { get; set; } = new ProductUnit.ProductUnit();
        public string Name { get; set; } = String.Empty;
        public string Slug { get; set; } = String.Empty;
        public bool DailyOffer { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        private string _beginDate;
        private string _endDate;
        public DateTime BeginDate
        {
            get => DateTime.Parse(_beginDate);
            set => _beginDate = value.ToString("yyyy-MM-dd");
        }

        public DateTime EndDate
        {
            get => DateTime.Parse(_endDate);
            set => _endDate = value.ToString("yyyy-MM-dd");
        }

        public FileDto Image { get; set; } = new FileDto();

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Store)}: {Store}, {nameof(ProductGuid)}: {ProductGuid}, {nameof(Category)}: {Category}, {nameof(Name)}: {Name}, {nameof(Slug)}: {Slug}, {nameof(DailyOffer)}: {DailyOffer}, {nameof(Price)}: {Price}, {nameof(Discount)}: {Discount}, {nameof(BeginDate)}: {BeginDate}, {nameof(EndDate)}: {EndDate}, {nameof(Image)}: {Image}";
        }
    }
}