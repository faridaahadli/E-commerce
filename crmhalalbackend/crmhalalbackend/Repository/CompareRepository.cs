using CRMHalalBackEnd.DB;
using CRMHalalBackEnd.Models.CompareProduct;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using CRMHalalBackEnd.Helpers;

namespace CRMHalalBackEnd.Repository
{
    public class CompareRepository
    {
        private static readonly log4net.ILog Log =
        log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string Insert(string productGuid, string tenantId, int userId)
        {
            Compare compare;
            var compareId = 0;
            try
            {

                using (var con = new DbHandler())
                {
                    compareId = con.ExecStoredProcWithReturnIntValue("[CompareInsert]", new[]
                    {
                        DbHandler.SetParameter("@pProductGuid",SqlDbType.VarChar,-1,ParameterDirection.Input,productGuid),
                        DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                }


            }
            catch (Exception ex)
            {
                Log.Warn("Could not CompareInsert...");
                Log.Error(ex);
                throw;
            }
            return "Okay";
        }
        public Compare GetProductCompareById(int lang, string stringLang, string guid)
        {

            string sql = 
                $@"SELECT
	                (
	                SELECT
		                PRODUCT_GUID AS Id,
		                PRODUCT_ID AS ProductId,
		                NAME{ (lang == 1 ? "" : lang.ToString()) } + ISNULL(
			                STUFF(
				                (
				                SELECT
					                ' ' + [VALUE{(lang == 1 ? "" : lang.ToString())}] 
				                FROM
					                NEW_PRODUCT_VARIATION t1 
				                WHERE
					                t1.PRODUCT_ID = P.PRODUCT_ID 
					                AND ( SELECT [SHOW_IN_NAME] FROM NEW_VARIATION WHERE VARIATION_ID = t1.VARIATION_ID ) = 1 
					                AND t1.IS_ACTIVE= 1 FOR XML PATH ( '' ) 
				                ),
				                1,
				                0,
				                '' 
			                ),
			                '' 
		                ) Name,
		                Json_Query (
			                (
			                SELECT
				                PR_CAT_ID AS Id,
				                NAME{ (lang == 1 ? "" : lang.ToString()) } Name
			                FROM
				                NEW_PRODUCT_CATEGORY PC 
			                WHERE
				                PC.PR_CAT_ID= P.PR_CAT_ID FOR json path,
				                without_array_wrapper 
			                ) 
		                ) Category,
		                MANUFACTURER,
		                PRICE,
		                DISCOUNTED_PRICE Discount,
		                (
		                SELECT
			                V.NAME{ (lang == 1 ? "" : lang.ToString()) } Name,
			                ( SELECT [VALUE{(lang == 1 ? "" : lang.ToString())}] [Value] FROM NEW_PRODUCT_VARIATION t1 WHERE t1.PRODUCT_ID = P.PRODUCT_ID AND t1.VARIATION_ID = V.VARIATION_ID ) 
		                VALUE
			                
		                FROM
			                NEW_VARIATION V 
		                WHERE
			                V.GROUP_ID = P.GROUP_ID 
			                AND V.IS_ACTIVE= 1 FOR json path 
		                ) Attributes,
		                (
		                SELECT
			                PF.[UPLOAD_FILE_IMAGE_ID] Id,
			                UF.PATH + UF.FILENAME + UF.EXTENSION FilePath 
		                FROM
			                NEW_PRODUCT_FILE PF
			                INNER JOIN NEW_UPLOAD_FILE UF ON UF.UPLOAD_FILE_ID = PF.UPLOAD_FILE_IMAGE_ID 
		                WHERE
			                PF.PRODUCT_ID = P.PRODUCT_ID 
			                AND PF.IS_ACTIVE = 1 FOR json path 
		                ) [Images],
		                WIDTH,
		                HEIGHT,
		                LENGTH,
		                NET_WEIGHT 
	                FROM
		                NEW_PRODUCT P 
	                WHERE
		                P.PRODUCT_GUID=@productGuid FOR json path,
	                without_array_wrapper 
	                ) Json";


            Compare compare = null;
            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@productGuid",SqlDbType.VarChar,50,ParameterDirection.Input,guid)

                    });

                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                }

                compare = JsonConvert.DeserializeObject<Compare>(json);
                if (compare != null) compare.Slug = compare.Name.UrlFriendly(stringLang) + "-" + compare.ProductId;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return compare;
        }

        public List<Compare> GetAllCompare(int lang, string stringLang, string tenantId, int userId)
        {
            string sql = 
                $@"SELECT
	                (
	                SELECT
		                PRODUCT_GUID AS Id,
		                PRODUCT_ID AS ProductId,
		                P.NAME{ (lang == 1 ? "" : lang.ToString()) } + ISNULL(
			                STUFF(
				                (
				                SELECT
					                ' ' + [VALUE{(lang == 1 ? "" : lang.ToString())}] 
				                FROM
					                NEW_PRODUCT_VARIATION t1 
				                WHERE
					                t1.PRODUCT_ID = P.PRODUCT_ID 
					                AND ( SELECT V.[SHOW_IN_NAME] FROM NEW_VARIATION V WHERE V.VARIATION_ID = t1.VARIATION_ID AND V.IS_ACTIVE = 1 ) = 1 
					                AND t1.IS_ACTIVE= 1 FOR XML PATH ( '' ) 
				                ),
				                1,
				                0,
				                '' 
			                ),
			                '' 
		                ) Name,
		                Json_Query ( ( SELECT PR_CAT_ID AS Id, NAME{(lang == 1 ? "" : lang.ToString())} Name FROM NEW_PRODUCT_CATEGORY PC WHERE PC.PR_CAT_ID= P.PR_CAT_ID FOR json path, without_array_wrapper ) ) Category,
		                MANUFACTURER,
		                PRICE,
		                DISCOUNTED_PRICE Discount,
		                (
		                SELECT
			                V.NAME{ (lang == 1 ? "" : lang.ToString()) } Name,
			                ( SELECT [VALUE{(lang == 1 ? "" : lang.ToString())}] [Value] FROM NEW_PRODUCT_VARIATION t1 WHERE t1.PRODUCT_ID = P.PRODUCT_ID AND t1.VARIATION_ID = V.VARIATION_ID ) 
		                VALUE
			                
		                FROM
			                NEW_VARIATION V 
		                WHERE
			                V.GROUP_ID = P.GROUP_ID 
			                AND V.IS_ACTIVE= 1 FOR json path 
		                ) Attributes,
		                (
		                SELECT
			                PF.[UPLOAD_FILE_IMAGE_ID] Id,
			                UF.PATH + UF.FILENAME + UF.EXTENSION FilePath 
		                FROM
			                NEW_PRODUCT_FILE PF
			                INNER JOIN NEW_UPLOAD_FILE UF ON UF.UPLOAD_FILE_ID = PF.UPLOAD_FILE_IMAGE_ID 
		                WHERE
			                PF.PRODUCT_ID = P.PRODUCT_ID 
			                AND PF.IS_ACTIVE = 1 FOR json path 
		                ) [Images],
		                WIDTH,
		                HEIGHT,
		                LENGTH,
		                NET_WEIGHT,
		                ( SELECT AVG ( CAST ( RATE_SCORE AS DECIMAL ) ) FROM NEW_PRODUCT_COMMENT PC WHERE PC.GROUP_ID= P.GROUP_ID ) Raiting 
	                FROM
		                NEW_PRODUCT P 
	                WHERE
		                P.PRODUCT_ID IN ( SELECT PRODUCT_ID FROM USER_COMPARE_PRODUCT WHERE IS_ACTIVE = 1 AND TENANT_ID =@tenantId AND USER_ID = @userId ) 
		                AND P.TENANT_ID=@tenantId 
	                AND IS_ACTIVE = 1 FOR json path 
	                ) Json";


            List<Compare> compare = null;
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

                compare = JsonConvert.DeserializeObject<List<Compare>>(json);
                compare?.ForEach(x =>
                {
                    x.Slug = x.Name.UrlFriendly(stringLang) + "-" + x.ProductId;
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return compare;
        }

        public List<Compare> SearchProduct (int lang,string key, string tenantId)
        {
            string sql = $@"SELECT
	                        (
	                        SELECT
		                        PRODUCT_GUID AS Id,
		                        P.NAME{ (lang == 1 ? "" : lang.ToString()) } + ISNULL(
			                        STUFF(
				                        (
				                        SELECT
					                        ' ' + [VALUE{ (lang == 1 ? "" : lang.ToString()) }] 
				                        FROM
					                        NEW_PRODUCT_VARIATION t1 
				                        WHERE
					                        t1.PRODUCT_ID = P.PRODUCT_ID 
					                        AND ( SELECT V.[SHOW_IN_NAME] FROM NEW_VARIATION V WHERE V.VARIATION_ID = t1.VARIATION_ID AND V.IS_ACTIVE = 1 ) = 1 
					                        AND t1.IS_ACTIVE= 1 FOR XML PATH ( '' ) 
				                        ),
				                        1,
				                        0,
				                        '' 
			                        ),
			                        '' 
		                        ) Name,
		                        Json_Query ( ( SELECT PR_CAT_ID AS Id, NAME{ (lang == 1 ? "" : lang.ToString()) } FROM NEW_PRODUCT_CATEGORY PC WHERE PC.PR_CAT_ID= P.PR_CAT_ID FOR json path, without_array_wrapper ) ) Category,
		                        PRICE,
		                        DISCOUNTED_PRICE Discount,
		                        (
		                        SELECT
			                        PF.[UPLOAD_FILE_IMAGE_ID] Id,
			                        UF.PATH + UF.FILENAME + UF.EXTENSION FilePath 
		                        FROM
			                        NEW_PRODUCT_FILE PF
			                        INNER JOIN NEW_UPLOAD_FILE UF ON UF.UPLOAD_FILE_ID = PF.UPLOAD_FILE_IMAGE_ID 
		                        WHERE
			                        PF.PRODUCT_ID = P.PRODUCT_ID 
			                        AND PF.IS_ACTIVE = 1 FOR json path 
		                        ) [Images] 
	                        FROM
		                        NEW_PRODUCT P 
	                        WHERE
		                        P.NAME LIKE '%' + @key + '%'
		                        AND P.TENANT_ID= @tenantId
	                        AND P.IS_ACTIVE = 1 FOR json path 
	                        ) Json";


            List<Compare> compare = null;
            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@tenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@key",SqlDbType.NVarChar,100,ParameterDirection.Input,key)

                    });

                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                }

                compare = JsonConvert.DeserializeObject<List<Compare>>(json);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return compare;
        }

        public int DeleteCompare(string compareGuid, string tenantId, int userId)  
        {
            int returnId = 0;
            try
            {
                using (var con = new DbHandler())
                {
                    returnId = con.ExecStoredProcWithReturnIntValue("CompareDelete", new[]
                    {
                        DbHandler.SetParameter("@pCompareGuid", SqlDbType.VarChar, -1, ParameterDirection.Input, compareGuid),
                        DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });

                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not DeleteCompare...");
                Log.Error(ex);
                throw;
            }

            return returnId;
        }
    }
}