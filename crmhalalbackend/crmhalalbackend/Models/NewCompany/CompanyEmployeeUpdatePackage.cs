using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.NewCompany
{
    public class CompanyEmployeeUpdatePackage
    {
        public CompanyRoleAddSendingData   WholeData { get; set; }
        public EmployeeUpdateSendData      EmployeeData { get; set; }
    }
}