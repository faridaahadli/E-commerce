using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Permission
{
    public class PermissionEmployeeUpdateDto
    {
        public int PermissionId { get; set; }
        public string PermissionName { get; set; }
        public List<PermissionData> DataOfPermissions { get; set; }
        public bool IsDeleted { get; set; }
    }
}