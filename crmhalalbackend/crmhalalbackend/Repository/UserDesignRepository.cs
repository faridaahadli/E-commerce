using CRMHalalBackEnd.DB;
using CRMHalalBackEnd.Models.UserDesign;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Repository
{
	public class UserDesignRepository
	{
		private static readonly log4net.ILog Log =
		log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public List<DesignResponse> Insert(DesignDto design, string tenantId,int userId)
        {
            DesignDto designDto;
            var designId = 0;
            try
            {

                using (var con = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(design);
                    designId = con.ExecStoredProcWithReturnIntValue("[UserDesignInsert]", new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson",SqlDbType.NVarChar,-1,ParameterDirection.Input,json),
                        DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                }


            }
            catch (Exception ex)
            {
                Log.Warn("Could not UserDesignInsert...");
                Log.Error(ex);
                throw;
            }
            return GetDesignById(tenantId) ;
        }

        public List<DesignResponse> GetDesignById(string tenantId)
        {

            string sql = @"SELECT(SELECT  [USER_DESIGN_ID] AS UserDesignId
                                         ,[TENANT_ID] AS TenantId
                                         ,[JSON_DATA] AS JsonData
                                         ,[STATUS] AS Status
                                         ,[CREATE_DATE] AS CreateDate
                                          FROM USER_DESIGN
                                          WHERE TENANT_ID = @tenantId for json path) Json";


            List<DesignResponse> designDto = null;
            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@tenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId)
                    });

                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                }
                
                designDto = JsonConvert.DeserializeObject<List<DesignResponse>>(json);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return designDto;
        }
        
        public List<UserDesignGet> GetDesign(string domian)
        {
            const string sql =
                @"SELECT
	                (
	                SELECT USER_DESIGN_ID AS UserDesignId, 
                           TENANT_ID AS TenantId,
                           JSON_DATA AS JsonData 
                           FROM USER_DESIGN 
                           WHERE STATUS=1 AND TENANT_ID=(select TENANT_ID from NEW_STORE where DOMAIN = @domain) FOR JSON PATH
	                ) Json";
            List<UserDesignGet> design = null;

            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@domain",SqlDbType.NVarChar,50,ParameterDirection.Input,domian)
                    });
                    design = new List<UserDesignGet>();
                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                        design = JsonConvert.DeserializeObject<List<UserDesignGet>>(json);
                        design = design ?? new List<UserDesignGet>();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
            return design; 
        }
        public List<UserDesignGet> GetDesignAdmin(string tenantId)
        {
            const string sql =
                @"SELECT
	                (
	                SELECT USER_DESIGN_ID AS UserDesignId, 
                           TENANT_ID AS TenantId,
                           JSON_DATA AS JsonData 
                           FROM USER_DESIGN 
                           WHERE STATUS=1 AND TENANT_ID=@tenantId FOR JSON PATH
	                ) Json";
            List<UserDesignGet> design = null;

            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@tenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId)
                    });
                    design = new List<UserDesignGet>();
                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                        design = JsonConvert.DeserializeObject<List<UserDesignGet>>(json);
                        design = design ?? new List<UserDesignGet>();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
            return design;
        }
        public int DeleteDesign(int designId,  string tenantId, int userId)  ///duzeltmelisen!
        {
            int returndesignId = 0;
            try
            {
                using (var con = new DbHandler())
                {
                    returndesignId = con.ExecStoredProcWithReturnIntValue("UserDesignDelete", new[]
                    {
                        DbHandler.SetParameter("@pUserDesignId", SqlDbType.Int, 10, ParameterDirection.Input, designId),
                       DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });

                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not UserDesignDelete...");
                Log.Error(ex);
                throw;
            }

            return returndesignId;
        }


    }
}