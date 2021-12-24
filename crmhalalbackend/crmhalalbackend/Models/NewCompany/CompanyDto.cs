using System;
using System.Collections.Generic;
using CRMHalalBackEnd.Models.Address;
using CRMHalalBackEnd.Models.Contact;

namespace CRMHalalBackEnd.Models.NewCompany
{
    public class CompanyDto
    {
        public string Name { get; set; } = String.Empty;
        public List<ContactResponse> Contacts { get; set; } = new List<ContactResponse>();
        public List<AddressResponse> Addresses { get; set; } = new List<AddressResponse>();
        public override string ToString()
        {
            return $"{nameof(Name)}: {Name}, {nameof(Contacts)}: {Contacts}, {nameof(Addresses)}: {Addresses}";
        }
    }
}