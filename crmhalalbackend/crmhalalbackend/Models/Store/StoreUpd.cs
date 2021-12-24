using CRMHalalBackEnd.Models.Address;
using CRMHalalBackEnd.Models.Contact;
using System.Collections.Generic;
using CRMHalalBackEnd.Models.Seo;

namespace CRMHalalBackEnd.Models.Store
{
    public class StoreUpd
    {
        public string TenantId { get; set; }
        public string Name { get; set; }
        public int LogoImgId { get; set; }
        public int? MainLogoImgId { get; set; }
        public int? Watermark { get; set; }
        public int? Favicon { get; set; }
        public string OwnerGuid { get; set; }
        public string WorkStartTime { get; set; }
        public string WorkEndTime { get; set; }
        public List<NewAddress> Addresses { get; set; }
        public List<NewContact> Contacts { get; set; }
        public SeoDto Seo { get; set; }
    }
}