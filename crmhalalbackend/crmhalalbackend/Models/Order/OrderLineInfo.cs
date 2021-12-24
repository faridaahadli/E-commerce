using CRMHalalBackEnd.Models.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Order
{
    public class OrderLineInfo
    {
        //    public List<Lines> Lines { get; set; }
        public UserData UserData { get; set; }
        public List<Products> Products { get; set; }
        public List<Promotions> Promotions { get; set; }
        public RefundResponse RefundData { get; set; }
        public List<StatusData> StatusData { get; set; }
    }
    public class UserData
    {
        public string UserName { get; set; }
        public string UserPhone { get; set; }
        public string PaymentMethod { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }
        public string SelectedTime { get; set; }
        public decimal DeliveryPrice { get; set; }

    }
    public class Products
    {
        public string ProductGuid { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public string Barcode { get; set; }
        public string Slug { get; set; }
        public string Currency { get; set; }
        public decimal TotalPrice { get; set; }
        public int LineId { get; set; }

    }
    public class Promotions
    {
        public int PromoId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Slug { get; set; }
        public decimal Quantity { get; set; }
        public string Currency { get; set; }
        public string TenantId { get; set; }
        public List<PromoProducts> PromoProduct { get; set; }
    }
    public class PromoProducts
    {
        public string ProductGuid { get; set; }
        public int ProductId { get; set; }
        public decimal Amount { get; set; }
        public string Slug { get; set; }
        public decimal PromoAmount { get; set; }
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public int LineId { get; set; }
    }
    public class StatusData
   {
       public string Status { get; set; }
       public DateTime UpdatedTime { get; set; }
       public string FullName { get; set; }

   }
}