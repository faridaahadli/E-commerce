using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Message.Package
{
    public class PackageHistory
    {
        public int CommonCount { get; set; }
        public int RemainderCount { get; set; }
        public List<Package> Packages { get; set; }
    }
}