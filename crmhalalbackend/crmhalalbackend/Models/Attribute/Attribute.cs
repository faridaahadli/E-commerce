using System;
using System.Collections.Generic;

namespace CRMHalalBackEnd.Models.Attribute
{
    public class Attribute
    {
        public string Id { get; set; } = String.Empty;
        public string Name { get; set; } = String.Empty;
        public string Name2 { get; set; } = String.Empty;
        public string Name3 { get; set; } = String.Empty;
        public string Name4 { get; set; } = String.Empty;
        public bool ShowInName { get; set; }
        public List<string> Value { get; set; } = new List<string>();
        public List<string> Value2 { get; set; } = new List<string>();
        public List<string> Value3 { get; set; } = new List<string>();
        public List<string> Value4 { get; set; } = new List<string>();
        public int VariationUpdateTypes { get; set; }
        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}, {nameof(ShowInName)}: {ShowInName}, {nameof(Value)}: {Value}";
        }
    }
}