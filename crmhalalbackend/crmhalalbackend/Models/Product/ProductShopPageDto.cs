using System;
using System.Collections.Generic;
using CRMHalalBackEnd.Models.Category;
using CRMHalalBackEnd.Models.File;
using CRMHalalBackEnd.Models.Seo;
using CRMHalalBackEnd.Models.Store;
using CRMHalalBackEnd.Models.Variation;

namespace CRMHalalBackEnd.Models.Product
{
    public class ProductShopPageDto
    {
        public string GroupId { get; set; } = String.Empty;
        public StoreResponse Store { get; set; } = new StoreResponse();
        public string YoutubeLink { get; set; } = String.Empty;
        public int CategoryId { get; set; }
        public List<NewCategory> Categories { get; set; }
        public string Manufacturer { get; set; } = String.Empty;
        public ProductUnit.ProductUnit MeasureType { get; set; } = new ProductUnit.ProductUnit();
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Length { get; set; }
        public decimal Weight { get; set; }
        public List<Attribute.Attribute> Attributes { get; set; } = new List<Attribute.Attribute>();
        public List<VariationShopPageDto> Variations { get; set; } = new List<VariationShopPageDto>();
        public SeoDto Seo { get; set; } = new SeoDto();
        public override string ToString()
        {
            return $"{nameof(GroupId)}: {GroupId}, {nameof(Store)}: {Store}, {nameof(YoutubeLink)}: {YoutubeLink}, {nameof(Manufacturer)}: {Manufacturer}, {nameof(MeasureType)}: {MeasureType}, {nameof(Width)}: {Width}, {nameof(Height)}: {Height}, {nameof(Length)}: {Length}, {nameof(Weight)}: {Weight}, {nameof(Attributes)}: {Attributes}, {nameof(Variations)}: {Variations}, {nameof(Seo)}: {Seo}";
        }
    }
}