using CRMHalalBackEnd.App_Code;
using CRMHalalBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace WebApi.Jwt.Filters
{
    public class JwtAuthenticationAttribute : Attribute, IAuthenticationFilter
    {


        public string Realm { get; set; }
        public bool AllowMultiple => false;

        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var request = context.Request;
            var authorization = request.Headers.Authorization;

            if (authorization == null || authorization.Scheme != "Bearer")
                return;

            if (string.IsNullOrEmpty(authorization.Parameter))
            {
                context.ErrorResult = new AuthenticationFailureResult("Missing Jwt Token", request);
                return;
            }

            var token = authorization.Parameter;
            var principal = await AuthenticateJwtToken(token);

            if (principal == null)
                context.ErrorResult = new AuthenticationFailureResult("Invalid token", request);

            else
                context.Principal = principal;
        }



        //private static bool ValidateToken(string token, out string username)
        //{
        //    username = null;

        //    var simplePrinciple = JwtManager.GetPrincipal(token);
        //    var identity = simplePrinciple?.Identity as ClaimsIdentity;

        //    if (identity == null)
        //        return false;

        //    if (!identity.IsAuthenticated)
        //        return false;

        //    var usernameClaim = identity.FindFirst(ClaimTypes.Name);
        //    username = usernameClaim?.Value;

        //    if (string.IsNullOrEmpty(username))
        //        return false;

        //    // More validate to check whether username exists in system

        //    return true;
        //}

        //protected Task<IPrincipal> AuthenticateJwtToken(string token)
        //{
        //    string username;
        //    if (ValidateToken(token, out username))
        //    {
        //        // Customer customer = new Customer();
        //        //DatabaseClass databaseClass = new DatabaseClass();
        //        //customer.username = username;
        //        //Customer checkCustomer = databaseClass.getCustomerInfoByUsername(customer);
        //        GetUserInfoClass getUserInfoClass = new GetUserInfoClass();
        //   UserModelClass userModel = new UserModelClass();
        //  userModel = getUserInfoClass.getUserInfo(username);
        //        // based on username to get more information from database in order to build local identity
        //        var claims = new List<Claim>
        //        {
        //            new Claim(ClaimTypes.Name, string.Format("{0} {1}", userModel.name,userModel.surname)),
        //            new Claim(ClaimTypes.NameIdentifier, userModel.id.ToString()),
        //            new Claim(ClaimTypes.Role, userModel.role)
        //            // Add more claims if needed: Roles, ...
        //        };

        //        var identity = new ClaimsIdentity(claims, "Jwt");
        //        IPrincipal user = new ClaimsPrincipal(identity);

        //        return Task.FromResult(user);
        //    }

        //    return Task.FromResult<IPrincipal>(null);
        //}


        //menimki
        private static bool ValidateToken(string token, out string userId)
        {

            userId = null;
            //role = null;

            var simplePrinciple = JwtManager.GetPrincipal(token);
            if (simplePrinciple == null)
                return false;
            var identity = simplePrinciple.Identity as ClaimsIdentity;

            if (identity == null || !identity.IsAuthenticated)
                return false;



            var userIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
            userId = userIdClaim?.Value;
            //var roleClaim = identity.FindFirst(ClaimTypes.Role);
            //role = roleClaim?.Value;
            var actor = identity.FindFirst(ClaimTypes.Actor)?.Value;
            if (string.IsNullOrEmpty(userId) || !actor.Equals("User"))
                return false;

            // More validate to check whether username exists in system

            return true;
        }

        protected Task<IPrincipal> AuthenticateJwtToken(string token)
        {
            //GetUserInfoClass getUserInfoClass = new GetUserInfoClass();
            //UserModelClass userModel = new UserModelClass();
            //userModel = getUserInfoClass.getUserInfo(username);

            string userId;
            if (ValidateToken(token, out userId))
            {
                // based on username to get more information from database 
                // in order to build local identity
                var claims = new List<Claim>
        {

            new Claim(ClaimTypes.NameIdentifier,userId),
            // Add more claims if needed: Roles, ...
        };

                var identity = new ClaimsIdentity(claims, "Jwt");
                IPrincipal user = new ClaimsPrincipal(identity);

                return Task.FromResult(user);
            }

            return Task.FromResult<IPrincipal>(null);
        }
        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            Challenge(context);
            return Task.FromResult(0);
        }

        private void Challenge(HttpAuthenticationChallengeContext context)
        {
            string parameter = null;

            if (!string.IsNullOrEmpty(Realm))
                parameter = "realm=\"" + Realm + "\"";

            context.ChallengeWith("Bearer", parameter);
        }
    }
}
