using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Message
{
    public class InsertMessage
    {
        public List<Provider> Provider { get; set; }
        public string Message { get; set; }
        public string Title { get; set; }
        public bool IsBodyHtml { get; set; } = false;
        public string To { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }
        public List<UserNums> UserNumbers { get; set; }
        public List<Files> Files { get; set; }
        public DateTime? SendDate { get; set; } = null;
        public int CountSms { get; set; }
    }

    public class Provider
    {
        public int ProviderTypeId { get; set; }
    }

    public class UserNums
    {
        public string UserNumber { get; set; }
    }
    public class Files
    {
        public int FileId { get; set; }
    }
}