using System;
using System.Collections.Generic;
using CRMHalalBackEnd.Models.Permission;

namespace CRMHalalBackEnd.Models.Role
{
    public class RoleResponse
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = String.Empty;
        public List<PermissionDto> Permissions { get; set; } = new List<PermissionDto>();
        public override string ToString()
        {
            return $"{nameof(RoleId)}: {RoleId}, {nameof(RoleName)}: {RoleName}, {nameof(Permissions)}: {Permissions}";
        }
    }
}