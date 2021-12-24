using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Message
{
    public class UserMailInfo
    {
        public int ProviderTypeId { get; set; }
        public string UserEmail { get; set; }
        public string Password { get; set; }
    }
}