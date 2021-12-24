using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Category
{
    public class CategoryGetDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
    }
}