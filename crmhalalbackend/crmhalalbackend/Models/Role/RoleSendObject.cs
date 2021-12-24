using CRMHalalBackEnd.Models.Permission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Role
{
    public class RoleSendObject
    {
        public int RoleId { get; set; }
        public string Name { get; set; }
        public List<PermissionDto> RolePermissions { get; set; }
    }
}