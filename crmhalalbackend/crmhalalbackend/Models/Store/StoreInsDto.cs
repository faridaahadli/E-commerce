using CRMHalalBackEnd.Models.Address;
using CRMHalalBackEnd.Models.Contact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRMHalalBackEnd.Models.Seo;

namespace CRMHalalBackEnd.Models.Store
{
    public class StoreInsDto
    {
        public string Name{ get; set; }
        public int? LogoImgId{ get; set; }
        public int? Watermark { get; set; }
        public int? Favicon { get; set; }
        public int RefundLimit { get; set; } = 14;
        public string OwnerGuid { get; set; }
        public string WorkStartTime { get; set; }
        public string WorkEndTime { get; set; }
        public string Domain { get; set; }
        public string SelectedPackage { get; set; }
        public List<NewAddress> Addresses { get; set; }
        public List<NewContact> Contacts { get; set; }
        public SeoDto Seo { get; set; }

    }
}