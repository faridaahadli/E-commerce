using System;
using System.Collections.Generic;

namespace CRMHalalBackEnd.Models.Category
{
    public class CategoryForProductPageDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Name2 { get; set; } = String.Empty;
        public string Name3 { get; set; } = String.Empty;
        public string Name4 { get; set; } = String.Empty;
        public bool IsVisible { get; set; }
        public List<CategoryForProductPageDto> SubCategory { get; set; } = new List<CategoryForProductPageDto>();

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name},{nameof(SubCategory)}: {SubCategory}";
        }
    }
}