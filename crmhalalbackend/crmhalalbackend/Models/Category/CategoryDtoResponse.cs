using CRMHalalBackEnd.Models.File;
using System;
using System.Collections.Generic;
using CRMHalalBackEnd.Models.Seo;

namespace CRMHalalBackEnd.Models.Category
{
    public class CategoryDtoResponse
    {
        public int Id { get; set; }
        public int? ParentCatId { get; set; }
        public string Name { get; set; }
        public string Name2 { get; set; } = String.Empty;
        public string Name3 { get; set; } = String.Empty;
        public string Name4 { get; set; } = String.Empty;

        public string ParentCatName { get; set; } = String.Empty;
        public string ParentCatName2 { get; set; } = String.Empty;
        public string ParentCatName3 { get; set; } = String.Empty;
        public string ParentCatName4 { get; set; } = String.Empty;

        public string ParentParentCatName { get; set; } = String.Empty;
        public string ParentParentCatName2 { get; set; } = String.Empty;
        public string ParentParentCatName3 { get; set; } = String.Empty;
        public string ParentParentCatName4 { get; set; } = String.Empty;
        public bool IsUpdating { get; set; }
        //  public string TenantId { get; set; }
        public FileDto MenuSliderId { get; set; }
        public string Color { get; set; }
        public FileDto MenuIconId { get; set; }
        public SeoDto Seo { get; set; }
        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(ParentCatId)}: {ParentCatId}, {nameof(Name)}: {Name}, {nameof(MenuSliderId)}: {MenuSliderId}, {nameof(Color)}: {Color}, {nameof(MenuIconId)}: {MenuIconId}, {nameof(Seo)}: {Seo}";
        }
    }
}