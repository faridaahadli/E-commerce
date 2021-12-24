using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Message.Package
{
    public class InsertPackage
    {
        public bool IsNote { get; set; }
        public int CommonMessageCount { get; set; }
        public decimal Amount { get; set; }
    }
}