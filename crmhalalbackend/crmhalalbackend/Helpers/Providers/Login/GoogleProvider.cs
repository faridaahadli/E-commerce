using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using CRMHalalBackEnd.Interfaces;
using Google.Apis.Auth;

namespace CRMHalalBackEnd.Helpers
{
    public class GoogleProvider : IProvider
    {
        public async Task<bool> TokenValidate(string token)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string>()
                           { "566532219091-r558i15mdlopb312ofbn134liolcq5fj.apps.googleusercontent.com" },
            };
            GoogleJsonWebSignature.Payload validPayload;

            string subject = null;
            try
            {
                validPayload = await GoogleJsonWebSignature.ValidateAsync(token, settings);

                subject = validPayload.Subject;
            }
            catch (InvalidJwtException e)
            {
                var message = e.Message;
                // Should probably log this.
                return false;
            }
            return true;
        }
    }
}