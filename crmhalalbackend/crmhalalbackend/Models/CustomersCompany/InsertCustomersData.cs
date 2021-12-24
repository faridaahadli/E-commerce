using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.CustomersCompany
{
    public class InsertCustomersData
    {
        public string CompanyName { get; set; }
        public List<Users> Users { get; set; }
    }
    public class Users
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime? BirthDay { get; set; } 
    }
}