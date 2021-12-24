using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.SiteSettings
{
    public class FooterResponseForShop
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Link { get; set; }
        public List<FooterResponseForShop> Child { get; set; }
    }
}