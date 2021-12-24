using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.MyUser
{
    public class UserUpdDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string  Password { get; set; }
        public string RepeatPassword { get; set; }
    }
}