using System;

namespace CRMHalalBackEnd.Models.Category
{
    public class NewCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Name2 { get; set; } = String.Empty;
        public string Name3 { get; set; } = String.Empty;
        public string Name4 { get; set; } = String.Empty;
        //public NewCategory ParentCategory { get; set; } = new NewCategory();
        public string  TenantId { get; set; } = String.Empty;
        public bool Status { get; set; }
        public string Slug { get; set; }
        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}, {nameof(TenantId)}: {TenantId}, {nameof(Status)}: {Status}";
        }
    }
}