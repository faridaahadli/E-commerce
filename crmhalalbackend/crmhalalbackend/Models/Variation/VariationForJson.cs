using System;
using System.Collections.Generic;
using CRMHalalBackEnd.Models.Attribute;
using CRMHalalBackEnd.Models.File;

namespace CRMHalalBackEnd.Models.Variation
{
    public class VariationForJson
    {
        public string Id { get; set; } = String.Empty;
        public decimal StockQuantity { get; set; }
        public decimal Discount { get; set; }
        public decimal Price { get; set; }
        public string  Barcode { get; set; }
        public string Sku { get; set; }
        public string Description { get; set; }
        public string Description2 { get; set; }
        public string Description3 { get; set; }
        public string Description4 { get; set; }
        public List<SelectedAttribute>Attributes { get; set; }
        public List<FileDto> Images { get; set; } = new List<FileDto>();
        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Attributes)}: {Attributes}, {nameof(Images)}: {Images}";
        }
    }
}