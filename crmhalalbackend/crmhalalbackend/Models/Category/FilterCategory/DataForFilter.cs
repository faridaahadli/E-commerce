using System.Collections.Generic;

namespace CRMHalalBackEnd.Models.Category
{
    public class DataForFilter
    {
        public int CategoryId { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public bool IsDiscount { get; set; }
        public string OrderBy { get; set; }
        public string OrderType { get; set; }
        public List<KeyValuePair<string, string>> Attributes { get; set; } = new List<KeyValuePair<string, string>>();

        public override string ToString()
        {
            return $"{nameof(CategoryId)}: {CategoryId}, {nameof(MinPrice)}: {MinPrice}, {nameof(MaxPrice)}: {MaxPrice}, {nameof(IsDiscount)}: {IsDiscount}, {nameof(OrderBy)}: {OrderBy}, {nameof(OrderType)}: {OrderType}, {nameof(Attributes)}: {Attributes}";
        }
    }
}