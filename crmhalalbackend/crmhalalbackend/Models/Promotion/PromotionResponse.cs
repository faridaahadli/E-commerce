using CRMHalalBackEnd.Models.File;
using CRMHalalBackEnd.Models.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.Promotion
{
    public class PromotionResponse
    {
        public int PromotionId { get; set; }
        public int TypeId { get; set; }
        public StoreResponse Store { get; set; }
        public string Description { get; set; }
        public string Description2 { get; set; }
        public string Description3 { get; set; }
        public string Description4 { get; set; }
        public string Text { get; set; }
        public string Text2 { get; set; }
        public string Text3 { get; set; }
        public string Text4 { get; set; }
        public decimal Amount { get; set; }
        public string Slug { get; set; }
        public decimal PromoAmount { get; set; }
        public FileDto Image { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<PromoProductResponse> PromoProducts { get; set; }
    }
}