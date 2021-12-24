using CRMHalalBackEnd.Models.Permission;
using CRMHalalBackEnd.Models.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.NewCompany
{
    public class EmployeeUpdateSendData
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeSurname { get; set; }
        public string UserGuid { get; set; }
        public bool IsAdmin { get; set; }
        public List<RoleSendObject> RolesOfUser { get; set; }
        public List<PermissionDto> Permissions { get; set; }
    }
}