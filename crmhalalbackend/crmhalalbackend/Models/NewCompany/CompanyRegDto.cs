using System.Collections.Generic;

namespace CRMHalalBackEnd.Models.NewCompany
{
    public class CompanyRegDto
    {
        public string Name { get; set; }
        public List<Contact.NewContact> Contacts { get; set; }
        public List<Address.NewAddress> Addresses { get; set; }
        public List<CompCategories> CompCategories { get; set; }
        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(Contacts)}: {Contacts}, {nameof(Addresses)}: {Addresses},{nameof(CompCategories)}: {CompCategories}";
        }
    }

    public class CompCategories
    {
        public int CompCategoryId { get; set; }
    }
}