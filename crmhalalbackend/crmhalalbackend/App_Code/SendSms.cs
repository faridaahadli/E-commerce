using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using CRMHalalBackEnd.SmsService;

namespace CRMHalalBackEnd.App_Code
{
    public class SendSms
    {
        private string username = "note.az_service";
        private string password = "note678";
        public class SendSmsRequest
        {
            public ArrayOfString mobileNumber { get; set; }
            public string messageText { get; set; }
        }
        public ArrayOfString SendSmsResponse;

        public ArrayOfString sendSms(SendSmsRequest sendSmsRequest)
        {
            smsserviceSoapClient SmsserviceSoapClient = new smsserviceSoapClient();
          
            return SendSmsResponse = SmsserviceSoapClient.SmsInsert_1_N(username, password, null, null, sendSmsRequest.mobileNumber, sendSmsRequest.messageText);
        }
        public async Task<SmsInsert_1_NResponse> sendSMSAsync(SendSmsRequest sendSmsRequest)
        {
            smsserviceSoapClient SmsserviceSoapClient = new smsserviceSoapClient();
            return await SmsserviceSoapClient.SmsInsert_1_NAsync(username, password, null, null, sendSmsRequest.mobileNumber, sendSmsRequest.messageText);
        }
        public ArrayOfString checkSms(ArrayOfString messageId)
        {
            smsserviceSoapClient SmsserviceSoapClient = new smsserviceSoapClient();
            return SendSmsResponse = SmsserviceSoapClient.SmsStatus(username, password, messageId);
        }
    }
}