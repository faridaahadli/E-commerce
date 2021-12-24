using CRMHalalBackEnd.Models.Address;
using CRMHalalBackEnd.Models.Contact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace CRMHalalBackEnd.Models.MyUser
{
    public class NewUser
    {
        public int UserId { get; set; }
        public string UserGuid { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public UserRole UserRole { get; set; } 
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public NewContact Contact { get; set; }
        public bool IsActive { get; set; }
        public NewAddress Address { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public int LastFailedRetries { get; set; }
        public string LastLoginIp { get; set; } 
        public string SocialProvider { get; set; }
        public string SocialToken { get; set; }
        public bool IsNotificate { get; set; }
        public bool? IsVerifyNeeded { get; set; }
        public string Code { get; set; }
        public override string ToString()
        {
            return $"{nameof(UserId)}: {UserId}, {nameof(UserGuid)}: {UserGuid}, {nameof(Email)}: {Email}, {nameof(Password)}: {Password}, {nameof(FirstName)}: {FirstName}, {nameof(UserRole)}: {UserRole}, {nameof(LastName)}: {LastName}, {nameof(BirthDate)}: {BirthDate}, {nameof(Contact)}: {Contact}, {nameof(IsActive)}: {IsActive}, {nameof(Address)}: {Address}, {nameof(LastLoginDate)}: {LastLoginDate}, {nameof(LastFailedRetries)}: {LastFailedRetries}, {nameof(LastLoginIp)}: {LastLoginIp}, {nameof(SocialProvider)}: {SocialProvider}, {nameof(SocialToken)}: {SocialToken}, {nameof(IsNotificate)}: {IsNotificate}, {nameof(Code)}: {Code}";
        }
    }
}
