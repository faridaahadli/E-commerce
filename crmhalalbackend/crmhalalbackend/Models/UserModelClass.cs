using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models
{
    public class UserModelClass
    {
        public int id { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string telephone { get; set; }
        public string address { get; set; }
        public DateTime? birthdate { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public int role_id { get; set; }
        public string role { get; set; }
        public int? company_id { get; set; }
        public int? position_id { get; set; }
        public int? division_id { get; set; }
        public int? branch_id { get; set; }
        public bool active { get; set; }
        public string code { get; set; }
    }

}