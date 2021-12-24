using CRMHalalBackEnd.Models;
using System.Collections.Generic;

namespace CRMHalalBackEnd.App_Code
{
    public class ErrorMappingClass
    {
        private Dictionary<string, string> errorMapping = new Dictionary<string, string>()
        {
            { "0000", "OK" },
            { "0001", "General Error" },
            { "0002", "Database Error" },
            { "0003", "Authorization Error" },
            { "9999", "System malfunction"}
        };
        
        public UserResponseModel errorDescription(string errorCode)
        {

            UserResponseModel exceptionModel = new UserResponseModel();
           /* exceptionModel.response_code = errorCode;
            if (errorMapping.ContainsKey(errorCode))
                exceptionModel.response_text = errorMapping[errorCode];
            else
                exceptionModel.response_text = errorMapping["9999"];*/
            return exceptionModel;
        }
    }
}