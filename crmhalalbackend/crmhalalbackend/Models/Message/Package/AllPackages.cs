using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Message.Package
{
    public class AllPackages
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
    }
}