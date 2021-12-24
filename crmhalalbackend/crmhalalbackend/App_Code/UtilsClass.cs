using CRM_Halal.App_Code;
using CRMHalalBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Web;


namespace CRMHalalBackEnd.App_Code
{
    public class UtilsClass
    {
        public string addNulls(int code)
        {
            string result = code.ToString();
            while (result.Length < 6)
            {
                result = "0" + result;
            }
            return result;
        }
        public string getUserId(ClaimsIdentity claims)
        {

            return claims.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier)
               .Select(c => c.Value).SingleOrDefault().ToString();
        }
        public string getActiveUserId(ClaimsIdentity claims)
        {

            return claims.Claims.Where(c => c.Type == ClaimTypes.UserData)
                .Select(c => c.Value).SingleOrDefault().ToString();
        }
        public string getDefaultUserId(ClaimsIdentity claims)
        {
            string actor = claims.Claims.Where(c => c.Type == ClaimTypes.Actor).Select(c => c.Value).SingleOrDefault();

            if (actor.Equals("Company"))
            {
                return getActiveUserId(claims);
            }
            if (actor.Equals("User"))
            {
                return getUserId(claims);
            }

            return null;
        }
        public string getTenantId(ClaimsIdentity claims)
        {

            return claims.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier)
                .Select(c => c.Value).SingleOrDefault().ToString();
        }
        public string getUserName(ClaimsIdentity claims)
        {

            return claims.Claims.Where(c => c.Type == ClaimTypes.Name)
               .Select(c => c.Value).SingleOrDefault().ToString();
        }
        public string getUserRole(ClaimsIdentity claims)
        {
            return claims.Claims.Where(c => c.Type == ClaimTypes.Role)
               .Select(c => c.Value).SingleOrDefault().ToString();
        }
        
    }
    
}