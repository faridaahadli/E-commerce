using CRMHalalBackEnd.Models.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRMHalalBackEnd.Models.Seo;

namespace CRMHalalBackEnd.Models.Category
{
    public class AllCategories
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string ParentCatName { get; set; } = String.Empty;
        public string ParentParentCatName { get; set; } = String.Empty;
        public bool IsDeleting { get; set; }
        public bool IsShowProduct { get; set; }
        public int ProductCount { get; set; }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id},  {nameof(Name)}: {Name}, {nameof(ParentCatName)}: {ParentCatName}, {nameof(ParentParentCatName)}: {ParentParentCatName}";
        }
    }
}