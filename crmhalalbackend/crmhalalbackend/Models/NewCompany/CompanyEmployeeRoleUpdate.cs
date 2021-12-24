using CRMHalalBackEnd.Models.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.NewCompany
{
    public class CompanyEmployeeRoleUpdate
    {
        public int RoleId { get; set; }
        public List<PermissionDto> RolePermissions { get; set; }
        public bool IsDeleted { get; set; }
    }
}