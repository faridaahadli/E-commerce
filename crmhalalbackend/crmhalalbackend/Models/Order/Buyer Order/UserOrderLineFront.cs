using CRMHalalBackEnd.Models.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Order.Buyer_Order
{
    public class UserOrderLineFront
    {
        public List<StoreDataFront> StoreData { get; set; }
        public RefundResponse RefundData { get; set; }
        public CommonData CommonData { get; set; }

    }

    public class StoreDataFront
    {
        public string TenantId { get; set; }
        public string StoreName { get; set; }
        public string Domain { get; set; }
        public string PaymentType { get; set; }
        public DateTime? DeliveryTime { get; set; } = null;
        public decimal DeliveryPrice { get; set; }
        public List<OrderDataFront> OrderData { get; set; }
        public List<Promotions> Promotions { get; set; }

    }
    public class OrderDataFront
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class PromotionSplitForTenant
    {
        public string TenantId { get; set; }
        public List<Promotions> PromotionsList { get; set; }
    }

}