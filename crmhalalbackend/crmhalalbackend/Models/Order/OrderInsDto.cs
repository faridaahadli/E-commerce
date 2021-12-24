using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Order
{
    public class OrderInsDto
    {
        public string Currency { get; set; } = "azn";
        public decimal TotalPrice { get; set; }
        public decimal BasketPrice { get; set; }
        public string Person { get; set; }
        public string Phone { get; set; }
        public int AddressId { get; set; }
        //Table da note qoymaq unutma
        public string Note { get; set; }
        public IEnumerable<OrderByStore> Stores { get; set; }
    }
}