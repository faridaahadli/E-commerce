using CRMHalalBackEnd.Models.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Message
{
    public class AllEmailFront
    {
        public string Message { get; set; }
        public string Title { get; set; }
        public DateTime CreateDate { get; set; }
        public List<FileDto> File { get; set; }
        public List<UserEmails> UserEmail { get; set; }
    }
  
    public class UserEmails
    {
        public string UserName { get; set; }
        public string UserMail { get; set; }
    }
}