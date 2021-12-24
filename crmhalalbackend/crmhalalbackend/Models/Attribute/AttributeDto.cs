using System;

namespace CRMHalalBackEnd.Models.Attribute
{
    public class AttributeDto
    {
        public string Id { get; set; } = String.Empty;
        public string Name  { get; set; }
        public string Value { get; set; } = String.Empty;

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}, {nameof(Value)}: {Value}";
        }
    }
}