using CRMHalalBackEnd.App_Code;
using CRMHalalBackEnd.DB;
using CRMHalalBackEnd.Models.SmsVerification;
using CRMHalalBackEnd.SmsService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace CRMHalalBackEnd.Repository
{
    public class SmsVerificationRepository
    {
        private static readonly log4net.ILog Log =
           log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public SmsVerificationInfo Insert(SmsVerificationInfo model)
        {

            try
            {
                using(var conn =new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(model);
                    conn.ExecStoredProcWithReturnIntValue("SmsVrfInfoInsert", new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson",SqlDbType.NVarChar,-1,ParameterDirection.Input,json)
                    });
                }
            }catch(Exception ex)
            {
                Log.Warn("Could not UserInsUpdHelper...");
                Log.Error(ex);
                throw;
            }
            return model;
        }

        public SmsVerificationInfo Update(SmsVerificationInfo model)
        {

            try
            {
                using (var conn = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(model);
                    conn.ExecStoredProcWithReturnIntValue("SmsVerificationInfoUpd", new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson",SqlDbType.NVarChar,-1,ParameterDirection.Input,json)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not UserInsUpdHelper...");
                Log.Error(ex);
                throw;
            }
            return model;
        }
        public SmsVerificationInfo GetSmsInfoByUserGuid(string userGuid)
        {
            SmsVerificationInfo smsInfo = null;
            var userId = 0;
            var selectUserIdSql = @"select  [USER_ID] from NEW_USER where USER_GUID=@pUserGuid";
            var smsInfoSql = @"select top 1 * from SMS_VERIFICATION_INFO where USER_ID=@pUserId 
                           order by SMS_VRF_INFO_ID desc ";
            try
            {
                using(var conn=new DbHandler())
                {
                    var idRead=conn.ExecuteSql(selectUserIdSql, new[]
                     {
                        DbHandler.SetParameter("@pUserGuid",SqlDbType.NVarChar,50,ParameterDirection.Input,userGuid)
                    });
                    if (idRead.Read())
                    {
                        userId = idRead.GetInt("USER_ID");
                    }
                    idRead.Close();
                  var dr=conn.ExecuteSql(smsInfoSql, new[] {
                    DbHandler.SetParameter("@pUserId",SqlDbType.Int,10,ParameterDirection.Input,userId)
                    });
                    if (dr.Read())
                    {
                        smsInfo = new SmsVerificationInfo()
                        {
                            VerificationCode = dr.GetString("VERIFICATION_CODE"),
                            ExpiredDate = (DateTime)dr.GetDateTime("EXPIRED_DATE"),
                            UserGuid=userGuid
                        };
                    }
                }
                
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return smsInfo;
        }

        public bool SmsSend(string code,string number,string userGuid)
        {

            try
            {

                using (var conn = new DbHandler())
                {
                    conn.ExecStoredProcWithReturnIntValue("[VerifyNumber]", new[]
                      {
                        DbHandler.SetParameter("@pUserGuid",SqlDbType.VarChar,50,ParameterDirection.Input,userGuid),
                        DbHandler.SetParameter("@pNumber",SqlDbType.VarChar,50,ParameterDirection.Input,number)
                      });
                }
               
            }
            catch (Exception ex)
            {
                Log.Warn("VerifyNumber");
                Log.Error(ex);
                throw;
            }
            SendSms sendSms = new SendSms();
            
            ArrayOfString mobileNumbers = new ArrayOfString { number };
            string messageText = string.Format("Registration code: {0}", code);
            SendSms.SendSmsRequest smsRequest = new SendSms.SendSmsRequest();
            smsRequest.mobileNumber = mobileNumbers;
            smsRequest.messageText = messageText;
            try
            {
                ArrayOfString smsResponse = sendSms.sendSms(smsRequest);
            }
            catch(Exception ex)
            {
                //return false;

            }

            return true;
        }

    }
}