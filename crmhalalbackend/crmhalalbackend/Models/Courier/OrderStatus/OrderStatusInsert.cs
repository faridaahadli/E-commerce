using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Courier.OrderStatus
{
    public class OrderStatusInsert
    {
        public int Id { get; set; }
        public string UserStatus { get; set; }
        public string UserStatus2 { get; set; }
        public string UserStatus3 { get; set; }
        public string UserStatus4 { get; set; }
        public bool? IsAfterPayment { get; set; }
        public string AdminStatus { get; set; }
        public string AdminStatus2 { get; set; }
        public string AdminStatus3 { get; set; }
        public string AdminStatus4 { get; set; }

    }
}