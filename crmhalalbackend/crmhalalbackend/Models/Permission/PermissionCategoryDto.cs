using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Permission
{
    public class PermissionCategoryDto
    {
        public int PermissionCategoryId { get; set; }
        public string PermissionCategoryName { get; set; } = String.Empty;

        public override string ToString()
        {
            return $"{nameof(PermissionCategoryId)}: {PermissionCategoryId}, {nameof(PermissionCategoryName)}: {PermissionCategoryName}";
        }
    }
}