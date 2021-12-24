using CRMHalalBackEnd.Models.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.NewCompany
{
    public class CompanyEmployeeInsDto
    {
        public string UserGuid { get; set; }
        public bool IsAdmin { get; set; }
        public List<CompanyEmployeeRole> RolesOfUser { get; set; }
        public List<PermissionDto>  Permissions { get; set; }
    }
}