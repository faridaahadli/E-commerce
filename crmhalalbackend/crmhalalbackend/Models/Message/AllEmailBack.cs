using CRMHalalBackEnd.Models.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Message
{
    public class AllEmailBack
    {
        public string To { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public DateTime CreateDate { get; set; }
        public List<FileDto> File { get; set; }

    }
}