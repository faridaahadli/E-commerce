using System.Collections.Generic;
using CRMHalalBackEnd.Models.Attribute;
using CRMHalalBackEnd.Models.File;

namespace CRMHalalBackEnd.Models.Variation
{
    public class VariationUpdDto
    {
        public string Id { get; set; }
        public string GroupId { get; set; }
        public string Description { get; set; }
        public string Description2 { get; set; }
        public string Description3 { get; set; }
        public string Description4 { get; set; }
        public string TenantId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Barcode { get; set; }
        public bool IsUpdating { get; set; }
        public bool ShowMainProduct { get; set; }
        public string Sku { get; set; }
        public int Stock { get; set; }
        public decimal Discount { get; set; }
        public bool ShowOnMain { get; set; }
        public bool ShowPrice { get; set; }
        public List<SelectedAttribute> Attributes { get; set; }
        public List<FileDto> Images { get; set; } = new List<FileDto>();
        public List<FileDto> AllImages { get; set; } = new List<FileDto>();
        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(GroupId)}: {GroupId}, {nameof(TenantId)}: {TenantId}, {nameof(Name)}: {Name}, {nameof(Price)}: {Price}, {nameof(Barcode)}: {Barcode}, {nameof(Sku)}: {Sku}, {nameof(Stock)}: {Stock}, {nameof(Discount)}: {Discount}, {nameof(ShowOnMain)}: {ShowOnMain}, {nameof(ShowPrice)}: {ShowPrice}, {nameof(Attributes)}: {Attributes}, {nameof(Images)}: {Images}, {nameof(AllImages)}: {AllImages}";
        }
    }
}