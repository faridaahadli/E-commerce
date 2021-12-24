using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRMHalalBackEnd.Models.File;

namespace CRMHalalBackEnd.Models.Order
{
    public class OrdersForUser
    {
        public int OrderId { get; set; }
        public List<ForStore> ForStore { get; set; }
    }
    public class ForStore
    {
        public List<OrderLines> OrderLines { get; set; }
    }
    public class OrderLines
    {
        public int LineId { get; set; }
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public string Status { get; set; }
        public string ProductName { get; set; }
        public string StoreName { get; set; }
        public FileDto Image { get; set; }

    }
}