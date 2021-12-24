using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Faq
{
    public class AllFaq
    {
        public int ModId { get; set; }
        public string ModName { get; set; } = String.Empty;
        public List<FaqForAll> FAQ { get; set; }
    }
}