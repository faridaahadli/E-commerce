using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Message
{
    public class GetUsers
    {
        public string UserName{get; set;}
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Guid { get; set; }
    }
}