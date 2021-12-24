using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace CRM_Halal.App_Code
{
    public static class PasswordClass
    {
        private static string GetRandomSalt() //$2a$13$UeiaMgVIEAyyJZ7Aw9u/Mu
        {
            var slt= DevOne.Security.Cryptography.BCrypt.BCryptHelper.GenerateSalt(13);
            return slt;
        }

        public static string HashPassword(string password)
        {
            return DevOne.Security.Cryptography.BCrypt.BCryptHelper.HashPassword(password, GetRandomSalt());
        }
        //public static string NewHashPassword(string password)
        //{
        //    return DevOne.Security.Cryptography.BCrypt.BCryptHelper.HashPassword(password, GetSalt());
        //}
        public static bool ValidatePassword(string password, string correctHash)
        {
            return DevOne.Security.Cryptography.BCrypt.BCryptHelper.CheckPassword(password, correctHash);
        }
        //public static string GetSalt()
        //{
        //    return "$2a$13$UeiaMgVIEAyyJZ7Aw9u/Mu";
        //}
    }
}