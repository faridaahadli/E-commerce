using System;
using System.Collections.Generic;

namespace CRMHalalBackEnd.Models.Permission
{
    public class PermissionDto
    {
        public int PermissionId { get; set; }
        public string PermissionName { get; set; } = String.Empty;
        public PermissionCategoryDto PermissionCategory { get; set; } = new PermissionCategoryDto();
        public List<PermissionData> DataOfPermissions { get; set; }
        public override string ToString()
        {
            return $"{nameof(PermissionId)}: {PermissionId}, {nameof(PermissionName)}: {PermissionName}, {nameof(PermissionCategory)}: {PermissionCategory}, {nameof(DataOfPermissions)}: {DataOfPermissions}";
        }
        
    }
}