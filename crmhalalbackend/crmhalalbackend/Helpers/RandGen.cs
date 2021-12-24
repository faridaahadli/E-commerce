using CRMHalalBackEnd.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Helpers
{
    public static class RandGen
    {
        public static string Generate(int minLength, int maxLength)
        {
            string randCode = "";
            try
            {
                using (var conn = new DbHandler())
                {
                    randCode = conn.ExecStoredProcWithOutputValue("GenerateRandId", "@randomString", SqlDbType.VarChar, 50,
                        new[]{
                            DbHandler.SetParameter("@minLength",SqlDbType.Int,10,ParameterDirection.Input,minLength),
                            DbHandler.SetParameter("@maxLength",SqlDbType.Int,10,ParameterDirection.Input,maxLength)
                        });
                }

            }
            catch
            {

            }
            return randCode;
        }
    }
}