using System;

namespace CRMHalalBackEnd.Models.SpecialOffer
{
    public class SpecialOfferInsDto
    {
        public string ProductGuid { get; set; }
        public bool DailyOffer { get; set; }

        private string _beginDate;
        private string _endDate;
        public DateTime BeginDate
        {
            get => DateTime.Parse(_beginDate);
            set => _beginDate = value.ToString("yyyy-MM-dd");
        }

        public DateTime? EndDate
        {
            get => _endDate == null ? (DateTime?) null : DateTime.Parse(_endDate);
            set => _endDate = value.HasValue ? value.Value.ToString("yyyy-MM-dd") : null;
        }

        public override string ToString()
        {
            return $"{nameof(ProductGuid)}: {ProductGuid}, {nameof(DailyOffer)}: {DailyOffer}, {nameof(BeginDate)}: {BeginDate}, {nameof(EndDate)}: {EndDate}";
        }
    }
}