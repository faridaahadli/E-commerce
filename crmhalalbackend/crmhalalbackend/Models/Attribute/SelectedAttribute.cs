using System;

namespace CRMHalalBackEnd.Models.Attribute
{
    public class SelectedAttribute
    {
        public string Id { get; set; } = String.Empty;
        public string Value { get; set; } = String.Empty;
        public string Value2 { get; set; } = String.Empty;
        public string Value3 { get; set; } = String.Empty;
        public string Value4 { get; set; } = String.Empty;

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Value)}: {Value}";
        }
    }
}