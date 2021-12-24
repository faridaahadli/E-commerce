using CRMHalalBackEnd.Models.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRMHalalBackEnd.Models.Store;

namespace CRMHalalBackEnd.Models.Basket
{
    public class NewBasketResponse
    {
        public string ProductId { get; set; }
        public string Id { get; set; }
        public PromotionBasketResponse Promotion{ get; set; }
        public StoreResponse Store { get; set; } = new StoreResponse();
        public string BasketGuid { get; set; } = String.Empty;
        public string ProductName { get; set; } = String.Empty;
        public ProductUnit.ProductUnit MeasureType { get; set; }
        public List<FileDto> Images { get; set; } = new List<FileDto>();
        public decimal Price { get; set; }
        public string Slug { get; set; }
        public decimal Quantity { get; set; }
        public decimal Discount { get; set; }
        public int StockQuantity { get; set; }
        public decimal LastPrice { get; set; }
        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Store)}: {Store}, {nameof(BasketGuid)}: {BasketGuid}, {nameof(ProductName)}: {ProductName}, {nameof(Images)}: {Images}, {nameof(Price)}: {Price}, {nameof(Quantity)}: {Quantity}, {nameof(Discount)}: {Discount}, {nameof(LastPrice)}: {LastPrice}";
        }
    }
}