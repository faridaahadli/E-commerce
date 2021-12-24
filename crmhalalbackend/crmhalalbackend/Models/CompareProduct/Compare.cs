using CRMHalalBackEnd.Models.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.CompareProduct
{
    public class Compare
    {
        public string Id { get; set; }
        public int ProductId { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public Category Category { get; set; }
        public string Manufacturer { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public List<Attributes> Attributes { get; set; }
        public List<FileDto> Images { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Length { get; set; }
        public decimal Weight { get; set; }
        public decimal Raiting { get; set; }
    }
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class Attributes
    {
        public string Name { get; set; }
        public string Value { get; set; }

    }
}