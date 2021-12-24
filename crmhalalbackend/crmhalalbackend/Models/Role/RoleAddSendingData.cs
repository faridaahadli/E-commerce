using CRMHalalBackEnd.Models.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Role
{
    public class RoleAddSendingData
    {
        public List<PermissionCategoryDto> PermissionCategories{ get; set; }
        public List<PermissionDto> Permissions { get; set; }
    }
}