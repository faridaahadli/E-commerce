using CRMHalalBackEnd.Models.Category;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Permission
{
    public class PermissionData
    {
        public string  DataName  {get; set; }
        public List<ExpandoObject> DataList  {get; set; }
    }
}