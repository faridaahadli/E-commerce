using System.Collections.Generic;
using CRMHalalBackEnd.Models.File;
using CRMHalalBackEnd.Models.ProductCategory;
using CRMHalalBackEnd.Models.Seo;

namespace CRMHalalBackEnd.Models.Product
{
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Name2 { get; set; }
        public string Name3 { get; set; }
        public string Name4 { get; set; }
        public string YoutubeLink { get; set; }
        public int CategoryId { get; set; }
        public string ManufacturerId { get; set; }
        public string GroupId { get; set; }
        public int MeasureTypeId { get; set; }
        public decimal Weight { get; set; }
        public decimal GrossWeight { get; set; }
        public decimal Height { get; set; }
        public decimal Width { get; set; }
        public decimal Length { get; set; }
        public decimal Volume { get; set; }
        public List<Attribute.Attribute> Attributes { get; set; } = new List<Attribute.Attribute>();
        public List<Attribute.Attribute> DeletedAttributes { get; set; } = new List<Attribute.Attribute>();
        public List<Variation.Variation> Variations { get; set; } = new List<Variation.Variation>();
        public List<Variation.Variation> DeletedVariations { get; set; } = new List<Variation.Variation>();
        public List<FileDto> AllImages { get; set; } = new List<FileDto>();
        public SeoDto Seo { get; set; }
        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}, {nameof(YoutubeLink)}: {YoutubeLink}, {nameof(CategoryId)}: {CategoryId}, {nameof(ManufacturerId)}: {ManufacturerId}, {nameof(GroupId)}: {GroupId}, {nameof(MeasureTypeId)}: {MeasureTypeId}, {nameof(Weight)}: {Weight}, {nameof(GrossWeight)}: {GrossWeight}, {nameof(Height)}: {Height}, {nameof(Width)}: {Width}, {nameof(Length)}: {Length}, {nameof(Volume)}: {Volume}, {nameof(Attributes)}: {Attributes}, {nameof(Variations)}: {Variations}, {nameof(AllImages)}: {AllImages}, {nameof(Seo)}: {Seo}";
        }
    }
}