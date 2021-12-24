using CRMHalalBackEnd.Models.Address;
using CRMHalalBackEnd.Models.Contact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.SiteSettings
{
    public class Contact
    {
        public List<AddressResponse> Addresses { get; set; }
        public List<NewContact> Contacts { get; set; }
        public string WorkStartTime { get; set; }
        public string WorkEndTime { get; set; }

    }
}