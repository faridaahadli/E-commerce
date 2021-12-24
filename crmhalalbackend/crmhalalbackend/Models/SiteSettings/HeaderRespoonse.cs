using CRMHalalBackEnd.Models.Address;
using CRMHalalBackEnd.Models.Contact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.SiteSettings
{
    public class HeaderRespoonse
    {
        public int StoreId { get; set; }
        public NewContact Contacts { get; set; }
        public string WorkStartTime { get; set; }
        public string WorkEndTime { get; set; }
        public NewAddress Addresses { get; set; }
    }
}
