using CRMHalalBackEnd.Models.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.NewCompany
{
    public class CompanyEmployeeUpdateDto
    {
        public int EmployeeId { get; set; }
        public string UserGuid { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsDeleted { get; set; } 
        public List<CompanyEmployeeRoleUpdate> RolesOfUser { get; set; }
        public List<PermissionEmployeeUpdateDto> Permissions { get; set; }
    }
}