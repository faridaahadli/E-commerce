using CRMHalalBackEnd.DB;
using CRMHalalBackEnd.Models.Order;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Ajax.Utilities;
using System.Linq;
using Castle.Core.Internal;
using CRMHalalBackEnd.Helpers;
using CRMHalalBackEnd.Models.Employee;
using CRMHalalBackEnd.Models.Order.Buyer_Order;
using CRMHalalBackEnd.Models.Payment;
using CRMHalalBackEnd.Models.Store;

namespace CRMHalalBackEnd.Repository
{
    public class OrderRepository
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly EmployeeRepository _repositoryEmployee = new EmployeeRepository();
        public int Insert(OrderInsDto order, int userId)
        {
            string returnJson = String.Empty;
            int orderId;
            List<EmployeeUserData> employeeEmail = new List<EmployeeUserData>();
            try
            {
                using (var con = new DbHandler())
                {
                    
                    var json = JsonConvert.SerializeObject(order);
                    returnJson = con.ExecStoredProcWithOutputValue("[OrderInsert]", "@pResult",SqlDbType.NVarChar,-1, new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson", SqlDbType.NVarChar, -1, ParameterDirection.Input, json),
                            DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                        });
                    dynamic obj = JsonConvert.DeserializeObject(returnJson);
                    orderId = obj.OrderId;
                    bool isVerified = obj.IsVerified;
                    if (isVerified)
                    {
                        employeeEmail = _repositoryEmployee.GetEmployeeEmailForOrder(orderId);
                        EmailSend.SendEmail(employeeEmail);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not OrderInsert...");
                Log.Error(ex);
                throw;
            }
            return orderId;
        }


        public IEnumerable<StorePaymentDto> GetPaymentMethod(int lang, List<string> tenantIds)
        {
            string sql = @"SELECT
	                        (
	                        SELECT
		                        S.TENANT_ID TenantId,
		                        (
		                        SELECT
			                        PM.PAYMENT_METHOD_ID Id,
			                        PMT.NAME Name
		                        FROM
			                        NEW_PAYMENT_METHOD PM
			                        INNER JOIN NEW_PAYMENT_MET_TRANSLATE PMT ON PMT.PAYMENT_METHOD_ID = PM.PAYMENT_METHOD_ID 
		                        WHERE
			                        PMT.LANGUAGE_ID = @langId 
			                        AND PM.PAYMENT_METHOD_VALUE& S.STORE_PAYMENT_TYPES > 0 FOR json path 
		                        ) PaymentMethods 
	                        FROM
		                        NEW_STORE S 
	                        WHERE
	                        S.TENANT_ID IN ( SELECT VALUE FROM openjson ( @tenantId ) ) FOR json path 
	                        ) Json";
            string json = String.Empty;
            IEnumerable<StorePaymentDto> methods = new List<StorePaymentDto>();

            try
            {
                using (var conn = new DbHandler())
                {
                    var reader = conn.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@tenantId",SqlDbType.NVarChar,-1,ParameterDirection.Input,JsonConvert.SerializeObject(tenantIds)),
                        DbHandler.SetParameter("@langId",SqlDbType.Int,10,ParameterDirection.Input,lang)
                    });

                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }

                    methods = JsonConvert.DeserializeObject<IEnumerable<StorePaymentDto>>(json);
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not GetPaymentMethods...");
                Log.Error(ex);
                throw;
            }

            return methods;
        }

        public IEnumerable<PaymentMethodDto> GetAllPaymentMethod(int lang, string tenantId)
        {
            string sql = $@"SELECT
	                        (
	                        SELECT
		                        PM.PAYMENT_METHOD_ID Id,
		                        PMT.NAME Name,
		                        ( SELECT CASE WHEN S.STORE_PAYMENT_TYPES& PM.PAYMENT_METHOD_VALUE> 0 THEN 1 ELSE 0 END FROM NEW_STORE S WHERE S.TENANT_ID= @tenantId ) Selected 
	                        FROM
		                        NEW_PAYMENT_METHOD PM
		                        INNER JOIN NEW_PAYMENT_MET_TRANSLATE PMT ON PMT.PAYMENT_METHOD_ID = PM.PAYMENT_METHOD_ID 
	                        WHERE
	                        PMT.LANGUAGE_ID = @langId FOR json path 
	                        ) Json";
            string json = String.Empty;
            IEnumerable<PaymentMethodDto> methods = new List<PaymentMethodDto>();

            try
            {
                using (var conn = new DbHandler())
                {
                    var reader = conn.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@tenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@langId",SqlDbType.Int,10,ParameterDirection.Input,lang),
                    });

                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }

                    methods = JsonConvert.DeserializeObject<IEnumerable<PaymentMethodDto>>(json);
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not GetAllPaymentMethod...");
                Log.Error(ex);
                throw;
            }

            return methods;
        }
        public List<BuyerOrder> AllUserOrder(int userId,string lang)
        {
            string sql = $@"select * from dbo.GetOrderListForUser(@userId, @lang) order by OrderId";

            List<BuyerOrder> buyerOrders = new List<BuyerOrder>();

            try
            {
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@userId", SqlDbType.Int, 10, ParameterDirection.Input, userId),
                        DbHandler.SetParameter("@lang", SqlDbType.VarChar, 2, ParameterDirection.Input, lang)
                    });

                    while (reader.Read())
                    {
                        BuyerOrder buyerOrder = new BuyerOrder();
                        buyerOrder.OrderId = int.Parse(reader["OrderId"].ToString());
                        buyerOrder.StoreName = reader["StoreName"].ToString();
                        buyerOrder.Amount = decimal.Parse(reader["Amount"].ToString());
                        buyerOrder.Currency = reader["Currency"].ToString();
                        buyerOrder.Status = reader["Status"].ToString();
						buyerOrder.Date = DateTime.Parse(reader["Date"].ToString());
                        buyerOrder.Lang = reader["Lang"].ToString();
						buyerOrders.Add(buyerOrder);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not AllUserOrders...");
                Log.Error(ex);
                throw;
            }

            return buyerOrders;
        }

        public UserOrderLineFront AllOrderLineForUser(int orderId, int userId,int langId)
        {
            string sql = $@"SELECT
	(
	SELECT
		(
		SELECT
			(
			SELECT
				PR.NAME{(langId == 1 ? "" : langId.ToString())}  + ' ' + ISNULL(
					STUFF(
						(
						SELECT
							' ' + [VALUE{(langId == 1 ? "" : langId.ToString())}] 
						FROM
							NEW_PRODUCT_VARIATION t1 
						WHERE
							t1.PRODUCT_ID = PR.PRODUCT_ID 
							AND ( SELECT V.[SHOW_IN_NAME] FROM NEW_VARIATION V WHERE V.VARIATION_ID = t1.VARIATION_ID AND V.IS_ACTIVE = 1 ) = 1 
							AND t1.IS_ACTIVE= 1 FOR XML PATH ( '' ) 
						),
						1,
						0,
						'' 
					),
					'' 
				) 
			FROM
				NEW_PRODUCT PR 
			WHERE
				PR.PRODUCT_ID= SOL.PRODUCT_ID 
			) Name,
            SOL.PRODUCT_ID ProductId,
			( SELECT DELIVERY_TIME FROM NEW_ORDER_DELIVERY WHERE ORDER_ID = @orderId AND TENANT_ID = SOL.TENANT_ID ) DeliveryTime,
			SOL.TENANT_ID TenantId,
			( SELECT DELIVERY_PRICE FROM NEW_ORDER_DELIVERY WHERE ORDER_ID = @orderId AND TENANT_ID = SOL.TENANT_ID ) DeliveryPrice,
			SOL.QUANTITY Quantity,
			SOL.PRICE Price,
			( SELECT NAME FROM NEW_STORE WHERE TENANT_ID = SOL.TENANT_ID ) StoreName,
			( SELECT DOMAIN FROM NEW_STORE WHERE TENANT_ID = SOL.TENANT_ID ) DOMAIN,
            (
			SELECT
				PMT.NAME 
			FROM
				NEW_PAYMENT_METHOD PM
				INNER JOIN NEW_PAYMENT_MET_TRANSLATE PMT ON PMT.PAYMENT_METHOD_ID = PM.PAYMENT_METHOD_ID 
			WHERE
				PM.PAYMENT_METHOD_ID = ( SELECT OPT.PAYMENT_TYPE_ID FROM NEW_ORDER_PAYMENT_TYPE OPT WHERE OPT.ORDER_ID = @orderId AND OPT.TENANT_ID = SOL.TENANT_ID ) 
				AND PMT.LANGUAGE_ID = {langId}
			) PaymentType
		FROM
			NEW_SALES_ORDER_LINE SOL 
		WHERE
			SOL.SO_ID=@orderId 
			AND SOL.SO_ID IN ( SELECT SO_ID FROM NEW_SALES_ORDER WHERE USER_ID =@userId AND SOL.PRODUCT_ID IS NOT NULL ) FOR json path 
		) OrderData,
		Json_Query (
			(
			SELECT
				(
				SELECT DISTINCT
					ADDRESS 
				FROM
					NEW_ADDRESS 
				WHERE
					ADDRESS_ID IN ( SELECT DELIVERY_ADDRESS_ID FROM NEW_COMMON_DELIVERY_DATA WHERE COMMON_DELIVERY_DATA_ID IN ( SELECT COMMON_DELIVERY_DATA_ID FROM NEW_ORDER_DELIVERY WHERE ORDER_ID =@orderId ) ) 
				) AddressName,
				(
				SELECT
					NOTE 
				FROM
					NEW_COMMON_DELIVERY_DATA 
				WHERE
					COMMON_DELIVERY_DATA_ID IN ( SELECT COMMON_DELIVERY_DATA_ID FROM NEW_ORDER_DELIVERY WHERE ORDER_ID =@orderId AND ORDER_ID IN ( SELECT SO_ID FROM NEW_SALES_ORDER WHERE USER_ID =@userId ) ) 
				) Note FOR json path,
				without_array_wrapper 
			) 
		) CommonData,
		(
		SELECT
			(
			SELECT
				P.[DESCRIPTION{(langId == 1 ? "" : langId.ToString())}] Description,
                P.PROMO_ID PromoId,
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
						P.TENANT_ID TenantId,
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
											NAME{(langId == 1 ? "" : langId.ToString())} AS Name,
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
											NAME{(langId == 1 ? "" : langId.ToString())} AS Name,
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
											NAME{(langId == 1 ? "" : langId.ToString())},
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
							P.PROMO_ID IN ( SELECT SOL.PROMOTION_ID FROM NEW_SALES_ORDER_LINE SOL WHERE SOL.SO_ID= @orderId ) FOR json path 
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
		          ) RefundData

					FOR json path,
				without_array_wrapper 
	) Json";
            string json = String.Empty;
            UserOrderLineFront orderLineFront = new UserOrderLineFront();
            try
            {
                UserOrderLine orderLine;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]

                    {
                        DbHandler.SetParameter("@orderId", SqlDbType.Int, 10, ParameterDirection.Input, orderId),
                          DbHandler.SetParameter("@userId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }

                    orderLine = JsonConvert.DeserializeObject<UserOrderLine>(json);
                    orderLine = orderLine ?? new UserOrderLine();
                }
                //for CommonData
                orderLineFront.CommonData = new CommonData();

                orderLineFront.CommonData.AddressName = orderLine.CommonData.AddressName;
                orderLineFront.CommonData.Note = orderLine.CommonData.Note;
				orderLineFront.RefundData = orderLine.RefundData;
                //orderLineFront.CommonData.PaymentType = orderLine.CommonData.PaymentType;

                orderLineFront.StoreData = new List<StoreDataFront>();
                StoreDataFront storeDataFront = null;
                //var groupByPromotionsDataForTenant = orderLine.Promotions?.GroupBy(p => p.TenantId, (p, l) =>
                //{
                //    storeDataFront = new StoreDataFront();
                //    //StoreDataFront storeDataFrontPromotion = new StoreDataFront();
                //    storeDataFront.Promotions = l.ToList();
                //    return storeDataFront;
                //});




                var groupByPromotionsDataByTenant = orderLine.Promotions?.GroupBy(p => p.TenantId, (p, l) =>
                {
                    PromotionSplitForTenant promotionSplitForTenant = new PromotionSplitForTenant();
                    promotionSplitForTenant.PromotionsList = l.ToList();
                    promotionSplitForTenant.PromotionsList.ForEach(a =>
                    {
                        a.Slug = a.Description.UrlFriendly("en") + "-" + a.PromoId;
                    });
                    promotionSplitForTenant.TenantId = p/*l.FirstOrDefault().TenantId*/;
                    return promotionSplitForTenant;
                });

                var groupByOrderDataForTenant = orderLine.OrderData.GroupBy(a => a.TenantId, (a, b) =>
                 {
                     storeDataFront = new StoreDataFront();
                     storeDataFront.TenantId = b.FirstOrDefault().TenantId;
                     storeDataFront.StoreName = b.FirstOrDefault().StoreName;
                     storeDataFront.Domain = b.FirstOrDefault().Domain;
                     storeDataFront.PaymentType = b.FirstOrDefault().PaymentType;
                     if (!b.IsNullOrEmpty())
                     {
                         storeDataFront.DeliveryTime = b.First().DeliveryTime;
                     }
                     storeDataFront.DeliveryPrice = b.FirstOrDefault().DeliveryPrice;
                     storeDataFront.OrderData = b.Where(c => c.TenantId == storeDataFront.TenantId).Select(d =>
                     {
                         OrderDataFront orderDataFront = new OrderDataFront();
                         orderDataFront.ProductId = d.ProductId;
                         orderDataFront.Name = d.Name;
                         orderDataFront.Slug = d.Name.UrlFriendly("en") + "-" + d.ProductId;
                         orderDataFront.Price = d.Price;
                         orderDataFront.Quantity = d.Quantity;
                         return orderDataFront;
                     }).ToList();
                     storeDataFront.Promotions = groupByPromotionsDataByTenant
                         ?.Where(k => k.TenantId == storeDataFront.TenantId).Select(k => k.PromotionsList).FirstOrDefault();
                     return storeDataFront;
                 });
                //groupByPromotionsDataForTenant.ForEach(p => orderLineFront.StoreData.Add(p));
                groupByOrderDataForTenant.ForEach(a => orderLineFront.StoreData.Add(a));

            }
            catch (Exception ex)
            {
                Log.Warn("Could not Orders...");
                Log.Error(ex);
                throw;
            }
            return orderLineFront;

        }

        

    }
}