using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Product
{
    public class CommentResponse
    {
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public string Comment { get; set; }
        public string UserGuid { get; set; }
        public string GroupId { get; set; }
        public bool ShowName { get; set; }
        public string UserName { get; set; }
        public int Star { get; set; }
    }
}