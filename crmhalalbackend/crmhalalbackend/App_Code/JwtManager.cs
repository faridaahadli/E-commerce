using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using CRMHalalBackEnd.Models;
using CRMHalalBackEnd.Models.MyUser;
using CRMHalalBackEnd.Repository;
using System.Collections.Generic;
using CRMHalalBackEnd.Models.Employee;
using CRMHalalBackEnd.Models.NewCompany;

namespace WebApi.Jwt
{
    public static class JwtManager
    {
        /// <summary>
        /// Use the below code to generate symmetric Secret Key
        ///     var hmac = new HMACSHA256();
        ///     var key = Convert.ToBase64String(hmac.Key);
        /// </summary>
        /// 

        private const string Secret = "db3OIsj+BXE9NZDy0t8W3TcNekrF+2d/1sFnWG4HnV8TZY30iTOdtVWJG8abWvB1GlOgJuQZdcF2Luqm/hccMw==";

        public static UserResponseModel GenerateToken(UserModelClass user, int expireMinutes = 60 * 24 * 365) //60*24*365 mins = 1 year
        {
            var symmetricKey = Convert.FromBase64String(Secret);
            var tokenHandler = new JwtSecurityTokenHandler();

            var now = DateTime.UtcNow;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Name, user.email),
                            new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                            new Claim(ClaimTypes.Role, user.role)
                        }),

                Expires = now.AddMinutes(Convert.ToInt32(expireMinutes)),

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var stoken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(stoken);

            UserResponseModel userResponse = new UserResponseModel();
            userResponse.name = user.name;
            userResponse.surname = user.surname;
            userResponse.token = token;
            userResponse.id = user.id;
            userResponse.role_name = user.role;

            return userResponse;
        }
        public static UserResponseModel NewGenerateToken(NewUser user) 
        {
            var symmetricKey = Convert.FromBase64String(Secret);
            var tokenHandler = new JwtSecurityTokenHandler();
            //var repo = new RoleRepository();
            var userRepo = new UserRepository();
            var now = DateTime.UtcNow;
            var userId = userRepo.GetUserByEmail(user.Email).UserId;
            //var role = repo.GetRoleByUserId(userId);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                        {
                            //new Claim(ClaimTypes.Name, user.Email),
                            new Claim(ClaimTypes.NameIdentifier,userId.ToString()),
                            //new Claim(ClaimTypes.Role,role),
                            new Claim(ClaimTypes.Actor,"User")
                        }),

                Expires = now.AddDays(7),

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var stoken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(stoken);

            UserResponseModel userResponse = new UserResponseModel();
            userResponse.name = user.FirstName;
            userResponse.surname = user.LastName;
            userResponse.birthdate = user.BirthDate;
            userResponse.email = user.Email;
            if (user.Contact != null)
            {
                userResponse.contact = user.Contact.Text;

            }
            if (user.Address != null)
            {
                userResponse.address = user.Address.Address;

            }
            
            userResponse.token = token;
            userResponse.social_token = user.SocialToken;
            userResponse.guid = user.UserGuid;
            userResponse.Code = user.Code;
            userResponse.IsNotificate = user.IsNotificate;
            userResponse.IsVerifyNeeded = user.IsVerifyNeeded;
            //userResponse.role_name = role;
           // userResponse.id = user.UserId;
            return userResponse;
        }
        public static EmployeeTokenData NewGenerateTokenForCompany(EmployeeTokenData employee)
        {
            var symmetricKey = Convert.FromBase64String(Secret);
            var tokenHandler = new JwtSecurityTokenHandler();
            //var repo = new RoleRepository();
            var now = DateTime.UtcNow.ToLocalTime();
            //var userId = userRepo.GetUserByEmail(user.Email).UserId;
            //var role = repo.GetRoleByUserId(userId);
            var tokenDescriptor = new SecurityTokenDescriptor();
            try
            {
                tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier,employee.TenantId),
                        new Claim(ClaimTypes.UserData,Convert.ToString(employee.ActiveUserId)),
                        new Claim(ClaimTypes.Actor,"Company"),
                        new Claim(ClaimTypes.Role,employee.Role),
                        new Claim("permission",employee.Permission)
                        //!company.Role.Equals("admin")?new Claim("permission",company.Permission):new Claim("permission","")
                    }),

                    Expires = now.AddDays(7),

                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
                };
            }
            catch (Exception e)
            {
                throw new Exception("User Duzgun deyil");
            }
            
            
            var stoken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(stoken);
            employee.Token = token;
            employee.ActiveUserId = 0;
            return employee;
        }
        public static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                    return null;

                var symmetricKey = Convert.FromBase64String(Secret);

                var validationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
                };

                SecurityToken securityToken;
                var principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);

                return principal;
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}