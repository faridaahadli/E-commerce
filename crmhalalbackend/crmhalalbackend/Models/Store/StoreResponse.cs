using CRMHalalBackEnd.Models.Address;
using CRMHalalBackEnd.Models.Contact;
using CRMHalalBackEnd.Models.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRMHalalBackEnd.Models.Seo;

namespace CRMHalalBackEnd.Models.Store
{
    public class StoreResponse
    {
        public string StoreGuid { get; set; } = String.Empty;
        public string Name { get; set; } = String.Empty;
        public List<FileDto> LogoImage { get; set; } = new List<FileDto>();
        public FileDto Watermark { get; set; } = new FileDto();
        public FileDto Favicon { get; set; } = new FileDto();
        public FileDto MainLogoImgId { get; set; } = new FileDto();
        public string TenantId { get; set; } = String.Empty;
        public int  ProductCount { get; set; }
        public string WorkStartTime { get; set; } = String.Empty;
        public string WorkEndTime { get; set; } = String.Empty;
        public bool ShowSpecial { get; set; }
        public string Slug { get; set; } = String.Empty;
        public string Domain { get; set; }
        public List<AddressResponse> Addresses { get; set; } = new List<AddressResponse>();
        public List<ContactResponse> Contacts { get; set; } = new List<ContactResponse>();
        public SeoDto Seo { get; set; } = new SeoDto();
        public bool Status { get; set; }
        public bool IsSales { get; set; }
        public override string ToString()
        {
            return $"{nameof(StoreGuid)}: {StoreGuid}, {nameof(Name)}: {Name}, {nameof(LogoImage)}: {LogoImage}, {nameof(TenantId)}: {TenantId}, {nameof(ProductCount)}: {ProductCount}, {nameof(WorkStartTime)}: {WorkStartTime}, {nameof(WorkEndTime)}: {WorkEndTime}, {nameof(ShowSpecial)}: {ShowSpecial}, {nameof(Slug)}: {Slug}, {nameof(Addresses)}: {Addresses}, {nameof(Contacts)}: {Contacts}, {nameof(Seo)}: {Seo}";
        }
    }
}