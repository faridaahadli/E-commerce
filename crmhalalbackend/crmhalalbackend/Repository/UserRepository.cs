using CRMHalalBackEnd.DB;
using CRMHalalBackEnd.Models;
using CRMHalalBackEnd.Models.Address;
using CRMHalalBackEnd.Models.Contact;
using CRMHalalBackEnd.Models.MyUser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using Castle.Core.Internal;

namespace CRMHalalBackEnd.Repository
{
    public class UserRepository
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public NewUser Upsert(string domain,NewUser user, int userId)
        {
            user.UserId = userId;
            try
            {
                using (var conn = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(user);
                    userId = conn.ExecStoredProcWithReturnIntValue("[UserInsUpd]", new[] {
                        DbHandler.SetParameter("@pRequestAsJson", SqlDbType.NVarChar, -1, ParameterDirection.Input, json),
                        DbHandler.SetParameter("@pLogUserId",SqlDbType.Int,10,ParameterDirection.Input,userId),
                        DbHandler.SetParameter("@pDomain", SqlDbType.VarChar, 63, ParameterDirection.Input, domain)
                    });

                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not UserInsUpdHelper...");
                Log.Error(ex);
                throw;
            }
            return GetUserById(userId);
        }

       


        public NewUser GetUserById(int userId)
        {
            const string sql =
                @"SELECT u.USER_GUID,u.[EMAIL],u.FIRST_NAME,u.LAST_NAME,u.BIRTH_DATE,u.CODE,
                  cnt.[TEXT],cnt.NOTE, u.SOCIAL_TOKEN,u.SOCIAL_PROVIDER,u.IS_ACTIVE,u.IS_VERIFIED_NEEDED,
                  u_role.ROLE_ID,u.CODE Code,
                  adrs.[ADDRESS],
	                case when (select count(*) from NEW_NOTIFICATION N where N.TO_USER_ID = u.[USER_ID] and N.STATUS_ID=1)>0 
								then 1 else 0 end IsNotificate FROM [NEW_USER] as u
                  left join [NEW_CONTACT] as cnt
                  on u.USER_ID=cnt.USER_ID and cnt.IS_ACTIVE=1
                  left join [NEW_ADDRESS] as adrs 
                  on u.[USER_ID]=adrs.[USER_ID] and adrs.IS_ACTIVE=1
                  left join [NEW_USER_ROLE] as u_role
				  on u.USER_ID=u_role.USER_ID
                  where u.[USER_ID]=@pUserId ";
            NewUser user = null;
            try
            {
                using (var conn = new DbHandler())
                {
                    var dr = conn.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@pUserId",SqlDbType.Int,10,ParameterDirection.Input,userId),
                    });
                    if (dr.Read())
                    {
                        //UserRole role = new UserRole()
                        //{
                        //    RoleId = dr.GetInt("ROLE_ID")
                        //};
                        NewContact contact = new NewContact()
                        {
                            Text = dr.GetString("TEXT"),
                            Note = dr.GetString("NOTE")
                        };
                        NewAddress address = new NewAddress()
                        {
                            Address = dr.GetString("ADDRESS")
                        };
                        bool? verifyNeeded=null;
                        bool result;
                        if (Boolean.TryParse(dr["IS_VERIFIED_NEEDED"].ToString(), out result))
                            verifyNeeded = Convert.ToBoolean(dr["IS_VERIFIED_NEEDED"]);

                        user = new NewUser()
                        {
                            UserGuid = dr.GetString("USER_GUID"),
                            Email = dr.GetString("EMAIL"),
                            FirstName = dr.GetString("FIRST_NAME"),
                            LastName = dr.GetString("LAST_NAME"),
                            BirthDate = dr.GetDateTime("BIRTH_DATE"),
                            SocialToken = dr.GetString("SOCIAL_TOKEN"),
                            IsActive = Convert.ToBoolean(dr["IS_ACTIVE"]),
                            IsNotificate = Convert.ToBoolean(dr["IsNotificate"]),
                            IsVerifyNeeded=verifyNeeded,
                            Code = dr.GetString("Code"),
                            //UserRole=role,
                            Contact = contact,
                            Address = address
                        };

                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
            return user;
        }
        


        public NewUser GetUserByEmail(string email)
        {
            var sql = @"select * from NEW_USER where EMAIL=@pEmail";
            NewUser user=null;
            try
            {
                using (var conn=new DbHandler())
                {
                    var dr = conn.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@pEmail",SqlDbType.VarChar,50,ParameterDirection.Input,email)
                    });
                    if (dr.Read())
                    {
                        user = new NewUser()
                        {
                            Email = dr.GetString("EMAIL"),
                            FirstName = dr.GetString("FIRST_NAME"),
                            LastName = dr.GetString("LAST_NAME"),
                            BirthDate =dr.GetDateTime("BIRTH_DATE"),
                            Password = dr.GetString("PASSWORD"),
                            UserId = dr.GetInt("USER_ID"),
                            IsActive = Convert.ToBoolean(dr["IS_ACTIVE"]),
                            LastFailedRetries = Convert.ToInt16(dr["LAST_FAILED_RETRIES"]),
                            LastLoginIp = dr.GetString("LAST_LOGIN_IP"),
                            LastLoginDate=dr.GetDateTime("LAST_LOGIN_DATE"),
                            SocialToken =dr.GetString("SOCIAL_TOKEN"),
                            SocialProvider=dr.GetString("SOCIAL_PROVIDER")

                        };
                     
                    }
                }
            }
            catch(Exception ex){
                Console.WriteLine(ex.Message);
            }
            return user;
        }
        public NewUser GetUserBySocialToken(string socialToken)
        {
            var sql = @"select * from NEW_USER where SOCIAL_TOKEN=@pSocialToken";
            NewUser user = null;
            try
            {
                using (var conn = new DbHandler())
                {
                    var dr = conn.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@pSocialToken",SqlDbType.VarChar,255,ParameterDirection.Input,socialToken)
                    });
                    if (dr.Read())
                    {
                        user = new NewUser()
                        {
                            Email = dr.GetString("EMAIL"),
                            FirstName = dr.GetString("FIRST_NAME"),
                            LastName = dr.GetString("LAST_NAME"),
                            BirthDate = dr.GetDateTime("BIRTH_DATE"),
                            Password = dr.GetString("PASSWORD"),                        
                            UserId = dr.GetInt("USER_ID"),
                            IsActive = Convert.ToBoolean(dr["IS_ACTIVE"]),
                            LastFailedRetries = Convert.ToInt16(dr["LAST_FAILED_RETRIES"]),
                            LastLoginIp = dr.GetString("LAST_LOGIN_IP"),
                            LastLoginDate = dr.GetDateTime("LAST_LOGIN_DATE"),
                            SocialToken = dr.GetString("SOCIAL_TOKEN"),
                            IsVerifyNeeded = Convert.ToBoolean(dr["IS_VERIFIED_NEEDED"])

                        };

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return user;
        }

        public NewUser GetUserByGuid(string userGuid)
        {
            var sql = @"select * from NEW_USER where USER_GUID=@pUserGuid";
            NewUser user = null;
            try
            {
                using (var conn = new DbHandler())
                {
                    var dr = conn.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@pUserGuid",SqlDbType.VarChar,50,ParameterDirection.Input,userGuid)
                    });
                    if (dr.Read())
                    {
                        user = new NewUser()
                        {
                           
                            UserId = dr.GetInt("USER_ID"),

                            Email = dr.GetString("EMAIL"),
                            FirstName = dr.GetString("FIRST_NAME"),
                            LastName = dr.GetString("LAST_NAME"),
                            BirthDate = dr.GetDateTime("BIRTH_DATE"),
                            Password = dr.GetString("PASSWORD"),
                            

                            LastFailedRetries = Convert.ToInt16(dr["LAST_FAILED_RETRIES"]),
                            LastLoginIp = dr.GetString("LAST_LOGIN_IP"),
                            LastLoginDate = dr.GetDateTime("LAST_LOGIN_DATE"),
                            SocialToken = dr.GetString("SOCIAL_TOKEN"),
                            SocialProvider = dr.GetString("SOCIAL_PROVIDER")
                        };

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return user;
        }
        public void DeActiveUserContact(int userId)
        {
            var sql = @"update NEW_CONTACT set IS_ACTIVE=0 where USER_ID=@pUserId";
            try
            {
                using (var conn = new DbHandler())
                {
                   conn.ExecuteSql(sql, new[]
                     {
                        DbHandler.SetParameter("@pUserId",SqlDbType.Int,10,ParameterDirection.Input,userId)
                    }); 
                    
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void DeActiveUserAddress(int userId)
        {
            var sql = @"update NEW_ADDRESS set IS_ACTIVE=0 where USER_ID=@pUserId";
            try
            {
                using (var conn = new DbHandler())
                {
                    conn.ExecuteSql(sql, new[]
                      {
                        DbHandler.SetParameter("@pUserId",SqlDbType.Int,10,ParameterDirection.Input,userId)
                    }); 

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void UserVerificationUpdate(UserVerificationData vrfData)
        {
            try {
                var json = JsonConvert.SerializeObject(vrfData);
                using(var conn=new DbHandler())
                {
                    conn.ExecuteStoredProcedure("UserVerificationUpd", new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson",SqlDbType.NVarChar,-1,ParameterDirection.Input,json)
                    }) ;
                }
                 

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void ChangeUserActivation(int userId,int num)
        {
            var sql = @"update NEW_USER set IS_ACTIVE=@pNum where USER_ID=@pUserId";
            try
            {
                using (var conn = new DbHandler())
                {
                    conn.ExecuteSql(sql, new[]
                      {
                        DbHandler.SetParameter("@pUserId",SqlDbType.Int,10,ParameterDirection.Input,userId),
                        DbHandler.SetParameter("@pNum",SqlDbType.Int,10,ParameterDirection.Input,num)
                    });

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void LastFailedRetryUpdate(int userId,int count)
        {
            var sql = @"update NEW_USER set LAST_FAILED_RETRIES=@pCount where
                     USER_ID=@pUserId and IS_ACTIVE=1";
            try
            {
                using (var conn = new DbHandler())
                {
                    conn.ExecuteSql(sql, new[]
                      {
                        DbHandler.SetParameter("@pUserId",SqlDbType.Int,10,ParameterDirection.Input,userId),
                        DbHandler.SetParameter("@pCount",SqlDbType.Int,10,ParameterDirection.Input,count)

                    }); 

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void UpdateUserPassword(string email,string pass)
        {
            try
            {
                using(var conn=new DbHandler())
                {
                    conn.ExecuteStoredProcedure("UserPassUpdate", new[] {
                    DbHandler.SetParameter("@pUserEmail",SqlDbType.NVarChar,50,ParameterDirection.Input,email),
                    DbHandler.SetParameter("@pPassword",SqlDbType.NVarChar,-1,ParameterDirection.Input,pass)
                   });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not User Password Update...");
                Log.Error(ex);
                throw;
            }
        }

        public NewUser Update(UserUpdDto user,int userId)
        {
            try
            {
                using (var conn = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(user);
                   conn.ExecuteStoredProcedure("[UserUpdate]", new[] {
                        DbHandler.SetParameter("@pRequestAsJson", SqlDbType.NVarChar, -1, ParameterDirection.Input, json),
                        DbHandler.SetParameter("@pLogUserId",SqlDbType.Int,10,ParameterDirection.Input,userId)
                    });

                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not User Update Helper...");
                Log.Error(ex);
                throw;
            }
            return GetUserById(userId);
        }

        public UserResponseModel GetUserByCodeOrEmail(int userId,string tenantId,string email,string code)
        {
             string sql = $@"SELECT(Select USER_GUID guid,FIRST_NAME name,LAST_NAME surname 
                             from NEW_USER where  {(!code.IsNullOrEmpty() ? "CODE = @pCode" :String.Empty)}
                             {(!email.IsNullOrEmpty() ? "EMAIL = @pEmail" :String.Empty)} 
                             and IS_ACTIVE=1 and EXISTS(select * from dbo.GetEmployeePermission(@userId, @tenantId,'70'))
                                for json path,without_array_wrapper)JSON";
            UserResponseModel user = null;
            string json = String.Empty;
            try
            {
                using (var conn = new DbHandler())
                {
                   var dr=conn.ExecuteSql(sql, new[]
                    { 
                    DbHandler.SetParameter("@pEmail", SqlDbType.VarChar, 50, ParameterDirection.Input, email),
                    DbHandler.SetParameter("@pCode", SqlDbType.VarChar, 50, ParameterDirection.Input, code),
                    DbHandler.SetParameter("@userId", SqlDbType.Int, 10, ParameterDirection.Input, userId),
                    DbHandler.SetParameter("@tenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId)
                    });

                    if (dr.Read())
                    {
                        json = dr.GetString("JSON");
                    }
                    user = JsonConvert.DeserializeObject<UserResponseModel>(json);
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not Get by email or code...");
                Log.Error(ex);
                throw;
            }
            return user;
        }

        public string SetContact(string number,string userGuid)
        {
            try {

                using (var conn = new DbHandler())
                {
                    
                    conn.ExecuteStoredProcedure("[SetNumber]", new[] {
                        DbHandler.SetParameter("@pUserGuid", SqlDbType.NVarChar, 50, ParameterDirection.Input,userGuid),
                        DbHandler.SetParameter("@pNumber",SqlDbType.NVarChar,50,ParameterDirection.Input,number)
                    });

                }

            }
            catch(Exception ex) {

                Log.Warn("Could not SetNumber...");
                Log.Error(ex);
                throw;

            }
            return "okay";
        }
       
    }
}