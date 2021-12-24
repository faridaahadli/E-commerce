using CRMHalalBackEnd.Models.Seo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.ExcelImport
{
    public class AllDataFront
    {
        public string GroupId { get; set; }
        public List<Attribute.Attribute> Attributes { get; set; }
        public SeoDto Seo { get; set; }

        public List<AllData> GroupData { get; set; }
    }
   
}