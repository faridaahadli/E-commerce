using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models
{
    public class UserResponseModel
    {
        public int id { get; set; }
        public string guid { get; set; }
        public string token { get; set; }
        public string email { get; set; }
        public string contact { get; set; }
        public string address { get; set; }
        public DateTime? birthdate { get; set; }
        public string social_token { get; set; }
        public string role_name { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public bool IsNotificate { get; set; }
        public bool? IsVerifyNeeded { get; set; }
        public string Code { get; set; }

    }
  
}