using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Payment
{
    public class RefundList
    {
        public string Username { get; set; }
        public string UserSurname { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreateDate { get; set; }
    }
}