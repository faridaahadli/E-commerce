using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.UserDesign
{
    public class UserDesignGet
    {
        public string TenantId { get; set; }
        public int UserDesignId { get; set; }
        public string JsonData { get; set; }
    }
}