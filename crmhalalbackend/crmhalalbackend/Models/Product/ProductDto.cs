using System;
using System.Collections.Generic;
using CRMHalalBackEnd.Models.Category;
using CRMHalalBackEnd.Models.File;
using CRMHalalBackEnd.Models.Seo;
using CRMHalalBackEnd.Models.Variation;

namespace CRMHalalBackEnd.Models.Product
{
    public class ProductDto
    {
        public string GroupId { get; set; } = String.Empty;
        public string Name { get; set; } = String.Empty;
        public string Name2 { get; set; } = String.Empty;
        public string Name3 { get; set; } = String.Empty;
        public string Name4 { get; set; } = String.Empty;
        public string YoutubeLink { get; set; } = String.Empty;
        public NewCategory Category { get; set; } = new NewCategory();
        public string Manufacturer { get; set; } = String.Empty;
        public int MeasureType { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public bool IsUpdating { get; set; }
        public bool IsDeleting { get; set; }
        public decimal Length { get; set; }
        public decimal Weight { get; set; }
        public int ProductUpdateTypes { get; set; }
        public List<Attribute.Attribute> Attributes { get; set; } = new List<Attribute.Attribute>();
        public List<VariationForJson> Variations { get; set; } = new List<VariationForJson>();
        public List<FileDto> DefaultImages { get; set; } = new List<FileDto>();
        public List<FileDto> AllImages { get; set; } = new List<FileDto>();
        public SeoDto Seo { get; set; } = new SeoDto();
        public override string ToString()
        {
            return $"{nameof(GroupId)}: {GroupId}, {nameof(Name)}: {Name}, {nameof(YoutubeLink)}: {YoutubeLink}, {nameof(Category)}: {Category}, {nameof(Manufacturer)}: {Manufacturer}, {nameof(MeasureType)}: {MeasureType}, {nameof(Width)}: {Width}, {nameof(Height)}: {Height}, {nameof(Length)}: {Length}, {nameof(Weight)}: {Weight}, {nameof(Attributes)}: {Attributes}, {nameof(Variations)}: {Variations}, {nameof(DefaultImages)}: {DefaultImages}, {nameof(AllImages)}: {AllImages}, {nameof(Seo)}: {Seo}";
        }
    }
}