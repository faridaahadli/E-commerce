using CRMHalalBackEnd.DB;
using CRMHalalBackEnd.Models.Promotion;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using Castle.Core.Internal;
using CRMHalalBackEnd.Helpers;
using CRMHalalBackEnd.Models.Order;

namespace CRMHalalBackEnd.Repository
{
    public class PromotionRepository
    {
        private static readonly log4net.ILog Log =
           log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public IEnumerable<PromotionResponse> Insert(PromotionInsDto promo, string tenantId, int userId)
        {

            try
            {
                using (var conn = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(promo);
                    conn.ExecStoredProcWithReturnIntValue("[PromotionInsert]", new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson", SqlDbType.NVarChar, -1, ParameterDirection.Input, json),
                        DbHandler.SetParameter("@pTenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not ProductVariationSave...");
                Log.Error(ex);
                throw;
            }
            return GetPromotions(tenantId, userId);
        }


        public IEnumerable<PromotionResponse> GetPromotions(string tenantId, int userId, string date = "")
        {
            string sql = $@"Select(select PROMO_ID PromotionId,
                        PROMO_TYPE_ID TypeId,
                        BEGIN_DATE BeginDate,
                        END_DATE  EndDate,
                        AMOUNT Amount,
                        PROMO_AMOUNT PromoAmount,
                        [DESCRIPTION] [Description],
                        [DESCRIPTION2] [Description2],
                        [DESCRIPTION3] [Description3],
                        [DESCRIPTION4] [Description4],
                        [TEXT]     [Text],
                        [TEXT2]    [Text2],
                        [TEXT3]    [Text3],
                        [TEXT4]    [Text4],
                        (
                        select QUANTITY Quantity,IS_PROMO_PRODUCT IsPromoProduct,
                        (select top 1 PRODUCT_GUID from NEW_PRODUCT
                        where PRODUCT_ID=prmPrd.PRODUCT_ID) ProductGuid
                        from NEW_PROMOTION_PRODUCT prmPrd where prmPrd.PROMO_ID=np.PROMO_ID for json path
                        )[PromoProducts],
                         JSON_QUERY(
                        (
	                    SELECT top 1
		                    PROMO_IMG_ID Id,
		                    UF.PATH + UF.FILENAME + UF.EXTENSION FilePath 
	                    FROM NEW_UPLOAD_FILE UF where UF.UPLOAD_FILE_ID=PROMO_IMG_ID
		                FOR json path,without_array_wrapper
	                    ) )[Image]
                       from NEW_PROMOTION np
                       where TENANT_ID=@pTenantId and IS_ACTIVE=1 {(!date.IsNullOrEmpty() ? "AND @date BETWEEN BEGIN_DATE AND END_DATE" : "")} and exists(select * from dbo.GetEmployeePermission(@UserId,@pTenantId , '25'))
                        for json path)Json";
            List<PromotionResponse> promotions;
            try
            {
                string json = string.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@pTenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@UserId", SqlDbType.Int, 10, ParameterDirection.Input, userId),
                        DbHandler.SetParameter("@date", SqlDbType.VarChar, 15, ParameterDirection.Input, date)
                    });
                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                    promotions = JsonConvert.DeserializeObject<List<PromotionResponse>>(json);
                    promotions = promotions ?? new List<PromotionResponse>();
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not GetPromotions...");
                Log.Error(ex);
                throw;
            }
            return promotions;
        }

        public IEnumerable<PromotionResponse> GetPromotionsWithoutUser(int lang, string langString, string tenantId, string date = "")
        {
            string sql = $@"Select(select PROMO_ID PromotionId,
                        PROMO_TYPE_ID TypeId,
                        BEGIN_DATE BeginDate,
                        END_DATE  EndDate,
                        AMOUNT Amount,
                        PROMO_AMOUNT PromoAmount,
                        [DESCRIPTION{(lang == 1?"":lang.ToString())}] [Description],
                        [TEXT{(lang == 1 ? "" : lang.ToString())}]     [Text],
                        (
                        select QUANTITY Quantity,IS_PROMO_PRODUCT IsPromoProduct,
                        (select top 1 PRODUCT_GUID from NEW_PRODUCT
                        where PRODUCT_ID=prmPrd.PRODUCT_ID) ProductGuid
                        from NEW_PROMOTION_PRODUCT prmPrd where prmPrd.PROMO_ID=np.PROMO_ID for json path
                        )[PromoProducts],
                         JSON_QUERY(
                        (
	                    SELECT top 1
		                    PROMO_IMG_ID Id,
		                    UF.PATH + UF.FILENAME + UF.EXTENSION FilePath 
	                    FROM NEW_UPLOAD_FILE UF where UF.UPLOAD_FILE_ID=PROMO_IMG_ID
		                FOR json path,without_array_wrapper
	                    ) )[Image]
                       from NEW_PROMOTION np
                       where TENANT_ID=@pTenantId and IS_ACTIVE=1 {(!date.IsNullOrEmpty() ? "AND @date BETWEEN BEGIN_DATE AND END_DATE" : "")} for json path)Json";
            List<PromotionResponse> promotions;
            try
            {
                string json = string.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@pTenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@date", SqlDbType.VarChar, 15, ParameterDirection.Input, date)
                    });
                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                    promotions = JsonConvert.DeserializeObject<List<PromotionResponse>>(json);
                    promotions = promotions ?? new List<PromotionResponse>();
                    promotions.ForEach(x =>
                    {
                        x.Slug = x.Description.UrlFriendly(langString) + "-" + x.PromotionId;
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not GetPromotions...");
                Log.Error(ex);
                throw;
            }
            return promotions;
        }

        public PromotionResponse GetSinglePromotion(int lang,string langString, int promotionId, string domain)
        {
            string sql = $@"Select(select PROMO_ID PromotionId,
                        PROMO_TYPE_ID TypeId,
                        BEGIN_DATE BeginDate,
                        END_DATE  EndDate,
                        AMOUNT Amount,
                        PROMO_AMOUNT PromoAmount,
                        [DESCRIPTION{(lang==1?"":lang.ToString())}] [Description],
						[TEXT{(lang == 1 ? "" : lang.ToString())}]   [Text],
                        (
                        select QUANTITY Quantity,
						IS_PROMO_PRODUCT IsPromoProduct,
                        ( SELECT TOP 1 PRODUCT_ID FROM NEW_PRODUCT WHERE PRODUCT_ID = prmPrd.PRODUCT_ID ) PromoProductId,
                        (select top 1 PRODUCT_GUID from NEW_PRODUCT
                        where PRODUCT_ID=prmPrd.PRODUCT_ID) ProductGuid,
						(select PRICE FROM NEW_PRODUCT WHERE
						 PRODUCT_ID=prmPrd.PRODUCT_ID
						)Price,
						(select DISCOUNTED_PRICE FROM NEW_PRODUCT WHERE
						 PRODUCT_ID=prmPrd.PRODUCT_ID
						)Discount,
						  json_query((select PC.PR_CAT_ID Id, PC.NAME{(lang == 1 ? "" : lang.ToString())} Name from NEW_PRODUCT_CATEGORY PC where PC.PR_CAT_ID = prmPrd.PRODUCT_ID for json path, without_array_wrapper)) Category,
						(select [NAME{(lang == 1 ? "" : lang.ToString())}] + ISNULL(
			STUFF(
				(
				SELECT
					' ' + [VALUE{(lang == 1 ? "" : lang.ToString())}] 
				FROM
					NEW_PRODUCT_VARIATION t1 
				WHERE
					t1.PRODUCT_ID = prmPrd.PRODUCT_ID 
					AND ( SELECT [SHOW_IN_NAME] FROM NEW_VARIATION WHERE VARIATION_ID = t1.VARIATION_ID ) = 1 FOR XML PATH ( '' ) 
				),
				1,
				0,
				'' 
			),
			'' 
		) FROM NEW_PRODUCT WHERE
						 PRODUCT_ID=prmPrd.PRODUCT_ID
		)[Name],
		JSON_QUERY((
		                SELECT Top 1
			                PF.[UPLOAD_FILE_IMAGE_ID] Id,
			                UF.PATH + UF.FILENAME + UF.EXTENSION FilePath 
		                FROM
			                NEW_PRODUCT_FILE PF
			                INNER JOIN NEW_UPLOAD_FILE UF ON UF.UPLOAD_FILE_ID = PF.UPLOAD_FILE_IMAGE_ID 
		                WHERE
			                PF.PRODUCT_ID = prmPrd.PRODUCT_ID
			                AND PF.IS_ACTIVE = 1 AND PF.WEIGHT = 1 FOR json path,without_array_wrapper
		                )) [Image]

                        from NEW_PROMOTION_PRODUCT prmPrd where prmPrd.PROMO_ID=np.PROMO_ID for json path
                        )[PromoProducts],
						JSON_QUERY(
								(select 
               stor.STORE_GUID StoreGuid,
               stor.[NAME] [Name],
               stor.[STATUS] Status,
	           stor.TENANT_ID TenantId,
               stor.WORK_TIME_END WorkEndTime,
			   stor.WORK_TIME_START WorkStartTime,
               stor.DOMAIN [Domain],
               stor.SHOW_SPECIAL ShowSpecial,
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
	          where UF.UPLOAD_FILE_ID = stor.LOGO_IMG_ID for json path)LogoImage	
              from NEW_STORE as stor
              where TENANT_ID=np.TENANT_ID 
								 for json path, without_array_wrapper)
								)Store,

                         JSON_QUERY(
                        (
	                    SELECT top 1
		                    PROMO_IMG_ID Id,
		                    UF.PATH + UF.FILENAME + UF.EXTENSION FilePath 
	                    FROM NEW_UPLOAD_FILE UF where UF.UPLOAD_FILE_ID=PROMO_IMG_ID
		                FOR json path,without_array_wrapper
	                    ) )[Image]
                       from NEW_PROMOTION np
                       where PROMO_ID=@pPromotionId and TENANT_ID=(select TENANT_ID from NEW_STORE where DOMAIN=@pDomain)
					   and IS_ACTIVE=1  for json path,without_array_wrapper)Json";
            PromotionResponse promotion;
            try
            {
                string json = string.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {

                         DbHandler.SetParameter("@pPromotionId", SqlDbType.VarChar, 5, ParameterDirection.Input,promotionId),
                         DbHandler.SetParameter("@pDomain", SqlDbType.NVarChar, 50, ParameterDirection.Input,domain),
                    });
                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                    promotion = JsonConvert.DeserializeObject<PromotionResponse>(json);

                    promotion?.PromoProducts.ForEach(x =>
                    {
                        x.Slug = x.Name.UrlFriendly(langString) + "-" + x.PromoProductId;
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not GetPromotions...");
                Log.Error(ex);
                throw;
            }
            return promotion;
        }

        public void Delete(int promoId, string tenantId, int userId)
        {

            try
            {
                using (var conn = new DbHandler())
                {

                    conn.ExecStoredProcWithReturnIntValue("[PromotionDelete]", new[]
                    {
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId),
                        DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,-1,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@pPromotionId",SqlDbType.Int,10,ParameterDirection.Input,promoId),
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not Promotion delete...");
                Log.Error(ex);
                throw;
            }
        }

        public IEnumerable<OrderTypeOnePromo> GetTypeOnePromotions(List<OrderByStore> tenants)
        {

            string sql = @"	select(SELECT
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
				S.TENANT_ID =@pTenantId FOR json path,
				without_array_wrapper 
			) 
		) Store,
		Json_Query (
			(
			SELECT
				np.PROMO_ID PromotionId,
				np.[DESCRIPTION] [Description],
				JSON_QUERY (
					(
					SELECT TOP
						1 PROMO_IMG_ID Id,
						UF.PATH + UF.FILENAME + UF.EXTENSION FilePath 
					FROM
						NEW_UPLOAD_FILE UF 
					WHERE
						UF.UPLOAD_FILE_ID = PROMO_IMG_ID 
						AND UF.IS_ACTIVE = 1 FOR json path,
						without_array_wrapper 
					) 
				) [Image],
				(
				SELECT
					innerPrd.NAME + ISNULL(
						STUFF(
							(
							SELECT
								' ' + [VALUE] 
							FROM
								NEW_PRODUCT_VARIATION t1 
							WHERE
								t1.PRODUCT_ID = pmPrd.PRODUCT_ID
								AND ( SELECT [SHOW_IN_NAME] FROM NEW_VARIATION WHERE VARIATION_ID = t1.VARIATION_ID ) = 1 FOR XML PATH ( '' ) 
							),
							1,
							0,
							'' 
						),
						'' 
					) [ProductName],
					innerPrd.PRODUCT_GUID ProductGuid,
					pmPrd.PROMO_ID PromotionId,
					pmPrd.IS_PROMO_PRODUCT IsPromoProduct,
					pmPrd.QUANTITY Quantity,
					JSON_QUERY (
						(
						SELECT TOP
							1 PF.[UPLOAD_FILE_IMAGE_ID] Id,
							UF.PATH + UF.FILENAME + UF.EXTENSION FilePath 
						FROM
							NEW_PRODUCT_FILE PF
							LEFT JOIN NEW_UPLOAD_FILE UF ON UF.UPLOAD_FILE_ID = PF.UPLOAD_FILE_IMAGE_ID 
						WHERE
							PF.PRODUCT_ID = innerPrd.PRODUCT_ID 
							AND PF.IS_ACTIVE = 1 FOR json path,
							without_array_wrapper 
						) 
					) [Image] 
				FROM
					NEW_PRODUCT AS innerPrd
					LEFT JOIN [NEW_PROMOTION_PRODUCT] pmPrd ON pmPrd.PRODUCT_ID= innerPrd.PRODUCT_ID
					LEFT JOIN NEW_PR_UNIT_CODE AS untCode ON untCode.PR_UNIT_ID= innerPrd.UNIT_CODE_ID 
				WHERE
					innerPrd.PRODUCT_ID IN ( SELECT PRODUCT_ID FROM NEW_PROMOTION_PRODUCT WHERE PROMO_ID =np.PROMO_ID AND IS_ACTIVE = 1 ) 
					AND pmPrd.PROMO_ID= np.PROMO_ID FOR json path 
				) [PromoProducts] 
			FROM
				NEW_PROMOTION np
			WHERE
				np.TENANT_ID=@pTenantId and np.IS_ACTIVE=1 and np.PROMO_TYPE_ID=1
                and np.Amount<=@pStorePrice
			for json path) ) [Promotions]
		for json path,without_array_wrapper
		)json";


            string json = null;
            List<OrderTypeOnePromo> promotions = new List<OrderTypeOnePromo>();
            OrderTypeOnePromo promo = null;
            if (tenants == null)
                return null;
            try
            {
                using (var conn = new DbHandler())
                {

                    foreach (var obj in tenants)
                    {
                        var dr = conn.ExecuteSql(sql, new[]
                        {
                            DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,obj.TenantId),
                            DbHandler.SetParameter("@pStorePrice",SqlDbType.Money,5,ParameterDirection.Input,obj.StorePrice)
                        });

                        if (dr.Read())
                        {
                            json = dr["json"].ToString();
                        }
                        dr.Close();
                        promo = JsonConvert.DeserializeObject<OrderTypeOnePromo>(json);

                        promotions.Add(promo);

                    }

                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not CheckForPromotion...");
                Log.Error(ex);
                throw;
            }
            return promotions;
        }

        public IEnumerable<OrderTypeOnePromo> CheckTypeOnePromotion(OrderInsDto basket, int userId)
        {

            string tableTenant = null;
            List<OrderByStore> tenants = new List<OrderByStore>();
            try
            {
                using (var conn = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(basket);
                    tableTenant = conn.ExecStoredProcWithOutputValue("[CheckForPromotion]", "@pResult", SqlDbType.NVarChar, -1, new[]
                     {
                        DbHandler.SetParameter("@pRequestAsJson", SqlDbType.NVarChar, -1, ParameterDirection.Input, json),

                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });

                    tenants = JsonConvert.DeserializeObject<List<OrderByStore>>(tableTenant);

                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not CheckForPromotion...");
                Log.Error(ex);
                throw;
            }
            return GetTypeOnePromotions(tenants);
        }
    }
}