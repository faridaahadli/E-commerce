using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Castle.Core.Internal;
using CRMHalalBackEnd.DB;
using CRMHalalBackEnd.Models;
using CRMHalalBackEnd.Models.Payment;
using CRMHalalBackEnd.Models.Store;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using WebGrease.Css.Extensions;

namespace CRMHalalBackEnd.Repository
{
    public class PaymentRepository
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //public int InsertPayment(string domain, int userId, int orderId, decimal amount, int paymentMethodId)
        //{
        //    int paymentId = 0;
        //    try
        //    {
        //        using (var conn = new DbHandler())
        //        {
        //            paymentId = conn.ExecStoredProcWithReturnIntValue("PaymentOnlineCreate", new[]
        //            {
        //                DbHandler.SetParameter("@pDomain",SqlDbType.VarChar,63,ParameterDirection.Input,domain.Replace('_','.')),
        //                DbHandler.SetParameter("@pPaymentMethodId",SqlDbType.Int,10,ParameterDirection.Input,paymentMethodId),
        //                DbHandler.SetParameter("@pOrderId",SqlDbType.Int,10,ParameterDirection.Input,orderId),
        //                DbHandler.SetParameter("@pAmount",SqlDbType.Decimal,-1,ParameterDirection.Input,amount),
        //                DbHandler.SetParameter("@pLogUserId",SqlDbType.Int,10,ParameterDirection.Input,userId)
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Warn("Could not InsertPayment...");
        //        Log.Error(ex);
        //        throw;
        //    }
        //    return paymentId;
        //}

        public int InsertPayment(string domain, int userId, int orderId)
        {
            int paymentId = 0;
            try
            {
                using (var conn = new DbHandler())
                {
                    paymentId = conn.ExecStoredProcWithReturnIntValue("PaymentInsert", new[]
                    {
                        DbHandler.SetParameter("@pOrderId",SqlDbType.Int,10,ParameterDirection.Input,orderId),
                        DbHandler.SetParameter("@pDomain",SqlDbType.VarChar,63,ParameterDirection.Input,domain),
                        DbHandler.SetParameter("@pLogUserId",SqlDbType.Int,10,ParameterDirection.Input,userId)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not InsertPayment...");
                Log.Error(ex);
                throw;
            }
            return paymentId;
        }
        public PaymentDto GetPaymentId(string lang, int id)
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
                            Language = ("az".Equals(lang) || "en".Equals(lang) || "ru".Equals(lang)) ? lang : "en"
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

        public string UpdatePaymentAuthorization(PaymentCompleteResponse completeResponse, string transactionId)
        {
            string domain;
            try
            {
                using (var conn = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(completeResponse);
                    domain = conn.ExecStoredProcWithOutputValue("PaymentAuthorizationUpdate", "@pDomain", SqlDbType.VarChar, 63, new[]
                    {
                        DbHandler.SetParameter("@pTransactionId",SqlDbType.VarChar,200,ParameterDirection.Input,transactionId),
                        DbHandler.SetParameter("@pRequestAsJson",SqlDbType.NVarChar,-1,ParameterDirection.Input,json)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not UpdatePaymentAuthorization...");
                Log.Error(ex);
                throw;
            }
            return domain;
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

        public PaymentResponse GetPaymentDetailsByOrderId(int langId,int orderId)
        {
            string sql = $@"SELECT
	                        (
	                        SELECT
		                        ( U.FIRST_NAME + ' ' + U.LAST_NAME ) UserFullName,
		                        U.EMAIL UserEmail,
		                        ( SELECT C.[TEXT] FROM NEW_CONTACT C WHERE C.USER_ID = U.USER_ID AND C.IS_ACTIVE= 1 AND C.CONTACT_TYPE_ID= 2 ) UserPhone,
		                        (
		                        SELECT
			                        A.ADDRESS 
		                        FROM
			                        NEW_ADDRESS A 
		                        WHERE
			                        A.ADDRESS_ID = (
			                        SELECT
				                        CDD.DELIVERY_ADDRESS_ID 
			                        FROM
				                        NEW_COMMON_DELIVERY_DATA CDD 
			                        WHERE
				                        CDD.COMMON_DELIVERY_DATA_ID = ( SELECT TOP 1 OD.COMMON_DELIVERY_DATA_ID FROM NEW_ORDER_DELIVERY OD WHERE OD.ORDER_ID= SO.SO_ID ) 
			                        ) 
		                        ) DeliveryAddress,
		                        ( SELECT MAX ( OD.DELIVERY_TIME ) FROM NEW_ORDER_DELIVERY OD WHERE OD.ORDER_ID= SO.SO_ID ) DeliveryTime,
		                        (
		                        SELECT
                                    P.PAYMENT_METHOD_ID PaymentTypeId,
			                        P.AMOUNT PaymentAmount,
			                        ( SELECT
	                                    PMT.NAME 
	                                    FROM
		                                    NEW_PAYMENT_METHOD PM
		                                    INNER JOIN NEW_PAYMENT_MET_TRANSLATE PMT ON PMT.PAYMENT_METHOD_ID = PM.PAYMENT_METHOD_ID 
	                                    WHERE
		                                    PM.PAYMENT_METHOD_ID  = P.PAYMENT_METHOD_ID 
	                                    AND PMT.LANGUAGE_ID = { langId } 
	                                    ) PaymentType,
			                        ( SELECT PA.RESPONSE_MESSAGE FROM NEW_PAYMENT_AUTHORIZATION PA WHERE PA.PAYMENT_ID = P.PAYMENT_ID ) PaymentResponseMessage,
			                        ( SELECT PA.RESPONSE_CODE FROM NEW_PAYMENT_AUTHORIZATION PA WHERE PA.PAYMENT_ID = P.PAYMENT_ID ) PaymentResponseCode 
		                        FROM
			                        NEW_PAYMENT P 
		                        WHERE
			                        P.PAYMENT_ID IN ( SELECT PI.PAYMENT_ID FROM NEW_PAYMENT_INVOICE PI WHERE PI.INVOICE_ID IN ( SELECT INVOICE_ID FROM NEW_SALES_ORDER_PROCESS SOR WHERE SOR.SO_ID = SO.SO_ID ) ) FOR json path 
		                        ) PaymentDetails 
	                        FROM
		                        NEW_SALES_ORDER SO
		                        INNER JOIN NEW_USER U ON U.USER_ID = SO.USER_ID 
	                        WHERE
	                        SO.SO_ID = @orderId FOR json path, without_array_wrapper
	                        ) Json";
            PaymentResponse paymentResponse = null;
            string json = String.Empty;
            try
            {
                using (var conn = new DbHandler())
                {
                    var reader = conn.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@orderId",SqlDbType.Int,10,ParameterDirection.Input,orderId)
                    });

                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }

                    paymentResponse = JsonConvert.DeserializeObject<PaymentResponse>(json);

                    var paymentGroupBy = paymentResponse?.PaymentDetails.GroupBy(a => a.PaymentType);

                    List<PaymentDetails> detailsList = new List<PaymentDetails>();

                    foreach (var paymentDetails in paymentGroupBy)
                    {
                        PaymentDetails paymentDetail = new PaymentDetails();
                        paymentDetail.PaymentTypeId = paymentDetails.First().PaymentTypeId;
                        paymentDetail.PaymentType = paymentDetails.First().PaymentType;
                        paymentDetail.PaymentAmount = paymentDetails.Sum(a => a.PaymentAmount);
                        paymentDetail.PaymentResponseCode = paymentDetails.First().PaymentResponseCode;
                        paymentDetail.PaymentResponseMessage = paymentDetails.First().PaymentResponseMessage;
                        detailsList.Add(paymentDetail);
                    }

                    paymentResponse.PaymentDetails = detailsList;
                    var paymentDetailsEnumerable = paymentResponse.PaymentDetails.FirstOrDefault(x => x.PaymentTypeId == 3);

                    if (paymentDetailsEnumerable == null)
                    {
                        paymentResponse.PaymentMessage = "Sifarişiniz üçün təşəkkür edirik, sifariş haqqında məlumatı aşağıda əldə edə bilərsiniz.";
                        paymentResponse.PaymentStatus = true;
                    }
                    else
                    {
                        switch (paymentDetailsEnumerable.PaymentResponseMessage)
                        {
                            case "OK":
                                switch (paymentDetailsEnumerable.PaymentResponseCode)
                                {
                                    case "000":
                                        paymentResponse.PaymentMessage = "Sifarişiniz üçün təşəkkür edirik, sifariş haqqında məlumatı aşağıda əldə edə bilərsiniz.";
                                        paymentResponse.PaymentStatus = true;
                                        break;
                                    default:
                                        paymentResponse.PaymentMessage = "Əməliyyat uğursuz oldu. Yenidən yoxlayın.";
                                        paymentResponse.PaymentStatus = false;
                                        break;
                                }

                                break;
                            case "FAILED":
                                switch (paymentDetailsEnumerable.PaymentResponseCode)
                                {
                                    case "116":
                                        paymentResponse.PaymentMessage = "Hesabınızda kifayət qədər məbləğ yoxdur.";
                                        paymentResponse.PaymentStatus = false;
                                        break;
                                    case "129":
                                        paymentResponse.PaymentMessage = "Bank kartının vaxtı bitmişdir.";
                                        paymentResponse.PaymentStatus = false;
                                        break;
                                    default:
                                        paymentResponse.PaymentMessage = "Əməliyyat uğursuz oldu. Yenidən yoxlayın.";
                                        paymentResponse.PaymentStatus = false;
                                        break;
                                }

                                break;
                            default:
                                {
                                    if (!paymentDetailsEnumerable.PaymentResponseMessage.IsNullOrEmpty())
                                    {
                                        paymentResponse.PaymentMessage = "Əməliyyat uğursuz oldu. Yenidən yoxlayın.";
                                        paymentResponse.PaymentStatus = false;

                                    }

                                    break;
                                }
                        }
                    }




                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not GetPaymentDetailsByOrderId...");
                Log.Error(ex);
                throw;
            }
            return paymentResponse;
        }

        public int GetOrderIdByTransId(string transId)
        {
            string sql = @"SELECT
	                        distinct SOR.SO_ID 
                        FROM
	                        NEW_SALES_ORDER_PROCESS SOR 
                        WHERE
	                        SOR.INVOICE_ID = ( SELECT PI.INVOICE_ID FROM NEW_PAYMENT_INVOICE PI WHERE PI.PAYMENT_ID =
                            ( SELECT PA.PAYMENT_ID FROM NEW_PAYMENT_AUTHORIZATION PA WHERE PA.TRANSACTION_ID = @transId ) )";
            int paymentId = 0;
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
                        paymentId = int.Parse(reader["SO_ID"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not GetOrderIdByTransId...");
                Log.Error(ex);
                throw;
            }

            return paymentId;
        }

        //public int InsertPaymentCash(string tenantId, int userId, int orderId, int paymentMethodId)
        //{
        //    int paymentId = 0;
        //    try
        //    {
        //        using (var conn = new DbHandler())
        //        {
        //            paymentId = conn.ExecStoredProcWithReturnIntValue("PaymentCashCreate", new[]
        //            {
        //                DbHandler.SetParameter("@pOrderId",SqlDbType.Int,10,ParameterDirection.Input,orderId),
        //                DbHandler.SetParameter("@pPaymentMethodId",SqlDbType.Int,10,ParameterDirection.Input,paymentMethodId),
        //                DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
        //                DbHandler.SetParameter("@pLogUserId",SqlDbType.Int,10,ParameterDirection.Input,userId)
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Warn("Could not InsertPaymentCash...");
        //        Log.Error(ex);
        //        throw;
        //    }
        //    return paymentId;
        //}

        public StoreCertificateData GetStoreCertificateData(string tenantId)
        {
            string sql = "SELECT * FROM NEW_STORE_BANK_CERTIFICATION WHERE TENANT_ID = @tenantId";
            StoreCertificateData certificateData = null;
            try
            {
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@tenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId)
                    });

                    if (reader.Read())
                    {
                        certificateData = new StoreCertificateData()
                        {
                            TenantId = reader["TENANT_ID"].ToString(),
                            CertificatePath = reader["CERTIFICATE_PATH"].ToString(),
                            CertificatePassword = reader["CERTIFICATE_PASSWORD"].ToString()
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not GetStoreCertificateData...");
                Log.Error(ex);
                throw;
            }

            return certificateData;
        }

        public string GetTenantIdByTransId(string transId)
        {
            string sql = $@"SELECT  P.TENANT_ID TenantId  
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
    }
}