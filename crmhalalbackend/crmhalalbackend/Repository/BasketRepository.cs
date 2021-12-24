using CRMHalalBackEnd.DB;
using CRMHalalBackEnd.Models.Basket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CRMHalalBackEnd.Helpers;

namespace CRMHalalBackEnd.Repository
{
    public class BasketRepository
    {
        private static readonly log4net.ILog Log =
          log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public IEnumerable<NewBasketResponse> Insert(NewBasket basket,int lang, string langString, int userId)
        {
            if (!basket.IsBasket || !basket.IsLoggedIn)
                return GetBuyOneElement(lang, langString, basket.Id,  basket.Quantity);
            try
            {
                var json = JsonConvert.SerializeObject(basket);
                using (var conn = new DbHandler())
                {
                    conn.ExecuteStoredProcedure("BasketInsert", new[]
                  {
                        DbHandler.SetParameter("@pRequestAsJson",SqlDbType.NVarChar,-1,ParameterDirection.Input,json),
                       DbHandler.SetParameter("@pLogUserId",SqlDbType.Int,10,ParameterDirection.Input,userId)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not BasketInsert...");
                Log.Error(ex);
                throw;
            }
            return GetAllBasketElements(lang, langString, userId);
        }

        public IEnumerable<NewBasketResponse> Insert(PromotionBasketInsDto basket,int lang, string langString, int userId)
        {
            if (!basket.IsLoggedIn)
                return GetBuyOnePromotion(lang, langString, basket.PromotionId, basket.Quantity);
            try
            {
                var json = JsonConvert.SerializeObject(basket);
                using (var conn = new DbHandler())
                {
                    conn.ExecuteStoredProcedure("PromotionBasketInsert", new[]
                  {
                        DbHandler.SetParameter("@pRequestAsJson",SqlDbType.NVarChar,-1,ParameterDirection.Input,json),
                       DbHandler.SetParameter("@pLogUserId",SqlDbType.Int,10,ParameterDirection.Input,userId)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not PromotionBasketInsert...");
                Log.Error(ex);
                throw;
            }
            return GetAllBasketElements(lang, langString, userId);
        }

        public IEnumerable<NewBasketResponse> GetAllBasketElements(int lang, string langString, int userId)
        {
            string sql =
				$@"SELECT
	(
	SELECT
		bsk.BASKET_GUID AS BasketGuid,
		JSON_QUERY (
			(
			SELECT
				S.TENANT_ID TenantId,
                S.[STATUS] Status,
				S.STORE_GUID StoreGuid,
				S.NAME Name,
                S.DOMAIN [Domain],
                S.IS_SALES IsSales
			FROM
				NEW_STORE S 
			WHERE
				S.TENANT_ID =
			CASE
					
					WHEN DATALENGTH( ( SELECT rtrim( ltrim( prd.PRODUCT_GUID ) ) ) ) > 0 THEN
					prd.TENANT_ID ELSE ( SELECT TOP 1 TENANT_ID FROM NEW_PROMOTION WHERE PROMO_ID = bsk.PROMOTION_ID ) 
				END FOR json path,
				without_array_wrapper 
			) 
		) Store,
		CASE
					
					WHEN DATALENGTH( ( SELECT rtrim( ltrim( prd.PRODUCT_GUID ) ) ) ) > 0 THEN
					prd.PRICE ELSE ( SELECT Price from GetPromoPrice(bsk.PROMOTION_ID) ) 
				END as Price,

				CASE
					
					WHEN DATALENGTH( ( SELECT rtrim( ltrim( prd.PRODUCT_GUID ) ) ) ) > 0 THEN
					(case
		                 when prd.DISCOUNTED_PRICE>0 then (prd.PRICE-prd.DISCOUNTED_PRICE)
		                 else 0
		              end) 
	        	ELSE ( SELECT Discount from GetPromoPrice(bsk.PROMOTION_ID) ) 
				END as Discount,
	
		bsk.QUANTITY,
		usr.USER_GUID,
        prd.PRODUCT_ID ProductId,
		prd.PRODUCT_GUID AS Id,
		prd.NAME{(lang == 1 ? "" : lang.ToString())} + ISNULL(
			STUFF(
				(
				SELECT
					' ' + [VALUE{(lang == 1 ? "" : lang.ToString())}] 
				FROM
					NEW_PRODUCT_VARIATION t1 
				WHERE
					t1.PRODUCT_ID = prd.PRODUCT_ID 
					AND ( SELECT [SHOW_IN_NAME] FROM NEW_VARIATION WHERE VARIATION_ID = t1.VARIATION_ID ) = 1 FOR XML PATH ( '' ) 
				),
				1,
				0,
				'' 
			),
			'' 
		) ProductName,
        prd.STOCK_QUANTITY StockQuantity,
		(
		SELECT 
			 PF.[UPLOAD_FILE_IMAGE_ID] Id,
			UF.PATH + UF.FILENAME + UF.EXTENSION FilePath 
		FROM
			NEW_PRODUCT_FILE PF
			LEFT JOIN NEW_UPLOAD_FILE UF ON UF.UPLOAD_FILE_ID = PF.UPLOAD_FILE_IMAGE_ID 
		WHERE
			PF.PRODUCT_ID = prd.PRODUCT_ID 
			AND PF.IS_ACTIVE = 1 and PF.WEIGHT = 1 FOR json path 
		) [Images],
		Json_Query (
			(
			SELECT
				PROMOTION_ID PromotionId,
				[DESCRIPTION{(lang == 1 ? "" : lang.ToString())}] [Description],
				JSON_QUERY (
					(
					SELECT
						 PROMO_IMG_ID Id,
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
					innerPrd.NAME{(lang == 1 ? "" : lang.ToString())} + ISNULL(
						STUFF(
							(
							SELECT
								' ' + [VALUE{(lang == 1 ? "" : lang.ToString())}] 
							FROM
								NEW_PRODUCT_VARIATION t1 
							WHERE
								t1.PRODUCT_ID = innerPrd.PRODUCT_ID 
								AND ( SELECT [SHOW_IN_NAME] FROM NEW_VARIATION WHERE VARIATION_ID = t1.VARIATION_ID ) = 1 FOR XML PATH ( '' ) 
							),
							1,
							0,
							'' 
						),
						'' 
					) [ProductName],
                    innerPrd.STOCK_QUANTITY StockQuantity,
					innerPrd.PRODUCT_GUID ProductGuid,
                    innerPrd.PRODUCT_ID ProductId,
					pmPrd.PROMO_ID PromotionId,
					pmPrd.IS_PROMO_PRODUCT IsPromoProduct,
					pmPrd.QUANTITY Quantity,
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
			                    PUC.PR_UNIT_ID = innerPrd.UNIT_CODE_ID
			                    AND PUT.LANGUAGE_ID = ( SELECT SL.LANGUAGE_ID FROM NEW_STORE_LANGUAGE SL WHERE SL.IS_ACTIVE = 1 AND SL.TENANT_ID = innerPrd.TENANT_ID AND SL.NUMBER = {lang} ) FOR json path,
			                    without_array_wrapper 
		                    ) 
	                    ) MeasureType,
					JSON_QUERY (
						(
						SELECT
							 PF.[UPLOAD_FILE_IMAGE_ID] Id,
							UF.PATH + UF.FILENAME + UF.EXTENSION FilePath 
						FROM
							NEW_PRODUCT_FILE PF
							LEFT JOIN NEW_UPLOAD_FILE UF ON UF.UPLOAD_FILE_ID = PF.UPLOAD_FILE_IMAGE_ID 
						WHERE
							PF.PRODUCT_ID = innerPrd.PRODUCT_ID 
							AND PF.IS_ACTIVE = 1 and PF.WEIGHT = 1 FOR json path,
							without_array_wrapper 
						) 
					) [Image] 
				FROM
					NEW_PRODUCT AS innerPrd
					LEFT JOIN [NEW_PROMOTION_PRODUCT] pmPrd ON pmPrd.PRODUCT_ID= innerPrd.PRODUCT_ID
				WHERE
					innerPrd.PRODUCT_ID IN ( SELECT PRODUCT_ID FROM NEW_PROMOTION_PRODUCT WHERE PROMO_ID = bsk.PROMOTION_ID AND IS_ACTIVE = 1 ) 
					AND pmPrd.PROMO_ID = bsk.PROMOTION_ID FOR json path 
				) [PromoProducts] 
			FROM
				NEW_PROMOTION 
			WHERE
				PROMO_ID = bsk.PROMOTION_ID AND IS_ACTIVE = 1 FOR json path,
				without_array_wrapper 
			) 
		) [Promotion],
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
			         PUC.PR_UNIT_ID = prd.UNIT_CODE_ID
			         AND PUT.LANGUAGE_ID = ( SELECT SL.LANGUAGE_ID FROM NEW_STORE_LANGUAGE SL WHERE SL.IS_ACTIVE = 1 AND SL.TENANT_ID = prd.TENANT_ID AND SL.NUMBER = {lang} ) FOR json path,
			         without_array_wrapper 
		         ) 
	         ) MeasureType 
	FROM
		NEW_BASKET AS bsk
		LEFT JOIN [NEW_USER] AS usr ON bsk.[USER_ID] = usr.[USER_ID]
		LEFT JOIN [NEW_PRODUCT] AS prd ON bsk.PRODUCT_ID= prd.PRODUCT_ID
	WHERE
		bsk.[USER_ID] = @pUserId
	AND bsk.IS_ACTIVE= 1 FOR json path 
	) Json";

            List<NewBasketResponse> baskets = new List<NewBasketResponse>();

            //NewBasketResponse basket = null;
            try
            {
                string json = String.Empty;
                using (var conn = new DbHandler())
                {
                    var dr = conn.ExecuteSql(sql, new[]{
                        DbHandler.SetParameter("@pUserId",SqlDbType.Int,10,ParameterDirection.Input,userId)
                    });

                    while (dr.Read())
                    {
                        json = dr["Json"].ToString();
                        baskets = JsonConvert.DeserializeObject<List<NewBasketResponse>>(json);
                        baskets = baskets ?? new List<NewBasketResponse>();
                        //data null olanda xeta verir deyie butun Desrialize-larda null olub olmadigin check eliyirem.
                    }
                    //foreach (var basket in baskets)
                    //{
                    //    basket.LastPrice = (basket.Price - basket.Discount) * basket.Quantity;
                    //}
                }
                baskets.ForEach(action => action.LastPrice = (action.Price - action.Discount) * action.Quantity);
                baskets.ForEach(x =>
                {
                    if (x.ProductId != null)
                    {
                        x.Slug = x.ProductName.UrlFriendly(langString) + "-" + x.ProductId;
                    }
                    else
                    {
                        var min = x.Promotion?.PromoProducts.Min(y => y.StockQuantity);
                        if (min != null) x.StockQuantity = min.Value;

                        x.Slug = x.Promotion?.Description.UrlFriendly(langString) + "-" + x.Promotion?.PromotionId;
                    }

                });
            }
            catch (Exception ex)
            {
                Log.Warn("Could not GetBasketElements...");
                Log.Error(ex);
                throw;
            }
            return baskets;
        }

        public IEnumerable<NewBasketResponse> GetBuyOneElement(int lang,string langString, string productGuid, decimal quantity)
        {

            if (quantity <= 0)
                throw new Exception("Məhsul qiyməti 0-dan kiçik və ya bərabər ola bilməz");

            string sql =
				$@"SELECT
	(
	SELECT
		prd.PRICE as Price,
		(case
		when prd.DISCOUNTED_PRICE>0 then (prd.PRICE-prd.DISCOUNTED_PRICE)
		else 0
		end) as Discount,
		@pQuantity as Quantity,
		prd.PRODUCT_GUID AS Id,
        prd.PRODUCT_ID	ProductId,
		prd.NAME{(lang==1?"":lang.ToString())} + ISNULL(
			STUFF(
				(
				SELECT
					' ' + [VALUE{(lang == 1 ? "" : lang.ToString())}] 
				FROM
					NEW_PRODUCT_VARIATION t1 
				WHERE
					t1.PRODUCT_ID = prd.PRODUCT_ID 
					AND ( SELECT [SHOW_IN_NAME] FROM NEW_VARIATION WHERE VARIATION_ID = t1.VARIATION_ID ) = 1 FOR XML PATH ( '' ) 
				),
				1,
				0,
				'' 
			),
			'' 
		) ProductName,
				JSON_QUERY (
			(
			SELECT
				S.TENANT_ID TenantId,
                S.[STATUS] Status,
				S.STORE_GUID StoreGuid,
				S.NAME Name,
                S.DOMAIN [Domain],
                S.IS_SALES IsSales
			FROM
				NEW_STORE S 
			WHERE
				S.TENANT_ID =
			CASE
					
					WHEN DATALENGTH( ( SELECT rtrim( ltrim( prd.PRODUCT_GUID ) ) ) ) > 0 THEN
					prd.TENANT_ID
				END FOR json path,
				without_array_wrapper 
			) 
		) Store,
        prd.STOCK_QUANTITY StockQuantity,
		(
		SELECT 
			 PF.[UPLOAD_FILE_IMAGE_ID] Id,
			UF.PATH + UF.FILENAME + UF.EXTENSION FilePath 
		FROM
			NEW_PRODUCT_FILE PF
			LEFT JOIN NEW_UPLOAD_FILE UF ON UF.UPLOAD_FILE_ID = PF.UPLOAD_FILE_IMAGE_ID 
		WHERE
			PF.PRODUCT_ID = prd.PRODUCT_ID 
			AND PF.IS_ACTIVE = 1 and PF.WEIGHT = 1 FOR json path 
		) [Images],
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
			         PUC.PR_UNIT_ID = prd.UNIT_CODE_ID
			         AND PUT.LANGUAGE_ID = ( SELECT SL.LANGUAGE_ID FROM NEW_STORE_LANGUAGE SL WHERE SL.IS_ACTIVE = 1 AND SL.TENANT_ID = prd.TENANT_ID AND SL.NUMBER = {lang} ) FOR json path,
			         without_array_wrapper 
		         ) 
	         ) MeasureType
	FROM
		[NEW_PRODUCT] AS prd
	WHERE
		prd.PRODUCT_GUID = @pProductGuid
		and prd.IS_ACTIVE=1
	 FOR json path 
	) Json";

            List<NewBasketResponse> baskets = new List<NewBasketResponse>();


            try
            {
                string json = String.Empty;
                using (var conn = new DbHandler())
                {
                    var dr = conn.ExecuteSql(sql, new[]{
                        DbHandler.SetParameter("@pProductGuid",SqlDbType.VarChar,50,ParameterDirection.Input,productGuid),
                        DbHandler.SetParameter("@pQuantity",SqlDbType.Decimal,50,ParameterDirection.Input,quantity)
                    });

                    while (dr.Read())
                    {
                        json = dr["Json"].ToString();
                        baskets = JsonConvert.DeserializeObject<List<NewBasketResponse>>(json);
                        baskets = baskets ?? new List<NewBasketResponse>();

                    }
                }
                baskets?.ForEach(action => action.LastPrice = (action.Price - action.Discount) * action.Quantity);
				baskets?.ForEach(x => x.Slug = x.ProductName.UrlFriendly(langString) + "-" +x.ProductId);
            }
            catch (Exception ex)
            {
                Log.Warn("Could not GetOne Product");
                Log.Error(ex);
                throw;
            }
            return baskets;
        }

        public IEnumerable<NewBasketResponse> GetBuyOnePromotion(int lang, string langString, int promotionId, decimal quantity)
        {

            if (quantity <= 0)
                throw new Exception("Məhsul qiyməti 0-dan kiçik və ya bərabər ola bilməz");

            string sql =
				$@"SELECT
	(
	SELECT

		JSON_QUERY (
			(
			SELECT
				S.TENANT_ID TenantId,
                S.[STATUS] Status,
				S.STORE_GUID StoreGuid,
				S.NAME Name,
                S.DOMAIN [Domain]
			FROM
				NEW_STORE S 
			WHERE
				S.TENANT_ID =(SELECT TOP 1 TENANT_ID FROM NEW_PROMOTION WHERE PROMO_ID = Promo.PROMO_ID)
				 FOR json path,
				without_array_wrapper 
			) 
		) Store,
	  ( SELECT Price from GetPromoPrice(Promo.PROMO_ID) ) as Price,

    ( SELECT Discount from GetPromoPrice(Promo.PROMO_ID) ) as Discount,
		@pQuantity as Quantity,
		Json_Query (
			(
			SELECT
				Promo.PROMO_ID PromotionId,
				Promo.[DESCRIPTION{(lang == 1 ? "" : lang.ToString())}] [Description],
				JSON_QUERY (
					(
					SELECT
						 PROMO_IMG_ID Id,
						UF.PATH + UF.FILENAME + UF.EXTENSION FilePath 
					FROM
						NEW_UPLOAD_FILE UF 
					WHERE
						UF.UPLOAD_FILE_ID = PROMO_IMG_ID 
						AND UF.IS_ACTIVE = 1  FOR json path,
						without_array_wrapper 
					) 
				) [Image],
				(
				SELECT
					innerPrd.NAME{(lang == 1 ? "" : lang.ToString())} + ISNULL(
						STUFF(
							(
							SELECT
								' ' + [VALUE{(lang == 1 ? "" : lang.ToString())}] 
							FROM
								NEW_PRODUCT_VARIATION t1 
							WHERE
								t1.PRODUCT_ID = innerPrd.PRODUCT_ID
								AND ( SELECT [SHOW_IN_NAME] FROM NEW_VARIATION WHERE VARIATION_ID = t1.VARIATION_ID ) = 1 FOR XML PATH ( '' ) 
							),
							1,
							0,
							'' 
						),
						'' 
					) [ProductName],
                    innerPrd.STOCK_QUANTITY StockQuantity,
					innerPrd.PRODUCT_GUID ProductGuid,
                    innerPrd.PRODUCT_ID ProductId,
					pmPrd.PROMO_ID PromotionId,
					pmPrd.IS_PROMO_PRODUCT IsPromoProduct,
					pmPrd.QUANTITY Quantity,
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
			                 PUC.PR_UNIT_ID = innerPrd.UNIT_CODE_ID
			                 AND PUT.LANGUAGE_ID = ( SELECT SL.LANGUAGE_ID FROM NEW_STORE_LANGUAGE SL WHERE SL.IS_ACTIVE = 1 AND SL.TENANT_ID = innerPrd.TENANT_ID AND SL.NUMBER = {lang} ) FOR json path,
			                 without_array_wrapper 
		                 ) 
	                 ) MeasureType,
					JSON_QUERY (
						(
						SELECT 
                            PF.[UPLOAD_FILE_IMAGE_ID] Id,
							UF.PATH + UF.FILENAME + UF.EXTENSION FilePath 
						FROM
							NEW_PRODUCT_FILE PF
							LEFT JOIN NEW_UPLOAD_FILE UF ON UF.UPLOAD_FILE_ID = PF.UPLOAD_FILE_IMAGE_ID 
						WHERE
							PF.PRODUCT_ID = innerPrd.PRODUCT_ID 
							AND PF.IS_ACTIVE = 1 AND PF.WEIGHT = 1 FOR json path,
							without_array_wrapper 
						) 
					) [Image] 
				FROM
					NEW_PRODUCT AS innerPrd
					LEFT JOIN [NEW_PROMOTION_PRODUCT] pmPrd ON pmPrd.PRODUCT_ID= innerPrd.PRODUCT_ID
				WHERE
					innerPrd.PRODUCT_ID IN ( SELECT PRODUCT_ID FROM NEW_PROMOTION_PRODUCT WHERE PROMO_ID = promo.PROMO_ID AND IS_ACTIVE = 1 ) 
					AND pmPrd.PROMO_ID=promo.PROMO_ID for json path 
				) [PromoProducts] 
			FROM
				NEW_PROMOTION  FOR json path,
				without_array_wrapper 
			) 
		) [Promotion]
	FROM
	NEW_PROMOTION promo
	WHERE
	PROMO_ID=@pPromotionId
	and IS_ACTIVE=1
    FOR json path 
	) Json";

            List<NewBasketResponse> baskets = new List<NewBasketResponse>();


            try
            {
                string json = String.Empty;
                using (var conn = new DbHandler())
                {
                    var dr = conn.ExecuteSql(sql, new[]{
                        DbHandler.SetParameter("@pPromotionId",SqlDbType.VarChar,50,ParameterDirection.Input,promotionId),
                        DbHandler.SetParameter("@pQuantity",SqlDbType.Decimal,50,ParameterDirection.Input,quantity)
                    });

                    while (dr.Read())
                    {
                        json = dr["Json"].ToString();
                        baskets = JsonConvert.DeserializeObject<List<NewBasketResponse>>(json);
                        baskets = baskets ?? new List<NewBasketResponse>();

                    }
                }
                baskets.ForEach(action => action.LastPrice = (action.Price - action.Discount) * action.Quantity);
				baskets.ForEach(x =>
                {
					x.Slug = x.Promotion?.Description.UrlFriendly(langString) + "-" + x.Promotion?.PromotionId;
				});
            }
            catch (Exception ex)
            {
                Log.Warn("Could not GetOne Product");
                Log.Error(ex);
                throw;
            }
            return baskets;
        }

        //Get Promotions in basket without login
        public IEnumerable<NewBasketResponse> GetBasketPromotions(int lang, string langString,  List<PromotionBasketInsDto> promotions)
        {


            string sql =
				$@"SELECT
	(
	SELECT

		JSON_QUERY (
			(
			SELECT
				S.TENANT_ID TenantId,
                S.[STATUS] Status,
				S.STORE_GUID StoreGuid,
				S.NAME Name,
                S.DOMAIN [Domain],
                S.IS_SALES IsSales
			FROM
				NEW_STORE S 
			WHERE
				S.TENANT_ID =(SELECT TOP 1 TENANT_ID FROM NEW_PROMOTION WHERE PROMO_ID = Promo.PROMO_ID)
				 FOR json path,
				without_array_wrapper 
			) 
		) Store,
	  ( SELECT Price from GetPromoPrice(Promo.PROMO_ID) ) as Price,

    ( SELECT Discount from GetPromoPrice(Promo.PROMO_ID) ) as Discount,
		@pQuantity as Quantity,
		Json_Query (
			(
			SELECT
				Promo.PROMO_ID PromotionId,
				Promo.[DESCRIPTION{(lang == 1 ? "":lang.ToString())}] [Description],
				JSON_QUERY (
					(
					SELECT 
						 PROMO_IMG_ID Id,
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
					innerPrd.NAME{(lang == 1 ? "" : lang.ToString())} + ISNULL(
						STUFF(
							(
							SELECT
								' ' + [VALUE{(lang == 1 ? "" : lang.ToString())}] 
							FROM
								NEW_PRODUCT_VARIATION t1 
							WHERE
								t1.PRODUCT_ID = innerPrd.PRODUCT_ID
								AND ( SELECT [SHOW_IN_NAME] FROM NEW_VARIATION WHERE VARIATION_ID = t1.VARIATION_ID ) = 1 FOR XML PATH ( '' ) 
							),
							1,
							0,
							'' 
						),
						'' 
					) [ProductName],
                    innerPrd.STOCK_QUANTITY StockQuantity,
					innerPrd.PRODUCT_GUID ProductGuid,
                    innerPrd.PRODUCT_ID  ProductId,
					pmPrd.PROMO_ID PromotionId,
					pmPrd.IS_PROMO_PRODUCT IsPromoProduct,
					pmPrd.QUANTITY Quantity,
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
			                 PUC.PR_UNIT_ID = innerPrd.UNIT_CODE_ID
			                 AND PUT.LANGUAGE_ID = ( SELECT SL.LANGUAGE_ID FROM NEW_STORE_LANGUAGE SL WHERE SL.IS_ACTIVE = 1 AND SL.TENANT_ID = innerPrd.TENANT_ID AND SL.NUMBER = {lang} ) FOR json path,
			                 without_array_wrapper 
		                 ) 
	                 ) MeasureType,
					JSON_QUERY (
						(
						SELECT 
							 PF.[UPLOAD_FILE_IMAGE_ID] Id,
							UF.PATH + UF.FILENAME + UF.EXTENSION FilePath 
						FROM
							NEW_PRODUCT_FILE PF
							LEFT JOIN NEW_UPLOAD_FILE UF ON UF.UPLOAD_FILE_ID = PF.UPLOAD_FILE_IMAGE_ID 
						WHERE
							PF.PRODUCT_ID = innerPrd.PRODUCT_ID 
							AND PF.IS_ACTIVE = 1 and PF.WEIGHT = 1 FOR json path,
							without_array_wrapper 
						) 
					) [Image] 
				FROM
					NEW_PRODUCT AS innerPrd
					LEFT JOIN [NEW_PROMOTION_PRODUCT] pmPrd ON pmPrd.PRODUCT_ID= innerPrd.PRODUCT_ID
				WHERE
					innerPrd.PRODUCT_ID IN ( SELECT PRODUCT_ID FROM NEW_PROMOTION_PRODUCT WHERE PROMO_ID = promo.PROMO_ID AND IS_ACTIVE = 1 ) 
					AND pmPrd.PROMO_ID=promo.PROMO_ID for json path 
				) [PromoProducts] 
			FROM
				NEW_PROMOTION  FOR json path,
				without_array_wrapper 
			) 
		) [Promotion]
	FROM
	NEW_PROMOTION promo
	WHERE
	PROMO_ID=@pPromotionId
	and IS_ACTIVE=1
    FOR json path,without_array_wrapper
	) Json";

            List<NewBasketResponse> baskets = new List<NewBasketResponse>();
            NewBasketResponse basket = new NewBasketResponse();
            foreach (var promotion in promotions)
            {

                if (promotion.Quantity <= 0)
                    throw new Exception("Məhsul qiyməti 0-dan kiçik və ya bərabər ola bilməz");
                try
                {
                    string json = String.Empty;
                    using (var conn = new DbHandler())
                    {
                        var dr = conn.ExecuteSql(sql, new[]{
                        DbHandler.SetParameter("@pPromotionId",SqlDbType.VarChar,50,ParameterDirection.Input,promotion.PromotionId),
                        DbHandler.SetParameter("@pQuantity",SqlDbType.Decimal,50,ParameterDirection.Input,promotion.Quantity)
                    });

                        while (dr.Read())
                        {
                            json = dr["Json"].ToString();
                            basket = JsonConvert.DeserializeObject<NewBasketResponse>(json);
                        }
                        dr.Close();
                        baskets.Add(basket);
                    }

                }
                catch (Exception ex)
                {
                    Log.Warn("Could not GetOne Product");
                    Log.Error(ex);
                    throw;
                }
            }

            baskets.ForEach(action => action.LastPrice = (action.Price - action.Discount) * action.Quantity);
            baskets.ForEach(x =>
            {
				x.Slug = x.Promotion?.Description.UrlFriendly(langString) + "-" + x.Promotion?.PromotionId;
			});

			return baskets;
        }

        //Get Promotions in basket without login
        public IEnumerable<NewBasketResponse> GetBasketProducts(int lang, string langString, List<NewBasket> products)
        {


            string sql =
				$@"SELECT
	(
	SELECT
		prd.PRICE as Price,
		(case
		when prd.DISCOUNTED_PRICE>0 then (prd.PRICE-prd.DISCOUNTED_PRICE)
		else 0
		end) as Discount,
		@pQuantity as Quantity,
		prd.PRODUCT_GUID AS Id,
        prd.PRODUCT_ID AS ProductId,
		prd.NAME{(lang == 1?"":lang.ToString())} + ISNULL(
			STUFF(
				(
				SELECT
					' ' + [VALUE{(lang == 1 ? "" : lang.ToString())}] 
				FROM
					NEW_PRODUCT_VARIATION t1 
				WHERE
					t1.PRODUCT_ID = prd.PRODUCT_ID 
					AND ( SELECT [SHOW_IN_NAME] FROM NEW_VARIATION WHERE VARIATION_ID = t1.VARIATION_ID ) = 1 FOR XML PATH ( '' ) 
				),
				1,
				0,
				'' 
			),
			'' 
		) ProductName,
				JSON_QUERY (
			(
			SELECT
				S.TENANT_ID TenantId,
                S.[STATUS] Status,
				S.STORE_GUID StoreGuid,
				S.NAME Name,
                S.DOMAIN [Domain],
                S.IS_SALES IsSales
			FROM
				NEW_STORE S 
			WHERE
				S.TENANT_ID =
			CASE
					
					WHEN DATALENGTH( ( SELECT rtrim( ltrim( prd.PRODUCT_GUID ) ) ) ) > 0 THEN
					prd.TENANT_ID
				END FOR json path,
				without_array_wrapper 
			) 
		) Store,
        prd.STOCK_QUANTITY StockQuantity,
		(
		SELECT 
			PF.[UPLOAD_FILE_IMAGE_ID] Id,
			UF.PATH + UF.FILENAME + UF.EXTENSION FilePath 
		FROM
			NEW_PRODUCT_FILE PF
			LEFT JOIN NEW_UPLOAD_FILE UF ON UF.UPLOAD_FILE_ID = PF.UPLOAD_FILE_IMAGE_ID 
		WHERE
			PF.PRODUCT_ID = prd.PRODUCT_ID 
			AND PF.IS_ACTIVE = 1 and PF.WEIGHT = 1 FOR json path 
		) [Images],
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
			         PUC.PR_UNIT_ID = prd.UNIT_CODE_ID
			         AND PUT.LANGUAGE_ID = ( SELECT SL.LANGUAGE_ID FROM NEW_STORE_LANGUAGE SL WHERE SL.IS_ACTIVE = 1 AND SL.TENANT_ID = prd.TENANT_ID AND SL.NUMBER = {lang} ) FOR json path,
			         without_array_wrapper 
		         ) 
	         ) MeasureType
	FROM
		[NEW_PRODUCT] AS prd
		LEFT JOIN [NEW_PR_UNIT_CODE] AS unt ON prd.UNIT_CODE_ID= unt.PR_UNIT_ID 
	WHERE
		prd.PRODUCT_GUID = @pProductGuid
		and prd.IS_ACTIVE=1
	 FOR json path,without_array_wrapper
	) Json";

            List<NewBasketResponse> baskets = new List<NewBasketResponse>();
            NewBasketResponse basket = new NewBasketResponse();
            foreach (var product in products)
            {

                if (product.Quantity <= 0)
                    throw new Exception("Məhsul qiyməti 0-dan kiçik və ya bərabər ola bilməz");
                try
                {
                    string json = String.Empty;
                    using (var conn = new DbHandler())
                    {
                        var dr = conn.ExecuteSql(sql, new[]{
                        DbHandler.SetParameter("@pProductGuid",SqlDbType.VarChar,50,ParameterDirection.Input,product.Id),
                        DbHandler.SetParameter("@pQuantity",SqlDbType.Decimal,50,ParameterDirection.Input,product.Quantity)
                    });


                        while (dr.Read())
                        {
                            json = dr["Json"].ToString();
                            basket = JsonConvert.DeserializeObject<NewBasketResponse>(json);
                        }
                        dr.Close();
                        baskets.Add(basket);
                    }

                }
                catch (Exception ex)
                {
                    Log.Warn("Could not GetOne Product");
                    Log.Error(ex);
                    throw;
                }
            }

            baskets?.ForEach(action => action.LastPrice = (action.Price - action.Discount) * action.Quantity);
            baskets?.ForEach( x => x.Slug = x.ProductName.UrlFriendly(langString) + "-" + x.ProductId);

            return baskets;
        }

        public IEnumerable<NewBasketResponse> Update(NewBasketUpdDto basket,int lang, string langString, int userId)
        {
            try
            {
                var json = JsonConvert.SerializeObject(basket);
                using (var conn = new DbHandler())
                {
                    conn.ExecuteStoredProcedure("BasketUpdate", new[]
                  {
                        DbHandler.SetParameter("@pRequestAsJson",SqlDbType.NVarChar,-1,ParameterDirection.Input,json),
                       DbHandler.SetParameter("@pLogUserId",SqlDbType.Int,10,ParameterDirection.Input,userId)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not BasketInsert...");
                Log.Error(ex);
                throw;
            }
            return GetAllBasketElements(lang, langString, userId);
        }

        public IEnumerable<NewBasketResponse> Delete(string basketGuid,int lang, string langString, int userId)
        {
            try
            {

                using (var conn = new DbHandler())
                {
                    conn.ExecuteStoredProcedure("BasketDelete", new[]
                  {
                        DbHandler.SetParameter("@pBasketGuid",SqlDbType.NVarChar,50,ParameterDirection.Input,basketGuid),
                         DbHandler.SetParameter("@pLogUserId",SqlDbType.NVarChar,50,ParameterDirection.Input,userId)

                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not BasketInsert...");
                Log.Error(ex);
                throw;
            }
            return GetAllBasketElements(lang, langString, userId);
        }

    }
}