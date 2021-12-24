using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using CRMHalalBackEnd.Models.Employee;

namespace CRMHalalBackEnd.Helpers
{
    public class EmailSend
    {

        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                SmtpClient smtp = new SmtpClient("smtp.yandex.com", 587);
                smtp.UseDefaultCredentials = false;
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential("info@note.az", "info456456456");
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress("info@note.az");
                msg.To.Add(new MailAddress(email));
                msg.IsBodyHtml = true;
                msg.Subject = subject;
                msg.Body = message;
                await smtp.SendMailAsync(msg);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public static void SendEmail(string email, string subject, string message)
        {
            try
            {
                SmtpClient smtp = new SmtpClient("smtp.yandex.com", 587);
                smtp.UseDefaultCredentials = false;
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential("info@note.az", "info456456456");
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress("info@note.az");
                msg.To.Add(new MailAddress(email));
                msg.IsBodyHtml = true;
                msg.Subject = subject;
                msg.Body = message;
                smtp.Send(msg);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public static void SendEmail(List<EmployeeUserData> data)
        {
            foreach (var k in data)
            {
                EmailSend.SendEmail(k.UserEmail, k.StoreName + " - Yeni sifariş",
                    k.StoreName + " - Yeni sifirişiniz var.");
            }
        }

        public static string SendEmailCheck(string email, string subject, string message, string host=null, int port=0, string userName=null, string password=null )
        {
            try
            {
                SmtpClient smtp = new SmtpClient(host, port);
                smtp.UseDefaultCredentials = false;
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential(userName, password);
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(userName);
                msg.To.Add(new MailAddress(email));
                msg.IsBodyHtml = true;
                msg.Subject = subject;
                msg.Body = message;
                smtp.Send(msg);
            }
            catch (Exception ex)
            {

                Log.Warn("Could not send mail...");
                Log.Error(ex);

                if (ex.Message == "The SMTP server requires a secure connection or the client was not authenticated. The server response was: 5.7.0 Authentication Required. Learn more at")
                    throw new Exception ("Zəhmət olmazsa, daxil etdiyiniz email adresinizin \"Less secure app access\" funksionallığını aktiv edin.");

                //Zəhmət olmazsa, daxil etdiyiniz email adresinizin \"Less secure app access\" funksionallığını aktiv edin.
            }
            return "mail gonderildi";
        }
    }
}