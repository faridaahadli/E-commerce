using CRMHalalBackEnd.Models.Seo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.BlogCategory
{
    public class BlogCategoryResponse
    {
        public int    Id { get; set; }
        public string Name { get; set; }
        public string Name2 { get; set; }
        public string Name3 { get; set; }
        public string Name4 { get; set; }
        public SeoDto Seo { get; set; }
    }
}