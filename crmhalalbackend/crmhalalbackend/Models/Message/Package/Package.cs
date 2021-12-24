using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Message.Package
{
    public class Package
    {
        public decimal Amount { get; set; }
        public int MessageCount { get; set; }
        public bool FromNote { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}