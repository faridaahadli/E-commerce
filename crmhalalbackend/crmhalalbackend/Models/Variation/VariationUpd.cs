using System.Collections.Generic;
using CRMHalalBackEnd.Models.File;

namespace CRMHalalBackEnd.Models.Variation
{
    public class VariationUpd
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Description2 { get; set; }
        public string Description3 { get; set; }
        public string Description4 { get; set; }
        public string Sku { get; set; } = null;
        public string Barcode { get; set; } = null;
        public int? Stock { get; set; } = null;
        public bool? Status { get; set; } = null;
        public bool? ShowOnMain { get; set; } = null; 
        //public int? ShowPrice { get; set; } = null;
        public decimal? Price { get; set; } = null;
        public decimal? Discount { get; set; } = null;
        public List<FileDto> Images { get; set; } = null;
        public List<FileDto> AllImages { get; set; } = null;

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Sku)}: {Sku}, {nameof(Barcode)}: {Barcode}, {nameof(Stock)}: {Stock}, {nameof(Status)}: {Status}, {nameof(ShowOnMain)}: {ShowOnMain}, {nameof(Price)}: {Price}, {nameof(Discount)}: {Discount}, {nameof(Images)}: {Images}";
        }
    }
}