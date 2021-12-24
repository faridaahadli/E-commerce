using System;
using System.Collections.Generic;
using System.Data;
using CRMHalalBackEnd.DB;
using CRMHalalBackEnd.Models.Languages;

namespace CRMHalalBackEnd.Repository
{
    public class LanguagesRepository
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public IEnumerable<LanguagesDto> GetAllLanguage()
        {
            string sql = @"SELECT
                                L.LANGUAGE_ID LanguageId,
                                L.LANGUAGE_NAME Name,
                                L.LANGUAGE_CODE_2 CodeTwo,
                                L.LANGUAGE_CODE_3 CodeThree
                            FROM
                                NEW_LANGUAGES L";
            List<LanguagesDto> languages = new List<LanguagesDto>();
            try
            {
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql);

                    while (reader.Read())
                    {
                        LanguagesDto language = new LanguagesDto()
                        {
                            LanguageId = int.Parse(reader["LanguageId"].ToString()),
                            Name = reader["Name"].ToString(),
                            CodeTwo = reader["CodeTwo"].ToString(),
                            CodeThree = reader["CodeThree"].ToString()

                        };
                        languages.Add(language);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
            return languages;
        }

        public IEnumerable<StoreLanguageDto> GetLanguageByTenant(string tenantId,int userId)
        {
            string sql = $@"SELECT
                                SL.STORE_LANGUAGE_ID StoreLanguageId,
	                            L.LANGUAGE_ID LanguageId,
	                            L.LANGUAGE_NAME Name,
	                            L.LANGUAGE_CODE_2 CodeTwo,
	                            L.LANGUAGE_CODE_3 CodeThree,
	                            SL.IS_DEFAULT	IsDefault,
	                            SL.NUMBER Number
                            FROM
	                            NEW_STORE_LANGUAGE SL
	                            INNER JOIN NEW_LANGUAGES L ON L.LANGUAGE_ID = SL.LANGUAGE_ID 
                            WHERE
	                            SL.TENANT_ID = @tenantId 
	                            AND SL.IS_ACTIVE =1 {(userId==0?"": "AND EXISTS(select * from dbo.GetEmployeePermission(@userId,@tenantId, '110'))")}
	                            Order by SL.NUMBER";

            List<StoreLanguageDto> languages = new List<StoreLanguageDto>();
            try
            {
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql,new []
                    {
                        DbHandler.SetParameter("@userId",SqlDbType.Int,10,ParameterDirection.Input,userId),
                        DbHandler.SetParameter("@tenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId)
                    });

                    while (reader.Read())
                    {
                        StoreLanguageDto language = new StoreLanguageDto()
                        {
                            StoreLanguageId = int.Parse(reader["StoreLanguageId"].ToString()),
                            LanguageId = int.Parse(reader["LanguageId"].ToString()),
                            Name = reader["Name"].ToString(),
                            CodeTwo = reader["CodeTwo"].ToString(),
                            CodeThree = reader["CodeThree"].ToString(),
                            IsDefault = bool.Parse(reader["IsDefault"].ToString()),
                            Number = int.Parse(reader["Number"].ToString())
                        };
                        languages.Add(language);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
            return languages;
        }

        public int InsertLanguage(int languageId, int transNumId, int  line,string tenantId,int userId)
        {
            int storeLangId;
            try
            {
                using (var con = new DbHandler())
                {
                    storeLangId = con.ExecStoredProcWithReturnIntValue("[LanguageInsert]", new[]
                    {
                        DbHandler.SetParameter("@pTenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@pLanguageId", SqlDbType.Int, 10, ParameterDirection.Input, languageId),
                        DbHandler.SetParameter("@pTransNumId", SqlDbType.Int, 10, ParameterDirection.Input, transNumId),
                        DbHandler.SetParameter("@pNumber", SqlDbType.Int, 2, ParameterDirection.Input, line),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not InsertLanguage...");
                Log.Error(ex);
                throw;
            }

            return storeLangId;
        }

        public void LanguageDelete(int storeLanguageId, int defaultLanguageId ,string tenantId, int userId)
        {

            try
            {
                using (var con = new DbHandler())
                {
                    con.ExecuteStoredProcedure("[LanguageDelete]", new[]
                    {
                        DbHandler.SetParameter("@pTenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@pStoreLanguageId", SqlDbType.Int, 10, ParameterDirection.Input, storeLanguageId),
                        DbHandler.SetParameter("@pDefaultLanguageId", SqlDbType.Int, 10, ParameterDirection.Input, defaultLanguageId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not LanguageDelete...");
                Log.Error(ex);
                throw;
            }
        }

        public int GetLangNumberForStore(string langString,string tenantId)
        {
            string sql = @"SELECT
	                        SL.NUMBER Number
                        FROM
	                        NEW_STORE_LANGUAGE SL
	                        INNER JOIN NEW_LANGUAGES L ON L.LANGUAGE_ID = SL.LANGUAGE_ID 
                        WHERE
	                        SL.TENANT_ID = @tenantId
	                        AND L.LANGUAGE_CODE_2 = @lang 
	                        AND SL.IS_ACTIVE = 1";
            int langNumber = 0;
            try
            {
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql,new []
                    {
                        DbHandler.SetParameter("@tenantId",SqlDbType.VarChar,5,ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@lang",SqlDbType.VarChar,2,ParameterDirection.Input,langString)
                    });

                    if (reader.Read())
                    {
                        langNumber = int.Parse(reader["Number"].ToString());
                    }

                    if (langNumber == 0)
                    {
                        throw new ArgumentException("Mağazanın belə bir dili yoxdur.");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
            return langNumber;
        }
        public int GetLangId(string langString)
        {
            string sql = @"SELECT
	                        L.LANGUAGE_ID LangId 
                        FROM
	                        NEW_LANGUAGES L 
                        WHERE
	                        L.LANGUAGE_CODE_2 = @lang";
            int langId = 0;
            try
            {
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@lang",SqlDbType.VarChar,2,ParameterDirection.Input,langString)
                    });

                    if (reader.Read())
                    {
                        langId = int.Parse(reader["LangId"].ToString());
                    }

                    if (langId == 0)
                    {
                        throw new ArgumentException("Belə bir dil yoxdur.");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
            return langId;
        }

        public void LanguageStatusChange(int storeLanguageId, string tenantId, int userId)
        {
            try
            {
                using (var con = new DbHandler())
                {
                    con.ExecuteStoredProcedure("[LanguageUpdateStatus]", new[]
                    {
                        DbHandler.SetParameter("@pTenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@pStoreLanguageId", SqlDbType.Int, 10, ParameterDirection.Input, storeLanguageId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not LanguageDelete...");
                Log.Error(ex);
                throw;
            }
        }
    }
}