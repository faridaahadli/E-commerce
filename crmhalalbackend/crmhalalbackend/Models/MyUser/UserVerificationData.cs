using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Models.MyUser
{
    public class UserVerificationData
    {
        public int UserId { get; set; }
        public bool IsActive { get; set; } = false;
        public bool IsVerifiedNeed { get; set; }
        public bool IsVerified { get; set; }
        public DateTime VerifiedDate { get; set; }
    }
}