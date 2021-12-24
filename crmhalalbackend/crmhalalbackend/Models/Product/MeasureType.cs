using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Product
{
    public class MeasureType
    {
        public int Id { get; set; }
        public string UnitCode { get; set; }
        public string Name { get; set; }
        public bool IsDecimal { get; set; }
    }
}