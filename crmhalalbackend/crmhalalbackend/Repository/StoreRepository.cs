using CRMHalalBackEnd.DB;
using CRMHalalBackEnd.Models.Store;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CRMHalalBackEnd.Models.File;

namespace CRMHalalBackEnd.Repository
{
    public class StoreRepository
    {
        private static readonly log4net.ILog Log =
         log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public StoreResponse Insert(StoreInsDto store, int userId, string tenantId)
        {
            var storeName = "";
            try
            {
                var json = JsonConvert.SerializeObject(store);
                using (var conn = new DbHandler())
                {
                    storeName = conn.ExecStoredProcWithOutputValue("StoreInsert", "@pStoreName", SqlDbType.NVarChar, 100,
                        new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson",SqlDbType.NVarChar,-1,ParameterDirection.Input,json),
                        DbHandler.SetParameter("@pLogUserId",SqlDbType.Int,10,ParameterDirection.Input,userId),
                        DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not StoreInsert...");
                Log.Error(ex);
                throw;
            }
            return GetStoreByName(storeName);
        }
        public StoreResponse GetStoreByName(string slug)
        {
            const string sql =
                @"
               select(select 
               stor.STORE_GUID StoreGuid,
               stor.[NAME] Name,
	           stor.TENANT_ID TenantId,
               stor.WORK_TIME_END WorkEndTime,
			   stor.WORK_TIME_START WorkStartTime,
               stor.SHOW_SPECIAL ShowSpecial,
               stor.DOMAIN  Domain,
	          (select adr.[ADDRESS] [Address],
	          adr.[LONGITUDE] Longitude,adr.[LATITUDE] Latitude
              from NEW_ADDRESS adr
              where adr.[STORE_ID]=stor.[STORE_ID] for json path)[Addresses],
	          (select cnt.[TEXT] [Text],cnt.[NOTE] Note,
                cnt.CONTACT_TYPE_ID ContactTypeId
	          from NEW_CONTACT cnt
	          where cnt.[STORE_ID]=stor.[STORE_ID] for json path)[Contacts],
	          (select  stor.LOGO_IMG_ID Id,
	          (UF.PATH + UF.FILENAME + UF.EXTENSION) FilePath from NEW_UPLOAD_FILE UF
	          where UF.UPLOAD_FILE_ID = stor.LOGO_IMG_ID for json path)LogoImage,
		        Json_QUERY((select SEO.SEO_ID Id, SEO.SEO_JSON Seo from NEW_SEO SEO where SEO.TENANT_ID = stor.TENANT_ID 
                   AND SEO.IS_ACTIVE=1 for json path, without_array_wrapper)) Seo	
              from NEW_STORE as stor
              where NAME=@pName for json path, without_array_wrapper)Json
                  ";
            StoreResponse store = null;
            try
            {
                string json = String.Empty;
                using (var conn = new DbHandler())
                {
                    var dr = conn.ExecuteSql(sql, new[]{
                        DbHandler.SetParameter("@pName",SqlDbType.NVarChar,100,ParameterDirection.Input,slug)
                    });

                    if (dr.Read())
                    {
                        json = dr["Json"].ToString();
                    }
                    //Nese elave edende evvelce check etmek lazimdir.
                    store = JsonConvert.DeserializeObject<StoreResponse>(json);
                    store = store ?? new StoreResponse();
                }

                store.Slug = store.Name.ToLower();
            }
            catch (Exception ex)
            {
                Log.Warn("Could not get Store...");
                Log.Error(ex);
                throw;
            }
            return store;
        }

        public List<StoreResponse> GetStoresForNote()
        {
            string sql = @"SELECT
	                        (
	                        SELECT
		                        stor.STORE_GUID StoreGuid,
		                        stor.[NAME] Name,
		                        stor.[DOMAIN] DOMAIN,
		                        ( SELECT COUNT ( PRODUCT_ID ) FROM NEW_PRODUCT WHERE IS_ACTIVE = 1 AND IS_VISIBLE = 1 AND TENANT_ID = stor.TENANT_ID ) ProductCount,
		                        (
		                        SELECT
			                        UF.UPLOAD_FILE_ID Id,
			                        ( UF.PATH + UF.FILENAME + UF.EXTENSION ) FilePath 
		                        FROM
			                        NEW_UPLOAD_FILE UF 
		                        WHERE
			                        UF.UPLOAD_FILE_ID = ( CASE WHEN stor.MAIN_LOGO_IMG_ID IS NOT NULL THEN stor.MAIN_LOGO_IMG_ID ELSE stor.LOGO_IMG_ID END ) FOR json path 
		                        ) LogoImage 
	                        FROM
		                        NEW_STORE AS stor 
	                        WHERE
		                        stor.IS_ACTIVE= 1 
	                        AND stor.IS_ALLOWED= 1 FOR json path 
	                        ) Json";
            List<StoreResponse> storeResponses = new List<StoreResponse>();
            try
            {
                using (var con = new DbHandler())
                {
                    var dr = con.ExecuteSql(sql);
                    if (dr.Read())
                    {
                        var json = dr["Json"].ToString();
                        storeResponses = JsonConvert.DeserializeObject<List<StoreResponse>>(json);
                    }


                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not GetStoresForNote...");
                Log.Error(ex);
                throw;
            }


            

            return storeResponses;
        }

        public bool GetStoreStatusByDomain(string domain)
        {
            string sql = "SELECT [STATUS] FROM NEW_STORE WHERE [DOMAIN] = @pDomain AND IS_ACTIVE = 1";

            bool storeStatus = false;
            try
            {
                using (var con = new DbHandler())
                {
                    var dr = con.ExecuteSql(sql, new[]{
                        DbHandler.SetParameter("@pDomain",SqlDbType.VarChar,63,ParameterDirection.Input,domain)
                    });

                    if (dr.Read())
                    {
                        storeStatus = bool.Parse(dr["STATUS"].ToString());
                    }


                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not GetStoreStatusByDomain...");
                Log.Error(ex);
                throw;
            }

            return storeStatus;
        }
        public StoreResponse GetStoreByDomain(string domain)
        {
            const string sql =
                @"
               select(select 
               stor.STORE_GUID StoreGuid,
               stor.[NAME] Name,
	           stor.TENANT_ID TenantId,
               stor.WORK_TIME_END WorkEndTime,
			   stor.WORK_TIME_START WorkStartTime,
               stor.SHOW_SPECIAL ShowSpecial,
               stor.DOMAIN  Domain,
               stor.IS_SALES IsSales,
	          (select  stor.LOGO_IMG_ID Id,
	          (UF.PATH + UF.FILENAME + UF.EXTENSION) FilePath from NEW_UPLOAD_FILE UF
	          where UF.UPLOAD_FILE_ID = stor.LOGO_IMG_ID for json path)LogoImage,JSON_QUERY (
			(
			SELECT
				stor.FAVICON_IMAGE Id,
				( UF.PATH + UF.FILENAME + UF.EXTENSION ) FilePath 
			FROM
				NEW_UPLOAD_FILE UF 
			WHERE
				UF.UPLOAD_FILE_ID = stor.FAVICON_IMAGE FOR json path,
				without_array_wrapper 
			) 
		) Favicon,
		        Json_QUERY((select SEO.SEO_ID Id, SEO.SEO_JSON Seo from NEW_SEO SEO where SEO.TENANT_ID = stor.TENANT_ID 
                   AND SEO.IS_ACTIVE=1 for json path, without_array_wrapper)) Seo	
              from NEW_STORE as stor
              where [DOMAIN]=@domain for json path, without_array_wrapper)Json
                  ";
            StoreResponse store = null;
            try
            {
                string json = String.Empty;
                using (var conn = new DbHandler())
                {
                    var dr = conn.ExecuteSql(sql, new[]{
                        DbHandler.SetParameter("@domain",SqlDbType.NVarChar,100,ParameterDirection.Input,domain)
                    });

                    if (dr.Read())
                    {
                        json = dr["Json"].ToString();
                    }
                    //Nese elave edende evvelce check etmek lazimdir.
                    store = JsonConvert.DeserializeObject<StoreResponse>(json);
                    store = store ?? new StoreResponse();
                }

                store.Slug = store.Name.ToLower();
            }
            catch (Exception ex)
            {
                Log.Warn("Could not get Store...");
                Log.Error(ex);
                throw;
            }
            return store;
        }
        public int InsertSliderForStore(SliderDto slider, string tenantId, int userId)
        {
            var json = JsonConvert.SerializeObject(slider);
            int sliderId = 0;
            try
            {
                using (var conn = new DbHandler())
                {
                    sliderId = conn.ExecStoredProcWithReturnIntValue("StoreSliderInsert", new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson",SqlDbType.NVarChar,-1,ParameterDirection.Input,json),
                        DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@pLogUserId",SqlDbType.Int,10,ParameterDirection.Input,userId)
                    });

                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not StoreSliderInsert...");
                Log.Error(ex);
                throw;
            }

            return sliderId;
        }
        public SliderDto GetStoreSliderById(int storeId)
        {
            var sql =
                @"SELECT
	                SS.STORE_SLIDER_ID ImageId,
	                ( UF.PATH + UF.FILENAME + UF.EXTENSION ) FilePath,
	                SS.URL Url
                FROM
	                NEW_STORE_SLIDER SS
	                INNER JOIN NEW_UPLOAD_FILE UF ON UF.UPLOAD_FILE_ID = SS.UPLOAD_FILE_ID 
                WHERE
	                SS.STORE_SLIDER_ID = @storeId";
            SliderDto sliderDto = null;
            try
            {
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@storeId", SqlDbType.Int, 10, ParameterDirection.Input, storeId)
                    });

                    if (reader.Read())
                    {
                        sliderDto = new SliderDto()
                        {
                            ImageId = reader.GetInt("ImageId"),
                            FilePath = reader["FilePath"].ToString(),
                            Url = reader["Url"].ToString()
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not GetStoreSliderById...");
                Log.Error(ex);
                throw;
            }


            return sliderDto;
        }
        public List<SliderDto> GetStoreSlidersByTenantId(string tenantId, int userId)
        {
            string sql =
                @"SELECT
	                SS.STORE_SLIDER_ID ImageId,
	                ( UF.PATH + UF.FILENAME + UF.EXTENSION ) FilePath,
	                SS.URL Url 
                FROM
	                NEW_STORE_SLIDER SS
	                INNER JOIN NEW_UPLOAD_FILE UF ON UF.UPLOAD_FILE_ID = SS.UPLOAD_FILE_ID 
                WHERE
	                SS.STORE_ID = ( SELECT S.STORE_ID FROM NEW_STORE S WHERE S.TENANT_ID = @tenantId ) 
	                AND SS.IS_ACTIVE = 1
                    AND EXISTS(select * from dbo.GetEmployeePermission(@userId,@tenantId,'19'))";

            List<SliderDto> sliderDtos = new List<SliderDto>();
            try
            {
                using (var conn = new DbHandler())
                {
                    var reader = conn.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@tenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@userId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                    while (reader.Read())
                    {
                        SliderDto sliderDto = new SliderDto()
                        {
                            ImageId = reader.GetInt("ImageId"),
                            FilePath = reader["FilePath"].ToString(),
                            Url = reader["Url"].ToString()
                        };
                        sliderDtos.Add(sliderDto);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not GetStoreSlidersByTenantId...");
                Log.Error(ex);
                throw;
            }

            return sliderDtos;
        }

        public List<SliderDto> GetStoreSlidersByTenantIdWithoutUserId(string tenantId)
        {
            string sql =
                @"SELECT
	                SS.STORE_SLIDER_ID ImageId,
	                ( UF.PATH + UF.FILENAME + UF.EXTENSION ) FilePath,
	                SS.URL Url 
                FROM
	                NEW_STORE_SLIDER SS
	                INNER JOIN NEW_UPLOAD_FILE UF ON UF.UPLOAD_FILE_ID = SS.UPLOAD_FILE_ID 
                WHERE
	                SS.STORE_ID = ( SELECT S.STORE_ID FROM NEW_STORE S WHERE S.TENANT_ID = @tenantId ) 
	                AND SS.IS_ACTIVE = 1";

            List<SliderDto> sliderDtos = new List<SliderDto>();
            try
            {
                using (var conn = new DbHandler())
                {
                    var reader = conn.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@tenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId)
                    });
                    while (reader.Read())
                    {
                        SliderDto sliderDto = new SliderDto()
                        {
                            ImageId = reader.GetInt("ImageId"),
                            FilePath = reader["FilePath"].ToString(),
                            Url = reader["Url"].ToString()
                        };
                        sliderDtos.Add(sliderDto);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not GetStoreSlidersByTenantId...");
                Log.Error(ex);
                throw;
            }

            return sliderDtos;
        }
        public void UpdateStoreShowSpecial(string tenantId, int userId, bool showSpecial)
        {
            try
            {
                using (var conn = new DbHandler())
                {
                    conn.ExecuteStoredProcedure("StoreShowSpecialUpdate", new[]
                    {
                        DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@pShowSpecial",SqlDbType.Bit,-1,ParameterDirection.Input,showSpecial),
                        DbHandler.SetParameter("@pLogUserId",SqlDbType.Int,10,ParameterDirection.Input,userId)
                    });
                }

            }
            catch (Exception ex)
            {
                Log.Warn("Could not UpdateStoreShowSpecial...");
                Log.Error(ex);
                throw;
            }
        }
        public void UpdateStore(string tenantId, int userId, StoreUpd store)
        {
            try
            {
                var json = JsonConvert.SerializeObject(store);
                using (var conn = new DbHandler())
                {
                    conn.ExecuteStoredProcedure("StoreUpdate", new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson",SqlDbType.NVarChar,-1,ParameterDirection.Input,json),
                        DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@pLogUserId",SqlDbType.Int,10,ParameterDirection.Input,userId)
                    });
                }

            }
            catch (Exception ex)
            {
                Log.Warn("Could not StoreUpdate...");
                Log.Error(ex);
                throw;
            }
        }

        public void UpdateStoreSettings(string tenantId, int userId, StoreSettingsUpd settingsUpd)
        {
            try
            {
                var json = JsonConvert.SerializeObject(settingsUpd);
                using (var conn = new DbHandler())
                {
                    conn.ExecuteStoredProcedure("[StoreSettingsUpdate]", new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson",SqlDbType.NVarChar,-1,ParameterDirection.Input,json),
                        DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@pLogUserId",SqlDbType.Int,10,ParameterDirection.Input,userId)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not StoreUpdate...");
                Log.Error(ex);
                throw;
            }
        }

        public int DeleteStoreSliderById(int sliderId, string tenantId, int userId)
        {
            int returnStoreId = 0;
            try
            {
                using (var con = new DbHandler())
                {
                    returnStoreId = con.ExecStoredProcWithReturnIntValue("StoreSliderDelete", new[]
                    {
                        DbHandler.SetParameter("@pSliderId", SqlDbType.Int, 10, ParameterDirection.Input, sliderId),
                        DbHandler.SetParameter("@pTenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });

                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not DeleteStoreSliderById...");
                Log.Error(ex);
                throw;
            }

            return returnStoreId;
        }
        public StoreResponse GetStoreByTenant(string tenantId, int userId)
        {

            const string sql =
                @"
               select(select 
               stor.STORE_GUID StoreGuid,
               stor.[NAME] Name,
	           stor.TENANT_ID TenantId,
               stor.WORK_TIME_END WorkEndTime,
			   stor.WORK_TIME_START WorkStartTime,
               stor.[DOMAIN] [Domain],
               stor.[STATUS] Status,
               stor.[IS_SALES] IsSales,
               stor.SHOW_SPECIAL ShowSpecial,
	          (select adr.[ADDRESS_ID] AddressId, adr.[ADDRESS] [Address],
	          adr.[LONGITUDE] Longitude,adr.[LATITUDE] Latitude
              from NEW_ADDRESS adr
              where  stor.DEFAULT_ADRESS_ID=adr.ADDRESS_ID for json path)[Addresses],
	          (select cnt.[CONTACT_ID] ContactId, cnt.[TEXT] [Text],cnt.[NOTE] Note,
                cnt.CONTACT_TYPE_ID ContactTypeId
	          from NEW_CONTACT cnt
	          where stor.DEFAULT_EMAIL_ID=cnt.CONTACT_ID OR stor.DEFAULT_PHONE_ID=cnt.CONTACT_ID  for json path)[Contacts],
	          (select  stor.LOGO_IMG_ID Id,
	          (UF.PATH + UF.FILENAME + UF.EXTENSION) FilePath from NEW_UPLOAD_FILE UF
	          where UF.UPLOAD_FILE_ID = stor.LOGO_IMG_ID for json path)LogoImage,
                    JSON_QUERY((
		                    SELECT
			                    stor.WATERMARK_IMAGE Id,
			                    ( UF.PATH + UF.FILENAME + UF.EXTENSION ) FilePath 
		                    FROM
			                    NEW_UPLOAD_FILE UF 
		                    WHERE
			                    UF.UPLOAD_FILE_ID = stor.WATERMARK_IMAGE FOR json path,without_array_wrapper
		                    )) Watermark,
                    JSON_QUERY (
			                    (
			                    SELECT
				                    stor.FAVICON_IMAGE Id,
				                    ( UF.PATH + UF.FILENAME + UF.EXTENSION ) FilePath 
			                    FROM
				                    NEW_UPLOAD_FILE UF 
			                    WHERE
				                    UF.UPLOAD_FILE_ID = stor.FAVICON_IMAGE FOR json path,
				                    without_array_wrapper 
			                    ) 
		                    ) Favicon,
                    JSON_QUERY (
			                    (
			                    SELECT
				                    stor.MAIN_LOGO_IMG_ID Id,
				                    ( UF.PATH + UF.FILENAME + UF.EXTENSION ) FilePath 
			                    FROM
				                    NEW_UPLOAD_FILE UF 
			                    WHERE
				                    UF.UPLOAD_FILE_ID = stor.MAIN_LOGO_IMG_ID FOR json path,
				                    without_array_wrapper 
			                    ) 
		                    ) MainLogoImgId,
		        Json_QUERY((select SEO.SEO_ID Id, SEO.SEO_JSON Seo from NEW_SEO SEO where SEO.TENANT_ID = stor.TENANT_ID 
                   AND SEO.IS_ACTIVE=1 for json path, without_array_wrapper)) Seo
              from NEW_STORE as stor
              where TENANT_ID=@tenantId AND EXISTS ( SELECT * FROM dbo.GetEmployeePermission ( @userId, @tenantId, '2' ) )
              for json path, without_array_wrapper)Json";
            StoreResponse store = null;
            try
            {
                string json = String.Empty;
                using (var conn = new DbHandler())
                {
                    var dr = conn.ExecuteSql(sql, new[]{
                        DbHandler.SetParameter("@tenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@userId",SqlDbType.Int,10,ParameterDirection.Input,userId)
                    });

                    if (dr.Read())
                    {
                        json = dr["Json"].ToString();
                    }
                    //Nese elave edende evvelce check etmek lazimdir.
                    store = JsonConvert.DeserializeObject<StoreResponse>(json);

                    if (store == null)
                    {
                        store = new StoreResponse();
                    }
                    store.Slug = store.Name.ToLower();
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not get Store...");
                Log.Error(ex);
                throw;
            }
            return store;
        }

        public string GetStoreDomain(string tenantId)
        {
            string sql = @"SELECT [DOMAIN] FROM NEW_STORE WHERE TENANT_ID = @tenantId AND IS_ACTIVE=1";
            string domain = null;
            try
            {
                using (DbHandler con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@tenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId)
                    });

                    if (reader.Read())
                    {
                        domain = reader["DOMAIN"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not get GetStoreDomain...");
                Log.Error(ex);
                throw;
            }

            return domain;
        }
        public string GetTenantIdByStoreName(string domain)
        {
            string tenantId = String.Empty;

            string sql =
                @"SELECT TENANT_ID FROM NEW_STORE WHERE [DOMAIN] = @domain and IS_ACTIVE=1";

            try
            {
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@domain", SqlDbType.VarChar, 63, ParameterDirection.Input,
                            domain)
                    });

                    if (reader.Read())
                    {
                        tenantId = reader["TENANT_ID"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not get GetTenantIdByStoreName...");
                Log.Error(ex);
                throw;
            }

            return tenantId;
        }
        public bool CheckDomain(string domain)
        {
            const string sql = @"select count(DOMAIN) as domain from NEW_STORE 
                               where DOMAIN=@pDomain";
            int count = 0;
            try
            {
                using (var con = new DbHandler())
                {
                    var dr = con.ExecuteSql(sql, new[]{
                        DbHandler.SetParameter("@pDomain",SqlDbType.NVarChar,100,ParameterDirection.Input,domain)
                    });

                    if (dr.Read())
                    {
                        count = dr.GetInt("domain");
                    }
                }
                if (count == 0)
                    return false;
            }
            catch (Exception ex)
            {
                Log.Warn("Could not DeleteStoreSliderById...");
                Log.Error(ex);
                throw;
            }

            return true;
        }

        public bool GetFollowingData(string tenantId, int userId = 0, string followingTenantId = "")
        {
            string sqlUser =
                @"select count(*) Count from NEW_FOLLOWERS where USER_ID = @userId and TENANT_ID=@tenantId and IS_ACTIVE=1";
            string sqlCompany =
                @"select count(*) Count from NEW_FOLLOWERS where FOLLOWER_TENANT_ID = @followingTenantId and TENANT_ID=@tenantId and IS_ACTIVE=1";
            int count = 0;
            try
            {
                using (var con = new DbHandler())
                {
                    SqlDataReader countReader;
                    if (userId != 0)
                    {
                        countReader = con.ExecuteSql(sqlUser, new[]
                        {
                            DbHandler.SetParameter("@userId", SqlDbType.Int, 10, ParameterDirection.Input, userId),
                            DbHandler.SetParameter("@tenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId)
                        });
                    }
                    else
                    {
                        countReader = con.ExecuteSql(sqlCompany, new[]
                        {
                            DbHandler.SetParameter("@followingTenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, followingTenantId),
                            DbHandler.SetParameter("@tenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId)
                        });
                    }

                    if (countReader.Read())
                    {
                        count = Convert.ToInt32(countReader["Count"]);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not GetFollowingData...");
                Log.Error(ex);
                throw;
            }

            return count > 0;
        }

        public FileDto GetWatermark(string tenantId)
        {
            string sql = @"SELECT
	                        (
	                        SELECT
		                        S.WATERMARK_IMAGE Id,
		                        ( UF.PATH + UF.FILENAME + UF.EXTENSION ) FilePath 
	                        FROM
		                        NEW_STORE S
		                        INNER JOIN NEW_UPLOAD_FILE UF ON UF.UPLOAD_FILE_ID = S.WATERMARK_IMAGE 
	                        WHERE
		                        S.TENANT_ID = @tenantId FOR json path,
	                        without_array_wrapper 
	                        ) Json";
            FileDto file;
            try
            {
                using (var con = new DbHandler())
                {
                    var dr = con.ExecuteSql(sql, new[]{
                        DbHandler.SetParameter("@tenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId)
                    });
                    string json = String.Empty;
                    if (dr.Read())
                    {
                        json = dr["Json"].ToString();
                    }

                    file = JsonConvert.DeserializeObject<FileDto>(json);
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not GetWatermark...");
                Log.Error(ex);
                throw;
            }

            return file;
        }
    }
}