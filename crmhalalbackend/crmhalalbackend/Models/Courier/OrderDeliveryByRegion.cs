using CRMHalalBackEnd.Models.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Courier
{
    public class OrderDeliveryByRegion
    {
        public int RegionId { get; set; }
        public IEnumerable<OrderByStore> Store { get; set; }
    }
}