using CRMHalalBackEnd.Models.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Order.Buyer_Order
{
    public class UserOrderLine
    {
        public List<OrderData> OrderData { get; set; } = new List<OrderData>();
        public CommonData CommonData { get; set; } = new CommonData();
        public RefundResponse RefundData { get; set; }
        public List<Promotions> Promotions { get; set; }
    }
    public class OrderData
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string TenantId { get; set; }
        public decimal Quantity { get; set; }
        public string PaymentType { get; set; }
        public decimal Price { get; set; }
        public string StoreName { get; set; }
        public string Domain { get; set; }
        public DateTime? DeliveryTime { get; set; }
        public decimal DeliveryPrice { get; set; }

    }
    public class CommonData
    {

        public string AddressName { get; set; }= String.Empty;
        public string Note { get; set; } = String.Empty;
    }
}