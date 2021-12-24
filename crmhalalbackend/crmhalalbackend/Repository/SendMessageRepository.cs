using CRMHalalBackEnd.DB;
using CRMHalalBackEnd.Helpers;
using CRMHalalBackEnd.Models.Message;
using CRMHalalBackEnd.Models.Message.Package;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace CRMHalalBackEnd.Repository
{
    public class SendMessageRepository
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Common
        public string GetUserEmailById(string UserIds)
        {

            var userIds = UserIds.Split(',');
            const string sql = "select EMAIL from NEW_USER where USER_GUID=@userGuid and IS_ACTIVE=1";
            string finalMail = "";
            foreach (var userGuid in userIds)
            {

                try
                {

                    using (var con = new DbHandler())
                    {
                        var reader = con.ExecuteSql(sql, new[]
                        {
                        DbHandler.SetParameter("@userGuid",SqlDbType.NVarChar,-1,ParameterDirection.Input,userGuid)
                        });

                        while (reader.Read())
                        {
                            string mail = reader["EMAIL"].ToString();
                            string mailItem = finalMail == "" ? finalMail + mail : finalMail + "," + mail;
                            finalMail = mailItem;

                        }

                    }

                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    throw;
                }

            }
            return finalMail;
        }
        public string GetStoreEmail(int userId, string tenantId)
        {
            string mail = String.Empty;
            const string sql = @"select EMAIL from NEW_STORE_PROVIDER_INFO where IS_ACTIVE=1 and PROVIDER_TYPE_ID!=1 and TENANT_ID=@tenantId
                                and EXISTS(SELECT* FROM dbo.GetEmployeePermission (@userId, @tenantId, 108 ) )";

            try
            {

                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@userId",SqlDbType.NVarChar,-1,ParameterDirection.Input,userId),
                        DbHandler.SetParameter("@tenantId",SqlDbType.NVarChar,-1,ParameterDirection.Input,tenantId)
                    });

                    while (reader.Read())
                    {
                        mail = reader["EMAIL"].ToString();
                    }

                }

            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }


            return mail;
        }
        public List<UserNums> GetUserNumsById(string UserIds)
        {

            var userIds = UserIds.Split(',');
            const string sql = "select [TEXT] from NEW_CONTACT where USER_ID in (select USER_ID from NEW_USER where USER_GUID=@userGuid) and CONTACT_TYPE_ID=2 and IS_ACTIVE=1";
            List<UserNums> userNums = new List<UserNums>();
            foreach (var userGuid in userIds)
            {
                try
                {

                    using (var con = new DbHandler())
                    {
                        var reader = con.ExecuteSql(sql, new[]
                        {
                        DbHandler.SetParameter("@userGuid",SqlDbType.NVarChar,-1,ParameterDirection.Input,userGuid)
                        });

                        while (reader.Read())
                        {
                            string number = reader["TEXT"].ToString();
                            UserNums num = new UserNums();
                            num.UserNumber = number;
                            userNums.Add(num);
                        }

                    }

                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    throw;
                }

            }
            return userNums;
        }
        public string InsertMessage(InsertMessage message, string tenantId, string userId)
        {
            string To = message.To;
            if (message.To != null)
            {
                foreach (var item in message.Provider)
                {
                    if (item.ProviderTypeId == 1)
                    {
                        message.UserNumbers = new List<UserNums>();
                        message.UserNumbers = GetUserNumsById(To);
                    }
                    else
                        message.To = GetUserEmailById(To);

                }
                message.CountSms = message.UserNumbers.Count;
            }

            try
            {
                using (var con = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(message);
                    con.ExecuteStoredProcedure("[InsertMessage]", new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson", SqlDbType.NVarChar, -1, ParameterDirection.Input, json),
                        DbHandler.SetParameter("@pTenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                }


            }
            catch (Exception ex)
            {
                Log.Warn("Could not insert message...");
                Log.Error(ex);
                throw;
            }
            return "mesaj elave olundu";
        }
        public List<GetUsers> GetUsers(string tenantId, int userId)
        {
            const string sql = @"select(select distinct U.FIRST_NAME+' '+U.LAST_NAME UserName, EMAIL Email, U.USER_GUID Guid, (select TOP(1)TEXT from 
                                NEW_CONTACT C where C.CONTACT_TYPE_ID=2 and  C.USER_ID=U.USER_ID) Phone
                                from NEW_USER U where ((U.USER_ID in (select USER_ID from NEW_FOLLOWERS where TENANT_ID=@tenantId
                                )) or (U.USER_ID in (select SO.USER_ID from NEW_SALES_ORDER SO where SO.SO_ID in 
                                (select SOL.SO_ID from NEW_SALES_ORDER_LINE SOL where TENANT_ID=@tenantId))) or TENANT_ID=@tenantId)
                                 and EXISTS ( SELECT * FROM dbo.GetEmployeePermission ( @userId, @tenantId, '108' ) ) for json path)Json";

            List<GetUsers> users = new List<GetUsers>();
            try

            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@tenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@userId",SqlDbType.Int,-1,ParameterDirection.Input,userId)
                    });

                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                }

                users = JsonConvert.DeserializeObject<List<GetUsers>>(json);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return users;

        }

        #endregion Common

        #region SMS

        public async Task<List<AllMessageFront>> GetAllMessages(string tenantId)
        {

            const string sql = @"select(select (select [MESSAGE] 
                                from NEW_MESSAGE where MESSAGE_ID=MQ.MESSAGE_ID) Message,
                                MQ.CREATE_DATE CreateDate, MQ.DATE_TO_SEND, MQ.MESSAGE_API_ID MessageApiId, MQ.MESSAGE_ID MessageId,
                                MQ.USER_NUMBER UserNumber, (select LAST_NAME+' '+FIRST_NAME 
                                from NEW_USER
                                where USER_ID in (select  TOP(1) USER_ID from NEW_CONTACT  
                                where TEXT Like '%'+MQ.USER_NUMBER+'%' and USER_ID is not null))UserName  
                                from NEW_MESSAGE_QUEUE MQ 
                                where SP_INFO_ID in (select SP_INFO_ID from NEW_STORE_PROVIDER_INFO 
                                where TENANT_ID=@tenantId and PROVIDER_TYPE_ID=1)for json path)Json";

            List<AllMessages> allMessages = null;
            List<AllMessageFront> finalData = new List<AllMessageFront>();
            try
            {
                string json2 = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@tenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId)
                    });

                    if (reader.Read())
                    {
                        json2 = reader["Json"].ToString();
                    }
                }

                allMessages = JsonConvert.DeserializeObject<List<AllMessages>>(json2);

                MessageApiRequest apiRequest = new MessageApiRequest();
                apiRequest.Username = "note.az_service";
                apiRequest.Password = "note678";
                if (allMessages == null)
                    throw new Exception("SMS yoxdur");

                foreach (var messageFinal in allMessages)
                {
                    if (messageFinal.MessageApiId != null)
                    {
                        apiRequest.MessageIds = new List<string>();
                        apiRequest.MessageIds.Add(messageFinal.MessageApiId);

                        HttpClient client = new HttpClient();
                        const string url = "https://www.poctgoyercini.com/api_json/v1/Sms/Status";
                        var json = JsonConvert.SerializeObject(apiRequest);
                        StringContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                        HttpResponseMessage response = client.PostAsync(url, httpContent).Result;
                        var customerJsonString = await response.Content.ReadAsStringAsync();
                        var deserialized = JsonConvert.DeserializeObject<CheckMessageApiResponse>(customerJsonString);
                        deserialized.Result.ForEach(x => { messageFinal.Status = x.SmsStatusDescription; });

                    }
                }


                var results = allMessages
                              .GroupBy(a => a.MessageId);

                foreach (var item in results)
                {
                    AllMessageFront messageFrontObj = new AllMessageFront();
                    List<User> userList = new List<User>();
                    foreach (var item2 in item)
                    {

                        messageFrontObj.CreateDate = item2.CreateDate;
                        messageFrontObj.SendDate = item2.SendDate.ToString() != "1/1/0001 12:00:00 AM" ? item2.SendDate.ToString("yyyyMMdd HH:mm") : null;
                        messageFrontObj.Message = item2.Message;
                        messageFrontObj.Status = item2.Status;
                        userList.Add(new User() { UserName = item2.UserName, UserNumber = item2.UserNumber });

                    }
                    messageFrontObj.Users = userList;
                    finalData.Add(messageFrontObj);
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return finalData;
        }


        #endregion

        #region MAIL

        public ProviderInfo GetProviderDataById(int providerTypeId)
        {
            ProviderInfo prInfo = new ProviderInfo();
            const string sql = @"select(select HOST as Host, PORT as Port from NEW_PROVIDER_TYPE where 
                                 PROVIDER_TYPE_ID=@providerTypeId and IS_ACTIVE=1 for json path, without_array_wrapper) Json";
            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@providerTypeId",SqlDbType.Int,-1,ParameterDirection.Input,providerTypeId)
                    });

                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                }

                prInfo = JsonConvert.DeserializeObject<ProviderInfo>(json);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return prInfo;
        }

        public List<ProviderTypes> GetEmailProviderName()
        {
            const string sql = "select NAME, PROVIDER_TYPE_ID TypeId from NEW_PROVIDER_TYPE where PROVIDER_TYPE_ID!=1";
            List<ProviderTypes> providersName = new List<ProviderTypes>();
            try
            {

                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql);

                    while (reader.Read())
                    {
                        ProviderTypes prType = new ProviderTypes();
                        prType.Name = reader["NAME"].ToString();
                        prType.TypeId = reader.GetInt("TypeId");
                        providersName.Add(prType);
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return providersName;
        }

        public string InsertMailInfo(UserMailInfo info, string tenantId, string userId)
        {
            var prInfo = GetProviderDataById(info.ProviderTypeId);

            var result = EmailSend.SendEmailCheck("noteaz.test@gmail.com", "Test", "Security Test", prInfo.Host, prInfo.Port, info.UserEmail, info.Password);
            //if (result == "xeta")
            //    return "Zəhmət olmazsa, daxil etdiyiniz email adresinizin \"Less secure app access\" funksionallığını aktiv edin.";

            try
            {
                using (var con = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(info);
                    con.ExecuteStoredProcedure("[StoreProviderInfoInsert]", new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson", SqlDbType.NVarChar, -1, ParameterDirection.Input, json),
                        DbHandler.SetParameter("@pTenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not insert mail info...");
                Log.Error(ex);
                throw;
            }

            return "store mail datalari elave olundu";
        }

        public List<AllEmailFront> GetAllEmail(int userId, string tenantId)
        {
            const string sql = @"select(select MQ.[TO]  [To], (select TITLE from NEW_MESSAGE M where MQ.MESSAGE_ID= M.MESSAGE_ID) Title,
								(select MESSAGE from NEW_MESSAGE M where MQ.MESSAGE_ID= M.MESSAGE_ID) Message,
								 (select UF.PATH+UF.FILENAME+UF.EXTENSION FilePath from NEW_UPLOAD_FILE UF where UF.UPLOAD_FILE_ID in (select MF.FILE_ID
								from NEW_MESSAGE_FILE MF where MF.MESSAGE_ID=MQ.MESSAGE_ID) for json path) [File] , MQ.CREATE_DATE CreateDate
								from NEW_MESSAGE_QUEUE MQ where SP_INFO_ID in (select SP_INFO_ID 
								from NEW_STORE_PROVIDER_INFO where PROVIDER_TYPE_ID!=1 and IS_ACTIVE=1 and TENANT_ID=@tenantId
								and EXISTS(SELECT* FROM dbo.GetEmployeePermission (@userId, @tenantId, 108 ) )) order by CREATE_DATE DESC for json path)Json";

            const string nameFromEmailSql = "select(select * from dbo.GetUsersMail( @pUserMail) for json path) Json";


            List<AllEmailBack> emailFromBack = new List<AllEmailBack>();
            List<AllEmailFront> emailToFront = new List<AllEmailFront>();
            try
            {
                var json = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@userId",SqlDbType.NVarChar,-1,ParameterDirection.Input,userId),
                        DbHandler.SetParameter("@tenantId",SqlDbType.NVarChar,-1,ParameterDirection.Input,tenantId)
                        });

                    while (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }

                }
                emailFromBack = JsonConvert.DeserializeObject<List<AllEmailBack>>(json);
                List<UserEmails> userDataList = new List<UserEmails>();
                if (emailFromBack == null)
                    throw new Exception("Mail yoxdur");
                foreach (var emailBack in emailFromBack)
                {
                    using (var con = new DbHandler())
                    {
                        var reader = con.ExecuteSql(nameFromEmailSql, new[]
                        {
                        DbHandler.SetParameter("@pUserMail",SqlDbType.NVarChar,-1,ParameterDirection.Input,emailBack.To)
                        });

                        while (reader.Read())
                        {
                            json = reader["Json"].ToString();
                        }
                    }
                    userDataList = JsonConvert.DeserializeObject<List<UserEmails>>(json);

                    AllEmailFront emailFront = new AllEmailFront();
                    emailFront.Title = emailBack.Title;
                    emailFront.Message = emailBack.Message;
                    emailFront.CreateDate = emailBack.CreateDate;
                    emailFront.UserEmail = userDataList;
                    emailFront.File = emailBack.File;
                    emailToFront.Add(emailFront);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;

            }
            return emailToFront;
        }
        #endregion

        #region Package

        public List<AllPackages> GetPackageById(int? packageId)
        {
            string sql = $@"select (select NAME Name, PRICE Price, MIN_VALUE MinValue, MAX_VALUE MaxValue 
                                from NEW_PACKET_CATEGORY where   {(packageId != null ? " PACKET_ID=@packageId and " : "")} IS_ACTIVE=1 for json path)Json";

            List<AllPackages> packages = new List<AllPackages>();
            try

            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {

                    var reader = con.ExecuteSql(sql, new[]
                    {
                          packageId!=null? DbHandler.SetParameter("@packageId",SqlDbType.Int,-1,ParameterDirection.Input,packageId):  DbHandler.SetParameter("@packageId",SqlDbType.Int,-1,ParameterDirection.Input,DBNull.Value)
                    });

                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                }

                packages = JsonConvert.DeserializeObject<List<AllPackages>>(json);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return packages;
        }

        public PackageHistory GetPackageHistory(int userId, string tenantId, bool? isNote = null, int? filterId = null)
        {
            string sql = $@"select(select PC.COMMON_MESSAGE_COUNT CommonCount, (PC.COMMON_MESSAGE_COUNT-PC.USED_MESSAGE_COUNT) RemainderCount,
                            (select  
                            PCI.CREATE_DATE CreatedDate, PCI.AMOUNT Amount,PCI.MESSAGE_COUNT MessageCount, PCI.IS_ACTIVE IsActive,
                            (case when PCI.PAYMENT_ID is null then 1
                            else  0 end) FromNote
                            from NEW_PACKET_COMPANY_INVOICE PCI 
                            {(isNote == true ? " where PCI.PAYMENT_ID is null " : "")}
                            {(isNote == false ? " where PCI.PAYMENT_ID is not null " : "")}
                            {(filterId == 1 ? " order by PCI.CREATE_DATE asc " : " order by PCI.CREATE_DATE desc ")}for json path) Packages
                            from NEW_PACKET_COMPANY PC where IS_ACTIVE=1 and PC.TEANAT_ID=@tenantId
                            and  EXISTS(SELECT* FROM dbo.GetEmployeePermission (@userId, @tenantId, 108 ) )
                            for json path,without_array_wrapper) Json";

            PackageHistory packages = new PackageHistory();
            try

            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {

                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@userId",SqlDbType.NVarChar,-1,ParameterDirection.Input,userId),
                        DbHandler.SetParameter("@tenantId",SqlDbType.NVarChar,-1,ParameterDirection.Input,tenantId)
                        });

                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                }

                packages = JsonConvert.DeserializeObject<PackageHistory>(json);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return packages;
        }

        public int InsertPackageForCompany(InsertPackage package, string tenantId, string userId)
        {
            if (package.CommonMessageCount==0)
                throw new Exception("Sms sayını daxil edin!");

            if (package.Amount == 0)
                throw new Exception("Qiyməti daxil edin!");

            if (package.CommonMessageCount < 100)
                throw new Exception("Sms sayı minimum 100 olmalıdır!");

            if (package.CommonMessageCount % 100 != 0)
                throw new Exception("Sms sayını yalnız 100-100 artıra bilərsiniz!");
            int paymentId;

            try
            {
                using (var con = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(package);
                    paymentId = con.ExecStoredProcWithReturnIntValue("[PacketCompanyInsertByStore]", new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson", SqlDbType.NVarChar, -1, ParameterDirection.Input, json),
                        DbHandler.SetParameter("@pTenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                }


            }
            catch (Exception ex)
            {
                Log.Warn("Could not insert package...");
                Log.Error(ex);
                throw;
            }
            return paymentId;
        }

        #endregion
    }
}