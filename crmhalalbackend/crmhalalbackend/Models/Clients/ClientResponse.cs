using System;
using System.Collections.Generic;
using CRMHalalBackEnd.Models.Address;
using CRMHalalBackEnd.Models.Contact;

namespace CRMHalalBackEnd.Models.Clients
{
    public class ClientResponse
    {
        public string Guid { get; set; }
        public string Email { get; set; }
        public ContactResponse Contact { get; set; }
        public AddressResponse Address { get; set; }
        public DateTime? Birthdate { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}