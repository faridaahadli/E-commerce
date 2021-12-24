using CRMHalalBackEnd.DB;
using CRMHalalBackEnd.Models.Order;
using CRMHalalBackEnd.Models.Payment;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using CRMHalalBackEnd.Helpers;
using CRMHalalBackEnd.Models.Store;

namespace CRMHalalBackEnd.Repository
{
    public class RefundRepository
    {
        private static readonly log4net.ILog Log =
          log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


		private readonly string MERCHANT_HANDLER = System.Configuration.ConfigurationManager.AppSettings["MERCHANT_HANDLER"];
        // PKCS#12 keystore with the Merchant's signed certificate
        private readonly string X509_CERTIFICATE_FILE = System.Web.Hosting.HostingEnvironment.MapPath("~/certificate/certificate.p12");
        //private readonly string X509_CERTIFICATE_FILE = "C:\\Users\\Farida Ahadli\\Desktop\\Hala_CRM\\crmhalalbackend\\certificate\\certificate.p12";
        // Keystore password
        private const string CERTIFICATE_PASSWORD = "P@ssword";



		public List<OrderLineInfo> GetRefund(string tenantId, int userId, int orderId,int langId)
        {
            string sql = $@"SELECT
	(
	SELECT
		JSON_QUERY (
			(
			SELECT
				U.LAST_NAME + ' ' + U.FIRST_NAME UserName,
				( SELECT TOP(1)TEXT FROM NEW_CONTACT C WHERE C.USER_ID = U.USER_ID and C.CONTACT_TYPE_ID=2 ) UserPhone,
				(
				SELECT
							( SELECT
	                                    PMT.NAME 
	                                    FROM
		                                    NEW_PAYMENT_METHOD PM
		                                    INNER JOIN NEW_PAYMENT_MET_TRANSLATE PMT ON PMT.PAYMENT_METHOD_ID = PM.PAYMENT_METHOD_ID 
	                                    WHERE
		                                    PM.PAYMENT_METHOD_ID  = OPT.PAYMENT_TYPE_ID
	                                    AND PMT.LANGUAGE_ID ={langId} 
					)   
				FROM
					NEW_ORDER_PAYMENT_TYPE OPT 
				WHERE
					OPT.ORDER_ID = @orderId 
					AND OPT.TENANT_ID = @tenantId 
				) PaymentMethod,
				(
				SELECT DISTINCT
					A.ADDRESS as Address
				FROM
					NEW_ADDRESS A
					INNER JOIN NEW_COMMON_DELIVERY_DATA CDA ON CDA.DELIVERY_ADDRESS_ID= A.ADDRESS_ID 
				WHERE
				CDA.COMMON_DELIVERY_DATA_ID IN ( SELECT COMMON_DELIVERY_DATA_ID FROM NEW_ORDER_DELIVERY WHERE ORDER_ID =@orderId and TENANT_ID=@tenantId ) 
				)   Address,
				( SELECT DISTINCT NOTE FROM NEW_COMMON_DELIVERY_DATA CDA WHERE CDA.COMMON_DELIVERY_DATA_ID IN ( SELECT COMMON_DELIVERY_DATA_ID FROM NEW_ORDER_DELIVERY WHERE ORDER_ID =@orderId and TENANT_ID=@tenantId) ) Note,
				( SELECT DISTINCT SELECTED_TIME AS SelectedTime FROM NEW_ORDER_DELIVERY WHERE ORDER_ID =@orderId and TENANT_ID=@tenantId) SelectedTime,
				(select DELIVERY_PRICE from NEW_ORDER_DELIVERY WHERE ORDER_ID =@orderId and TENANT_ID=@tenantId)DeliveryPrice
			FROM
				NEW_USER U
				INNER JOIN NEW_SALES_ORDER SO ON U.USER_ID = SO.USER_ID 
			WHERE
				SO.SO_ID= @orderId FOR json path,
				without_array_wrapper 
			) 
		) UserData,
					
				    (
		         select JSON_QUERY(
		        (select RF.REFUND_ID RefundId,
		        (select dbo.GetRefundedPrice(RF.REFUND_ID,Rf.IS_COURIER_COST_INCLUDED))Amount,
		          RF.IS_COURIER_COST_INCLUDED IsCourierCostInclude,		
				  


		         (		   
		          select  RD.PRODUCT_QUANTITY as Quantity,
				  RD.REASON_OF_REFUND Reason,


                    (case 
			          when (SO_LINE.PRODUCT_ID is null) then SO_LINE.PROMOTION_ID
			           else SO_LINE.PRODUCT_ID					  
			           end
			           )ItemId,

					   (case 
			          when (SO_LINE.PRODUCT_ID is null) then 0
			           else 1			  
			           end
			           )IsProduct
			
		    
		             from NEW_REFUND_DATA RD 
		            inner join NEW_SALES_ORDER_LINE SO_LINE
		            on SO_LINE.REFUND_DATA_ID=RD.REFUND_DATA_ID
		            where REFUND_ID=RF.REFUND_ID
		            for json path		
		   
		            ) RefundedItems

		            from NEW_REFUND RF where REFUND_ID = 
				   (select top 1 REFUND_ID from NEW_REFUND_DATA where REFUND_DATA_ID in
				    (select  REFUND_DATA_ID from NEW_SALES_ORDER_LINE where SO_ID=@orderId and IS_ACTIVE=1)
				   )
		          and IS_ACTIVE=1 
		          FOR json path,without_array_wrapper
		          )
		          )
		          ) RefundData,
(select distinct SU.UPDATE_DATE UpdatedTime, U.FIRST_NAME+' '+U.LAST_NAME FullName, SS.NAME [Status] from NEW_SO_UPDATE SU
	inner join NEW_SO_UPDATE_LINE SUL on SU.SO_UPD_ID=SUL.SO_UPD_ID
	inner join NEW_SO_STATUS SS on SS.SO_STATUS_ID=SU.SO_STATUS_ID 
	inner join NEW_USER U on U.USER_ID=SU.UPDATED_BY
	inner join NEW_SALES_ORDER_LINE SOL on SOL.SO_LINE_ID=SUL.SO_LINE_ID where SOL.SO_ID=@orderId and  SOL.TENANT_ID=@tenantId for json path) StatusData,
		(
		SELECT
			(
			SELECT
				[DESCRIPTION{(langId == 1 ? "" : langId.ToString())}] Description,
				(
	select 
	CASE
	WHEN EFFECTIVE_AMOUNT is not null then EFFECTIVE_AMOUNT
	else  AMOUNT
	end
	 from NEW_SALES_ORDER_LINE SOL where SOL.SO_ID=@orderId and SOL.PROMOTION_ID=P.PROMO_ID) Amount,
	 P.PROMO_ID PromoId,
				( SELECT SOL.QUANTITY FROM NEW_SALES_ORDER_LINE SOL WHERE SOL.PROMOTION_ID= P.PROMO_ID  and SOL.SO_ID=@orderId ) Quantity,
				Json_query (
					(
					SELECT
					CASE
							
						WHEN
							P.PROMO_TYPE_ID= 3 THEN
								(
								SELECT
									PRODUCT_GUID ProductGuid,
                                    PRODUCT_ID ProductId,
									NAME{(langId == 1 ? "" : langId.ToString())} as Name,
									( SELECT SOL.QUANTITY FROM NEW_SALES_ORDER_LINE SOL WHERE SOL.PROMOTION_ID= P.PROMO_ID and SOL.SO_ID=@orderId ) Quantity,
									PRICE as Price
								FROM
									NEW_PRODUCT NP 
								WHERE
									NP.PRODUCT_ID IN (
									SELECT
										PRODUCT_ID 
									FROM
										NEW_PROMOTION_PRODUCT 
									WHERE
										PROMO_ID IN (
										SELECT
											NSOL.PROMOTION_ID 
										FROM
											NEW_SALES_ORDER_LINE NSOL 
										WHERE
											NSOL.SO_ID= @orderId 
											AND NSOL.PROMOTION_ID IN ( SELECT PROMO_ID FROM NEW_PROMOTION WHERE PROMO_TYPE_ID = 3 ) 
										) 
									) FOR json path 
								) 
								WHEN P.PROMO_TYPE_ID= 2 THEN
								(
								SELECT
									PRODUCT_GUID ProductGuid,
                                    PRODUCT_ID ProductId,
									NAME{(langId == 1 ? "" : langId.ToString())} as Name,
									( SELECT SOL.QUANTITY FROM NEW_SALES_ORDER_LINE SOL WHERE SOL.PROMOTION_ID= P.PROMO_ID and SOL.SO_ID=@orderId ) Quantity,
									PRICE as Price
								FROM
									NEW_PRODUCT NP 
								WHERE
									NP.PRODUCT_ID IN (
									SELECT
										PRODUCT_ID 
									FROM
										NEW_PROMOTION_PRODUCT 
									WHERE
										PROMO_ID IN (
										SELECT
											NSOL.PROMOTION_ID 
										FROM
											NEW_SALES_ORDER_LINE NSOL 
										WHERE
											NSOL.SO_ID= @orderId 
											AND NSOL.PROMOTION_ID IN ( SELECT PROMO_ID FROM NEW_PROMOTION WHERE PROMO_TYPE_ID = 2 ) 
										) 
									) FOR json path 
								) 
								WHEN P.PROMO_TYPE_ID= 1 THEN
								(
								SELECT
									PRODUCT_GUID ProductGuid,
                                    PRODUCT_ID ProductId,
									NAME{(langId == 1 ? "" : langId.ToString())},
									( SELECT SOL.QUANTITY FROM NEW_SALES_ORDER_LINE SOL WHERE SOL.PROMOTION_ID= P.PROMO_ID and SOL.SO_ID=@orderId ) Quantity,
									PRICE 
							FROM
									NEW_PRODUCT NP 
								WHERE
									NP.PRODUCT_ID IN (
									SELECT
										PRODUCT_ID 
									FROM
										NEW_PROMOTION_PRODUCT 
									WHERE
										PROMO_ID IN (
										SELECT
											NSOL.PROMOTION_ID 
									FROM
										NEW_SALES_ORDER_LINE NSOL 
										WHERE
											NSOL.SO_ID= @orderId 
											AND NSOL.PROMOTION_ID IN ( SELECT PROMO_ID FROM NEW_PROMOTION WHERE PROMO_TYPE_ID = 1 ) 
										) 
									) FOR json path 
								) 
							END AS Promotion 
						) 
					) PromoProduct 
				FROM
					NEW_PROMOTION P 
				WHERE
					P.PROMO_ID IN ( SELECT SOL.PROMOTION_ID FROM NEW_SALES_ORDER_LINE SOL WHERE SOL.SO_ID= @orderId and P.TENANT_ID=@tenantId ) FOR json path 
			) Json 
			) Promotions,
			(
			SELECT
				(
				SELECT
					P.NAME{(langId == 1 ? "" : langId.ToString())} + ' ' + ISNULL(
						STUFF(
							(
							SELECT
								' ' + [VALUE{(langId == 1 ? "" : langId.ToString())}] 
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
					) AS NAME,
					PRODUCT_GUID ProductGuid,
                    P.PRODUCT_ID ProductId,
					BARCODE AS Barcode,
					SOL.SO_LINE_ID LineId,
					SOL.QUANTITY AS Quantity,
					SOL.PRICE AS Price,
					( CASE WHEN SOL.EFFECTIVE_AMOUNT IS NOT NULL THEN SOL.EFFECTIVE_AMOUNT ELSE SOL.AMOUNT END ) AS TotalPrice,
					( SELECT CURRENCY FROM NEW_SALES_ORDER NSO WHERE SOL.SO_ID= NSO.SO_ID ) Currency 
				FROM
					NEW_PRODUCT P
					INNER JOIN NEW_SALES_ORDER_LINE SOL ON SOL.PRODUCT_ID= P.PRODUCT_ID
					INNER JOIN NEW_SALES_ORDER SO ON SO.SO_ID= SOL.SO_ID 
				WHERE
				SO.SO_ID=@orderId 
				and P.TENANT_ID=@tenantId FOR json path 
				) Products 		) Products FOR json path 
	) Json";

			List<OrderLineInfo> order;

			try
			{
				string json = String.Empty;
				using (var con = new DbHandler())
				{
					var reader = con.ExecuteSql(sql, new[]
				   {
						DbHandler.SetParameter("@tenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
						DbHandler.SetParameter("@userId",SqlDbType.Int,10,ParameterDirection.Input,userId),
						DbHandler.SetParameter("@orderId",SqlDbType.Int,10,ParameterDirection.Input,orderId)


					});
					if (reader.Read())
					{
						json = reader["Json"].ToString();
					}
				}

				order = JsonConvert.DeserializeObject<List<OrderLineInfo>>(json);
				order = order ?? new List<OrderLineInfo>();
				order.ForEach(x =>
				{
					x.Products?.ForEach(y => y.Slug = y.Name.UrlFriendly("en") + "-" + y.ProductId);
					x.Promotions?.ForEach(y =>
					{
						y.Slug = y.Description.UrlFriendly("en") + "-" + y.PromoId;
					});
				});
			}
			catch (Exception ex)
			{
				Log.Error(ex);
				throw;
			}

			return order;
		}



		public OrderLineInfo GetRefundForAdmin(string tenantId, int userId, int orderId,int langId)
		{
			string sql = $@"SELECT
	(
	SELECT
		JSON_QUERY (
			(
			SELECT
				U.LAST_NAME + ' ' + U.FIRST_NAME UserName,
				( SELECT TOP(1)TEXT FROM NEW_CONTACT C WHERE C.USER_ID = U.USER_ID and C.CONTACT_TYPE_ID=2 ) UserPhone,
				(
				SELECT
					( SELECT
	                                    PMT.NAME 
	                                    FROM
		                                    NEW_PAYMENT_METHOD PM
		                                    INNER JOIN NEW_PAYMENT_MET_TRANSLATE PMT ON PMT.PAYMENT_METHOD_ID = PM.PAYMENT_METHOD_ID 
	                                    WHERE
		                                    PM.PAYMENT_METHOD_ID  = OPT.PAYMENT_TYPE_ID
	                                    AND PMT.LANGUAGE_ID ={langId} 
					)  
				FROM
					NEW_ORDER_PAYMENT_TYPE OPT 
				WHERE
					OPT.ORDER_ID = @orderId 
					AND OPT.TENANT_ID = @tenantId 
				) PaymentMethod,
				(
				SELECT DISTINCT
					A.ADDRESS as Address
				FROM
					NEW_ADDRESS A
					INNER JOIN NEW_COMMON_DELIVERY_DATA CDA ON CDA.DELIVERY_ADDRESS_ID= A.ADDRESS_ID 
				WHERE
				CDA.COMMON_DELIVERY_DATA_ID IN ( SELECT COMMON_DELIVERY_DATA_ID FROM NEW_ORDER_DELIVERY WHERE ORDER_ID =@orderId and TENANT_ID=@tenantId ) 
				)   Address,
				( SELECT DISTINCT NOTE FROM NEW_COMMON_DELIVERY_DATA CDA WHERE CDA.COMMON_DELIVERY_DATA_ID IN ( SELECT COMMON_DELIVERY_DATA_ID FROM NEW_ORDER_DELIVERY WHERE ORDER_ID =@orderId and TENANT_ID=@tenantId) ) Note,
				( SELECT DISTINCT SELECTED_TIME AS SelectedTime FROM NEW_ORDER_DELIVERY WHERE ORDER_ID =@orderId and TENANT_ID=@tenantId) SelectedTime,
				(select DELIVERY_PRICE from NEW_ORDER_DELIVERY WHERE ORDER_ID =@orderId and TENANT_ID=@tenantId)DeliveryPrice
			FROM
				NEW_USER U
				INNER JOIN NEW_SALES_ORDER SO ON U.USER_ID = SO.USER_ID 
			WHERE
				SO.SO_ID= @orderId FOR json path,
				without_array_wrapper 
			) 
		) UserData,
					
				    (
		         select JSON_QUERY(
		        (select RF.REFUND_ID RefundId,
		        (select dbo.GetRefundedPrice(RF.REFUND_ID,Rf.IS_COURIER_COST_INCLUDED))Amount,
		          RF.IS_COURIER_COST_INCLUDED IsCourierCostInclude,		
				   RF.IS_CONFIRMED IsConfirm,
                   RF.NOTE Reason,

		         (		   
		          select  RD.PRODUCT_QUANTITY as Quantity,
				  RD.REASON_OF_REFUND Reason,
                  RD.REASON_OF_REJECT RejectReason,
				  RD.AMOUNT Amount,
                  RD.IS_CONFIRMED IsConfirm,

                    (case 
			          when (SO_LINE.PRODUCT_ID is null) then SO_LINE.PROMOTION_ID
			           else SO_LINE.PRODUCT_ID					  
			           end
			           )ItemId,

					   (case 
			          when (SO_LINE.PRODUCT_ID is null) then 0
			           else 1			  
			           end
			           )IsProduct
			
		    
		             from NEW_REFUND_DATA RD 
		            inner join NEW_SALES_ORDER_LINE SO_LINE
		            on SO_LINE.REFUND_DATA_ID=RD.REFUND_DATA_ID
		            where REFUND_ID=RF.REFUND_ID
		            for json path		
		   
		            ) RefundedItems

		            from NEW_REFUND RF where REFUND_ID = 
				   (select top 1 REFUND_ID from NEW_REFUND_DATA where REFUND_DATA_ID in
				    (select  REFUND_DATA_ID from NEW_SALES_ORDER_LINE where SO_ID=@orderId and IS_ACTIVE=1)
				   )
		          and IS_ACTIVE=1 
		          FOR json path,without_array_wrapper
		          )
		          )
		          ) RefundData,
		(
		SELECT
			(
			SELECT
				[DESCRIPTION{(langId == 1 ? "" : langId.ToString())}] Description,
				(
	select 
	CASE
	WHEN EFFECTIVE_AMOUNT is not null then EFFECTIVE_AMOUNT
	else  AMOUNT
	end
	 from NEW_SALES_ORDER_LINE SOL where SOL.SO_ID=@orderId and SOL.PROMOTION_ID=P.PROMO_ID) Amount,
	 P.PROMO_ID PromoId,
				( SELECT SOL.QUANTITY FROM NEW_SALES_ORDER_LINE SOL WHERE SOL.PROMOTION_ID= P.PROMO_ID  and SOL.SO_ID=@orderId ) Quantity,
				Json_query (
					(
					SELECT
					CASE
							
						WHEN
							P.PROMO_TYPE_ID= 3 THEN
								(
								SELECT
									PRODUCT_GUID ProductGuid,
                                    PRODUCT_ID ProductId,
									NAME{(langId == 1 ? "" : langId.ToString())} as Name,
									( SELECT SOL.QUANTITY FROM NEW_SALES_ORDER_LINE SOL WHERE SOL.PROMOTION_ID= P.PROMO_ID and SOL.SO_ID=@orderId ) Quantity,
									PRICE as Price
								FROM
									NEW_PRODUCT NP 
								WHERE
									NP.PRODUCT_ID IN (
									SELECT
										PRODUCT_ID 
									FROM
										NEW_PROMOTION_PRODUCT 
									WHERE
										PROMO_ID IN (
										SELECT
											NSOL.PROMOTION_ID 
										FROM
											NEW_SALES_ORDER_LINE NSOL 
										WHERE
											NSOL.SO_ID= @orderId 
											AND NSOL.PROMOTION_ID IN ( SELECT PROMO_ID FROM NEW_PROMOTION WHERE PROMO_TYPE_ID = 3 ) 
										) 
									) FOR json path 
								) 
								WHEN P.PROMO_TYPE_ID= 2 THEN
								(
								SELECT
									PRODUCT_GUID ProductGuid,
                                    PRODUCT_ID ProductId,
									NAME{(langId == 1 ? "" : langId.ToString())} as Name,
									( SELECT SOL.QUANTITY FROM NEW_SALES_ORDER_LINE SOL WHERE SOL.PROMOTION_ID= P.PROMO_ID and SOL.SO_ID=@orderId ) Quantity,
									PRICE as Price
								FROM
									NEW_PRODUCT NP 
								WHERE
									NP.PRODUCT_ID IN (
									SELECT
										PRODUCT_ID 
									FROM
										NEW_PROMOTION_PRODUCT 
									WHERE
										PROMO_ID IN (
										SELECT
											NSOL.PROMOTION_ID 
										FROM
											NEW_SALES_ORDER_LINE NSOL 
										WHERE
											NSOL.SO_ID= @orderId 
											AND NSOL.PROMOTION_ID IN ( SELECT PROMO_ID FROM NEW_PROMOTION WHERE PROMO_TYPE_ID = 2 ) 
										) 
									) FOR json path 
								) 
								WHEN P.PROMO_TYPE_ID= 1 THEN
								(
								SELECT
									PRODUCT_GUID ProductGuid,
                                    PRODUCT_ID ProductId,
									NAME{(langId == 1 ? "" : langId.ToString())} as Name,
									( SELECT SOL.QUANTITY FROM NEW_SALES_ORDER_LINE SOL WHERE SOL.PROMOTION_ID= P.PROMO_ID and SOL.SO_ID=@orderId ) Quantity,
									PRICE 
							FROM
									NEW_PRODUCT NP 
								WHERE
									NP.PRODUCT_ID IN (
									SELECT
										PRODUCT_ID 
									FROM
										NEW_PROMOTION_PRODUCT 
									WHERE
										PROMO_ID IN (
										SELECT
											NSOL.PROMOTION_ID 
									FROM
										NEW_SALES_ORDER_LINE NSOL 
										WHERE
											NSOL.SO_ID= @orderId 
											AND NSOL.PROMOTION_ID IN ( SELECT PROMO_ID FROM NEW_PROMOTION WHERE PROMO_TYPE_ID = 1 ) 
										) 
									) FOR json path 
								) 
							END AS Promotion 
						) 
					) PromoProduct 
				FROM
					NEW_PROMOTION P 
				WHERE
					P.PROMO_ID IN ( SELECT SOL.PROMOTION_ID FROM NEW_SALES_ORDER_LINE SOL WHERE SOL.SO_ID= @orderId and P.TENANT_ID=@tenantId ) FOR json path 
			) Json 
			) Promotions,
			(
			SELECT
				(
				SELECT
					P.NAME{(langId == 1 ? "" : langId.ToString())} + ' ' + ISNULL(
						STUFF(
							(
							SELECT
								' ' +  [VALUE{(langId == 1 ? "" : langId.ToString())}] 
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
					) AS NAME,
					PRODUCT_GUID ProductGuid,
                    P.PRODUCT_ID ProductId,
					BARCODE AS Barcode,
					SOL.SO_LINE_ID LineId,
					SOL.QUANTITY AS Quantity,
					SOL.PRICE AS Price,
					( CASE WHEN SOL.EFFECTIVE_AMOUNT IS NOT NULL THEN SOL.EFFECTIVE_AMOUNT ELSE SOL.AMOUNT END ) AS TotalPrice,
					( SELECT CURRENCY FROM NEW_SALES_ORDER NSO WHERE SOL.SO_ID= NSO.SO_ID ) Currency 
				FROM
					NEW_PRODUCT P
					INNER JOIN NEW_SALES_ORDER_LINE SOL ON SOL.PRODUCT_ID= P.PRODUCT_ID 
					INNER JOIN NEW_SALES_ORDER SO ON SO.SO_ID= SOL.SO_ID 
				WHERE
				SO.SO_ID=@orderId and SOL.REFUND_DATA_ID is not null
				and P.TENANT_ID=@tenantId FOR json path 
				) Products 		) Products FOR json path, without_array_wrapper 
	) Json";

			OrderLineInfo order=null;

			try
			{
				string json = String.Empty;
				using (var con = new DbHandler())
				{
					var reader = con.ExecuteSql(sql, new[]
				   {
						DbHandler.SetParameter("@tenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
						DbHandler.SetParameter("@userId",SqlDbType.Int,10,ParameterDirection.Input,userId),
						DbHandler.SetParameter("@orderId",SqlDbType.Int,10,ParameterDirection.Input,orderId)


					});
					if (reader.Read())
					{
						json = reader["Json"].ToString();
					}
				}

				order = JsonConvert.DeserializeObject<OrderLineInfo>(json);
				order = order ?? new OrderLineInfo();

				order.Products?.ForEach(y => y.Slug = y.Name.UrlFriendly("en") + "-" + y.ProductId);
				order.Promotions?.ForEach(y =>
				{
					y.Slug = y.Description.UrlFriendly("en") + "-" + y.PromoId;
				});


			}
			catch (Exception ex)
			{
				Log.Error(ex);
				throw;
			}

			return order;
		}


		public List<RefundList> GetRefundListForAdmin(string tenantId, int currentPage, int perPage)
		{
			string sql = @"SELECT
	(
	SELECT distinct
		USR.FIRST_NAME Username,USR.LAST_NAME UserSurname,
		SO.SO_ID OrderId,
		 RF.REFUND_ID RefundId,
		RF.CREATE_DATE CreateDate,
		(select dbo.GetRefundedPrice(RF.REFUND_ID,Rf.IS_COURIER_COST_INCLUDED))Amount
		 from NEW_SALES_ORDER_LINE SOL		
		 inner join NEW_SALES_ORDER SO on SO.SO_ID=SOL.SO_ID
		 inner join NEW_USER USR on USR.[USER_ID]=SO.[USER_ID]
		 inner join NEW_REFUND RF on  RF.REFUND_ID=(select REFUND_ID from NEW_REFUND_DATA where REFUND_DATA_ID=SOL.REFUND_DATA_ID)
		where SOL.TENANT_ID=@tenantId and SOL.REFUND_DATA_ID is not null and SOL.IS_ACTIVE=1
		 ORDER BY RF.CREATE_DATE desc
         OFFSET (@pCurrentPage-1)*@pPerPage ROWS FETCH NEXT @pPerPage ROWS ONLY 
			 FOR json path
	) Json";

			List<RefundList> order;

			try
			{
				string json = String.Empty;
				using (var con = new DbHandler())
				{
					var reader = con.ExecuteSql(sql, new[]
				   {
						DbHandler.SetParameter("@tenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
						DbHandler.SetParameter("@pCurrentPage",SqlDbType.Int,50,ParameterDirection.Input,currentPage),
						DbHandler.SetParameter("@pPerPage",SqlDbType.Int,50,ParameterDirection.Input,perPage)


					});
					if (reader.Read())
					{
						json = reader["Json"].ToString();
					}
				}

				order = JsonConvert.DeserializeObject<List<RefundList>>(json);
				order = order ?? new List<RefundList>();
				
			}
			catch (Exception ex)
			{
				Log.Error(ex);
				throw;
			}

			return order;
		}


		public int GetRefundSize(string tenantId,int perPage)
		{
			string sql = @"select CEILING(cast(
                    (select COUNT(*) cnt from NEW_REFUND where REFUND_ID in(select  REFUND_ID from NEW_REFUND_DATA where REFUND_DATA_ID in
				    (select  REFUND_DATA_ID from NEW_SALES_ORDER_LINE where TENANT_ID=@tenantId and IS_ACTIVE=1) 
				   ))as float)/cast(@perPage as float))totalpage
";

			int size = 0;

			try
			{
				string json = String.Empty;
				using (var con = new DbHandler())
				{
					var reader = con.ExecuteSql(sql, new[]
				   {
						DbHandler.SetParameter("@tenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
						DbHandler.SetParameter("@perPage", SqlDbType.VarChar, 5, ParameterDirection.Input, perPage),


					});
					if (reader.Read())
					{
						json = reader["totalpage"].ToString();
					}

                    if(json!=null)	
                        size = Convert.ToInt32(json);
				}

				

			}
			catch (Exception ex)
			{
				Log.Error(ex);
				throw;
			}

			return size;
		}


		public int InsertRefund(int userId,Refund refund)
        {

            string json = JsonConvert.SerializeObject(refund);
            int refundId = 0;
            try
            {
                using (var conn = new DbHandler())
                {
                    refundId=conn.ExecStoredProcWithReturnIntValue("RefundRequest", new[]
                   {
                        DbHandler.SetParameter("@pRequestAsJson",SqlDbType.NVarChar,-1,ParameterDirection.Input,json),
                        DbHandler.SetParameter("@pLogUserId",SqlDbType.Int,10,ParameterDirection.Input,userId)
                    });
                   
                }

            }
            catch (Exception ex)
            {
                Log.Warn("Could not InsertRefund...");
                Log.Error(ex);
                throw;
            }
            return refundId;
  
        }



		public int InsertRefundByAdmin(int userId,string tenantId, Refund refund)
		{
			decimal price = 0;
			string transId = null;
			string jsonInsert = JsonConvert.SerializeObject(refund);
			string jsonConfirm = null;
			int refundId = 0;
			try
			{

				refund.RefundByAdmin = true;

				using (var conn = new DbHandler())
				{
					refundId = conn.ExecStoredProcWithReturnIntValue("RefundRequest", new[]
				   {
						DbHandler.SetParameter("@pRequestAsJson",SqlDbType.NVarChar,-1,ParameterDirection.Input,jsonInsert),
						DbHandler.SetParameter("@pLogUserId",SqlDbType.Int,10,ParameterDirection.Input,userId)
					});

				}

                price = Convert.ToDecimal(GetRefundPrice(refundId, (bool)refund.IsCourierCostInclude));

                if (refund.PaymentTypeId == 3 && CheckPaymentType(refund.PaymentTypeId, refundId))
                {
                    transId = GetTransactionId(refundId);
                    RefundPay(transId, price,tenantId);
                }

				RefundConfirm refundConfirm = new RefundConfirm()
				{
					RefundId = refundId,
					PaymentTypeId = refund.PaymentTypeId,
					TotalAmount=refund.TotalAmount+refund.DeliveryPrice,
					IsAccepted = true,
					IsCourierCostInclude = (bool)refund.IsCourierCostInclude,
					ConfirmDate = refund.ConfirmDate
				};

			     jsonConfirm = JsonConvert.SerializeObject(refundConfirm);

				using (var conn = new DbHandler())
				{
					refundId = conn.ExecStoredProcWithReturnIntValue("RefundConfirm", new[]
				   {
						DbHandler.SetParameter("@pRequestAsJson",SqlDbType.NVarChar,-1,ParameterDirection.Input,jsonConfirm),
						DbHandler.SetParameter("@pLogUserId",SqlDbType.Int,10,ParameterDirection.Input,userId),
						DbHandler.SetParameter("@pTenantId",SqlDbType.NVarChar,5,ParameterDirection.Input,tenantId),
					});

				}

				//using (var conn = new DbHandler())
				//{
				//	refundId = conn.ExecStoredProcWithReturnIntValue("[dbo].[RefundRequestByAdmin]", new[]
				//   {
				//		DbHandler.SetParameter("@pRequestAsJson",SqlDbType.NVarChar,-1,ParameterDirection.Input,json),
				//		DbHandler.SetParameter("@pLogUserId",SqlDbType.Int,10,ParameterDirection.Input,userId)
				//	});

				//}

			}
			catch (Exception ex)
			{
				Log.Warn("Could not InsertRefund...");
				Log.Error(ex);
				throw;
			}
			return refundId;
			
		}

		public int ConfirmRefund(string tenantId,int userId, RefundConfirm refund)
		{
			string transId = null;
			decimal price = 0;
			string json = JsonConvert.SerializeObject(refund);
			int refundId = 0;
			try
			{
				if (refund.IsAccepted)
				{

					
				    price = Convert.ToDecimal(GetRefundPrice(refund.RefundId,refund.IsCourierCostInclude));

                    if (refund.PaymentTypeId == 3 && CheckPaymentType(refund.PaymentTypeId, refund.RefundId))
                    {
						transId = GetTransactionId(refund.RefundId);
						RefundPay(transId, price,tenantId);
					}
					   

				}


				using (var conn = new DbHandler())
				{
					refundId = conn.ExecStoredProcWithReturnIntValue("RefundConfirm", new[]
				   {
						DbHandler.SetParameter("@pRequestAsJson",SqlDbType.NVarChar,-1,ParameterDirection.Input,json),
						DbHandler.SetParameter("@pLogUserId",SqlDbType.Int,10,ParameterDirection.Input,userId),
						DbHandler.SetParameter("@pTenantId",SqlDbType.NVarChar,5,ParameterDirection.Input,tenantId),
					});

				}
			}
			catch (Exception ex)
			{
				Log.Warn("Could not RefundConfirm...");
				Log.Error(ex);
				throw;
			}
			return refundId;

		}

		public string GetTransactionId(int refundId)
		{
			string transId = String.Empty;
			try
			{
				using (var conn = new DbHandler())
				{
					transId=conn.ExecStoredProcWithOutputValue("GetTransIdByRefundId","@pResult", SqlDbType.NVarChar,50, new[]
				    {						
						DbHandler.SetParameter("@pRefundId",SqlDbType.Int,10,ParameterDirection.Input,refundId),						
					});

				}
			}
			catch (Exception ex)
			{
				Log.Warn("Could not GetTransIdByRefundId...");
				Log.Error(ex);
				throw;
			}
			return transId;

		}


		public decimal GetRefundPrice(int refundId,bool isCourierPriceInclude)
		{
			string sql= @"select dbo.GetRefundedPrice(@pRefundId,@pIsCourierPriceIncluded) as Amount";
			decimal price = 0;
			try
			{
				using (var conn = new DbHandler())
				{
			      var dr=conn.ExecuteSql(sql,new[]
				    {
						DbHandler.SetParameter("@pRefundId",SqlDbType.Int,10,ParameterDirection.Input,refundId),
						DbHandler.SetParameter("@pIsCourierPriceIncluded",SqlDbType.Bit,10,ParameterDirection.Input,isCourierPriceInclude)

					});
					if (dr.Read())
						price = Convert.ToDecimal(dr["Amount"]);

				}
			}
			catch (Exception ex)
			{
				Log.Warn("Could not GetTransIdByRefundId...");
				Log.Error(ex);
				throw;
			}
			return price;

		}


		public StoreCertificateData GetCertificateData(string tenantId)
		{
			string sql = @"select(
                           (select TENANT_ID TenantId,
                            CERTIFICATE_PASSWORD CertificatePassword,
	                        CERTIFICATE_PATH CertificatePath
                            from NEW_STORE_BANK_CERTIFICATION
                            where TENANT_ID=@pTenantId
                            for json path,without_array_wrapper)
                            )as json";
			string json = null;
			StoreCertificateData certificateData = null;
			try
			{
				using (var conn = new DbHandler())
				{
					var dr = conn.ExecuteSql(sql, new[]
					  {
						DbHandler.SetParameter("@pTenantId",SqlDbType.NVarChar,5,ParameterDirection.Input,tenantId)

					});

					if (dr.Read())
					{
						json = dr["json"].ToString();
					}
					certificateData = JsonConvert.DeserializeObject<StoreCertificateData>(json);
				}
			}
			catch (Exception ex)
			{
				Log.Warn("Could not GetTransIdByRefundId...");
				Log.Error(ex);
				throw;
			}
			return certificateData;

		}


		public bool CheckPaymentType(int paymentTypeId, int refundId=0, int orderId=0)
		{

			bool result=false;
			string sql = @"select [dbo].CheckPaymentType(@pOrderId,@pRefundId,@pPaymentType) as priceCheck";
			//decimal price = 0;
			try
			{
				using (var conn = new DbHandler())
				{
					var dr = conn.ExecuteSql(sql, new[]
					  {
						DbHandler.SetParameter("@pOrderId",SqlDbType.Int,10,ParameterDirection.Input,orderId),
						DbHandler.SetParameter("@pRefundId",SqlDbType.Int,10,ParameterDirection.Input,refundId),
						DbHandler.SetParameter("@pPaymentType",SqlDbType.Int,10,ParameterDirection.Input,paymentTypeId)

					});
					if (dr.Read())
						result = Convert.ToBoolean(dr["priceCheck"]);

				}
			}
			catch (Exception ex)
			{
				Log.Warn("Could not GetTransIdByRefundId...");
				Log.Error(ex);
				throw;
			}
			return result;

		}


		private string readResponse(Stream stream)
		{
			string response = null;
			using (StreamReader reader = new StreamReader(stream))
			{
				do
				{
					response += Convert.ToChar(reader.Read());
				} while (reader.Peek() >= 0);
			}
			return response;
		}

	
		public void RefundPay(string transId,decimal price,string tenantId)
		{

            try
            {
				StringBuilder sb = new StringBuilder(MERCHANT_HANDLER)
				.Append("command=k")
				.Append("&trans_id=").Append(HttpUtility.UrlEncode(transId))
				.Append("&amount=").Append(price);

				StoreCertificateData certificateData = GetCertificateData(tenantId);
				if (certificateData is null)
					throw new Exception("Bu mağazaya aid ödəniş məlumatı yoxdur.");

				// Calling the ECOMM module
				X509Certificate2Collection certificate = new X509Certificate2Collection();
				certificate.Import(System.Web.Hosting.HostingEnvironment.MapPath(certificateData.CertificatePath), certificateData.CertificatePassword,
					X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);

				ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
				ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true;
				HttpWebRequest req = (HttpWebRequest)WebRequest.Create(sb.ToString());
				req.AllowAutoRedirect = true;
				req.ClientCertificates = certificate;
				req.Method = "POST";
				req.ContentType = "application/x-www-form-urlencoded";
				Stream postStream = req.GetRequestStream();
				HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

				Stream stream = resp.GetResponseStream();

				string response = readResponse(stream);
				stream.Close();
			}
			catch(Exception ex)
            {
				Console.WriteLine(ex.Message);
            }

			
		}


	}
}