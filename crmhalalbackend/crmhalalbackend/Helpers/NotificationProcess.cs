using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CRMHalalBackEnd.Models.Notification;
using Newtonsoft.Json;

namespace CRMHalalBackEnd.Helpers
{
    public static class NotificationProcess
    {
        public static async Task SendNotification(NotificationDto notification,HttpRequestMessage requestMessage)
        {
            string url = System.Configuration.ConfigurationManager.AppSettings["Notification API"] + "/note/notification/Notification/CreateNotification";
            var json = JsonConvert.SerializeObject(notification);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = "POST";
            request.ContentType = "application/json; charset=UTF-8";

            request.Headers.Add("Authorization:" + requestMessage.Headers.Authorization.Scheme + " " + requestMessage.Headers.Authorization.Parameter);
            using (StreamWriter stream = new StreamWriter(request.GetRequestStream()))
            {
                await stream.WriteAsync(json);
            }
            await request.GetResponseAsync();

        }

        public static async Task ConfirmNotification(int notificationId,bool confirm, HttpRequestMessage requestMessage)
        {
            string url = System.Configuration.ConfigurationManager.AppSettings["Notification API"] + $"/note/notification/Notification/PostUserNotificationUpdateConfirm?notificationId={notificationId}&confirmed={confirm}";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = "POST";
            request.ContentType = "application/json; charset=UTF-8";

            request.Headers.Add("Authorization:" + requestMessage.Headers.Authorization.Scheme + " " + requestMessage.Headers.Authorization.Parameter);
            using (StreamWriter stream = new StreamWriter(request.GetRequestStream()))
            {
                await stream.WriteAsync("{}");
            }

            await request.GetResponseAsync();
        }

    }
}