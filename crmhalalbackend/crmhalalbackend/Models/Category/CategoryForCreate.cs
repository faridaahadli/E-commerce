using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Category
{
    public class CategoryForCreate
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public CategoryForCreate ParentCategory { get; set; } = null;
    }
}