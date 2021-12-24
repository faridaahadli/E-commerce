using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Message
{
    public class MessageApiRequest
    {
        public List<string> MessageIds { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}