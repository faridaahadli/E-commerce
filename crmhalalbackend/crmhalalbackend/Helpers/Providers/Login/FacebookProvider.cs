using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using CRMHalalBackEnd.Interfaces;
using Newtonsoft.Json;

namespace CRMHalalBackEnd.Helpers
{
    public class FacebookProvider : IProvider
    {
        public async Task<bool> TokenValidate(string token)
        {
            HttpClient request = new HttpClient();
            string urlFB = $"https://graph.facebook.com/debug_token?input_token={token}&access_token=363972818571603|ecdecb206b616d5e19ffce10c10ccb60";
          
            var result = await request.GetAsync(urlFB);

            var responseFb = await result.Content.ReadAsStringAsync();
            dynamic obj = JsonConvert.DeserializeObject<dynamic>(responseFb);

            if (!(bool)obj.data.is_valid)
                return false;
            return true;
        }
    }
}