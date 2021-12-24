using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Order
{
    public class OrderStatusUpdate
    {
        public List<OrderLineIds> OrderLineIds { get; set; }
        public int StatusId { get; set; }
    }
    public class OrderLineIds
    {
        public int OrderLineId { get; set; }
    }
}