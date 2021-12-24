using System;

namespace CRMHalalBackEnd.Models.ProductUnit
{
    public class ProductUnit
    {
        public int Id { get; set; }
        public string UnitCode { get; set; }
        public bool IsDecimal { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

    }
}