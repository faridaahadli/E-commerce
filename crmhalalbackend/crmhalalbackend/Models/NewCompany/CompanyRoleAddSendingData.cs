using CRMHalalBackEnd.Models.Permission;
using CRMHalalBackEnd.Models.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.NewCompany
{
    public class CompanyRoleAddSendingData
    {
        public List<RoleSendObject> RolesOfUser { get; set; }
        public List<PermissionDto> Permissions { get; set; }
    }
}