using CRMHalalBackEnd.DB;
using CRMHalalBackEnd.Models.Courier.OrderStatus;
using CRMHalalBackEnd.Models.Order;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using CRMHalalBackEnd.Helpers;

namespace CRMHalalBackEnd.Repository
{
    public class OrderAssignRepository
    {
        private static readonly log4net.ILog Log =
          log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region OrderPage
        public List<AllOrderForAdmin> GetAllOrderForAdmin(int lang, string tenantId, int userId)
        {
            string sql =
                    $@"SELECT
	                  (
	                  SELECT
		                  SO.SO_ID AS OrderId,
		                  ( SELECT LAST_NAME + ' ' + FIRST_NAME FROM NEW_USER U WHERE U.USER_ID = SO.USER_ID ) AS Buyer,
		                  AMOUNT Price,
		                  CURRENCY Currency,
		                  ( SELECT NAME{(lang==1?"":lang.ToString())} FROM NEW_SO_STATUS SS WHERE SS.SO_STATUS_ID= SO.SO_STATUS_ID AND SS.IS_ACTIVE= 1 ) AS Status,
		                  CREATE_DATE AS CreatedDate 
	                  FROM
		                  NEW_SALES_ORDER SO 
	                  WHERE
		                  SO.SO_STATUS_ID IN ( SELECT SO_STATUS_ID FROM NEW_SO_STATUS WHERE TENANT_ID =@tenantId ) 
		                  AND SO.IS_ACTIVE= 1 
	                  AND EXISTS ( SELECT * FROM dbo.GetEmployeePermission ( @userId, @tenantId, '106' ) ) FOR json path 
	                  ) Json";
            List<AllOrderForAdmin> orders;

            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                   {
                        DbHandler.SetParameter("@tenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@userId",SqlDbType.Int,10,ParameterDirection.Input,userId)

                    });
                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                }

                orders = JsonConvert.DeserializeObject<List<AllOrderForAdmin>>(json);
                orders = orders ?? new List<AllOrderForAdmin>();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return orders;
        }


        public List<OrderLineInfo> GetAllOrderLineForAdmin(int lang, string tenantId, int userId, int orderId)
        {
            string sql =
                             $@"SELECT
	(
	SELECT
		JSON_QUERY (
			(
			SELECT
				U.LAST_NAME + ' ' + U.FIRST_NAME UserName,
				( SELECT TOP ( 1 ) TEXT FROM NEW_CONTACT C WHERE C.USER_ID = U.USER_ID AND C.CONTACT_TYPE_ID= 2 ) UserPhone,
				(
				SELECT
	                (
	                SELECT
		                PMT.NAME 
	                FROM
		                NEW_PAYMENT_METHOD PM
		                INNER JOIN NEW_PAYMENT_MET_TRANSLATE PMT ON PMT.PAYMENT_METHOD_ID = PM.PAYMENT_METHOD_ID 
	                WHERE
		                PM.PAYMENT_METHOD_ID= OPT.PAYMENT_TYPE_ID 
		                AND PMT.LANGUAGE_ID = ( SELECT SL.LANGUAGE_ID FROM NEW_STORE_LANGUAGE SL WHERE SL.TENANT_ID = OPT.TENANT_ID AND SL.IS_ACTIVE = 1 AND SL.NUMBER = { lang } ) 
	                ) 
                FROM
	                NEW_ORDER_PAYMENT_TYPE OPT 
                WHERE
	                OPT.ORDER_ID = @orderId 
	                AND OPT.TENANT_ID = @tenantId
				) PaymentMethod,
				(
				SELECT DISTINCT
					A.ADDRESS AS Address
				FROM
					NEW_ADDRESS A
					INNER JOIN NEW_COMMON_DELIVERY_DATA CDA ON CDA.DELIVERY_ADDRESS_ID= A.ADDRESS_ID 
				WHERE
				    CDA.COMMON_DELIVERY_DATA_ID IN ( SELECT COMMON_DELIVERY_DATA_ID FROM NEW_ORDER_DELIVERY WHERE ORDER_ID =@orderId AND TENANT_ID =@tenantId ) 
				) Address,
				( SELECT DISTINCT NOTE FROM NEW_COMMON_DELIVERY_DATA CDA WHERE CDA.COMMON_DELIVERY_DATA_ID IN ( SELECT COMMON_DELIVERY_DATA_ID FROM NEW_ORDER_DELIVERY WHERE ORDER_ID =@orderId AND TENANT_ID =@tenantId ) ) Note,
				( SELECT DISTINCT SELECTED_TIME AS SelectedTime FROM NEW_ORDER_DELIVERY WHERE ORDER_ID =@orderId AND TENANT_ID =@tenantId ) SelectedTime,
				( SELECT DELIVERY_PRICE FROM NEW_ORDER_DELIVERY WHERE ORDER_ID =@orderId AND TENANT_ID =@tenantId ) DeliveryPrice
			FROM
				NEW_USER U
				INNER JOIN NEW_SALES_ORDER SO ON U.USER_ID = SO.USER_ID 
			WHERE
				SO.SO_ID= @orderId FOR json path,
				without_array_wrapper 
			) 
		) UserData,
        (
		SELECT DISTINCT
		   SU.UPDATE_DATE UpdatedTime,
		   U.FIRST_NAME+ ' ' + U.LAST_NAME FullName,
		   SS.NAME{(lang==1?"":lang.ToString())} [Status] 
		FROM
		   NEW_SO_UPDATE SU
		   INNER JOIN NEW_SO_UPDATE_LINE SUL ON SU.SO_UPD_ID= SUL.SO_UPD_ID
		   INNER JOIN NEW_SO_STATUS SS ON SS.SO_STATUS_ID= SU.SO_STATUS_ID
		   INNER JOIN NEW_USER U ON U.USER_ID = SU.UPDATED_BY
		   INNER JOIN NEW_SALES_ORDER_LINE SOL ON SOL.SO_LINE_ID= SUL.SO_LINE_ID 
		WHERE
		   SOL.SO_ID=@orderId 
		   AND SOL.TENANT_ID=@tenantId FOR json path 
		) StatusData,
		(
		SELECT
			(
			SELECT
				[DESCRIPTION{(lang == 1 ? "" : lang.ToString())}],
				(
				                    SELECT
				                    CASE
						                    
					                    WHEN
						                    EFFECTIVE_AMOUNT IS NOT NULL THEN
							                    EFFECTIVE_AMOUNT ELSE AMOUNT 
						                    END 
						                    FROM
							                    NEW_SALES_ORDER_LINE SOL 
						                    WHERE
							                    SOL.SO_ID=@orderId 
							                    AND SOL.PROMOTION_ID= P.PROMO_ID 
						                    ) Amount,
						                    P.PROMO_ID PromoId,
						                    ( SELECT SOL.QUANTITY FROM NEW_SALES_ORDER_LINE SOL WHERE SOL.PROMOTION_ID= P.PROMO_ID AND SOL.SO_ID=@orderId ) Quantity,
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
											                    NAME{(lang == 1 ? "" : lang.ToString())} AS Name,
											                    ( SELECT SOL.QUANTITY FROM NEW_SALES_ORDER_LINE SOL WHERE SOL.PROMOTION_ID= P.PROMO_ID AND SOL.SO_ID=@orderId ) Quantity,
											                    PRICE AS Price 
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
											                    NAME{(lang == 1 ? "" : lang.ToString())} AS Name,
											                    ( SELECT SOL.QUANTITY FROM NEW_SALES_ORDER_LINE SOL WHERE SOL.PROMOTION_ID= P.PROMO_ID AND SOL.SO_ID=@orderId ) Quantity,
											                    PRICE AS Price 
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
											                    NAME{(lang == 1 ? "" : lang.ToString())},
											                    ( SELECT SOL.QUANTITY FROM NEW_SALES_ORDER_LINE SOL WHERE SOL.PROMOTION_ID= P.PROMO_ID AND SOL.SO_ID=@orderId ) Quantity,
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
							                    P.PROMO_ID IN ( SELECT SOL.PROMOTION_ID FROM NEW_SALES_ORDER_LINE SOL WHERE SOL.SO_ID= @orderId AND P.TENANT_ID=@tenantId ) FOR json path 
						                    ) Json 
					                    ) Promotions,
	    (
		         select JSON_QUERY(
		        (select RF.REFUND_ID RefundId,
		        (select dbo.GetRefundedPrice(RF.REFUND_ID,Rf.IS_COURIER_COST_INCLUDED))Amount,
		          RF.IS_COURIER_COST_INCLUDED IsCourierCostInclude,	

                  RF.IS_CONFIRMED IsConfirm,
				  RF.NOTE Reason,
				  RF.REFUND_BY_ADMIN RefundByAdmin,
				  
		         (		   
		          select  RD.PRODUCT_QUANTITY as Quantity,
				  RD.REASON_OF_REFUND Reason,
                  RD.IS_CONFIRMED IsConfirm,
				  RD.REASON_OF_REJECT RejectReason,

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
					P.NAME{(lang == 1 ? "" : lang.ToString())} + ' ' + ISNULL(
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
				AND P.TENANT_ID=@tenantId FOR json path 
				) Products
) Products FOR json path 
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
                    x.Products?.ForEach(y => y.Slug = y.Name.UrlFriendly("en") + "-" + y.ProductId );
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

        public List<OrderStatusInsert> GetAllStatusForOrder(int lang, string tenantId, int userId)
        {
            string sql =
                              $@"SELECT
	                            (
	                            SELECT
		                            SO_STATUS_ID Id,
		                            NAME{(lang==1?"":lang.ToString())} AS AdminStatus 
	                            FROM
		                            NEW_SO_STATUS 
	                            WHERE
		                            TENANT_ID =@tenantId 
		                            AND IS_ACTIVE = 1 
	                            AND EXISTS ( SELECT * FROM dbo.GetEmployeePermission ( @userId, @tenantId, '106' ) ) FOR json path 
	                            ) Json";
            List<OrderStatusInsert> status;

            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                   {
                        DbHandler.SetParameter("@tenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@userId",SqlDbType.Int,10,ParameterDirection.Input,userId)

                    });
                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                }

                status = JsonConvert.DeserializeObject<List<OrderStatusInsert>>(json);
                status = status ?? new List<OrderStatusInsert>();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return status;
        }

        public int OrderStatusUpdate(OrderStatusUpdate orderStatus, string tenantId, int userId)
        {
            var id = 0;
            try
            {
                using (var con = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(orderStatus);
                    id = con.ExecStoredProcWithReturnIntValue("[OrderStatusUpdate]", new[]
                     {
                        DbHandler.SetParameter("@pRequestAsJson", SqlDbType.NVarChar, -1, ParameterDirection.Input, json),
                        DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not OrderStatusUpdate...");
                Log.Error(ex);
                throw;
            }

            return id;
        }

      
        public List<Customers> GetAllCustomer(string tenantId, int userId)
        {
            const string sql =
                              @"SELECT
	                            (
	                            SELECT DISTINCT
		                            U.LAST_NAME+ ' ' + U.FIRST_NAME UserName,
		                            U.USER_ID Id 
	                            FROM
		                            NEW_USER U
		                            INNER JOIN NEW_SALES_ORDER SO ON SO.USER_ID = U.USER_ID 
	                            WHERE
		                            SO.SO_ID IN ( SELECT SO_ID FROM NEW_SALES_ORDER_LINE WHERE TENANT_ID =@tenantId ) 
	                            AND EXISTS ( SELECT * FROM dbo.GetEmployeePermission ( @userId, @tenantId, '106' ) ) FOR json path 
	                            ) Json"; 
                                
            List<Customers> customers;

            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                   {
                        DbHandler.SetParameter("@tenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@userId",SqlDbType.Int,10,ParameterDirection.Input,userId)

                    });
                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                }

                customers = JsonConvert.DeserializeObject<List<Customers>>(json);
                customers = customers ?? new List<Customers>();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return customers;
        }

        public List<AllOrderForAdmin> OrderPagination(int userId,string tenantId, int page, int perSize = 10)
        {
            const string sql =
                @"SELECT
	                (
	                SELECT
		                SO.SO_ID AS OrderId,
		                ( SELECT LAST_NAME + ' ' + FIRST_NAME FROM NEW_USER U WHERE U.USER_ID = SO.USER_ID ) AS Buyer,
		                AMOUNT Price,
		                CURRENCY Currency,
		                ( SELECT NAME FROM NEW_SO_STATUS SS WHERE SS.SO_STATUS_ID= SO.SO_STATUS_ID AND SS.IS_ACTIVE= 1 ) AS Status,
		                CREATE_DATE AS CreatedDate 
	                FROM
		                NEW_SALES_ORDER SO 
	                WHERE
		                SO.SO_STATUS_ID IN ( SELECT SO_STATUS_ID FROM NEW_SO_STATUS WHERE TENANT_ID =@tenantId ) 
		                AND SO.IS_ACTIVE= 1 
                        AND SO.IS_VERIFIED = 1
		                AND EXISTS ( SELECT * FROM dbo.GetEmployeePermission ( @userId, @tenantId, '106' ) ) 
	                ORDER BY
	                CREATE_DATE DESC OFFSET @PAGE *@PER_SIZE row FETCH NEXT @PER_SIZE row ONLY FOR json path 
	                ) Json";

            List<AllOrderForAdmin> orders;
            try

            {
                string json = String.Empty;

                using (var con = new DbHandler())
                {

                    page = page - 1 >= 0 ? page - 1 : throw new Exception("Page length must be greater than 0");
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@PAGE",SqlDbType.Int,10,ParameterDirection.Input,page),
                        DbHandler.SetParameter("@PER_SIZE",SqlDbType.Int,10,ParameterDirection.Input,perSize),
                        DbHandler.SetParameter("@tenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@userId",SqlDbType.Int,-1,ParameterDirection.Input,userId)

                    });

                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                    orders = JsonConvert.DeserializeObject<List<AllOrderForAdmin>>(json);
                    orders = orders ?? new List<AllOrderForAdmin>();
                }
                
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return orders;
        }

        public List<AllOrderForAdmin> OrderFilter(int lang,int? statusId, int? customerId, string tenantId, int userId, string beginDate, string deliveryTime, int? filterId, int page, int perSize = 10)
        {
            page = page - 1 >= 0 ? page - 1 : throw new Exception("Page length must be greater than 0");

            string sql =
                $@"SELECT
	                (
	                SELECT
		                SO.SO_ID AS OrderId,
		                ( SELECT LAST_NAME + ' ' + FIRST_NAME FROM NEW_USER U WHERE U.USER_ID = SO.USER_ID ) AS Buyer,
		                ( SELECT SO_LINE_ID LineId FROM NEW_SALES_ORDER_LINE WHERE SO_ID = SO.SO_ID FOR json path ) Lines,
		                AMOUNT Price,
		                ( SELECT OD.DELIVERY_PRICE FROM NEW_ORDER_DELIVERY OD WHERE OD.ORDER_ID = SO.SO_ID AND OD.TENANT_ID = @tenantId ) DeliveryPrice,
		                CURRENCY Currency,
		                (
		                SELECT
			                NAME{(lang==1?"":lang.ToString())}
		                FROM
			                NEW_SO_STATUS SS 
		                WHERE
			                SS.SO_STATUS_ID IN ( SELECT TOP ( 1 ) SO_STATUS_ID FROM NEW_SALES_ORDER_LINE SOL WHERE SOL.SO_ID= SO.SO_ID ) 
			                AND SS.IS_ACTIVE= 1 
		                ) AS Status,
		                (
		                SELECT
			                SO_STATUS_ID 
		                FROM
			                NEW_SO_STATUS SS 
		                WHERE
			                SS.SO_STATUS_ID IN ( SELECT TOP ( 1 ) SO_STATUS_ID FROM NEW_SALES_ORDER_LINE SOL WHERE SOL.SO_ID= SO.SO_ID ) 
			                AND SS.IS_ACTIVE= 1 
		                ) AS StatusId,
		                CREATE_DATE AS CreatedDate 
	                FROM
		                NEW_SALES_ORDER SO 
	                WHERE
		                { ( beginDate != null ? " CONVERT(VARCHAR,SO.CREATE_DATE,23) LIKE '%' +  @beginDate + '%' and " : "" ) } 
		                { ( deliveryTime != null ? " (select OD.ORDER_ID from NEW_ORDER_DELIVERY OD where OD.ORDER_ID=SO.SO_ID and CONVERT(VARCHAR,OD.DELIVERY_TIME,23) LIKE '%' +  @deliveryTime + '%'  ) is not null and " : "" ) } 
		                { ( customerId != null ? "  USER_ID=@customerId and " : "" ) }
		                { ( statusId != null ? "(select top(1) SO_STATUS_ID from NEW_SALES_ORDER_LINE where SO_ID=SO.SO_ID and TENANT_ID=@tenantId )=@statusId and  " : "" ) } EXISTS ( SELECT * FROM dbo.GetEmployeePermission ( @userId, @tenantId, '106' ) ) 
		                AND SO.IS_VERIFIED = 1 
		                AND SO.SO_ID IN ( SELECT SO_ID FROM NEW_SALES_ORDER_LINE SOL WHERE SOL.PRODUCT_ID IN ( SELECT PRODUCT_ID FROM NEW_PRODUCT WHERE TENANT_ID =@tenantId ) ) 
                        { ( filterId == 1 || filterId == null ? "order by CREATE_DATE DESC  OFFSET @page*@perSize row FETCH NEXT @perSize row ONLY" : "" ) }
		                { ( filterId == 2 ? "order by CREATE_DATE ASC OFFSET @page*@perSize row FETCH NEXT @perSize row ONLY" : "" ) }
		                { ( filterId == 3 ? "order by AMOUNT DESC OFFSET @page*@perSize row FETCH NEXT @perSize row ONLY" : "" ) }
	                { ( filterId == 4 ? "order by AMOUNT ASC OFFSET @page*@perSize row FETCH NEXT @perSize row ONLY " : "" ) } FOR json path 
	                ) Json";

            List<AllOrderForAdmin> orders;

            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {

                    var reader = con.ExecuteSql(sql, new[]
                   {
                        DbHandler.SetParameter("@page",SqlDbType.Int,10,ParameterDirection.Input,page),
                        DbHandler.SetParameter("@perSize",SqlDbType.Int,10,ParameterDirection.Input,perSize),
                        DbHandler.SetParameter("@tenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@userId", SqlDbType.Int, -1, ParameterDirection.Input, userId),
                        beginDate!=null?DbHandler.SetParameter("@beginDate",SqlDbType.VarChar,-1,ParameterDirection.Input,beginDate):DbHandler.SetParameter("@beginDate",SqlDbType.VarChar,-1,ParameterDirection.Input,DBNull.Value),
                        deliveryTime!=null? DbHandler.SetParameter("@deliveryTime",SqlDbType.VarChar,-1,ParameterDirection.Input,deliveryTime):DbHandler.SetParameter("@deliveryTime",SqlDbType.VarChar,-1,ParameterDirection.Input,DBNull.Value),
                        customerId!=null? DbHandler.SetParameter("@customerId",SqlDbType.Int,-1,ParameterDirection.Input,customerId):DbHandler.SetParameter("@customerId",SqlDbType.Int,-1,ParameterDirection.Input,DBNull.Value),
                        statusId != null ? DbHandler.SetParameter("@statusId", SqlDbType.Int, -1, ParameterDirection.Input, statusId) : DbHandler.SetParameter("@statusId", SqlDbType.Int, -1, ParameterDirection.Input, DBNull.Value)
                    });
                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                }

                orders = JsonConvert.DeserializeObject<List<AllOrderForAdmin>>(json);
                orders = orders ?? new List<AllOrderForAdmin>();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return orders;
        }

        public int GetOrderSize(int? statusId, int? customerId, string tenantId, int userId, string beginDate, string deliveryTime, int perSize=10)
        {
             string sql =
                $@"SELECT 
                    COUNT( * ) [COUNT] 
                FROM
	                (
	                SELECT
		                SO.SO_ID AS OrderId,
		                ( SELECT LAST_NAME + ' ' + FIRST_NAME FROM NEW_USER U WHERE U.USER_ID = SO.USER_ID ) AS Buyer,
		                AMOUNT Price,
		                CURRENCY Currency,
		                (
		                SELECT
			                NAME 
		                FROM
			                NEW_SO_STATUS SS 
		                WHERE
			                SS.SO_STATUS_ID IN ( SELECT TOP ( 1 ) SO_STATUS_ID FROM NEW_SALES_ORDER_LINE SOL WHERE SOL.SO_ID= SO.SO_ID ) 
			                AND SS.IS_ACTIVE= 1 
		                ) AS Status,
		                CREATE_DATE AS CreatedDate 
	                FROM
		                NEW_SALES_ORDER SO 
	                WHERE
		                { (beginDate != null ? " CONVERT(VARCHAR,SO.CREATE_DATE,23) LIKE '%' +  @beginDate + '%' and " : "") } 
		                { (deliveryTime != null ? " (select OD.ORDER_ID from NEW_ORDER_DELIVERY OD where OD.ORDER_ID=SO.SO_ID and CONVERT(VARCHAR,OD.DELIVERY_TIME,23) LIKE '%' +  @deliveryTime + '%'  ) is not null and " : "") } 
		                { (customerId != null ? "  USER_ID=@customerId and " : "") }
		                { (statusId != null ? "(select top(1) SO_STATUS_ID from NEW_SALES_ORDER_LINE where SO_ID=SO.SO_ID and TENANT_ID=@tenantId )=@statusId and  " : "") } EXISTS ( SELECT * FROM dbo.GetEmployeePermission ( @userId, @tenantId, '106' ) ) 
	                AND SO.SO_ID IN ( SELECT SO_ID FROM NEW_SALES_ORDER_LINE SOL WHERE SOL.PRODUCT_ID IN ( SELECT PRODUCT_ID FROM NEW_PRODUCT WHERE TENANT_ID =@tenantId ) ) 
	                ) AA";

            int size = 0;
            try
            {
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@perSize",SqlDbType.Int,10,ParameterDirection.Input,perSize),
                        DbHandler.SetParameter("@tenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@userId", SqlDbType.Int, -1, ParameterDirection.Input, userId),
                        beginDate!=null?DbHandler.SetParameter("@beginDate",SqlDbType.VarChar,-1,ParameterDirection.Input,beginDate):DbHandler.SetParameter("@beginDate",SqlDbType.VarChar,-1,ParameterDirection.Input,DBNull.Value),
                        deliveryTime!=null? DbHandler.SetParameter("@deliveryTime",SqlDbType.VarChar,-1,ParameterDirection.Input,deliveryTime):DbHandler.SetParameter("@deliveryTime",SqlDbType.VarChar,-1,ParameterDirection.Input,DBNull.Value),
                        customerId!=null? DbHandler.SetParameter("@customerId",SqlDbType.Int,-1,ParameterDirection.Input,customerId):DbHandler.SetParameter("@customerId",SqlDbType.Int,-1,ParameterDirection.Input,DBNull.Value),
                        statusId != null ? DbHandler.SetParameter("@statusId", SqlDbType.Int, -1, ParameterDirection.Input, statusId) : DbHandler.SetParameter("@statusId", SqlDbType.Int, -1, ParameterDirection.Input, DBNull.Value)                    });
                    int count = 0;
                    if (reader.Read())
                    {
                        count = reader.GetInt("COUNT");
                    }

                    if (count % perSize == 0)
                    {
                        size = count / perSize;
                    }
                    else
                    {
                        size = count / perSize + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return size;

        }
        #endregion


        #region StatusPage
        public string OrderStatusInsert(OrderStatusInsert status, string tenantId, int userId)
        {
            try
            {
                using (var con = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(status);
                    con.ExecuteStoredProcedure("[OrderStatusInsert]", new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson", SqlDbType.NVarChar, -1, ParameterDirection.Input, json),
                        DbHandler.SetParameter("@pTenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not OrderStatusInsert...");
                Log.Error(ex);
                throw;
            }
            return "Successful operation";
        }


        public List<OrderStatusInsert> GetAllStatus(string tenantId, int userId)
        {
            const string sql =
                              @"SELECT
	                                (
	                                SELECT
		                                SO_STATUS_ID Id,
		                                NAME AS AdminStatus,
                                        NAME2 AS AdminStatus2,
                                        NAME3 AS AdminStatus3,
                                        NAME4 AS AdminStatus4,
		                                IS_AFTER_PAYMENT IsAfterPayment,
		                                BUYER_NAME  AS UserStatus,
                                        BUYER_NAME2 AS UserStatus2,
                                        BUYER_NAME3 AS UserStatus3,
                                        BUYER_NAME4 AS UserStatus4 
	                                FROM
		                                NEW_SO_STATUS 
	                                WHERE
		                                TENANT_ID =@tenantId 
		                                AND IS_ACTIVE = 1 
	                                AND EXISTS ( SELECT * FROM dbo.GetEmployeePermission ( @userId, @tenantId, '104' ) ) FOR json path 
	                                ) Json";
            List<OrderStatusInsert> status;

            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                   {
                        DbHandler.SetParameter("@tenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@userId",SqlDbType.Int,10,ParameterDirection.Input,userId)

                    });
                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                }

                status = JsonConvert.DeserializeObject<List<OrderStatusInsert>>(json);
                status = status ?? new List<OrderStatusInsert>();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return status;
        }


        public int DeleteOrderStatus(int Id, string tenantId, int userId)
        {
            int returnId = 0;
            try
            {
                using (var con = new DbHandler())
                {
                    returnId = con.ExecStoredProcWithReturnIntValue("OrderStatusDelete", new[]
                    {
                       DbHandler.SetParameter("@pStatusId", SqlDbType.Int, 10, ParameterDirection.Input, Id),
                       DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId),
                       DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId)
                    });

                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not OrderStatusDelete...");
                Log.Error(ex);
                throw;
            }

            return returnId;

        }

        public void UpdateStatusPayment(int statusId, string tenantId, int userId)
        {
            try
            {
                using (var con = new DbHandler())
                {
                    con.ExecuteStoredProcedure("UpdateStatusPayment", new[]
                    {
                        DbHandler.SetParameter("@pStatusId", SqlDbType.Int, 10, ParameterDirection.Input, statusId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId),
                        DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId)
                    });

                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not UpdateStatusPayment...");
                Log.Error(ex);
                throw;
            }
        }

        #endregion

    }
}