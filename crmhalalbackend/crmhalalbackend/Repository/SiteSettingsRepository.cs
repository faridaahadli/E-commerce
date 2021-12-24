using CRMHalalBackEnd.DB;
using CRMHalalBackEnd.Models.SiteSettings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Repository
{
    public class SiteSettingsRepository
    {

        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region PrivacyPolicy
        public PrivacyPolicyAndAbout InsertPrivacyPolicy(PrivacyPolicyAndAbout privacyPolicy, string tenantId, string userId)
        {
            try
            {
                using (var con = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(privacyPolicy);
                    con.ExecuteStoredProcedure("[PrivacyPolicyInsUpd]", new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson", SqlDbType.NVarChar, -1, ParameterDirection.Input, json),
                        DbHandler.SetParameter("@pTenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                }


            }
            catch (Exception ex)
            {
                Log.Warn("Could not PrivacyPolicy...");
                Log.Error(ex);
                throw;
            }
            return privacyPolicy;
        }
        public PrivacyPolicyResponse GetPrivacyPolicy(string tenantId, int userId)
        {
            string sql =
                $@"SELECT
	                PRIVACY_POLICIES AS PrivacyPolicy,
                    PRIVACY_POLICIES2 AS PrivacyPolicy2,
                    PRIVACY_POLICIES3 AS PrivacyPolicy3,
                    PRIVACY_POLICIES4 AS PrivacyPolicy4
                FROM
	                NEW_SITE_SETTINGS 
                WHERE
	                TENANT_ID =@tenantId 
	                AND IS_ACTIVE = 1 
	                AND EXISTS ( SELECT * FROM dbo.GetEmployeePermission ( @userId, @tenantId, '28' ) )";

            PrivacyPolicyResponse privacyPolicies = null;

            try
            {
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@tenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@userId", SqlDbType.Int, 10, ParameterDirection.Input, userId)

                    });
                    while (reader.Read())
                    {
                        privacyPolicies = new PrivacyPolicyResponse()
                        {
                            PrivacyPolicy = reader["PrivacyPolicy"].ToString(),
                            PrivacyPolicy2 = reader["PrivacyPolicy2"].ToString(),
                            PrivacyPolicy3 = reader["PrivacyPolicy3"].ToString(),
                            PrivacyPolicy4 = reader["PrivacyPolicy4"].ToString()

                        };

                    }
                }
            }

            catch (Exception ex)
            {
                Log.Warn("Could not PrivacyPolicy...");
                Log.Error(ex);
                throw;
            }
            return privacyPolicies;
        }


        public PrivacyPolicyResponse GetPrivacyPolicyForShop(int lang,string domain)
        {
            string sql =
                $@"select PRIVACY_POLICIES{(lang == 1?"":lang.ToString())} As PrivacyPolicy from NEW_SITE_SETTINGS where TENANT_ID=(select TENANT_ID from NEW_STORE where DOMAIN=@domain AND IS_ACTIVE=1) AND IS_ACTIVE=1";

            PrivacyPolicyResponse privacyPolicies = null;

            try
            {
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@domain",SqlDbType.NVarChar,50,ParameterDirection.Input,domain)
                    });
                    while (reader.Read())
                    {
                        privacyPolicies = new PrivacyPolicyResponse()
                        {

                            PrivacyPolicy = reader["PrivacyPolicy"].ToString()

                        };

                    }
                }
            }

            catch (Exception ex)
            {
                Log.Warn("Could not PrivacyPolicy...");
                Log.Error(ex);
                throw;
            }
            return privacyPolicies;
        }
        #endregion PrivacyPolicy

        #region About
        public PrivacyPolicyAndAbout InsertAbout(PrivacyPolicyAndAbout about, string tenantId, string userId)
        {
            try
            {
                using (var con = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(about);
                    con.ExecuteStoredProcedure("[AboutInsUpd]", new[]
                    {
                       DbHandler.SetParameter("@pRequestAsJson", SqlDbType.NVarChar, -1, ParameterDirection.Input, json),
                        DbHandler.SetParameter("@pTenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                }


            }
            catch (Exception ex)
            {
                Log.Warn("Could not PrivacyPolicy...");
                Log.Error(ex);
                throw;
            }
            return about;
        }
        public PrivacyPolicyResponse GetAbout(string tenantId,int userId)
        {
            string sql =
                $@"SELECT
	                ABOUT AS About,
                    ABOUT2 AS About2,
                    ABOUT3 AS About3,
                    ABOUT4 AS About4
                FROM
	                NEW_SITE_SETTINGS 
                WHERE
	                TENANT_ID =@tenantId 
	                AND IS_ACTIVE = 1 
	                AND EXISTS ( SELECT * FROM dbo.GetEmployeePermission ( @userId, @tenantId, '32' ) )";

            PrivacyPolicyResponse about = null;

            try
            {
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@tenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId), 
                        DbHandler.SetParameter("@userId", SqlDbType.Int, 10, ParameterDirection.Input, userId)

                    });
                    while (reader.Read())
                    {
                        about = new PrivacyPolicyResponse()
                        {
                            About = reader["About"].ToString(),
                            About2 = reader["About2"].ToString(),
                            About3 = reader["About3"].ToString(),
                            About4 = reader["About4"].ToString(),
                        };

                    }
                }
            }

            catch (Exception ex)
            {
                Log.Warn("Could not About...");
                Log.Error(ex);
                throw;
            }
            return about;
        }
        public PrivacyPolicyResponse GetAboutForShop(int lang, string domain)
        {
            string sql =
                $@"select ABOUT{(lang == 1?"":lang.ToString())} As About from NEW_SITE_SETTINGS where TENANT_ID=(select TENANT_ID from NEW_STORE where DOMAIN=@domain) AND IS_ACTIVE=1";

            PrivacyPolicyResponse about = null;

            try
            {
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@domain",SqlDbType.NVarChar,50,ParameterDirection.Input,domain)
                    });
                    while (reader.Read())
                    {
                        about = new PrivacyPolicyResponse()
                        {

                            About = reader["About"].ToString()

                        };

                    }
                }
            }

            catch (Exception ex)
            {
                Log.Warn("Could not About...");
                Log.Error(ex);
                throw;
            }
            return about;
        }
        #endregion About

        #region SosialMedia
        public List<SosialMediaResponse> InsertSosialMedia(SosialMedia model, string tenantId, string userId)
        {
            var mediaId = 0;
            try
            {
                using (var con = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(model);
                    mediaId = con.ExecStoredProcWithReturnIntValue("[SosialMediaInsert]", new[]
                      {
                        DbHandler.SetParameter("@pRequestAsJson", SqlDbType.NVarChar, -1, ParameterDirection.Input, json),
                        DbHandler.SetParameter("@pTenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                }


            }
            catch (Exception ex)
            {
                Log.Warn("Could not SosialMediaInsert...");
                Log.Error(ex);
                throw;
            }
            return GetSosialMediaById(mediaId);
        }

        public List<SosialMediaResponse> GetSosialMediaById(int id)
        {

            string sql = $@"SELECT(SELECT SETTINGS_LINK_ID AS Id,
                                 NAME As Name, 
                                 LINK As Link,
                                 JSON_QUERY((
		                        SELECT
			                        ICON Id,
			                        ( UF.PATH + UF.FILENAME + UF.EXTENSION ) FilePath 
		                        FROM
			                        NEW_UPLOAD_FILE UF 
		                        WHERE
			                        UF.UPLOAD_FILE_ID = ICON FOR json path,without_array_wrapper
		                        )) AS Icon 
                                 FROM NEW_SITE_SETTINGS_LINKS where SETTINGS_LINK_ID=@id AND IS_ACTIVE=1 AND LINK_TYPE_ID=1  ORDER BY CREATE_DATE DESC for json path) Json";


            List<SosialMediaResponse> mediaDto = null;
            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@id",SqlDbType.Int,10,ParameterDirection.Input,id)
                    });

                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                }

                mediaDto = JsonConvert.DeserializeObject<List<SosialMediaResponse>>(json);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return mediaDto;
        }
        public List<SosialMediaResponse> GetSosialMediaForShop(string domain)
        {

            string sql = $@"SELECT(SELECT SETTINGS_LINK_ID AS Id,
                                 NAME As Name, 
                                 LINK As Link,
                                 JSON_QUERY((
		                        SELECT
			                        ICON Id,
			                        ( UF.PATH + UF.FILENAME + UF.EXTENSION ) FilePath 
		                        FROM
			                        NEW_UPLOAD_FILE UF 
		                        WHERE
			                        UF.UPLOAD_FILE_ID = ICON FOR json path,without_array_wrapper
		                        )) AS Icon 
                                 FROM NEW_SITE_SETTINGS_LINKS where SITE_SETTINGS_ID IN (select SITE_SETTINGS_ID from NEW_SITE_SETTINGS where TENANT_ID=(select TENANT_ID from NEW_STORE where DOMAIN=@domain)) AND IS_ACTIVE=1 AND LINK_TYPE_ID=1 ORDER BY CREATE_DATE DESC for json path) Json";


            List<SosialMediaResponse> mediaDto = null;
            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@domain",SqlDbType.NVarChar,50,ParameterDirection.Input,domain)
                    });

                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                }

                mediaDto = JsonConvert.DeserializeObject<List<SosialMediaResponse>>(json);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return mediaDto;
        }
        public int DeleteSosialMedia(int id, string tenantId, int userId)
        {
            int returnId = 0;
            try
            {
                using (var con = new DbHandler())
                {
                    returnId = con.ExecStoredProcWithReturnIntValue("SosialMediaDelete", new[]
                     {
                        DbHandler.SetParameter("@pSiteLinkId", SqlDbType.Int, 10, ParameterDirection.Input, id),
                        DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });

                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not SosialMediaDelete...");
                Log.Error(ex);
                throw;
            }

            return returnId;
        }

        public List<SosialMediaResponse> UpdateSosialMedia(UpdateSosialMedia sosialMedia, string tenantId, int userId)
        {
            var id = 0;
            try
            {
                using (var con = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(sosialMedia);
                    id = con.ExecStoredProcWithReturnIntValue("[SosialMediaUpdate]", new[]
                     {
                        DbHandler.SetParameter("@pRequestAsJson", SqlDbType.NVarChar, -1, ParameterDirection.Input, json),
                        DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not SosialMediaUpdate...");
                Log.Error(ex);
                throw;
            }

            return GetSosialMediaById(id);
        }


        public List<SosialMediaResponse> GetSosialMedia(string tenantId, int userId)
        {
            const string sql =
                @"SELECT(SELECT SETTINGS_LINK_ID AS Id,
                                 NAME As Name, 
                                 LINK As Link,
                                 JSON_QUERY((
		                        SELECT
			                        ICON Id,
			                        ( UF.PATH + UF.FILENAME + UF.EXTENSION ) FilePath 
		                        FROM
			                        NEW_UPLOAD_FILE UF 
		                        WHERE
			                        UF.UPLOAD_FILE_ID = ICON FOR json path,without_array_wrapper
		                        )) AS Icon 
                                 FROM NEW_SITE_SETTINGS_LINKS where IS_ACTIVE=1 AND  SITE_SETTINGS_ID In(select SITE_SETTINGS_ID from NEW_SITE_SETTINGS where TENANT_ID=@tenantId) AND LINK_TYPE_ID=1 AND EXISTS(select * from dbo.GetEmployeePermission(@userId, @tenantId, '41')) ORDER BY CREATE_DATE DESC for json path) Json";

            List<SosialMediaResponse> media = null;

            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {

                         DbHandler.SetParameter("@tenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                         DbHandler.SetParameter("@userId", SqlDbType.Int, 10, ParameterDirection.Input, userId)


                    });

                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                }

                media = JsonConvert.DeserializeObject<List<SosialMediaResponse>>(json);
            }

            catch (Exception ex)
            {
                Log.Warn("Could not PrivacyPolicy...");
                Log.Error(ex);
                throw;
            }
            return media;
        }
        #endregion SosialMedia

        #region Footer
        public List<FooterSettingsResponse> InsertFooter(FooterSettings footer, string tenantId, string userId)
        {
            var id = 0;
            try
            {
                using (var con = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(footer);
                    id = con.ExecStoredProcWithReturnIntValue("[FooterInsert]", new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson", SqlDbType.NVarChar, -1, ParameterDirection.Input, json),
                        DbHandler.SetParameter("@pTenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                }


            }
            catch (Exception ex)
            {
                Log.Warn("Could not FooterInsert...");
                Log.Error(ex);
                throw;
            }
            return GetFooterById(id);
        }

        public List<FooterSettingsResponse> GetFooterById(int id)
        {

            string sql = $@"SELECT(SELECT  SL.SETTINGS_LINK_ID AS Id,
                                 SL.NAME As Name, 
                                 SL.LINK As Link,
                                 SL.PARENT_ID,
                                (SELECT NAME FROM NEW_SITE_SETTINGS_LINKS WHERE SETTINGS_LINK_ID=SL.PARENT_ID AND IS_ACTIVE=1)  ParentCatName
                                FROM NEW_SITE_SETTINGS_LINKS SL where  SETTINGS_LINK_ID=@id AND IS_ACTIVE=1 AND LINK_TYPE_ID=2  ORDER BY CREATE_DATE DESC for json path) Json";


            List<FooterSettingsResponse> footer = null;
            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@id",SqlDbType.Int,10,ParameterDirection.Input,id)
                    });

                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                }

                footer = JsonConvert.DeserializeObject<List<FooterSettingsResponse>>(json);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return footer;
        }

        public List<FooterSettingsResponse> GetFooter(string tenantId, int userId)
        {
            string sql =
                $@"SELECT
	                    (
	                    SELECT
		                    SL.SETTINGS_LINK_ID AS Id,
		                    SL.NAME AS Name,
                            SL.NAME2 AS Name2,
                            SL.NAME3 AS Name3,
                            SL.NAME4 AS Name4,
		                    SL.LINK AS Link,
		                    SL.PARENT_ID,
		                    ( SELECT NAME FROM NEW_SITE_SETTINGS_LINKS WHERE SETTINGS_LINK_ID = SL.PARENT_ID AND IS_ACTIVE = 1 ) ParentCatName 
	                    FROM
		                    NEW_SITE_SETTINGS_LINKS SL 
	                    WHERE
		                    IS_ACTIVE = 1 
		                    AND LINK_TYPE_ID = 2 
		                    AND SITE_SETTINGS_ID IN ( SELECT SITE_SETTINGS_ID FROM NEW_SITE_SETTINGS WHERE TENANT_ID =@tenantId AND IS_ACTIVE=1 ) 
		                    AND EXISTS ( SELECT * FROM dbo.GetEmployeePermission ( @userId, @tenantId, '45' ) ) 
	                    ORDER BY
	                    CREATE_DATE DESC FOR json path 
	                    ) Json";

            List<FooterSettingsResponse> footer = null;

            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {

                         DbHandler.SetParameter("@tenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                         DbHandler.SetParameter("@userId", SqlDbType.Int, 10, ParameterDirection.Input, userId)

                    });

                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                }

                footer = JsonConvert.DeserializeObject<List<FooterSettingsResponse>>(json);
            }

            catch (Exception ex)
            {
                Log.Warn("Could not Footer...");
                Log.Error(ex);
                throw;
            }
            return footer;
        }

        public List<FooterSettingsResponse> UpdateFooter(UpdateFooter footer, string tenantId, int userId)
        {
            var id = 0;
            try
            {
                using (var con = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(footer);
                    id = con.ExecStoredProcWithReturnIntValue("[FooterUpdate]", new[]
                     {
                        DbHandler.SetParameter("@pRequestAsJson", SqlDbType.NVarChar, -1, ParameterDirection.Input, json),
                        DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not FooterUpdate...");
                Log.Error(ex);
                throw;
            }

            return GetFooterById(id);
        }

        public List<ParentFoooter> GetParentFooter(string tenantId, int userId)
        {
            const string sql =
                @"SELECT
	                (
	                SELECT
		                SETTINGS_LINK_ID AS Id,
		                NAME AS Name,
                        NAME2 AS Name2,
                        NAME3 AS Name3,
                        NAME4 AS Name4
	                FROM
		                NEW_SITE_SETTINGS_LINKS 
	                WHERE
		                PARENT_ID IS NULL 
		                AND IS_ACTIVE = 1 
		                AND SITE_SETTINGS_ID IN ( SELECT SITE_SETTINGS_ID FROM NEW_SITE_SETTINGS WHERE TENANT_ID =@tenantId AND IS_ACTIVE = 1 ) 
		                AND LINK_TYPE_ID = 2 
	                ORDER BY
	                CREATE_DATE DESC FOR json path 
	                ) Json";

            List<ParentFoooter> footer = null;

            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {

                         DbHandler.SetParameter("@tenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                                                 DbHandler.SetParameter("@userId", SqlDbType.Int, 10, ParameterDirection.Input, userId)

                    });

                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                }

                footer = JsonConvert.DeserializeObject<List<ParentFoooter>>(json);
            }

            catch (Exception ex)
            {
                Log.Warn("Could not Footer...");
                Log.Error(ex);
                throw;
            }
            return footer;
        }

        public int DeleteFooter(int id, string tenantId, int userId)
        {
            int returnId = 0;
            try
            {
                using (var con = new DbHandler())
                {
                    returnId = con.ExecStoredProcWithReturnIntValue("FooterDelete", new[]
                     {
                        DbHandler.SetParameter("@pSiteLinkId", SqlDbType.Int, 10, ParameterDirection.Input, id),
                        DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });

                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not FooterDelete...");
                Log.Error(ex);
                throw;
            }

            return returnId;
        }

        public List<FooterResponseForShop> GetFooterForShop(int lang, string domain)
        {

            string sql = $@"SELECT
	                            (
	                            SELECT
		                            S.SETTINGS_LINK_ID ParentCatId,
		                            S.NAME{(lang == 1?"":lang.ToString())} Name,
		                            S.LINK Link,
		                            JSON_QUERY ( ( SELECT SETTINGS_LINK_ID, NAME{(lang == 1 ? "" : lang.ToString())} Name, LINK FROM NEW_SITE_SETTINGS_LINKS WHERE PARENT_ID = S.SETTINGS_LINK_ID AND IS_ACTIVE = 1 FOR json path ) ) Child 
	                            FROM
		                            NEW_SITE_SETTINGS_LINKS S 
	                            WHERE
		                            PARENT_ID IS NULL 
		                            AND IS_ACTIVE = 1 
		                            AND LINK_TYPE_ID = 2 
	                            AND SITE_SETTINGS_ID IN ( SELECT SITE_SETTINGS_ID FROM NEW_SITE_SETTINGS WHERE TENANT_ID = ( SELECT TENANT_ID FROM NEW_STORE WHERE DOMAIN =@DOMAIN ) AND IS_ACTIVE=1 ) FOR json path 
	                            ) Json";


            List<FooterResponseForShop> footer = null;
            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@domain",SqlDbType.NVarChar,50,ParameterDirection.Input,domain)
                    });

                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                }

                footer = JsonConvert.DeserializeObject<List<FooterResponseForShop>>(json);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return footer;
        }



        #endregion Footer

        #region Contact



        public List<HeaderRespoonse> GetContactByTeanantId(string tenantId, int userId)
        {

            string sql = $@"SELECT(SELECT S.STORE_ID StoreId, 
                        JSON_QUERY((SELECT TEXT, CONTACT_TYPE_ID ContactTypeId,CONTACT_ID ContactId FROM NEW_CONTACT WHERE STORE_ID=S.STORE_ID AND IS_ACTIVE=1 for json path,without_array_wrapper)) Contacts,
                        S.WORK_TIME_START WorkStartTime, 
                        S.WORK_TIME_END WorkEndTime, 
                        JSON_QUERY((select ADDRESS, ADDRESS_ID AddressId, LATITUDE Latitude,LONGITUDE Longitude from NEW_ADDRESS where STORE_ID=S.STORE_ID for json path, without_array_wrapper)) Addresses 
                        FROM NEW_STORE S WHERE TENANT_ID=@tenantId AND EXISTS(select * from dbo.GetEmployeePermission(@userId, @tenantId, '51')) for json path) Json";


            List<HeaderRespoonse> contact = null;
            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                         DbHandler.SetParameter("@tenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                                                 DbHandler.SetParameter("@userId", SqlDbType.Int, 10, ParameterDirection.Input, userId)


                    });

                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                }

                contact = JsonConvert.DeserializeObject<List<HeaderRespoonse>>(json);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return contact;
        }


        public List<GetAllHeader> GetAllContactByTenantId(string tenantId, int userId)
        {

            string sql = $@"SELECT(SELECT S.STORE_ID StoreId, 
                        (Select(SELECT TEXT, CONTACT_TYPE_ID ContactTypeId,IS_WHATSAPP AS Whatsapp,CONTACT_ID ContactId FROM NEW_CONTACT WHERE STORE_ID=S.STORE_ID AND IS_ACTIVE=1 for json path)) Contacts,
                        S.WORK_TIME_START WorkStartTime, 
                        S.WORK_TIME_END WorkEndTime, 
                        JSON_QUERY((select ADDRESS, ADDRESS_ID AddressId, LATITUDE Latitude,LONGITUDE Longitude from NEW_ADDRESS where STORE_ID=S.STORE_ID for json path, without_array_wrapper)) Addresses 
                        FROM NEW_STORE S WHERE TENANT_ID=@tenantId AND IS_ACTIVE=1 AND EXISTS(select * from dbo.GetEmployeePermission(@userId, @tenantId, '51')) for json path) Json";


            List<GetAllHeader> contact = null;
            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                         DbHandler.SetParameter("@tenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                                                 DbHandler.SetParameter("@userId", SqlDbType.Int, 10, ParameterDirection.Input, userId)


                    });

                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                }

                contact = JsonConvert.DeserializeObject<List<GetAllHeader>>(json);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return contact;
        }

        public List<GetContactsForShop> GetAllContactForShop(string domain)
        {

            string sql = $@"SELECT(SELECT S.STORE_ID StoreId, 
                        (Select(SELECT TEXT, CONTACT_TYPE_ID ContactTypeId,IS_WHATSAPP AS Whatsapp,CONTACT_ID ContactId FROM NEW_CONTACT WHERE STORE_ID=S.STORE_ID AND IS_ACTIVE=1 AND CONTACT_TYPE_ID=2 for json path)) Phone,
			            (Select(SELECT TEXT, CONTACT_TYPE_ID ContactTypeId,IS_WHATSAPP AS Whatsapp,CONTACT_ID ContactId FROM NEW_CONTACT WHERE STORE_ID=S.STORE_ID AND IS_ACTIVE=1 AND CONTACT_TYPE_ID=1 for json path)) Email,
                        S.WORK_TIME_START WorkStartTime, 
                        S.WORK_TIME_END WorkEndTime, 
                        JSON_QUERY((select ADDRESS, ADDRESS_ID AddressId, LATITUDE Latitude,LONGITUDE Longitude from NEW_ADDRESS where STORE_ID=S.STORE_ID for json path, without_array_wrapper)) Addresses 
                        FROM NEW_STORE S WHERE DOMAIN=@domain AND IS_ACTIVE=1 for json path) Json";


            List<GetContactsForShop> contact = null;
            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                         DbHandler.SetParameter("@domain",SqlDbType.NVarChar,50,ParameterDirection.Input,domain)

                    });

                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                }

                contact = JsonConvert.DeserializeObject<List<GetContactsForShop>>(json);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return contact;
        }
        public List<ContactResponse> GetContactForShop(string domain)
        {

            string sql = $@"SELECT(SELECT S.STORE_ID StoreId, 
                        JSON_QUERY((SELECT TEXT, CONTACT_TYPE_ID ContactTypeId,CONTACT_ID contactId FROM NEW_CONTACT WHERE CONTACT_TYPE_ID=1 and STORE_ID=S.STORE_ID for json path))Phone,
                        JSON_QUERY((SELECT TEXT,CONTACT_TYPE_ID ContactTypeId,CONTACT_ID contactId FROM NEW_CONTACT WHERE CONTACT_TYPE_ID=2 and STORE_ID=S.STORE_ID for json path))Email, 
                        S.WORK_TIME_START WorkStartTime, 
                        S.WORK_TIME_END WorkEndTime, 
                        JSON_QUERY((select ADDRESS, ADDRESS_ID AddressId, LATITUDE Latitude,LONGITUDE Longitude from NEW_ADDRESS where STORE_ID=S.STORE_ID for json path, without_array_wrapper)) Adress 
                        FROM NEW_STORE S WHERE TENANT_ID=(select TENANT_ID from NEW_STORE where DOMAIN=@domain) for json path) Json";


            List<ContactResponse> contact = null;
            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                         DbHandler.SetParameter("@domain",SqlDbType.NVarChar,50,ParameterDirection.Input,domain)
                    });

                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                }

                contact = JsonConvert.DeserializeObject<List<ContactResponse>>(json);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return contact;
        }


        public List<HeaderRespoonse> InsertHeader(HeaderInsert header, string tenantId, int userId)
        {
            try
            {
                using (var con = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(header);
                    con.ExecuteStoredProcedure("[HeaderInsUpd]", new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson", SqlDbType.NVarChar, -1, ParameterDirection.Input, json),
                        DbHandler.SetParameter("@pTenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                }


            }
            catch (Exception ex)
            {
                Log.Warn("Could not TermsAndConditionsInsUpd...");
                Log.Error(ex);
                throw;
            }
            return GetContactByTeanantId(tenantId,userId);
        }


        public int DeleteContact(int id, string tenantId, int userId)
        {
            int returnId = 0;
            try
            {
                using (var con = new DbHandler())
                {
                    returnId = con.ExecStoredProcWithReturnIntValue("ContactDeleteForAdmin", new[]
                     {
                        DbHandler.SetParameter("@pContactId", SqlDbType.Int, 10, ParameterDirection.Input, id),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId),
                        DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId)

                    });

                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not ContactDeleteForAdmin...");
                Log.Error(ex);
                throw;
            }

            return returnId;
        }

        public int UpdateWhatsapp(int userId, string tenantId, int contactId)
        {
            var id = 0;
            try
            {
                using (var con = new DbHandler())
                {
                    id = con.ExecStoredProcWithReturnIntValue("[WhatsappCheck]", new[]
                     {
                        DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId),
                        DbHandler.SetParameter("@ContactId", SqlDbType.BigInt, -1, ParameterDirection.Input, contactId)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not WhatsappCheck...");
                Log.Error(ex);
                throw;
            }

            return id;
        }
        #endregion Contact

        #region TermsAndConditions

        public int InsertTermsConditions(PrivacyPolicyAndAbout termsConditions, string tenantId, string userId)
        {
            int id;
            try
            {
                using (var con = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(termsConditions);
                    id = con.ExecStoredProcWithReturnIntValue("[TermsAndConditionsInsUpd]", new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson", SqlDbType.NVarChar, -1, ParameterDirection.Input, json),
                        DbHandler.SetParameter("@pTenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                }


            }
            catch (Exception ex)
            {
                Log.Warn("Could not TermsAndConditionsInsUpd...");
                Log.Error(ex);
                throw;
            }
            return id;
        }


        public PrivacyPolicyResponse GetTermsAndConditions(string tenantId, int userId)
        {
            const string sql =
                @"SELECT
	                TERMS_CONDITIONS AS TermConditions,
                    TERMS_CONDITIONS2 AS TermConditions2,
                    TERMS_CONDITIONS3 AS TermConditions3,
                    TERMS_CONDITIONS4 AS TermConditions4
                FROM
	                NEW_SITE_SETTINGS 
                WHERE
	                TENANT_ID =@tenantId 
	                AND IS_ACTIVE = 1 
	                AND EXISTS ( SELECT * FROM dbo.GetEmployeePermission ( @userId, @tenantId, '63' ) )";

            PrivacyPolicyResponse privacyPolicies = null;

            try
            {
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@tenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@userId", SqlDbType.Int, 10, ParameterDirection.Input, userId)

                    });
                    while (reader.Read())
                    {
                        privacyPolicies = new PrivacyPolicyResponse()
                        {
                            TermConditions = reader["TermConditions"].ToString(),
                            TermConditions2 = reader["TermConditions2"].ToString(),
                            TermConditions3 = reader["TermConditions3"].ToString(),
                            TermConditions4 = reader["TermConditions4"].ToString(),
                        };

                    }
                }
            }

            catch (Exception ex)
            {
                Log.Warn("Could not TermConditions...");
                Log.Error(ex);
                throw;
            }
            return privacyPolicies;
        }

        public PrivacyPolicyResponse GetTermsForShop(int lang, string domain)
        {
            string sql =
                $@"select TERMS_CONDITIONS{(lang == 1?"":lang.ToString())} As TermConditions from NEW_SITE_SETTINGS where TENANT_ID=(select TENANT_ID from NEW_STORE where DOMAIN=@domain) AND IS_ACTIVE=1";

            PrivacyPolicyResponse privacyPolicies = null;

            try
            {
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@domain",SqlDbType.NVarChar,50,ParameterDirection.Input,domain)
                    });
                    while (reader.Read())
                    {
                        privacyPolicies = new PrivacyPolicyResponse()
                        {

                            TermConditions = reader["TermConditions"].ToString()

                        };

                    }
                }
            }

            catch (Exception ex)
            {
                Log.Warn("Could not TermConditions...");
                Log.Error(ex);
                throw;
            }
            return privacyPolicies;
        }
        #endregion TermsAndConditions

    }
}