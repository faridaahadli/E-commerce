using CRMHalalBackEnd.Models.File;

namespace CRMHalalBackEnd.Models.Product
{
    public class VariationDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        public bool IsDeleting { get; set; }
        public string Barcode { get; set; }
        public decimal? Price { get; set; } = null;
        public decimal? Discount { get; set; } = null;
        public int Stock { get; set; }
        public bool Status { get; set; }
        public FileDto Image { get; set; }
        public bool ShowOnMain { get; set; }
        public bool ShowPrice { get; set; }
        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}, {nameof(Sku)}: {Sku}, {nameof(Barcode)}: {Barcode}, {nameof(Price)}: {Price}, {nameof(Discount)}: {Discount}, {nameof(Stock)}: {Stock}, {nameof(Status)}: {Status}, {nameof(ShowOnMain)}: {ShowOnMain}, {nameof(ShowPrice)}: {ShowPrice}";
        }
    }
}