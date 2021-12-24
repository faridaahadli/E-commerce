using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Message
{
    public class AllMessageFront
    {
        public int MessageId { get; set; }
        public string Message { get; set; }
        public DateTime CreateDate { get; set; }
        public string SendDate { get; set; }
        public string Status { get; set; }
        public List<User> Users { get; set; }
    }
    public class User
    {
        public string UserNumber { get; set; }
        public string UserName { get; set; }
    }
}