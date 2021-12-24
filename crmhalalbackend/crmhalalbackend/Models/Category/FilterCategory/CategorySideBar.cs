
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRMHalalBackEnd.Models.Seo;

namespace CRMHalalBackEnd.Models.Category
{
    public class CategorySideBar
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Slug { get; set; } = String.Empty;
        public List<CategoryAttribute> Attributes { get; set; } = new List<CategoryAttribute>();
        public List<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
        public decimal MaxPrice { get; set; }
        public SeoDto Seo { get; set; } = new SeoDto();

    }
}