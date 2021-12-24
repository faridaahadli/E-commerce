using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace CRMHalalBackEnd.Helpers
{
    public class CreateSubDomain
    {
        private static string _key { get; } = "08bda9f5-df29-45d7-8330-1df227db0398";
        private static string _url { get; } = "https://api.dnsmadeeasy.com/V2.0/dns/managed/7564488/records/";
        private static string _apiKey { get; } = "0e66156b-024d-4fa2-840b-66ee644c0ada";

        public static async Task<string> CreateDomain(string domain)
        {

            var time = DateTime.UtcNow.ToString("r", DateTimeFormatInfo.InvariantInfo);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var hash = HashString(time, _key);


            HttpWebRequest request = WebRequest.CreateHttp(_url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers.Add("Access-Control-Allow-Origin", "*");
            request.Headers.Add("x-dnsme-apiKey", _apiKey);
            request.Headers.Add("x-dnsme-hmac", hash);
            request.Headers.Add("x-dnsme-requestDate", time);
            var obj = new { name = domain, value = "157.90.55.172", ttl = "1800", type = "A" };
            var json = JsonConvert.SerializeObject(obj);
            var data = Encoding.UTF8.GetBytes(json);
            using (Stream stream = request.GetRequestStream())
            {
                await stream.WriteAsync(data, 0, data.Length);
            }

            var res = await request.GetResponseAsync();
            string resStr;
            using (StreamReader sr = new StreamReader(res.GetResponseStream()))
            {
                //Need to return this response 
                resStr = await sr.ReadToEndAsync();
            }

            return resStr;
        }

        public static async Task<string> GetDomainByName(string domain)
        {

            var time = DateTime.UtcNow.ToString("r", DateTimeFormatInfo.InvariantInfo);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var hash = HashString(time, _key);


            HttpWebRequest request = WebRequest.CreateHttp(_url+ "?type=A&_search=true&nd=1627300317484&rows=10&page=1&sidx=name&sord=asc&filters= "+HttpUtility.UrlDecode("{\"groupOp\":\"AND\",\"rules\":[{\"field\":\"name\",\"op\":\"eq\",\"data\":\""+domain+"\"}]}") +"&searchField=&searchString=&searchOper=&_=1627300317485");
            request.Method = "Get";
            request.ContentType = "application/json";
            request.Headers.Add("Access-Control-Allow-Origin", "*");
            request.Headers.Add("x-dnsme-apiKey", _apiKey);
            request.Headers.Add("x-dnsme-hmac", hash);
            request.Headers.Add("x-dnsme-requestDate", time);

            var res = await request.GetResponseAsync();
            string resStr;
            using (StreamReader sr = new StreamReader(res.GetResponseStream()))
            {
                //Need to return this response 
                resStr = await sr.ReadToEndAsync();
            }

            return resStr;
        }

        public static string HashString(string stringToHash, string hachKey)
        {
            System.Text.UTF8Encoding myEncoder = new System.Text.UTF8Encoding();
            byte[] Key = myEncoder.GetBytes(hachKey);
            byte[] Text = myEncoder.GetBytes(stringToHash);
            HMACSHA1 myHMACSHA1 = new HMACSHA1(Key);
            byte[] HashCode = myHMACSHA1.ComputeHash(Text);
            string hash = BitConverter.ToString(HashCode).Replace("-", "");
            return hash.ToLower();
        }
    }
}