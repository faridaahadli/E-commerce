using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRMHalalBackEnd.Models.File;
using CRMHalalBackEnd.Models.Seo;

namespace CRMHalalBackEnd.Models.Category
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public int? ParentCatId { get; set; }
        public string Name { get; set; }
        public string Name2 { get; set; }
        public string Name3 { get; set; }
        public string Name4 { get; set; }
        //  public string TenantId { get; set; }
        public FileDto MenuSliderId { get; set; }
        public string Color { get; set; }
        public FileDto MenuIconId { get; set; }
        public FileDto GridIconId { get; set; }
        public SeoDto Seo { get; set; }
        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(ParentCatId)}: {ParentCatId}, {nameof(Name)}: {Name}, {nameof(MenuSliderId)}: {MenuSliderId}, {nameof(Color)}: {Color}, {nameof(MenuIconId)}: {MenuIconId}, {nameof(Seo)}: {Seo}";
        }
    }
}