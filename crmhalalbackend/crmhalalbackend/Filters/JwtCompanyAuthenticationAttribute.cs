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
    public class JwtCompanyAuthenticationAttribute : Attribute, IAuthenticationFilter
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

        private bool ValidateToken(string token, out string name, out string tenanrId, out string actor, out string activeUser/*, out string role*/)
        {
            name = null;
            tenanrId = null;
            actor = null;
            activeUser = null;
            //role = null;

            var simplePrinciple = JwtManager.GetPrincipal(token);
            if (simplePrinciple == null)
                return false;
            var identity = simplePrinciple.Identity as ClaimsIdentity;

            if (identity == null || !identity.IsAuthenticated)
                return false;

            var companyClaim = identity.FindFirst(ClaimTypes.Name);
            name = companyClaim?.Value;
            var companyIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
            tenanrId = companyIdClaim?.Value;
            actor = identity.FindFirst(ClaimTypes.Actor)?.Value;
            activeUser = identity.FindFirst(ClaimTypes.UserData)?.Value;
            //var roleClaim = identity.FindFirst(ClaimTypes.Role);
            //role = roleClaim?.Value;
            if (string.IsNullOrEmpty(tenanrId) || !actor.Equals("Company") /*|| string.IsNullOrEmpty(role)*/) ///
                return false;

            // More validate to check whether username exists in system

            return true;
        }

        protected Task<IPrincipal> AuthenticateJwtToken(string token)
        {
            //GetUserInfoClass getUserInfoClass = new GetUserInfoClass();
            //UserModelClass userModel = new UserModelClass();
            //userModel = getUserInfoClass.getUserInfo(username);
            string name;
            string tenantId;
            string actor;
            string activeUser;
            //string role;
            if (ValidateToken(token, out name, out tenantId, out actor, out activeUser/*, out role*/))
            {
                // based on username to get more information from database 
                // in order to build local identity
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier,tenantId),
            new Claim(ClaimTypes.Actor,actor),
            new Claim(ClaimTypes.UserData,activeUser)

            //new Claim(ClaimTypes.Role,role)
            // Add more claims if needed: Roles, ...
        };

                var identity = new ClaimsIdentity(claims, "Jwt");
                IPrincipal company = new ClaimsPrincipal(identity);

                return Task.FromResult(company);
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
