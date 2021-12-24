using System.Collections.Generic;
using CRMHalalBackEnd.Models.Attribute;

namespace CRMHalalBackEnd.Models.Variation
{
    public class Variation
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Description2 { get; set; }
        public string Description3 { get; set; }
        public string Description4 { get; set; }
        public string Sku { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string Barcode { get; set; }
        public decimal Discount { get; set; }
        public bool ShowPrice { get; set; }
        public bool ShowOnMain { get; set; }
        public List<SelectedAttribute> SelectedAttributes { get; set; }
        public List<File.FileDto> Images { get; set; }
        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Sku)}: {Sku}, {nameof(Price)}: {Price}, {nameof(StockQuantity)}: {StockQuantity}, {nameof(Barcode)}: {Barcode}, {nameof(Discount)}: {Discount}, {nameof(ShowPrice)}: {ShowPrice}, {nameof(ShowOnMain)}: {ShowOnMain}, {nameof(SelectedAttributes)}: {SelectedAttributes}, {nameof(Images)}: {Images}";
        }
    }
}