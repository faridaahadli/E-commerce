using CRMHalalBackEnd.Models.Address;
using CRMHalalBackEnd.Models.Contact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.SiteSettings
{
    public class ContactResponse
    {
        public int StoreId { get; set; }
        public List<NewContact> Phone { get; set; }
        public List<NewContact> Email { get; set; }
        public string WorkStartTime { get; set; }
        public string WorkEndTime { get; set; }
        public NewAddress Adress { get; set; }

    }
}