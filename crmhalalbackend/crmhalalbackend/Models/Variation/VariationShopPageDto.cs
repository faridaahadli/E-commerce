using System;
using System.Collections.Generic;
using CRMHalalBackEnd.Models.Attribute;
using CRMHalalBackEnd.Models.File;

namespace CRMHalalBackEnd.Models.Variation
{
    public class VariationShopPageDto
    {
        public int ProductId { get; set; }
        public string Guid { get; set; } = String.Empty;
        public string Name { get; set; } = String.Empty;
        public string Slug { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public string Sku { get; set; } = String.Empty;
        public string Barcode { get; set; } = String.Empty;
        public int Stock { get; set; }
        public List<AttributeDto> Attributes { get; set; } = new List<AttributeDto>();
        public List<FileDto> Images { get; set; } = new List<FileDto>();
        public override string ToString()
        {
            return $"{nameof(Guid)}: {Guid}, {nameof(Name)}: {Name}, {nameof(Slug)}: {Slug}, {nameof(Price)}: {Price}, {nameof(Discount)}: {Discount}, {nameof(Sku)}: {Sku}, {nameof(Barcode)}: {Barcode}, {nameof(Stock)}: {Stock}, {nameof(Attributes)}: {Attributes}, {nameof(Images)}: {Images}";
        }
    }
}