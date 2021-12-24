using CRMHalalBackEnd.DB;
using CRMHalalBackEnd.Models;
using ExcelDataReader.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Repository
{
    public class MessagePacketPaymentRepository
    {

        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public int InsertPaymentAuthorization(int userId, string transactionId, int paymentId)
        {
            int paymentAuthId = 0;
            try
            {
                using (var conn = new DbHandler())
                {
                    paymentAuthId = conn.ExecStoredProcWithReturnIntValue("PaymentAuthorizationInsert", new[]
                    {
                        DbHandler.SetParameter("@pTransactionId",SqlDbType.VarChar,200,ParameterDirection.Input,transactionId),
                        DbHandler.SetParameter("@pPaymentId",SqlDbType.Int,10,ParameterDirection.Input,paymentId),
                        DbHandler.SetParameter("@pLogUserId",SqlDbType.Int,10,ParameterDirection.Input,userId)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not InsertPaymentAuthorization...");
                Log.Error(ex);
                throw;
            }
            return paymentAuthId;
        }

        public PaymentDto GetPaymentId(int id)
        {
            string sql = @"SELECT
                            P.PAYMENT_ID,
                            P.AMOUNT,
                            P.CURRENCY,
                            U.LAST_LOGIN_IP
                                FROM
                            NEW_PAYMENT P
                            inner join NEW_USER U on U.USER_ID = P.USER_ID
                            WHERE
                            P.PAYMENT_ID = @paymentId";

            PaymentDto payment = null;

            try
            {
                using (var conn = new DbHandler())
                {
                    var reader = conn.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@paymentId",SqlDbType.Int,10,ParameterDirection.Input,id),

                    });
                    if (reader.Read())
                    {
                        payment = new PaymentDto()
                        {
                            PaymentId = int.Parse(reader["PAYMENT_ID"].ToString()),
                            Amount = decimal.Parse(reader["AMOUNT"].ToString()),
                            Currency = reader["CURRENCY"].ToString(),
                            IpAddress = reader["LAST_LOGIN_IP"].ToString(),
                        };
                    }


                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not GetPaymentId...");
                Log.Error(ex);
                throw;
            }

            return payment;
        }

        public string GetUserIpAddressByTrans(string id)
        {
            string sql = @"SELECT
	                        U.LAST_LOGIN_IP 
                        FROM
	                        NEW_PAYMENT_AUTHORIZATION PA
	                        INNER JOIN NEW_PAYMENT P ON p.PAYMENT_ID = PA.PAYMENT_ID
	                        INNER JOIN NEW_USER U ON U.USER_ID = P.USER_ID 
                        WHERE
	                        PA.TRANSACTION_ID  = @transId";


            string ipAddress = null;

            try
            {
                using (var conn = new DbHandler())
                {
                    var reader = conn.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@transId",SqlDbType.VarChar,200,ParameterDirection.Input,id),

                    });
                    if (reader.Read())
                    {
                        ipAddress = reader["LAST_LOGIN_IP"].ToString();
                    }


                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not GetUserIpAddressByTrans...");
                Log.Error(ex);
                throw;
            }

            return ipAddress;
        }

        public string GetTenantIdByTransId(string transId)
        {
            string sql = $@"SELECT P.TENANT_ID TenantId 
                                FROM
                            NEW_PAYMENT P
                            INNER JOIN NEW_PAYMENT_AUTHORIZATION PA ON PA.PAYMENT_ID = P.PAYMENT_ID
                            WHERE
                            PA.TRANSACTION_ID = @transId";
            string tenantId = null;
            try
            {
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@transId", SqlDbType.VarChar, 200, ParameterDirection.Input, transId)
                    });

                    if (reader.Read())
                    {
                        tenantId = reader["TenantId"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not GetStoreCertificateData...");
                Log.Error(ex);
                throw;
            }

            return tenantId;
        }


        public string UpdatePaymentAuthorization(PaymentCompleteResponse completeResponse, string transactionId)
        {
            string domain;
            try
            {
                using (var conn = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(completeResponse);
                    domain = conn.ExecStoredProcWithOutputValue("PaymentAuthorizationUpdateForMessage", "@pDomain", SqlDbType.VarChar, 63, new[]
                    {
                        DbHandler.SetParameter("@pTransactionId",SqlDbType.VarChar,200,ParameterDirection.Input,transactionId),
                        DbHandler.SetParameter("@pRequestAsJson",SqlDbType.NVarChar,-1,ParameterDirection.Input,json)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not PaymentAuthorizationUpdateForMessage...");
                Log.Error(ex);
                throw;
            }
            return domain;
        }

        public int GetMessageCountByTransId(string transId)
        {
            string sql = @" select distinct PCI.MESSAGE_COUNT MessageCount from NEW_PACKET_COMPANY_INVOICE PCI
							where PCI.PAYMENT_ID=(select PA.PAYMENT_ID from NEW_PAYMENT_AUTHORIZATION PA
							where PA.TRANSACTION_ID =@transId  )";
            int count = 0;
            try
            {
                using (var conn = new DbHandler())
                {
                    var reader = conn.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@transId",SqlDbType.VarChar,200,ParameterDirection.Input,transId),

                    });
                    if (reader.Read())
                    {
                        count = int.Parse(reader["MessageCount"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not GetMessageCountByTransId...");
                Log.Error(ex);
                throw;
            }

            return count;
        }

    }
}