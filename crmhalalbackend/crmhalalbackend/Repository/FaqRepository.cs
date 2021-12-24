using System;
using CRMHalalBackEnd.Models.Faq;
using System.Collections.Generic;
using System.Data;
using CRMHalalBackEnd.DB;
using Newtonsoft.Json;

namespace CRMHalalBackEnd.repository
{
    public class FaqRepository
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public FaqDto GetFaqById(int faqId, string tenantId)
        {
            const string sql =
                @"SELECT
	                (
	                SELECT
		                F.[FAQ_ID] AS FaqId,
		                F.[MOD_ID] AS ModId,
		                F.[QUESTION_TITLE] AS Question,
		                F.[QUESTION_TITLE2] AS Question2,
		                F.[QUESTION_TITLE3] AS Question3,
		                F.[QUESTION_TITLE4] AS Question4,
		                F.[ANSWER] AS Answer,
		                F.[ANSWER2] AS Answer2,
		                F.[ANSWER3] AS Answer3,
		                F.[ANSWER4] AS Answer4 
	                FROM
		                [FAQ] F 
	                WHERE
		                F.[STATUS] = 1 
		                AND @tenantId = ( SELECT M.TENANT_ID FROM MODULES M where M.MOD_ID = F.MOD_ID )
	                AND F.FAQ_ID = @faqId FOR json path, without_array_wrapper 
	                ) Json";

            FaqDto faq = null;
            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var dr = con.ExecuteSql(sql,
                        new[]
                        {
                            DbHandler.SetParameter("@faqId", SqlDbType.Int, 10, ParameterDirection.Input, faqId),
                            DbHandler.SetParameter("@tenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId)
                        });
                    if (dr.Read())
                    {
                        json = dr["Json"].ToString();
                    }
                }
                faq = JsonConvert.DeserializeObject<FaqDto>(json);
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }

            return faq;
        }
        public List<AllFaq> AllFaq(int lang, string tenantId, int userId)
        {
            string sql =
                $@"SELECT
                (
	             SELECT
		        M.MOD_ID As ModId,
		        M.TITLE{(lang == 1 ? "" : lang.ToString())} ModName,
		        ( SELECT FAQ_ID as FaqId, ANSWER{(lang == 1 ? "" : lang.ToString())} as Answer, QUESTION_TITLE{(lang == 1 ? "" : lang.ToString())} as Question FROM FAQ F WHERE F.MOD_ID= M.MOD_ID AND F.STATUS= 1 ORDER BY F.WEIGHT FOR json path ) AS FAQ 
	          FROM
		        MODULES M 
	            WHERE
	        	M.STATUS= 1 
	             AND TENANT_ID=@tenantId and EXISTS(select * from dbo.GetEmployeePermission(@userId,M.TENANT_ID , '34'))  FOR json path 
	            ) Json";

            List<AllFaq> faq = null;
            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var dr = con.ExecuteSql(sql, new[]
                        {
                            DbHandler.SetParameter("@tenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                            DbHandler.SetParameter("userId",SqlDbType.Int,10,ParameterDirection.Input,userId)
                        });

                    if (dr.Read())
                    {
                        json = dr["Json"].ToString();
                    }
                }
                faq = JsonConvert.DeserializeObject<List<AllFaq>>(json);
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }

            return faq;
        }


        public List<AllFaq> AllFaqForStore(int lang, string domain)
        {
            string sql =
                $@"Select(SELECT M.MOD_ID As ModId,
		            M.TITLE{(lang == 1 ? "" : lang.ToString())} ModName,
		            ( SELECT FAQ_ID as FaqId, ANSWER{(lang == 1 ? "" : lang.ToString())} as Answer, QUESTION_TITLE{(lang == 1 ? "" : lang.ToString())} as Question FROM FAQ F WHERE F.MOD_ID= M.MOD_ID AND F.STATUS= 1 ORDER BY F.WEIGHT FOR json path ) AS FAQ 
	                FROM
		            MODULES M 
	                WHERE
	                M.STATUS= 1 
	                    AND TENANT_ID=(select TENANT_ID from NEW_STORE Where DOMAIN=@domain)for json path) Json";

            List<AllFaq> faq = null;
            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var dr = con.ExecuteSql(sql,
                        new[] { DbHandler.SetParameter("@domain", SqlDbType.NVarChar, 50, ParameterDirection.Input, domain),
                        }
                        );

                    if (dr.Read())
                    {
                        json = dr["Json"].ToString();
                    }
                }
                faq = JsonConvert.DeserializeObject<List<AllFaq>>(json);
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }

            return faq;
        }
        public List<GetModForInsert> GetModList(string tenantId, int userId)
        {
            const string sql =
                @"SELECT
	                (
	                SELECT
		                M.[MOD_ID] AS Id,
		                M.[TITLE] AS Title,
                        M.[TITLE2] AS Title2,
                        M.[TITLE3] AS Title3,
                        M.[TITLE4] AS Title4
	                FROM
		                [MODULES] M 
	                WHERE
		                M.[STATUS] = 1 
		                AND M.TENANT_ID = @tenantId 
	                AND EXISTS ( SELECT * FROM dbo.GetEmployeePermission ( @userId, M.TENANT_ID , '36' ) ) FOR json path 
	                ) Json";

            List<GetModForInsert> list = null;
            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {

                    var reader = con.ExecuteSql(sql, new[]
                   {
                       DbHandler.SetParameter("@tenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                       DbHandler.SetParameter("@userId",SqlDbType.Int,10,ParameterDirection.Input,userId)
                    });


                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }

                }
                list = JsonConvert.DeserializeObject<List<GetModForInsert>>(json);
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }

            return list;
        }


        public List<FaqDto> GetFaqListByModuleId(int modId)
        {
            const string sql =
                @"SELECT  [FAQ_ID]
                            ,[MOD_ID]
                            ,[QUESTION_TITLE]
                            ,[ANSWER]
                            ,[STATUS]
                        FROM [FAQ]
                        WHERE [STATUS] = 1 and MOD_ID=@PMOD_ID";

            var list = new List<FaqDto>();
            try
            {
                using (var con = new DbHandler())
                {
                    var dr = con.ExecuteSql(sql,
                        new[] { DbHandler.SetParameter("@PMOD_ID", SqlDbType.Int, 10, ParameterDirection.Input, modId) });
                    while (dr.Read())
                    {
                        var faq = new FaqDto()
                        {
                            FaqId = dr.GetInt("FAQ_ID"),
                            ModId = dr.GetInt("MOD_ID"),
                            Question = dr["QUESTION_TITLE"].ToString(),
                            Answer = dr["ANSWER"].ToString()

                        };
                        list.Add(faq);
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

        //public FaqDto Upsert(FaqDto faq, int userId)
        //{
        //    try
        //    {
        //        using (var con = new DbHandler())
        //        {
        //            var json = JsonConvert.SerializeObject(faq);
        //            var newFaqId =
        //                con.ExecStoredProcWithReturnIntValue("[FaqInsUpd]", new[]
        //                {
        //                    DbHandler.SetParameter("@pRequestAsJson", SqlDbType.NVarChar, -1, ParameterDirection.Input, json),
        //                    DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId),

        //                });
        //            faq.FaqId = newFaqId;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Warn("Could not DepPosInsUpdHelper...");
        //        Log.Error(ex);
        //        throw;
        //    }

        //    return GetFaqById(faq.FaqId);
        //}

        public void Delete(int id, int userId, string tenantId)
        {
            try
            {
                using (var con = new DbHandler())
                {
                    con.ExecStoredProcWithReturnIntValue("[FaqDelete]",
                        new[]
                        {
                            DbHandler.SetParameter("@pFaqId", SqlDbType.Int, 10, ParameterDirection.Input, id),
                            DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId),
                            DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId)
                        });
                }
            }
            catch (Exception ex)
            {

                Log.Error(ex);
                throw;
            }
        }
        public int Insert(FaqDto faq, string tenantId, int userId)
        {
            var faqId = 0;
            try
            {

                using (var con = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(faq);
                    faqId = con.ExecStoredProcWithReturnIntValue("[FAQInsert]", new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson",SqlDbType.NVarChar,-1,ParameterDirection.Input,json),
                        DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@pLogUserId",SqlDbType.Int,10,ParameterDirection.Input,userId)
                    });
                }


            }
            catch (Exception ex)
            {
                Log.Warn("Could not UserDesignInsert...");
                Log.Error(ex);
                throw;
            }
            return faqId;
        }

        public int FaqUpdate(string tenantId, UpdateFaq faq, int userId)
        {
            int id;
            try
            {
                var json = JsonConvert.SerializeObject(faq);
                using (var conn = new DbHandler())
                {
                    id = conn.ExecStoredProcWithReturnIntValue("FAQUpdate", new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson",SqlDbType.NVarChar,-1,ParameterDirection.Input,json),
                        DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@pFaqId",SqlDbType.Int,10,ParameterDirection.Input,faq.FaqId),
                        DbHandler.SetParameter("@pLogUserId",SqlDbType.Int,5,ParameterDirection.Input,userId)

                    });
                }

            }
            catch (Exception ex)
            {
                Log.Warn("Could not CategoryUpdate...");
                Log.Error(ex);
                throw;
            }

            return id;
        }
        public void FaqDargDropUpdate(string tenantId, int userId, List<DragDrop> faq)
        {
            try
            {
                var json = JsonConvert.SerializeObject(faq);
                using (var conn = new DbHandler())
                {
                    conn.ExecuteStoredProcedure("FAQWeightUpdate", new[]
                    {
                         DbHandler.SetParameter("@pRequestAsJson",SqlDbType.NVarChar,-1,ParameterDirection.Input,json),
                         DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                         DbHandler.SetParameter("@pLogUserId",SqlDbType.Int,10,ParameterDirection.Input,userId)

                    });

                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not CategoryUpdate...");
                Log.Error(ex);
                throw;
            }
        }
    }
}