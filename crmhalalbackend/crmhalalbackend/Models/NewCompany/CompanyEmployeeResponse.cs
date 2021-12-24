using CRMHalalBackEnd.Models.Contact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.NewCompany
{
    public class CompanyEmployeeResponse
    {
        public int EmployeeId { get; set; }
        public string Roles { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsAccepted { get; set; }
        public string UserGuid { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public List<ContactResponse> Contacts { get; set; }              
        public string Code { get; set; }
    }
}