using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRMHalalBackEnd.Models.File;

namespace CRMHalalBackEnd.Models.Category
{
    public class AllCategoryForSelect
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public FileDto MenuSliderId { get; set; } = new FileDto();
        public string Color { get; set; } = String.Empty;
        public FileDto MenuIconId { get; set; } = new FileDto();
        public FileDto GridIconId { get; set; } = new FileDto();
        public string Slug { get; set; } = String.Empty;
        public List<AllCategoryForSelect> SubCategory { get; set; } = new List<AllCategoryForSelect>();
        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}, {nameof(MenuSliderId)}: {MenuSliderId}, {nameof(Color)}: {Color}, {nameof(MenuIconId)}: {MenuIconId}, {nameof(Slug)}: {Slug}, {nameof(SubCategory)}: {SubCategory}";
        }
    }
}