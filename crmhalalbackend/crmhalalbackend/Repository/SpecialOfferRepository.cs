using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Castle.Core.Internal;
using CRMHalalBackEnd.DB;
using CRMHalalBackEnd.Helpers;
using CRMHalalBackEnd.Models.SpecialOffer;
using Newtonsoft.Json;

namespace CRMHalalBackEnd.Repository
{
    public class SpecialOfferRepository
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public List<SpecialOfferDto> InsertSpecialOffer(SpecialOfferInsDto insDto,int lang, string langString, string tenantId, int userId)
        {
            List<SpecialOfferDto> specialOfferDtos;

            try
            {
                var json = JsonConvert.SerializeObject(insDto);
                using (var con = new DbHandler())
                {
                    con.ExecuteStoredProcedure("SpecialOfferInsert",new []
                    {
                        DbHandler.SetParameter("@pRequestAsJson", SqlDbType.NVarChar, -1, ParameterDirection.Input, json),
                        DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@pLogUserId",SqlDbType.Int,-1,ParameterDirection.Input,userId)

                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not InsertSpecialOffer...");
                Log.Error(ex);
                throw;
            }

            return GetAllSpecialOffer(lang, langString, insDto.DailyOffer,tenantId, userId);
        }
        public void DeleteSpecialOffer(int specialOfferId, string tenantId, int userId)
        {
            try
            {
                using (var con = new DbHandler())
                {
                    con.ExecuteStoredProcedure("SpecialOfferDelete", new[]
                    {
                        DbHandler.SetParameter("@pSpecialId", SqlDbType.Int, 10, ParameterDirection.Input,
                            specialOfferId),
                        DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@pLogUserId",SqlDbType.Int,-1,ParameterDirection.Input,userId)

                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not DeleteSpecialOffer...");
                Log.Error(ex);
                throw;
            }
        }

        public List<SpecialOfferDto> GetAllSpecialOffer(int lang, string langString, bool isDailyOffer, string tenantId,int userId)
        {
            string sql =
                $@"SELECT
	                (
	                SELECT
		                SO.SPECIAL_OFFER_ID Id,
		                JSON_QUERY (
			                (
			                SELECT
				                S.TENANT_ID TenantId,
				                S.STORE_GUID StoreGuid,
				                S.NAME Name,
                                S.DOMAIN [Domain]
			                FROM
				                NEW_STORE S 
			                WHERE
				                S.TENANT_ID = P.TENANT_ID FOR json path,
				                without_array_wrapper 
			                ) 
		                ) Store,
                        json_query((select PC.PR_CAT_ID Id, PC.NAME Name from NEW_PRODUCT_CATEGORY PC where PC.PR_CAT_ID = P.PR_CAT_ID for json path, without_array_wrapper)) Category,
		                P.PRODUCT_GUID ProductGuid,
		                P.NAME{(lang==1?"":lang.ToString())} + ISNULL(
			                STUFF(
				                (
				                SELECT
					                ' ' + [VALUE{(lang == 1 ? "" : lang.ToString())}] 
				                FROM
					                NEW_PRODUCT_VARIATION t1 
				                WHERE
					                t1.PRODUCT_ID = P.PRODUCT_ID 
					                AND ( SELECT [SHOW_IN_NAME] FROM NEW_VARIATION WHERE VARIATION_ID = t1.VARIATION_ID ) = 1 FOR XML PATH ( '' ) 
				                ),
				                1,
				                0,
				                '' 
			                ),
			                '' 
		                ) Name,
		                P.PRICE Price,
		                P.DISCOUNTED_PRICE Discount,
		                JSON_QUERY (
			                (
			                SELECT
				                PF.[UPLOAD_FILE_IMAGE_ID] Id,
				                UF.PATH + UF.FILENAME + UF.EXTENSION FilePath 
			                FROM
				                NEW_PRODUCT_FILE PF
				                INNER JOIN NEW_UPLOAD_FILE UF ON UF.UPLOAD_FILE_ID = PF.UPLOAD_FILE_IMAGE_ID 
			                WHERE
				                PF.PRODUCT_ID = P.PRODUCT_ID 
				                AND PF.IS_ACTIVE = 1 
				                AND PF.WEIGHT= 1 FOR json path,
				                without_array_wrapper 
			                ) 
		                ) [Image],
		                SO.BEGIN_DATE BeginDate,
		                SO.END_DATE EndDate,
		                SO.IS_DAILY_OFFER DailyOffer 
	                FROM
		                NEW_SPECIAL_OFFER SO
		                INNER JOIN NEW_PRODUCT P ON P.PRODUCT_ID = SO.PRODUCT_ID 
	                WHERE
		                SO.IS_ACTIVE= 1 
		                AND SO.TENANT_ID=@tenantId
		                AND IS_DAILY_OFFER = @isDailyOffer 
                        AND SO.PRODUCT_ID in (select PP.PRODUCT_ID from NEW_PRODUCT PP where PP.IS_VISIBLE=1)
                        AND EXISTS(SELECT * FROM dbo.GetEmployeePermission(@userId,@tenantId, '25')
)
	                ORDER BY
	                BEGIN_DATE DESC FOR json path 
	                ) Json";

            List<SpecialOfferDto> offerDtos;
            try
            {
                string json = string.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@tenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@isDailyOffer", SqlDbType.Bit, -1, ParameterDirection.Input, isDailyOffer),
                        DbHandler.SetParameter("@userId", SqlDbType.Int, 10, ParameterDirection.Input, userId)

                    });
                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                    //Nese elave edende evvelce check etmek lazimdir.
                    offerDtos = JsonConvert.DeserializeObject<List<SpecialOfferDto>>(json);
                    offerDtos = offerDtos ?? new List<SpecialOfferDto>();

                }
                offerDtos.ForEach(action => action.Slug = action.Name.UrlFriendly(langString));

            }
            catch (Exception ex)
            {
                Log.Warn("Could not GetAllSpecialOffer...");
                Log.Error(ex);
                throw;
            }

            return offerDtos;
        }
        public List<SpecialOfferDto> GetAllSpecialOfferByBeginAndEndDate(int lang, string langString, string tenantId,int userId,bool isDailyOffer, string beginDate,string endDate)
        {
            string sql =
                $@"select (SELECT
	                SO.SPECIAL_OFFER_ID Id,
                    JSON_QUERY (
			            (
			            SELECT
				            S.TENANT_ID TenantId,
				            S.STORE_GUID StoreGuid,
				            S.NAME Name 
			            FROM
				            NEW_STORE S 
			            WHERE
				            S.TENANT_ID = P.TENANT_ID FOR json path,
				            without_array_wrapper 
			            ) 
		            ) Store,
                    json_query((select PC.PR_CAT_ID Id, PC.NAME Name from NEW_PRODUCT_CATEGORY PC where PC.PR_CAT_ID = P.PR_CAT_ID for json path, without_array_wrapper)) Category,
                    P.PRODUCT_GUID ProductGuid,
	                P.NAME{(lang==1?"":lang.ToString())} + ISNULL(
		                STUFF(
			                (
			                SELECT
				                ' ' + [VALUE{(lang == 1 ? "" : lang.ToString())}] 
			                FROM
				                NEW_PRODUCT_VARIATION t1 
			                WHERE
				                t1.PRODUCT_ID = P.PRODUCT_ID 
				                AND ( SELECT [SHOW_IN_NAME] FROM NEW_VARIATION WHERE VARIATION_ID = t1.VARIATION_ID ) = 1 FOR XML PATH ( '' ) 
			                ),
			                1,
			                0,
			                '' 
		                ),
		                '' 
	                ) Name,
	                P.PRICE Price,
	                P.DISCOUNTED_PRICE Discount,
	                JSON_QUERY((
	                SELECT
		                PF.[UPLOAD_FILE_IMAGE_ID] Id,
		                UF.PATH + UF.FILENAME + UF.EXTENSION FilePath 
	                FROM
		                NEW_PRODUCT_FILE PF
		                INNER JOIN NEW_UPLOAD_FILE UF ON UF.UPLOAD_FILE_ID = PF.UPLOAD_FILE_IMAGE_ID 
	                WHERE
		                PF.PRODUCT_ID = P.PRODUCT_ID 
		                AND PF.IS_ACTIVE = 1 AND PF.WEIGHT=1 FOR json path,without_array_wrapper 
	                )) [Image],
                    SO.BEGIN_DATE BeginDate,
                    SO.END_DATE EndDate,
                    SO.IS_DAILY_OFFER DailyOffer
                FROM
	                NEW_SPECIAL_OFFER SO
	                INNER JOIN NEW_PRODUCT P ON P.PRODUCT_ID = SO.PRODUCT_ID
                WHERE
	                SO.IS_ACTIVE= 1 AND 
                    SO.TENANT_ID=@tenantId AND
                    SO.PRODUCT_ID in (select PP.PRODUCT_ID from NEW_PRODUCT PP where PP.IS_VISIBLE=1) 
                    IS_DAILY_OFFER=@isDailyOffer {(isDailyOffer ? "AND SO.BEGIN_DATE = @beginDate": "AND SO.BEGIN_DATE >= @beginDate AND SO.END_DATE <= @endDate")} AND
                    EXISTS(SELECT * FROM dbo.GetEmployeePermission(@userId,@tenantId, '25')
                    ORDER BY BEGIN_DATE DESC  for json path) Json";
            List<SpecialOfferDto> offerDtos;
            try
            {
                string json = string.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new []
                    {
                        DbHandler.SetParameter("@tenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@isDailyOffer", SqlDbType.Bit, -1, ParameterDirection.Input, isDailyOffer),
                        DbHandler.SetParameter("@beginDate", SqlDbType.VarChar, 15, ParameterDirection.Input, beginDate),
                        DbHandler.SetParameter("@endDate", SqlDbType.VarChar, 15, ParameterDirection.Input, endDate),
                        DbHandler.SetParameter("@userId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                });
                    
                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                    //Nese elave edende evvelce check etmek lazimdir.
                    offerDtos = JsonConvert.DeserializeObject<List<SpecialOfferDto>>(json);
                    offerDtos = offerDtos ?? new List<SpecialOfferDto>();
                }
                offerDtos.ForEach(action => action.Slug = action.Name.UrlFriendly(langString));

            }
            catch (Exception ex)
            {
                Log.Warn("Could not GetAllSpecialOfferByBeginAndEndDate...");
                Log.Error(ex);
                throw;
            }

            return offerDtos;
        }

        public List<SpecialOfferDto> GetAllSpecialOfferByStorePage(int lang, string langString, string tenantId, bool isDailyOffer, string beginDate, int userId)
        {
            string sql =
                $@"select (SELECT
	                SO.SPECIAL_OFFER_ID Id,
                        {(userId != 0 ? "(SELECT CASE WHEN P.PRODUCT_ID IN (select PRODUCT_ID from NEW_USER_FAV_PRODUCT where IS_ACTIVE=1 and USER_ID=@userId) THEN 1 ELSE 0 END AS IsFavorite)IsFavorite ," : "")} 
                    JSON_QUERY (
			            (
			            SELECT
				            S.TENANT_ID TenantId,
				            S.STORE_GUID StoreGuid,
				            S.NAME Name 
			            FROM
				            NEW_STORE S 
			            WHERE
				            S.TENANT_ID = P.TENANT_ID FOR json path,
				            without_array_wrapper 
			            ) 
		            ) Store,
                    json_query((select PC.PR_CAT_ID Id, PC.NAME{(lang==1?"":lang.ToString())} Name from NEW_PRODUCT_CATEGORY PC where PC.PR_CAT_ID = P.PR_CAT_ID for json path, without_array_wrapper)) Category,
                    P.PRODUCT_GUID ProductGuid,
                    P.PRODUCT_ID ProductId,
	                P.NAME{(lang == 1 ? "" : lang.ToString())} + ISNULL(
		                STUFF(
			                (
			                SELECT
				                ' ' + [VALUE{(lang == 1 ? "" : lang.ToString())}] 
			                FROM
				                NEW_PRODUCT_VARIATION t1 
			                WHERE
				                t1.PRODUCT_ID = P.PRODUCT_ID 
				                AND ( SELECT [SHOW_IN_NAME] FROM NEW_VARIATION WHERE VARIATION_ID = t1.VARIATION_ID ) = 1 FOR XML PATH ( '' ) 
			                ),
			                1,
			                0,
			                '' 
		                ),
		                '' 
	                ) Name,
                        JSON_QUERY (
	                    (
		                    SELECT
			                    PUC.PR_UNIT_ID Id,
			                    PUT.UNIT_CODE UnitCode,
			                    PUC.IS_DECIMAL IsDecimal,
			                    PUT.NAME Name 
		                    FROM
			                    NEW_PR_UNIT_CODE PUC
			                    INNER JOIN NEW_PR_UNIT_TRANSLATE PUT ON PUT.PR_UNIT_ID = PUC.PR_UNIT_ID 
		                    WHERE
			                    PUC.PR_UNIT_ID = P.UNIT_CODE_ID
			                    AND PUT.LANGUAGE_ID = ( SELECT SL.LANGUAGE_ID FROM NEW_STORE_LANGUAGE SL WHERE SL.IS_ACTIVE = 1 AND SL.TENANT_ID = P.TENANT_ID AND SL.NUMBER = {lang} ) FOR json path,
			                    without_array_wrapper 
		                    ) 
	                    ) MeasureType,
	                P.PRICE Price,
	                P.DISCOUNTED_PRICE Discount,
	                JSON_QUERY((
	                SELECT
		                PF.[UPLOAD_FILE_IMAGE_ID] Id,
		                UF.PATH + UF.FILENAME + UF.EXTENSION FilePath 
	                FROM
		                NEW_PRODUCT_FILE PF
		                INNER JOIN NEW_UPLOAD_FILE UF ON UF.UPLOAD_FILE_ID = PF.UPLOAD_FILE_IMAGE_ID 
	                WHERE
		                PF.PRODUCT_ID = P.PRODUCT_ID 
		                AND PF.IS_ACTIVE = 1 AND PF.WEIGHT=1 FOR json path,without_array_wrapper 
	                )) [Image],
                    SO.BEGIN_DATE BeginDate,
                    SO.END_DATE EndDate,
                    SO.IS_DAILY_OFFER DailyOffer
                FROM
	                NEW_SPECIAL_OFFER SO
	                INNER JOIN NEW_PRODUCT P ON P.PRODUCT_ID = SO.PRODUCT_ID
                WHERE
	                SO.IS_ACTIVE= 1 AND 
                    SO.TENANT_ID=@tenantId AND 
                    P.STOCK_QUANTITY > 0 AND
                    SO.PRODUCT_ID in (select PP.PRODUCT_ID from NEW_PRODUCT PP where PP.IS_VISIBLE=1)  AND
                    SO.IS_DAILY_OFFER=@isDailyOffer AND 
                    @date BETWEEN SO.BEGIN_DATE and SO.END_DATE ORDER BY BEGIN_DATE  for json path) Json";
            List<SpecialOfferDto> offerDtos;
            try
            {
                string json = string.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@tenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@isDailyOffer", SqlDbType.Bit, -1, ParameterDirection.Input, isDailyOffer),
                        DbHandler.SetParameter("@date", SqlDbType.VarChar, 15, ParameterDirection.Input, beginDate),
                        DbHandler.SetParameter("@userId",SqlDbType.Int,10,ParameterDirection.Input,userId)

                    });

                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                    //Nese elave edende evvelce check etmek lazimdir.
                    offerDtos = JsonConvert.DeserializeObject<List<SpecialOfferDto>>(json);
                    offerDtos = offerDtos ?? new List<SpecialOfferDto>();
                }
                offerDtos.ForEach(action => action.Slug = action.Name.UrlFriendly(langString) + "-" + action.ProductId);

            }
            catch (Exception ex)
            {
                Log.Warn("Could not GetAllSpecialOfferByBeginDate...");
                Log.Error(ex);
                throw;
            }

            return offerDtos;
        }

    }
}