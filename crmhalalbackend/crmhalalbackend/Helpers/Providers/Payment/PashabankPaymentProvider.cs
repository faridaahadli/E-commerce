﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Hosting;
using Castle.Core.Internal;
using CRMHalalBackEnd.Interfaces;
using CRMHalalBackEnd.Models;
using CRMHalalBackEnd.Models.Employee;
using CRMHalalBackEnd.Models.Store;
using CRMHalalBackEnd.Repository;

namespace CRMHalalBackEnd.Helpers
{
    public class PashabankPaymentProvider:IPaymentProvider
    {

        private readonly string MERCHANT_HANDLER = System.Configuration.ConfigurationManager.AppSettings["MERCHANT_HANDLER"];
        private readonly string CLIENT_HANDLER = System.Configuration.ConfigurationManager.AppSettings["CLIENT_HANDLER"];


        private readonly PaymentRepository _repository = new PaymentRepository();
        private readonly EmployeeRepository _repositoryEmployee = new EmployeeRepository();

        // Method to read response from ECOMM module and to give an acceptable look to the response
        private string readResponsePost(Stream stream)
        {
            string response = null;
            using (StreamReader reader = new StreamReader(stream))
            {
                string line = reader.ReadLine();
                while (line != null)
                {
                    response += line;
                    line = reader.ReadLine();
                }
            }
            return response;
        }
        private string readResponseComplete(Stream stream)
        {
            string response = null;
            using (StreamReader reader = new StreamReader(stream))
            {
                do
                {
                    response += Convert.ToChar(reader.Read());
                } while (reader.Peek() >= 0);
            }
            return response;
        }
        public string GetPaymentPageUrl(string lang, int paymentId,int userId,string tenantId)
        {
            PaymentDto paymentData = _repository.GetPaymentId(lang,paymentId);
            StoreCertificateData certificateData = _repository.GetStoreCertificateData(tenantId);
            string X509_CERTIFICATE_FILE = HostingEnvironment.MapPath(certificateData.CertificatePath);
            StringBuilder sb = new StringBuilder(MERCHANT_HANDLER)
           .Append("command=v")
           .Append("&amount=").Append((paymentData.Amount * 100).ToString("############"))
           .Append("&currency=").Append(paymentData.Currency.ToUpper().Equals("AZN") ? "944" : "")
           .Append("&msg_type=DMS")
           .Append("&client_ip_addr=").Append(paymentData.IpAddress);

            if (!string.IsNullOrEmpty(paymentData.Language))
            {
                sb.Append("&language=").Append(paymentData.Language);
            }

            // Calling ECOMM module
            X509Certificate2Collection certificate = new X509Certificate2Collection();
            certificate.Import(X509_CERTIFICATE_FILE, certificateData.CertificatePassword,
                X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(sb.ToString());
            req.AllowAutoRedirect = true;
            req.ClientCertificates = certificate;
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            Stream postStream = req.GetRequestStream();

            // Xeta =>'The underlying connection was closed: An unexpected error occurred on a send.'
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();

            // Response from ECOMM module.
            // example of returned response:
            // TRANSACTION_ID: TwXcbhBgrIsMY0A7s982nx/pSzE=
            string res = readResponsePost(stream);
            stream.Close();
            string transId = res.Split(' ')[1];
            // Compiling a query in ClientHandler of the ECOMM module, trans_id field
            // may contain special characters, they must be escaped for URL
            _repository.InsertPaymentAuthorization(userId, transId, paymentId);
            sb = new StringBuilder(CLIENT_HANDLER)
                .Append("trans_id=")
                .Append(HttpUtility.UrlEncode(transId));
            return sb.ToString();
        }

        public string Complete(string transId)
        {
            // Compiling a request on the MerchantHandler of the ECOMM module
            string ipAddress = _repository.GetUserIpAddressByTrans(transId);
            string tenantId = _repository.GetTenantIdByTransId(transId);
            StoreCertificateData certificateData = _repository.GetStoreCertificateData(tenantId);

            string X509_CERTIFICATE_FILE = HostingEnvironment.MapPath(certificateData.CertificatePath);
            StringBuilder sb = new StringBuilder(MERCHANT_HANDLER)
                .Append("command=c")
                .Append("&trans_id=").Append(HttpUtility.UrlEncode(transId))
                .Append("&client_ip_addr=").Append(ipAddress);

            // Calling the ECOMM module
            X509Certificate2Collection certificate = new X509Certificate2Collection();
            certificate.Import(X509_CERTIFICATE_FILE, certificateData.CertificatePassword,
                X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(sb.ToString());
            req.AllowAutoRedirect = true;
            req.ClientCertificates = certificate;
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            Stream postStream = req.GetRequestStream();
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();

            string response = readResponseComplete(stream);
            stream.Close();

            // Adding parameters to the dictionary
            PaymentCompleteResponse completeResponse = new PaymentCompleteResponse();
            foreach (string paramValue in response.Split(new char[] { Convert.ToChar(0x0a) }))
            {
                string param = paramValue.Split(':')[0];
                switch (param)
                {
                    case "RESULT":
                        completeResponse.RESULT = paramValue.Split(':')[1].Substring(1);
                        break;
                    case "RESULT_PS":
                        completeResponse.RESULT_PS = paramValue.Split(':')[1].Substring(1);
                        break;
                    case "RESULT_CODE":
                        completeResponse.RESULT_CODE = paramValue.Split(':')[1].Substring(1);
                        break;
                    case "3DSECURE":
                        completeResponse.ThreeDSecure = paramValue.Split(':')[1].Substring(1);
                        break;
                    case "RRN":
                        completeResponse.RRN = paramValue.Split(':')[1].Substring(1);
                        break;
                    case "APPROVAL_CODE":
                        completeResponse.APPROVAL_CODE = paramValue.Split(':')[1].Substring(1);
                        break;
                    case "CARD_NUMBER":
                        completeResponse.CARD_NUMBER = paramValue.Split(':')[1].Substring(1);
                        break;
                    case "RECC_PMNT_ID":
                        completeResponse.RECC_PMNT_ID = paramValue.Split(':')[1].Substring(1);
                        break;
                    case "RECC_PMNT_EXPIRY":
                        completeResponse.RECC_PMNT_EXPIRY = paramValue.Split(':')[1].Substring(1);
                        break;
                }
            }
            string domain = _repository.UpdatePaymentAuthorization(completeResponse, transId);
            int orderId = _repository.GetOrderIdByTransId(transId);
            if (completeResponse.RESULT.Equals("OK"))
            {
                List<EmployeeUserData> employeeEmail = _repositoryEmployee.GetEmployeeEmailForOrder(orderId);
                EmailSend.SendEmail(employeeEmail);
            }
            domain = domain.IsNullOrEmpty() ? System.Configuration.ConfigurationManager.AppSettings["Note Home Page"] : domain;
            var redirectUrl = new StringBuilder("https://" + domain);
            redirectUrl.Append("/payment-status?orderId=").Append(orderId);
            return redirectUrl.ToString();
        }
    }
}