using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using Castle.Core.Internal;
using WebApi.Jwt;
using WebApi.Jwt.Filters;

namespace CRMHalalBackEnd.Filters
{
    public class JwtRoleAuthenticationAttribute : Attribute, IAuthenticationFilter
    {

        public string Realm { get; set; }
        public bool AllowMultiple => false;
        public string Permission { get; set; }
        public string Actor { get; set; }
        public string Role { get; set; }


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

        private bool ValidateToken(string token, out string tenantId, out string actor, out string activeUser, out string permission, out string userId, out string role)
        {
            tenantId = null;
            actor = null;
            activeUser = null;
            permission = null;
            userId = null;
            role = null;



            var simplePrinciple = JwtManager.GetPrincipal(token);
            if (simplePrinciple == null)
                return false;
            var identity = simplePrinciple.Identity as ClaimsIdentity;

            if (identity == null || !identity.IsAuthenticated)
                return false;
            var companyIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
            tenantId = companyIdClaim?.Value;
            var companyRoleClaim = identity.FindFirst(ClaimTypes.Role);
            role = companyRoleClaim?.Value;
            actor = identity.FindFirst(ClaimTypes.Actor)?.Value;
            activeUser = identity.FindFirst(ClaimTypes.UserData)?.Value;
            permission = identity.FindFirst("permission")?.Value;
            var userIdClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
            userId = userIdClaim?.Value;

            if (Actor == null && Role == null && Permission == null)
            {
                if (actor == "Company" || actor == "User")
                    return true;

            }

            //for User

            if (Actor == actor)
            {

                if (!(string.IsNullOrEmpty(userId) || !actor.Equals("User")))
                    return true;

            }

            //for Company

            if (Actor == actor && Actor == "Company")
            {
                if (Role.IsNullOrEmpty() && Permission.IsNullOrEmpty())
                {
                    if (!role.IsNullOrEmpty() || !permission.IsNullOrEmpty())
                    {
                        return true;
                    }
                }

                if ((Role == role && Role == "Admin") && Permission == null)
                {
                    if (!(string.IsNullOrEmpty(tenantId)))
                        return true;
                }

                if (Permission != null)
                {

                    if (role == "Admin")
                    {
                        return true;
                    }

                    var perFromClaim = permission.Split(',');
                    var perFromAttribute = Permission.Split(',');

                    foreach (var itemAttr in perFromClaim)
                    {

                        var result = Array.Find(perFromAttribute, x => x == itemAttr);
                        if (result != null)
                            return true;
                    }
                    return false;
                }

                return false;
            }

            return false;
        }


        protected Task<IPrincipal> AuthenticateJwtToken(string token)
        {
            string tenantId;
            string actor;
            string activeUser;
            string permission;
            string userId;
            string role;
            if (ValidateToken(token, out tenantId, out actor, out activeUser, out permission, out userId, out role))
            {
                if (actor == "Company")
                {
                    var claims = new List<Claim>
                       {

                           new Claim(ClaimTypes.NameIdentifier,tenantId),
                           new Claim(ClaimTypes.Actor,actor),
                           new Claim(ClaimTypes.UserData,activeUser),
                           new Claim("permission",permission),
                           new Claim(ClaimTypes.Role,role)

                        };

                    var identity = new ClaimsIdentity(claims, "Jwt");
                    IPrincipal company = new ClaimsPrincipal(identity);

                    return Task.FromResult(company);
                }


                else
                {
                    var claims = new List<Claim>
                          {
                              new Claim(ClaimTypes.Actor,actor),
                           new Claim(ClaimTypes.NameIdentifier,userId)

                          };

                    var identity = new ClaimsIdentity(claims, "Jwt");
                    IPrincipal user = new ClaimsPrincipal(identity);

                    return Task.FromResult(user);
                }

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