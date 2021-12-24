using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Order
{
    public class AllOrderForAdmin
    {
        public int OrderId { get; set; }
        public string Buyer { get; set; }
        public decimal Price { get; set; }
        public decimal DeliveryPrice { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public string StatusId { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<Lines> Lines { get; set; }
    }
    public class Lines
    {
        public int LineId { get; set; }
    }
}