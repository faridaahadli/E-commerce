using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.UserDesign
{
    public class DesignResponse
    {
        public string TenantId { get; set; }
        public DateTime CreateDate { get; set; }
        public int UserDesignId { get; set; }
        public string JsonData { get; set; }
        public int Status { get; set; }
    }
}