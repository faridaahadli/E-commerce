using System;
using CRMHalalBackEnd.Models.Module;
using System.Collections.Generic;
using CRMHalalBackEnd.DB;
using CRMHalalBackEnd.Models.Faq;

namespace CRMHalalBackEnd.Repository
{
    public class CommonRepository
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public List<Module> GetModuleList()
        {
            const string sql =
                @"SELECT [MOD_ID]
                  ,[TITLE]
                  ,[STATUS]
                  ,[CREATE_DATE]
              FROM [MODULES] where [STATUS]=1";

            var list = new List<Module>();
            try
            {
                using (var con = new DbHandler())
                {
                    var dr = con.ExecuteSql(sql);
                    while (dr.Read())
                    {
                        var module = new Module()
                        {
                            
                            ModId = dr.GetInt("MOD_ID"),
                            Title = dr["TITLE"].ToString(),
                            Status = dr.GetInt("STATUS")
                        };
                        list.Add(module);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }

            return list;
        }
    }
}