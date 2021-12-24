using CRMHalalBackEnd.Models.Address;
using CRMHalalBackEnd.Models.Contact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.MyUser
{
    public class MyUserResponse
    {
        public string Guid { get; set; }
        public string Email { get; set; }
        public List<ContactResponse> Contacts { get; set; }
        public List<AddressResponse> Addresses { get; set; }
        public DateTime? Birthdate { get; set; }
        //public string Role_name { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}