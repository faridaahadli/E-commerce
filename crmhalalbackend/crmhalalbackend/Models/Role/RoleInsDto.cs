using System.Collections.Generic;
using CRMHalalBackEnd.Models.Permission;

namespace CRMHalalBackEnd.Models.Role
{
    public class RoleInsDto
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public List<PermissionDto> Permissions { get; set; }

        public override string ToString()
        {
            return $"{nameof(RoleId)}: {RoleId}, {nameof(RoleName)}: {RoleName}, {nameof(Permissions)}: {Permissions}";
        }
    }
}