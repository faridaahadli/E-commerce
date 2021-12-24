using CRMHalalBackEnd.Models.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.SiteSettings
{
    public class SosialMediaResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public FileDto Icon { get; set; }
        public string Link { get; set; }
    }
}