using CRMHalalBackEnd.DB;
using CRMHalalBackEnd.Models.Courier;
using CRMHalalBackEnd.Models.Courier.OrderStatus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Repository
{
    public class CourierRepository
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        #region Courier
        public List<RegionResponse> GetRegions(int langId)
        {
            const string sql =
                @"SELECT
	                RD.REGION_DELIVERY_ID AS Id,
	                RDT.REGION_NAME AS Name 
                FROM
	                NEW_REGION_DELIVERY RD
	                INNER JOIN NEW_REGION_DEL_TRANSLATE RDT ON RDT.REGION_DEL_ID = RD.REGION_DELIVERY_ID 
                WHERE
	                RD.IS_ACTIVE = 1
	                and RDT.LANGUAGE_ID = @langId";
            List<RegionResponse> regions = new List<RegionResponse>();

            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql,new []
                    {
                        DbHandler.SetParameter("@langId",SqlDbType.Int,10,ParameterDirection.Input,langId)
                    });

                    while (reader.Read())
                    {
                        RegionResponse regionResponse = new RegionResponse()
                        {
                            Id = int.Parse(reader["Id"].ToString()),
                            Name = reader["Name"].ToString()
                        };
                        regions.Add(regionResponse);
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return regions;
        }

        public string RegionDeliveryInsert(CourierInsDto delivery,string tenantId,int userId)
        {
            try
            {
                using (var con = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(delivery);
                    con.ExecuteStoredProcedure("[CourierForRegionInsert]", new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson", SqlDbType.NVarChar, -1, ParameterDirection.Input, json),
                        DbHandler.SetParameter("@pTenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not CourierForRegionInsert...");
                Log.Error(ex);
                throw;
            }
            return "okay";
        }
        public string StoreDeliveryInfoInsUpd(CourierInsDto delivery, string tenantId, int userId)
        {
            try
            {
                using (var con = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(delivery);
                    con.ExecuteStoredProcedure("[StoreDeliveryInfoIns]", new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson", SqlDbType.NVarChar, -1, ParameterDirection.Input, json),
                        DbHandler.SetParameter("@pTenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not StoreDeliveryInfoIns...");
                Log.Error(ex);
                throw;
            }
            return "okay";
        }

        public List<RegionDeliveryResponse> GetRegionDelivery(int lang, string tenantId,int userId)
        {
           string sql =
               $@"SELECT
	                (
	                SELECT
		                SDI.IS_DELIVERY_EXIST IsDeliveryExist,
		                SDI.MIN_PRICE_FOR_FREE MinPriceForFree,
		                SDI.NOTE Note,
                        SDI.NOTE2 Note2,
                        SDI.NOTE3 Note3,
                        SDI.NOTE4 Note4,
		                json_query (
			                (
			                SELECT
				                RDT.REGION_NAME [Name],
				                DP.DPRICING_ID Id,
				                DP.PRICE Price 
			                FROM
				                NEW_DELIVERY_PRICING DP
				                INNER JOIN NEW_REGION_DELIVERY RD ON DP.REGION_DELIVERY_ID= RD.REGION_DELIVERY_ID
				                INNER JOIN NEW_REGION_DEL_TRANSLATE RDT ON RDT.REGION_DEL_ID = RD.REGION_DELIVERY_ID 
			                WHERE
				                DP.TENANT_ID= SDI.TENANT_ID 
				                AND DP.IS_ACTIVE= 1 
				                AND RD.IS_ACTIVE= 1 
				                AND RDT.LANGUAGE_ID = {lang} FOR json path 
			                ) 
		                ) RegionPrices 
	                FROM
		                NEW_STORE_DELIVERY_INFO SDI 
	                WHERE
		                SDI.TENANT_ID = @pTenantId 
	                AND SDI.IS_ACTIVE = 1 and EXISTS(select * from dbo.GetEmployeePermission(@pUserId,@pTenantId, '96')) FOR json path 
	                ) Json";
            string json = String.Empty;
            List<RegionDeliveryResponse> response = null;
            try
            {
                
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql,new[] {
                        DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@pUserId",SqlDbType.Int,10,ParameterDirection.Input,userId)
                    });

                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                    response = JsonConvert.DeserializeObject<List<RegionDeliveryResponse>>(json);
                }
                
            }
            catch (Exception ex)
            {
                Log.Warn("Could not Get Region Delivery Data...");
                Log.Error(ex);
                throw;
            }
            return response;
        }
        public List<RegionDeliveryResponse> GetRegionDeliveryForShopPage(int lang, string tenantId)
        {
            string sql =
                $@"SELECT
	                (
	                SELECT
		                SDI.IS_DELIVERY_EXIST IsDeliveryExist,
		                SDI.MIN_PRICE_FOR_FREE MinPriceForFree,
		                SDI.NOTE Note{(lang==1?"":lang.ToString())},
		                json_query (
			                (
			                SELECT
				                RDT.REGION_NAME [Name],
				                DP.DPRICING_ID Id,
				                DP.PRICE Price 
			                FROM
				                NEW_DELIVERY_PRICING DP
				                INNER JOIN NEW_REGION_DELIVERY RD ON DP.REGION_DELIVERY_ID= RD.REGION_DELIVERY_ID
				                INNER JOIN NEW_REGION_DEL_TRANSLATE RDT ON RDT.REGION_DEL_ID = RD.REGION_DELIVERY_ID 
			                WHERE
				                DP.TENANT_ID= SDI.TENANT_ID 
				                AND DP.IS_ACTIVE= 1 
				                AND RD.IS_ACTIVE= 1 
				                AND RDT.LANGUAGE_ID = ( SELECT SL.LANGUAGE_ID FROM NEW_STORE_LANGUAGE SL WHERE SL.TENANT_ID = @pTenantId AND SL.IS_ACTIVE = 1 AND SL.NUMBER = {lang} ) FOR json path 
			                ) 
		                ) RegionPrices 
	                FROM
		                NEW_STORE_DELIVERY_INFO SDI 
	                WHERE
		                SDI.TENANT_ID = @pTenantId 
	                AND SDI.IS_ACTIVE = 1 FOR json path 
	                ) Json";
            string json = String.Empty;
            List<RegionDeliveryResponse> response = null;
            try
            {

                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[] {
                        DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                    });

                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                    response = JsonConvert.DeserializeObject<List<RegionDeliveryResponse>>(json);
                }

            }
            catch (Exception ex)
            {
                Log.Warn("Could not Get Region Delivery Data For Shop Page...");
                Log.Error(ex);
                throw;
            }
            return response;
        }

        public List<RegionDeliveryResponse> GetRegionDeliveryForStore(int langId,int langNumber, string tenantId, int userId)
        {
            string sql =
                $@"SELECT
	                (
	                SELECT
		                SDI.IS_DELIVERY_EXIST IsDeliveryExist,
		                SDI.MIN_PRICE_FOR_FREE MinPriceForFree,
		                SDI.NOTE{(langNumber==1?"":langNumber.ToString())} Note,
		                json_query (
			                (
			                SELECT
				                RDT.REGION_NAME [Name],
				                DP.DPRICING_ID Id,
				                DP.PRICE Price 
			                FROM
				                NEW_DELIVERY_PRICING DP
				                INNER JOIN NEW_REGION_DELIVERY RD ON DP.REGION_DELIVERY_ID= RD.REGION_DELIVERY_ID
				                INNER JOIN NEW_REGION_DEL_TRANSLATE RDT ON RDT.REGION_DEL_ID = RD.REGION_DELIVERY_ID 
			                WHERE
				                DP.TENANT_ID= SDI.TENANT_ID 
				                AND DP.IS_ACTIVE= 1 
				                AND RD.IS_ACTIVE= 1 
				                AND RDT.LANGUAGE_ID = {langId} FOR json path 
			                ) 
		                ) RegionPrices 
	                FROM
		                NEW_STORE_DELIVERY_INFO SDI 
	                WHERE
		                SDI.TENANT_ID = @pTenantId 
	                AND SDI.IS_ACTIVE = 1 FOR json path 
	                ) Json";
            string json = String.Empty;
            List<RegionDeliveryResponse> response = null;
            try
            {

                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[] {
                        DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@pUserId",SqlDbType.Int,10,ParameterDirection.Input,userId)
                    });

                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                    response = JsonConvert.DeserializeObject<List<RegionDeliveryResponse>>(json);
                }

            }
            catch (Exception ex)
            {
                Log.Warn("Could not Get Region Delivery Data...");
                Log.Error(ex);
                throw;
            }
            return response;
        }

        public int DeleteDeliveryPricing(int id, string tenantId, int userId)  ///duzeltmelisen!
        {
            int returnId = 0;
            try
            {
                using (var con = new DbHandler())
                {
                    returnId = con.ExecStoredProcWithReturnIntValue("RegionDeliveryDelete", new[]
                    {
                        DbHandler.SetParameter("@pDPricingId", SqlDbType.Int, 10, ParameterDirection.Input, id),
                        DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });

                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not RegionDeliveryDelete...");
                Log.Error(ex);
                throw;
            }

            return returnId;
        }

        public List<AllRegions> GetAllRegions(int langId, string tenantId,int userId)
        {
                string sql =
                    $@"SELECT
	                    RD.REGION_DELIVERY_ID Id,
	                    RDT.REGION_NAME Name,
	                    DP.PRICE AS Price 
                    FROM
	                    NEW_DELIVERY_PRICING DP
	                    INNER JOIN NEW_REGION_DELIVERY RD ON RD.REGION_DELIVERY_ID = DP.REGION_DELIVERY_ID
	                    INNER JOIN NEW_REGION_DEL_TRANSLATE RDT ON RDT.REGION_DEL_ID = RD.REGION_DELIVERY_ID 
                    WHERE
	                    DP.TENANT_ID = @tenantId 
	                    AND DP.IS_ACTIVE = 1 
	                    AND DP.DPRICING_TYPE_ID = 2 
	                    AND RDT.LANGUAGE_ID = {langId}
	                    AND EXISTS ( SELECT * FROM dbo.GetEmployeePermission ( @userId, @tenantId, '77' ) )";
            List<AllRegions> regions;

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

                regions = JsonConvert.DeserializeObject<List<AllRegions>>(json);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return regions;
        }

        public int DeleteDelivery(int id, string tenantId, int userId)  
        {
            int returnId = 0;
            try
            {
                using (var con = new DbHandler())
                {
                    returnId = con.ExecStoredProcWithReturnIntValue("DeliveryDelete", new[]
                    {
                        DbHandler.SetParameter("@pDeliveryId", SqlDbType.Int, 10, ParameterDirection.Input, id),
                       DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });

                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not DeliveryDelete...");
                Log.Error(ex);
                throw;
            }

            return returnId;
        
    }

        #endregion


        public IEnumerable<OrderDeliveryByRegionResponse> GetDeliveryPriceByStore(OrderDeliveryByRegion delivery)
        {
            List<OrderDeliveryByRegionResponse> datas = new List<OrderDeliveryByRegionResponse>();
            string sql = @"select * from dbo.GetDeliveryPriceByRegion(@pRequestAsJson)";
            try
            {
                using (var conn = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(delivery);
                    var dr= conn.ExecuteSql(sql,new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson", SqlDbType.NVarChar, -1, ParameterDirection.Input, json),                      
                    });

                    while (dr.Read())
                    {
                        OrderDeliveryByRegionResponse response = new OrderDeliveryByRegionResponse()
                        {
                            TenantId = dr.GetString("TenantId"),
                            StoreName=dr.GetString("StoreName"),
                            DeliveryPricingId=Convert.ToInt32(dr["DPricingId"]),
                            DeliveryPrice=Convert.ToDecimal(dr["Price"])
                        };
                        datas.Add(response);
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Warn("Could not GetDeliveryPriceByStore...");
                Log.Error(ex);
                throw;
            }
            return datas;
        }



    }
}
