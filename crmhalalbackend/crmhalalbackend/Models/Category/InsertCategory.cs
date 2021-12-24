using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Category
{
    public class InsertCategory
    {
        public int Id { get; set; }
        public int? ParentCatId { get; set; }
        public string Name { get; set; }
        //  public string TenantId { get; set; }
        public int? SliderIconId { get; set; }
        public string Color { get; set; }
        public int? MenuIconId { get; set; }
        //    public bool Status { get; set; }
        public string Slug { get; set; }
        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(ParentCatId)}: {ParentCatId}, {nameof(Name)}: {Name}, {nameof(SliderIconId)}: {SliderIconId},  {nameof(Slug)}: {Slug},{nameof(MenuIconId)}: {MenuIconId}";
        }
    }
}
