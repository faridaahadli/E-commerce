using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Category
{
    public class CategoryAttribute
    {
        public string Name { get; set; }
        public List<string> Values { get; set; }
    }
}