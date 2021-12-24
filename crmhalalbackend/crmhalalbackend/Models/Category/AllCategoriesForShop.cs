using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Category
{
    public class AllCategoriesForShop
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Name2 { get; set; } = String.Empty;
        public string Name3 { get; set; } = String.Empty;
        public string Name4 { get; set; } = String.Empty;
        public bool IsVisible { get; set; }
        public List<AllCategoriesForShop> SubCategory { get; set; } = new List<AllCategoriesForShop>();

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}, {nameof(SubCategory)}: {SubCategory}";
        }
    }
}