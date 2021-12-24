using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Contact
{
    public class ContactResponse
    {
        public int ContactId { get; set; }
        public string Text { get; set; } = String.Empty;
        public string Note { get; set; } = String.Empty;
        public int ContactTypeId { get; set; }
    }
}