using CRMHalalBackEnd.Models.Address;
using CRMHalalBackEnd.Models.Contact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.SiteSettings
{
    public class GetContactsForShop
    {
        public int StoreId { get; set; }
        public List<NewContact> Phone { get; set; }
        public List<NewContact> Email { get; set; }
        public DateTime WorkStartTime { get; set; }
        public DateTime WorkEndTime { get; set; }
        public NewAddress Addresses { get; set; }
    }
}