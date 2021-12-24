using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.NewCompany
{
    public class CompanyCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<CompanyCategory> SubCategory { get; set; }

    }
}