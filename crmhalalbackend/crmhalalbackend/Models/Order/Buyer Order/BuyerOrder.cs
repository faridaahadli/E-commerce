using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Order.Buyer_Order
{
    public class BuyerOrder
    {
        public int OrderId { get; set; }
        public string StoreName { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }
        public string Lang { get; set; }
    }
}