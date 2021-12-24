using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Category
{
    public class CategoryFilter
    {
        public int Id { get; set; }
        public int? ParentCatId { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<ParentCategory> ParentCategory { get; set; } = null;
    }
}