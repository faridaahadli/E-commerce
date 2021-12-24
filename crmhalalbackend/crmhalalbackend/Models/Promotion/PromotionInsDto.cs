using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRMHalalBackEnd.Models.File;

namespace CRMHalalBackEnd.Models.Promotion
{
    public class PromotionInsDto
    {
        public int PromotionId { get; set; }
        public int Type { get; set; }
        public string Text { get; set; }
        public string Text2 { get; set; }
        public string Text3 { get; set; }
        public string Text4 { get; set; }
        public string Description { get; set; }
        public string Description2 { get; set; }
        public string Description3 { get; set; }
        public string Description4 { get; set; }
        public decimal Amount { get; set; }
        public decimal PromoAmount { get; set; }
        public FileDto Image { get; set; }

        private string _beginDate;
        private string _endDate;
        public DateTime BeginDate
        {
            get => DateTime.Parse(_beginDate);
            set => _beginDate = value.ToString("yyyy-MM-dd");
        }

        public DateTime EndDate
        {
            get => DateTime.Parse(_endDate);
            set => _endDate = value.ToString("yyyy-MM-dd");
        }

        public List<PromoProductInsDto> PromoProducts { get; set; }

        public override string ToString()
        {
            return $"{nameof(PromotionId)}: {PromotionId}, {nameof(Type)}: {Type}, {nameof(Description)}: {Description}, {nameof(Amount)}: {Amount}, {nameof(PromoAmount)}: {PromoAmount}, {nameof(Image)}: {Image}, {nameof(BeginDate)}: {BeginDate}, {nameof(EndDate)}: {EndDate}, {nameof(PromoProducts)}: {PromoProducts}";
        }
    }
}