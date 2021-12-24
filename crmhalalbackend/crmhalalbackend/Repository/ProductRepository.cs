using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Castle.Core.Internal;
using CRMHalalBackEnd.DB;
using CRMHalalBackEnd.Helpers;
using CRMHalalBackEnd.Models.Category;
using CRMHalalBackEnd.Models.File;
using CRMHalalBackEnd.Models.Product;
using CRMHalalBackEnd.Models.ProductUnit;
using CRMHalalBackEnd.Models.Variation;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;

namespace CRMHalalBackEnd.Repository
{
    public class ProductRepository
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void Insert(Product product, string tenantId, int userId)
        {
            try
            {
                using (var con = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(product);
                    con.ExecuteStoredProcedure("[ProductVariationSave]", new[]
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
        }
        public int Update(VariationUpd variationUpd, string tenantId, int userId)
        {
            try
            {
                using (var con = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(variationUpd);
                    con.ExecuteStoredProcedure("[ProductVariationUpdate]", new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson", SqlDbType.NVarChar, -1, ParameterDirection.Input, json),
                        DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not ProductVariationUpdate...");
                Log.Error(ex);
                throw;
            }

            return 0;
        }
        public int UpdateModel(Product model, string tenantId, int userId)
        {
            try
            {
                using (var con = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(model);
                    con.ExecuteStoredProcedure("[ProductModelUpdate]", new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson", SqlDbType.NVarChar, -1, ParameterDirection.Input, json),
                        DbHandler.SetParameter("@pTenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not ProductModelUpdate...");
                Log.Error(ex);
                throw;
            }

            return 0;
        }
        public void UpdateProductShowOnMainPage(string guid, bool isShow, string tenantId, int userId)
        {
            try
            {
                using (var con = new DbHandler())
                {
                    con.ExecuteStoredProcedure("[ProductShowOnMainPageUpdate]", new[]
                    {
                        DbHandler.SetParameter("@pProductGuid", SqlDbType.NVarChar, -1, ParameterDirection.Input, guid),
                        DbHandler.SetParameter("@pShowOnMainPage", SqlDbType.Int, 10, ParameterDirection.Input, isShow),
                        DbHandler.SetParameter("@pTenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not ProductShowOnMainPageUpdate...");
                Log.Error(ex);
                throw;
            }
        }
        public string Delete(string id, string tenantId, int userId)
        {
            string guid = null;
            try
            {
                using (var con = new DbHandler())
                {
                    guid = con.ExecStoredProcWithOutputValue("[ProductVariationDelete]", "@pOut", SqlDbType.NVarChar, 50, new[]
                     {
                        DbHandler.SetParameter("@pProductVariationGuid", SqlDbType.NVarChar, 50, ParameterDirection.Input, id),
                        DbHandler.SetParameter("@pTenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not ProductVariationUpdate...");
                Log.Error(ex);
                throw;
            }

            return guid;
        }
        public void DeleteProductModel(string groupId, string tenantId, int userId)
        {
            try
            {
                using (var con = new DbHandler())
                {
                    con.ExecuteStoredProcedure("[ProductModelDelete]", new[]
                    {
                        DbHandler.SetParameter("@pGroupId", SqlDbType.NVarChar, 50, ParameterDirection.Input, groupId),
                        DbHandler.SetParameter("@pTenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not ProductModelDelete...");
                Log.Error(ex);
                throw;
            }
        }
        public string GetGroupId(int minLength, int maxLength)
        {
            string newGroupId;
            try
            {
                using (var con = new DbHandler())
                {
                    newGroupId = con.ExecStoredProcWithOutputValue("GenerateGroupIdForProduct", "@pRandomString", SqlDbType.VarChar, 200, new[]
                    {
                        DbHandler.SetParameter("@pMinLength", SqlDbType.Int, 10, ParameterDirection.Input,
                            minLength),
                        DbHandler.SetParameter("@pMaxLength", SqlDbType.Int, 10, ParameterDirection.Input,
                            maxLength)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not GenerateGuid...");
                Log.Error(ex);
                throw;
            }
            return newGroupId;
        }
        public List<string> GetAllManufacturer(string tenantId, int userId)
        {
            const string sql =
                @"SELECT DISTINCT MANUFACTURER from NEW_PRODUCT where IS_ACTIVE = 1 AND TENANT_ID=@tenantId
                    AND EXISTS(select * from dbo.GetEmployeePermission(@userId,@tenantId, '13,21'))";

            List<string> manufacturers = new List<string>();
            try
            {
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@tenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@userId",SqlDbType.Int,10,ParameterDirection.Input,userId)
                    });

                    while (reader.Read())
                    {
                        string manufacturer = reader["MANUFACTURER"].ToString();
                        manufacturers.Add(manufacturer);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
            return manufacturers;
        }
        public List<Dictionary<string,dynamic>> GetAllMeasureType(string tenantId)
        {
            const string sql = @"SELECT
	                                PUC.PR_UNIT_ID Id,
	                                PUT.UNIT_CODE UnitCode,
	                                PUT.NAME Name,
	                                L.LANGUAGE_CODE_2 Code 
                                FROM
	                                NEW_PR_UNIT_CODE PUC
	                                INNER JOIN NEW_PR_UNIT_TRANSLATE PUT ON PUT.PR_UNIT_ID = PUC.PR_UNIT_ID
	                                INNER JOIN NEW_LANGUAGES L ON L.LANGUAGE_ID = PUT.LANGUAGE_ID
	                                INNER JOIN NEW_STORE_LANGUAGE SL ON SL.TENANT_ID = @tenantId 
	                                AND SL.LANGUAGE_ID = PUT.LANGUAGE_ID 
                                WHERE
	                                SL.IS_ACTIVE = 1";
            List<Dictionary<string, dynamic>> productUnits = new List<Dictionary<string, dynamic>>();
            List<ProductUnit> productUnitServers = new List<ProductUnit>();
            try
            {
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@tenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId)
                    });

                    while (reader.Read())
                    {
                        ProductUnit productUnitServer = new ProductUnit();
                        productUnitServer.Id = int.Parse(reader["Id"].ToString());
                        productUnitServer.UnitCode = reader["UnitCode"].ToString();
                        productUnitServer.Name = reader["Name"].ToString();
                        productUnitServer.Code = reader["Code"].ToString();
                        productUnitServers.Add(productUnitServer);
                    }

                    var data = productUnitServers.GroupBy(a => a.Id);

                    data.ForEach(a =>
                    {
                        Dictionary<string,dynamic> productUnit = new Dictionary<string, dynamic>();
                        productUnit.Add("Id", a.First().Id);
                        a.ForEach(b =>
                        {
                            dynamic dyn = new { UnitCode = b.UnitCode, Name = b.Name };
                            productUnit.Add(b.Code, dyn);
                        });
                        productUnits.Add(productUnit);
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return productUnits;
        }
        [Obsolete]
        public List<VariationDto> GetAllProductOld(string tenantId, int page, int perSize = 10)
        {
            const string sql =
                @"SELECT
	                PR.PRODUCT_GUID ID,
	                PR.NAME + ' ' + ISNULL(
		                STUFF(
			                    (
			                       SELECT
				                    ' ' + [VALUE] 
			                       FROM
				                   NEW_PRODUCT_VARIATION t1 
			                       WHERE
				                    t1.PRODUCT_ID = PR.PRODUCT_ID 
				                    AND ( SELECT V.[SHOW_IN_NAME] FROM NEW_VARIATION V WHERE V.VARIATION_ID = t1.VARIATION_ID AND V.IS_ACTIVE = 1 ) = 1 AND t1.IS_ACTIVE=1 FOR XML PATH ( '' ) 
			                     ),
			                     1,
			                     0,
			                     '' 
		                     ),
		                '' 
	                    ) AS NAME,
	                PR.CODE SKU,
                    PR.PRICE,PR.DISCOUNTED_PRICE,
	                PR.BARCODE,
	                PR.STOCK_QUANTITY AS STOCK,
	                PR.IS_VISIBLE AS STATUS,
                    PR.SHOW_ON_MAIN,
					PR.SHOW_PRICE
                FROM
	            NEW_PRODUCT PR WHERE IS_ACTIVE=1 AND TENANT_ID=@tenantId
                    ORDER BY PRODUCT_ID DESC OFFSET @PAGE*@PER_SIZE row FETCH NEXT @PER_SIZE row ONLY";

            List<VariationDto> variationDtos = new List<VariationDto>();
            try

            {
                using (var con = new DbHandler())
                {
                    page = page - 1 >= 0 ? page - 1 : throw new Exception("Page length must be greater than 0");
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@PAGE",SqlDbType.Int,10,ParameterDirection.Input,page),
                        DbHandler.SetParameter("@PER_SIZE",SqlDbType.Int,10,ParameterDirection.Input,perSize),
                        DbHandler.SetParameter("@tenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId)
                    });

                    while (reader.Read())
                    {
                        VariationDto variationDto = new VariationDto
                        {
                            Id = reader["ID"].ToString(),
                            Name = reader["NAME"].ToString(),
                            Sku = reader["SKU"].ToString(),
                            Price = Convert.ToDecimal(reader["PRICE"].ToString()),
                            Discount = Convert.ToDecimal(reader["DISCOUNTED_PRICE"].ToString()),
                            Barcode = reader["BARCODE"].ToString(),
                            Stock = reader.GetInt("STOCK"),
                            Status = Convert.ToBoolean(reader["STATUS"].ToString()),
                            ShowOnMain = Convert.ToBoolean(reader["SHOW_ON_MAIN"].ToString()),
                            ShowPrice = Convert.ToBoolean(reader["SHOW_PRICE"].ToString()),

                        };
                        variationDtos.Add(variationDto);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return variationDtos;
        }

        public List<VariationDto> GetAllProduct(int lang, string tenantId, int page, int pageSize, string guid, string brent, int categoryId,
            int minPrice, int maxPrice, string sort, int userId)
        {
            string sql = $@"SELECT
	                        PR.PRODUCT_GUID ID,
	                        PR.NAME{(lang == 1 ? "" : lang.ToString())} + ' ' + ISNULL(
		                        STUFF(
			                        (
			                        SELECT
				                        ' ' + [VALUE{(lang == 1 ? "" : lang.ToString())}] 
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
	                        ) AS NAME,
	                        PR.CODE SKU,
	                        PR.PRICE,
	                        PR.DISCOUNTED_PRICE,
	                        PR.BARCODE,
                            (select dbo.GetPermissionDataForCategory(@userId, @tenantId, 24, PR.PR_CAT_ID)) IsDeleting,
	                        PR.STOCK_QUANTITY AS STOCK,
	                        PR.IS_VISIBLE AS STATUS,
	                        PR.SHOW_ON_MAIN,
	                        PR.SHOW_PRICE,
                            JSON_QUERY (
				                (
				                SELECT
					                PF.[UPLOAD_FILE_IMAGE_ID] Id,
					                UF.PATH + UF.FILENAME + UF.EXTENSION FilePath 
				                FROM
					                NEW_PRODUCT_FILE PF
					                INNER JOIN NEW_UPLOAD_FILE UF ON UF.UPLOAD_FILE_ID = PF.UPLOAD_FILE_IMAGE_ID 
				                WHERE
					                PF.PRODUCT_ID = PR.PRODUCT_ID 
					                AND PF.IS_ACTIVE = 1 
					                AND PF.WEIGHT= 1 FOR json path,
					                without_array_wrapper 
				                ) 
			                ) [Image] 
                        FROM
	                        (
	                        SELECT
		                        * 
	                        FROM
		                        NEW_PRODUCT P 
	                        WHERE
		                        P.IS_ACTIVE = 1
		                        AND P.TENANT_ID = @tenantId
		                        {(categoryId != 0 ? "AND P.PR_CAT_ID IN ( SELECT PR_CAT_ID FROM NEW_PRODUCT_CATEGORY WHERE PR_CAT_ID = @categoryId OR P_CAT_ID = @categoryId OR P_CAT_ID IN ( SELECT PR_CAT_ID FROM NEW_PRODUCT_CATEGORY WHERE P_CAT_ID = @categoryId ) ) " : "")}
                                {(!guid.IsNullOrEmpty() ? "AND P.PRODUCT_GUID = @guid" : "")}
                                {(!brent.IsNullOrEmpty() ? "AND P.MANUFACTURER = @brent" : "")}
		                        AND P.PRICE >= @minPrice
		                        AND P.PRICE <= @maxPrice 
		                        AND P.DISCOUNTED_PRICE= 0
                                AND EXISTS(select * from dbo.GetEmployeePermission(@userId,@tenantId, '13')) 
                                AND (select dbo.GetPermissionDataForCategory(@userId, @tenantId, 13, P.PR_CAT_ID)) = 1 UNION ALL
	                        SELECT
		                        * 
	                        FROM
		                        NEW_PRODUCT P 
	                        WHERE
		                        P.IS_ACTIVE = 1
		                        AND P.TENANT_ID = @tenantId
		                        {(categoryId != 0 ? "AND P.PR_CAT_ID IN ( SELECT PR_CAT_ID FROM NEW_PRODUCT_CATEGORY WHERE PR_CAT_ID = @categoryId OR P_CAT_ID = @categoryId OR P_CAT_ID IN ( SELECT PR_CAT_ID FROM NEW_PRODUCT_CATEGORY WHERE P_CAT_ID = @categoryId ) ) " : "")}
                                {(!guid.IsNullOrEmpty() ? "AND P.PRODUCT_GUID = @guid" : "")}
                                {(!brent.IsNullOrEmpty() ? "AND P.MANUFACTURER = @brent" : "")}
		                        AND P.DISCOUNTED_PRICE >= @minPrice
		                        AND P.DISCOUNTED_PRICE <= @maxPrice
                                AND P.DISCOUNTED_PRICE != 0
                                AND EXISTS(select * from dbo.GetEmployeePermission(@userId,@tenantId, '13'))
                                AND (select dbo.GetPermissionDataForCategory(@userId, @tenantId, 13, P.PR_CAT_ID))=1
	                        ) AS PR 
                        ORDER BY
	                        {(sort.Equals("date_asc") ? "PR.PRODUCT_ID ASC" : sort.Equals("date_desc") ? "PR.PRODUCT_ID DESC" : sort.Equals("price_asc") ? "PR.PRICE ASC" : sort.Equals("price_desc") ? "PR.PRICE DESC" : sort.Equals("name_asc") ? "PR.NAME ASC" : "PR.NAME DESC")} {(page != 0 && pageSize != 0 ? "OFFSET @page*@pageSize row FETCH NEXT @pageSize row ONLY" : "")}";
            List<VariationDto> variationDtos = new List<VariationDto>();
            try
            {
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@tenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@categoryId",SqlDbType.Int,10,ParameterDirection.Input,categoryId),
                        DbHandler.SetParameter("@guid",SqlDbType.VarChar,50,ParameterDirection.Input,guid),
                        DbHandler.SetParameter("@brent",SqlDbType.NVarChar,50,ParameterDirection.Input,brent),
                        DbHandler.SetParameter("@minPrice",SqlDbType.Int,10,ParameterDirection.Input,minPrice),
                        DbHandler.SetParameter("@maxPrice",SqlDbType.Int,10,ParameterDirection.Input,maxPrice),
                        DbHandler.SetParameter("@page",SqlDbType.Int,10,ParameterDirection.Input,page-1),
                        DbHandler.SetParameter("@pageSize",SqlDbType.Int,5,ParameterDirection.Input,pageSize),
                        DbHandler.SetParameter("@userId",SqlDbType.Int,10,ParameterDirection.Input,userId),
                    });

                    while (reader.Read())
                    {
                        VariationDto variationDto = new VariationDto
                        {
                            Id = reader["ID"].ToString(),
                            Name = reader["NAME"].ToString(),
                            Sku = reader["SKU"].ToString(),
                            Price = Convert.ToDecimal(reader["PRICE"].ToString()),
                            Discount = Convert.ToDecimal(reader["DISCOUNTED_PRICE"].ToString()),
                            IsDeleting = Convert.ToBoolean(reader["IsDeleting"].ToString()),
                            Barcode = reader["BARCODE"].ToString(),
                            Stock = reader.GetInt("STOCK"),
                            Status = Convert.ToBoolean(reader["STATUS"].ToString()),
                            ShowOnMain = Convert.ToBoolean(reader["SHOW_ON_MAIN"].ToString()),
                            ShowPrice = Convert.ToBoolean(reader["SHOW_PRICE"].ToString()),
                            Image = JsonConvert.DeserializeObject<FileDto>(reader["Image"].ToString())

                        };
                        variationDtos.Add(variationDto);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return variationDtos;
        }

        public List<VariationDto> GetAllCategoryProduct(int lang, string tenantId, int categoryId, int userId)
        {
            string sql = $@"SELECT
	                        PR.PRODUCT_GUID ID,
	                        PR.NAME{(lang == 1 ? "" : lang.ToString())} + ' ' + ISNULL(
		                        STUFF(
			                        (
			                        SELECT
				                        ' ' + [VALUE{(lang == 1 ? "" : lang.ToString())}] 
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
	                        ) AS NAME,
	                        PR.CODE SKU,
	                        PR.PRICE,
	                        PR.DISCOUNTED_PRICE,
	                        PR.BARCODE,
                            (select dbo.GetPermissionDataForCategory(@userId, @tenantId, 7, PR.PR_CAT_ID)) IsDeleting,
	                        PR.STOCK_QUANTITY AS STOCK,
	                        PR.IS_VISIBLE AS STATUS,
	                        PR.SHOW_ON_MAIN,
	                        PR.SHOW_PRICE 
                        FROM
	                        (
	                        SELECT
		                        * 
	                        FROM
		                        NEW_PRODUCT P 
	                        WHERE
		                        P.IS_ACTIVE = 1
		                        AND P.TENANT_ID = @tenantId
		                        {(categoryId != 0 ? "AND P.PR_CAT_ID IN ( SELECT PR_CAT_ID FROM NEW_PRODUCT_CATEGORY WHERE PR_CAT_ID = @categoryId OR P_CAT_ID = @categoryId OR P_CAT_ID IN ( SELECT PR_CAT_ID FROM NEW_PRODUCT_CATEGORY WHERE P_CAT_ID = @categoryId ) ) " : "")}
		                        AND P.DISCOUNTED_PRICE= 0
                                AND EXISTS(select * from dbo.GetEmployeePermission(@userId,@tenantId, '6')) 
                                AND (select dbo.GetPermissionDataForCategory(@userId, @tenantId, 6, P.PR_CAT_ID)) = 1 UNION ALL
	                        SELECT
		                        * 
	                        FROM
		                        NEW_PRODUCT P 
	                        WHERE
		                        P.IS_ACTIVE = 1
		                        AND P.TENANT_ID = @tenantId
                                AND P.DISCOUNTED_PRICE != 0
		                        {(categoryId != 0 ? "AND P.PR_CAT_ID IN ( SELECT PR_CAT_ID FROM NEW_PRODUCT_CATEGORY WHERE PR_CAT_ID = @categoryId OR P_CAT_ID = @categoryId OR P_CAT_ID IN ( SELECT PR_CAT_ID FROM NEW_PRODUCT_CATEGORY WHERE P_CAT_ID = @categoryId ) ) " : "")}
                                AND EXISTS(select * from dbo.GetEmployeePermission(@userId,@tenantId, '6'))
                                AND (select dbo.GetPermissionDataForCategory(@userId, @tenantId, 6, P.PR_CAT_ID))=1
	                        ) AS PR ";
            List<VariationDto> variationDtos = new List<VariationDto>();
            try
            {
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@tenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@categoryId",SqlDbType.Int,10,ParameterDirection.Input,categoryId),
                        DbHandler.SetParameter("@userId",SqlDbType.Int,10,ParameterDirection.Input,userId),
                    });

                    while (reader.Read())
                    {
                        VariationDto variationDto = new VariationDto
                        {
                            Id = reader["ID"].ToString(),
                            Name = reader["NAME"].ToString(),
                            Sku = reader["SKU"].ToString(),
                            Price = Convert.ToDecimal(reader["PRICE"].ToString()),
                            Discount = Convert.ToDecimal(reader["DISCOUNTED_PRICE"].ToString()),
                            IsDeleting = Convert.ToBoolean(reader["IsDeleting"].ToString()),
                            Barcode = reader["BARCODE"].ToString(),
                            Stock = reader.GetInt("STOCK"),
                            Status = Convert.ToBoolean(reader["STATUS"].ToString()),
                            ShowOnMain = Convert.ToBoolean(reader["SHOW_ON_MAIN"].ToString()),
                            ShowPrice = Convert.ToBoolean(reader["SHOW_PRICE"].ToString())

                        };
                        variationDtos.Add(variationDto);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return variationDtos;
        }

        [Obsolete]
        public int GetAllProductPageSizeOld(int perSize, string tenantId)
        {
            const string sql =
                @"SELECT COUNT(*) AS [COUNT] FROM NEW_PRODUCT WHERE IS_ACTIVE=1 AND TENANT_ID=@tenantId";

            int size = 0;
            try
            {
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@tenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId)
                    });
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
        public int GetAllProductPageSize(string tenantId, int pageSize, string guid, string brent, int categoryId,
            int minPrice, int maxPrice, int userId)
        {
            string sql =
    $@"SELECT count(*) [COUNT] FROM
	                        (
	                        SELECT
		                        * 
	                        FROM
		                        NEW_PRODUCT P 
	                        WHERE
		                        P.IS_ACTIVE = 1
		                        AND P.TENANT_ID = @tenantId
		                        {(categoryId != 0 ? "AND P.PR_CAT_ID IN ( SELECT PR_CAT_ID FROM NEW_PRODUCT_CATEGORY WHERE PR_CAT_ID = @categoryId OR P_CAT_ID = @categoryId OR P_CAT_ID IN ( SELECT PR_CAT_ID FROM NEW_PRODUCT_CATEGORY WHERE P_CAT_ID = @categoryId ) ) " : "")}
                                {(!guid.IsNullOrEmpty() ? "AND P.PRODUCT_GUID = @guid" : "")}
                                {(!brent.IsNullOrEmpty() ? "AND P.MANUFACTURER = @brent" : "")}
		                        AND P.PRICE >= @minPrice
		                        AND P.PRICE <= @maxPrice 
		                        AND P.DISCOUNTED_PRICE= 0
                                AND EXISTS(select * from dbo.GetEmployeePermission(@userId,@tenantId, '13')) 
                                AND (select dbo.GetPermissionDataForCategory(@userId, @tenantId, 13, P.PR_CAT_ID)) = 1 UNION ALL
	                        SELECT
		                        * 
	                        FROM
		                        NEW_PRODUCT P 
	                        WHERE
		                        P.IS_ACTIVE = 1
		                        AND P.TENANT_ID = @tenantId
		                        {(categoryId != 0 ? "AND P.PR_CAT_ID IN ( SELECT PR_CAT_ID FROM NEW_PRODUCT_CATEGORY WHERE PR_CAT_ID = @categoryId OR P_CAT_ID = @categoryId OR P_CAT_ID IN ( SELECT PR_CAT_ID FROM NEW_PRODUCT_CATEGORY WHERE P_CAT_ID = @categoryId ) ) " : "")}
                                {(!guid.IsNullOrEmpty() ? "AND P.PRODUCT_GUID = @guid" : "")}
                                {(!brent.IsNullOrEmpty() ? "AND P.MANUFACTURER = @brent" : "")}
		                        AND P.DISCOUNTED_PRICE >= @minPrice
		                        AND P.DISCOUNTED_PRICE <= @maxPrice
                                AND P.DISCOUNTED_PRICE != 0
                                AND EXISTS(select * from dbo.GetEmployeePermission(@userId,@tenantId, '13'))
                                AND (select dbo.GetPermissionDataForCategory(@userId, @tenantId, 13, P.PR_CAT_ID))=1
	                        ) AS PR";

            int size = 0;
            try
            {
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@tenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@categoryId",SqlDbType.Int,10,ParameterDirection.Input,categoryId),
                        DbHandler.SetParameter("@guid",SqlDbType.VarChar,50,ParameterDirection.Input,guid),
                        DbHandler.SetParameter("@brent",SqlDbType.NVarChar,50,ParameterDirection.Input,brent),
                        DbHandler.SetParameter("@minPrice",SqlDbType.Int,10,ParameterDirection.Input,minPrice),
                        DbHandler.SetParameter("@maxPrice",SqlDbType.Int,10,ParameterDirection.Input,maxPrice),
                        DbHandler.SetParameter("@pageSize",SqlDbType.Int,5,ParameterDirection.Input,pageSize),
                        DbHandler.SetParameter("@userId",SqlDbType.Int,10,ParameterDirection.Input,userId),
                    });
                    int count = 0;
                    if (reader.Read())
                    {
                        count = reader.GetInt("COUNT");
                    }

                    if (count % pageSize == 0)
                    {
                        size = count / pageSize;
                    }
                    else
                    {
                        size = count / pageSize + 1;
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
        public VariationUpdDto GetVariationById(int lang, string id, string tenantId, int userId)
        {
            string sql =
                $@"SELECT
	                (
	                SELECT
		                P.PRODUCT_GUID Id,
		                P.GROUP_ID GroupId,
		                P.NAME{(lang == 1 ? "" : lang.ToString())} + ISNULL(
			                STUFF(
				                (
				                SELECT
					                ' ' + [VALUE{(lang == 1 ? "" : lang.ToString())}] 
				                FROM
					                NEW_PRODUCT_VARIATION t1 
				                WHERE
					                t1.PRODUCT_ID = P.PRODUCT_ID 
					                AND ( SELECT V.[SHOW_IN_NAME] FROM NEW_VARIATION V WHERE V.VARIATION_ID = t1.VARIATION_ID AND V.IS_ACTIVE = 1 ) = 1 AND t1.IS_ACTIVE=1 FOR XML PATH ( '' ) 
				                ),
				                1,
				                0,
				                '' 
			                ),
			                '' 
		                ) Name,
		                P.PRICE Price,
		                P.DISCOUNTED_PRICE Discount,
		                P.BARCODE Barcode,
		                P.CODE Sku,
                        P.DESCRIPTION Description,
                        P.DESCRIPTION2 Description2,
                        P.DESCRIPTION3 Description3,
                        P.DESCRIPTION4 Description4,
                        (select dbo.GetPermissionDataForCategory(@UserId, @pTenantId, 18, P.PR_CAT_ID)) IsUpdating,
                        (select dbo.GetPermissionDataForCategory(@UserId, @pTenantId, 11, P.PR_CAT_ID)) ShowMainProduct,
		                P.STOCK_QUANTITY Stock,
		                (
		                SELECT
			                PV.PRODUCT_VAR_ID Id,
			                PV.[VALUE] [Value] 
		                FROM
			                NEW_PRODUCT_VARIATION PV 
		                WHERE
			                PV.PRODUCT_ID = P.PRODUCT_ID 
			                AND PV.IS_ACTIVE = 1 FOR json path 
		                ) [Attributes],
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
                            ORDER BY
			                    PF.WEIGHT FOR json path
		                ) [Images],
		                JSON_QUERY ( P.IMAGES ) [AllImages],
		                P.SHOW_PRICE [ShowPrice],
		                P.SHOW_ON_MAIN [ShowOnMain]
	                FROM
		                NEW_PRODUCT P
	                WHERE
		                PRODUCT_GUID = @Guid AND TENANT_ID = @pTenantId AND IS_ACTIVE=1 
                        AND  EXISTS(select * from dbo.GetEmployeePermission(@UserId, @pTenantId,'13')) 
                        AND (select dbo.GetPermissionDataForCategory(@UserId, @pTenantId, 13, P.PR_CAT_ID))=1 FOR json path,
	                without_array_wrapper 
	                ) Json";
            VariationUpdDto _variationUpdDto = null;

            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@Guid",SqlDbType.VarChar,100,ParameterDirection.Input,id),
                        DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@UserId",SqlDbType.Int,10,ParameterDirection.Input,userId)
                    });

                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                }
                //Nese elave edende evvelce check etmek lazimdir.
                _variationUpdDto = JsonConvert.DeserializeObject<VariationUpdDto>(json);
                _variationUpdDto = _variationUpdDto ?? new VariationUpdDto();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return _variationUpdDto;
        }
        public ProductDto GetProductModelByGroupId(string groupId, string tenantId, int userId)
        {
            string sql =
                @"SELECT
	                (
	                SELECT DISTINCT
		                P.GROUP_ID GroupId,
		                P.NAME Name,
		P.NAME2 Name2,
		P.NAME3 Name3,
		P.NAME4 Name4,
        (case dbo.CheckProductForOrderAndBasket(P.GROUP_ID) when 1 then P.PRODUCT_UPDATE_TYPES when 0 then 15 end) ProductUpdateTypes,
                        P.VIDEO_URL YoutubeLink,
		                Json_Query (
			                (
			                SELECT
				                PC.PR_CAT_ID Id,
				                PC.NAME Name,
                PC.NAME2 Name2,
				PC.NAME3 Name3,
				PC.NAME4 Name4 
			                FROM
				                NEW_PRODUCT_CATEGORY PC 
			                WHERE
				                PC.PR_CAT_ID = P.PR_CAT_ID 
				                AND PC.IS_ACTIVE= 1 FOR json path,
				                without_array_wrapper 
			                ) 
		                ) Category,
		                P.MANUFACTURER Manufacturer,
		                P.UNIT_CODE_ID MeasureType,
		                P.WIDTH Width,
		                P.HEIGHT Height,
                        (select dbo.GetPermissionDataForCategory(@UserId, @pTenantId, 16, P.PR_CAT_ID)) IsUpdating,
                        (select dbo.GetPermissionDataForCategory(@UserId, @pTenantId, 23, P.PR_CAT_ID)) IsDeleting,
		                P.LENGTH Length,
		                P.NET_WEIGHT Weight,
		                (
		                SELECT
			                V.VARIATION_ID Id,
			                V.NAME Name,
            V.NAME2 Name2,
			V.NAME3 Name3,
			V.NAME4 Name4,
            (case dbo.CheckProductForOrderAndBasket(V.GROUP_ID) when 1 then V.VARIATION_UPDATE_TYPES when 0 then 15 end) VariationUpdateTypes,
			                V.SHOW_IN_NAME ShowInName,
			                JSON_QUERY ( V.VALUE ) [Value],
			JSON_QUERY ( V.VALUE2 ) [Value2],
			JSON_QUERY ( V.VALUE3 ) [Value3],
			JSON_QUERY ( V.VALUE4 ) [Value4]
		                FROM
			                NEW_VARIATION V 
		                WHERE
			                V.GROUP_ID = P.GROUP_ID 
			                AND V.IS_ACTIVE= 1 FOR json path 
		                ) Attributes,
		                (
		                SELECT
			                PR.PRODUCT_GUID Id,
                            PR.DISCOUNTED_PRICE Discount,
                            PR.STOCK_QUANTITY StockQuantity,
                            PR.PRICE Price,
                            PR.BARCODE Barcode,
                            PR.CODE Sku,
            PR.[DESCRIPTION] [Description],
            PR.[DESCRIPTION2] [Description2],
			PR.[DESCRIPTION3] [Description3],
			PR.[DESCRIPTION4] [Description4],
			                (SELECT VARIATION_ID as Id , [VALUE] [Value], [VALUE2] [Value2], [VALUE3] [Value3], [VALUE4] [Value4] FROM NEW_PRODUCT_VARIATION t1 WHERE t1.PRODUCT_ID = PR.PRODUCT_ID AND t1.IS_ACTIVE= 1 FOR json PATH ) AS Attributes,
							 (
		                SELECT
			                PF.[UPLOAD_FILE_IMAGE_ID] Id,
			                UF.PATH + UF.FILENAME + UF.EXTENSION FilePath 
		                FROM
			                NEW_PRODUCT_FILE PF
			                INNER JOIN NEW_UPLOAD_FILE UF ON UF.UPLOAD_FILE_ID = PF.UPLOAD_FILE_IMAGE_ID 
		                WHERE
			                PF.PRODUCT_ID = PR.PRODUCT_ID 
			                AND PF.IS_ACTIVE = 1 
                            ORDER BY
			                    PF.WEIGHT FOR json path
		                ) [Images]
		                FROM
			                NEW_PRODUCT PR 
		                WHERE
			                PR.GROUP_ID = P.GROUP_ID 
			                AND PR.IS_ACTIVE= 1 FOR json path 
		                ) Variations,
		                (
		                SELECT
			                UPLOAD_FILE_IMAGE_ID Id,
			                (select PATH+FILENAME+EXTENSION from NEW_UPLOAD_FILE UF where UPLOAD_FILE_ID = UPLOAD_FILE_IMAGE_ID) FilePath
		                FROM
			                NEW_PRODUCT_FILE 
		                WHERE
			                PRODUCT_ID IN ( SELECT PRODUCT_ID FROM NEW_PRODUCT WHERE GROUP_ID = P.GROUP_ID AND IS_ACTIVE = 1 ) 
			                AND IS_ACTIVE = 1 
		                GROUP BY
			                UPLOAD_FILE_IMAGE_ID 
		                HAVING
			                COUNT ( UPLOAD_FILE_IMAGE_ID ) = ( SELECT COUNT ( PRODUCT_ID ) FROM NEW_PRODUCT WHERE GROUP_ID = P.GROUP_ID AND IS_ACTIVE = 1 ) FOR json path
		                ) [DefaultImages],
		                JSON_QUERY(P.IMAGES) AllImages,
		                JSON_QUERY(
			                (select SEO.SEO_ID Id, SEO.SEO_JSON Seo from NEW_SEO SEO where SEO.GROUP_ID = P.GROUP_ID AND SEO.IS_ACTIVE=1 for json path, without_array_wrapper)
		                ) Seo
	                FROM
		                NEW_PRODUCT P 
	                WHERE
		                GROUP_ID = @GroupId AND TENANT_ID = @pTenantId AND P.IS_ACTIVE=1
                        AND  EXISTS(select * from dbo.GetEmployeePermission(@UserId, @pTenantId,'11'))
                        AND (select dbo.GetPermissionDataForCategory(@UserId, @pTenantId, 13, P.PR_CAT_ID)) = 1 FOR json auto,
	                without_array_wrapper 
	                ) Json";

            ProductDto productUpdDto = null;
            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@GroupId",SqlDbType.VarChar,10,ParameterDirection.Input,groupId),
                        DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@UserId",SqlDbType.Int,10,ParameterDirection.Input,userId)
                    });

                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                }
                //Nese elave edende evvelce check etmek lazimdir.
                productUpdDto = JsonConvert.DeserializeObject<ProductDto>(json);
                productUpdDto = productUpdDto ?? new ProductDto();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return productUpdDto;
        }
        public int GetProductCount(string tenantId, int userId)
        {
            //AND EXISTS(SELECT* FROM dbo.GetEmployeePermission (@userId, @tenantId, '13' ) ) 
            //AND(SELECT dbo.GetPermissionDataForCategory(@userId, @tenantId, 13, PR_CAT_ID)) = 1
            string sql =
                @"SELECT
	                (
	                SELECT COUNT
		                ( PRODUCT_ID ) 
	                FROM
		                NEW_PRODUCT 
	                WHERE
		                IS_ACTIVE = 1 
		                AND TENANT_ID = @tenantId 
	                ) ProductCount";
            int ProductCount = 0;
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
                        ProductCount = reader.GetInt("ProductCount");
                    }
                }
            }

            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return ProductCount;
        }

        public ProductShopPageDto GetProductModelByProductGuid(int lang, string stringLang, string domain, int id)
        {
            string sql =
                $@"SELECT
	                (
	                SELECT DISTINCT
		                P.GROUP_ID GroupId,
                        P.VIDEO_URL YoutubeLink,
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
				                S.TENANT_ID = P.TENANT_ID FOR json path,
				                without_array_wrapper 
			                ) 
		                ) Store,
		                P.PR_CAT_ID CategoryId,
		                P.MANUFACTURER Manufacturer,
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
	                    ) MeasureType ,
		                P.WIDTH Width,
		                P.HEIGHT Height,
		                P.LENGTH Length,
		                P.NET_WEIGHT Weight,
		                (
		                SELECT
			                V.VARIATION_ID Id,
			                V.NAME{(lang == 1 ? "" : lang.ToString())} Name,
			                V.SHOW_IN_NAME ShowInName,
			                JSON_QUERY ( V.[VALUE{(lang == 1 ? "" : lang.ToString())}] ) [Value] 
		                FROM
			                NEW_VARIATION V 
		                WHERE
			                V.GROUP_ID = P.GROUP_ID 
			                AND V.IS_ACTIVE = 1 FOR json path 
		                ) Attributes,
		                (
		                SELECT
                            PP.PRODUCT_ID ProductId,
			                PP.PRODUCT_GUID Guid,
			                PP.NAME{(lang == 1 ? "" : lang.ToString())} + ISNULL(
				                STUFF(
					                (
					                SELECT
						                ' ' + [VALUE{(lang == 1 ? "" : lang.ToString())}] 
					                FROM
						                NEW_PRODUCT_VARIATION t1 
					                WHERE
						                t1.PRODUCT_ID = PP.PRODUCT_ID 
						                AND ( SELECT [SHOW_IN_NAME] FROM NEW_VARIATION WHERE VARIATION_ID = t1.VARIATION_ID ) = 1 
						                AND t1.IS_ACTIVE= 1 FOR XML PATH ( '' ) 
					                ),
					                1,
					                0,
					                '' 
				                ),
				                '' 
			                ) Name,
			                PP.PRICE Price,
			                PP.DISCOUNTED_PRICE Discount,
			                PP.CODE Sku,
                            PP.DESCRIPTION{(lang == 1 ? "" : lang.ToString())} Description,
			                PP.BARCODE Barcode,
			                PP.STOCK_QUANTITY Stock,
			                (
			                SELECT
				                PV.VARIATION_ID Id,
                                (SELECT V.NAME{(lang == 1 ? "" : lang.ToString())}  FROM NEW_VARIATION V WHERE V.VARIATION_ID = PV.VARIATION_ID) Name,
				                PV.[VALUE{(lang == 1 ? "" : lang.ToString())}] [Value] 
			                FROM
				                NEW_PRODUCT_VARIATION PV 
			                WHERE
				                PV.PRODUCT_ID = PP.PRODUCT_ID 
				                AND PV.IS_ACTIVE= 1 FOR json path 
			                ) Attributes,
			                (
			                SELECT
				                PF.[UPLOAD_FILE_IMAGE_ID] Id,
				                UF.PATH + UF.FILENAME + UF.EXTENSION FilePath 
			                FROM
				                NEW_PRODUCT_FILE PF
				                INNER JOIN NEW_UPLOAD_FILE UF ON UF.UPLOAD_FILE_ID = PF.UPLOAD_FILE_IMAGE_ID 
			                WHERE
				                PF.PRODUCT_ID = PP.PRODUCT_ID 
				                AND PF.IS_ACTIVE = 1 ORDER BY PF.WEIGHT FOR json path 
			                ) Images 
		                FROM
			                NEW_PRODUCT PP 
		                WHERE
			                PP.GROUP_ID = P.GROUP_ID 
			                AND PP.IS_ACTIVE= 1 
			                AND PP.IS_VISIBLE= 1 FOR json path 
		                ) Variations,
		                JSON_QUERY(
			                (select SEO.SEO_ID Id, SEO.SEO_JSON Seo from NEW_SEO SEO where SEO.GROUP_ID = P.GROUP_ID AND SEO.IS_ACTIVE=1 for json path, without_array_wrapper)
		                ) Seo
	                FROM
		                NEW_PRODUCT P 
	                WHERE
		                P.GROUP_ID = ( SELECT GROUP_ID FROM NEW_PRODUCT WHERE PRODUCT_ID = @id )
                        {((!domain.IsNullOrEmpty() && System.Configuration.ConfigurationManager.AppSettings["Note Home Page"] != domain) ? "AND P.TENANT_ID = (SELECT ST.TENANT_ID from NEW_STORE ST WHERE ST.DOMAIN = @domain)" : "")}
                        AND P.IS_ACTIVE= 1 
		                AND P.IS_VISIBLE= 1 FOR json path,
	                without_array_wrapper 
	                ) Json";
            string categorySql =
                $@"SELECT
	                (
	                SELECT
		                PC.PR_CAT_ID Id,
		                PC.NAME{(lang == 1 ? "" : lang.ToString())} Name
	                FROM
		                NEW_PRODUCT_CATEGORY PC 
	                WHERE
		                PC.PR_CAT_ID = @id 
		                OR PC.PR_CAT_ID = ( SELECT PPC.P_CAT_ID FROM NEW_PRODUCT_CATEGORY PPC WHERE PPC.PR_CAT_ID = @id ) 
	                OR PC.PR_CAT_ID = ( SELECT PPC.P_CAT_ID FROM NEW_PRODUCT_CATEGORY PPC WHERE PPC.PR_CAT_ID = ( SELECT PPPC.P_CAT_ID FROM NEW_PRODUCT_CATEGORY PPPC WHERE PPPC.PR_CAT_ID = @id ) ) ORDER BY PC.P_CAT_ID FOR json path 
	                ) Json";
            string json = String.Empty;
            ProductShopPageDto product = null;
            try
            {
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@id", SqlDbType.Int, 10, ParameterDirection.Input,id),
                        DbHandler.SetParameter("@domain", SqlDbType.VarChar, 63, ParameterDirection.Input,domain)
                    });
                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                    reader.Close();
                    //Nese elave edende evvelce check etmek lazimdir.
                    product = JsonConvert.DeserializeObject<ProductShopPageDto>(json);
                    product = product ?? new ProductShopPageDto();

                    reader = con.ExecuteSql(categorySql, new[]
                    {
                        DbHandler.SetParameter("@id", SqlDbType.Int, 10, ParameterDirection.Input, product.CategoryId)
                    });

                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }

                    product.Categories = JsonConvert.DeserializeObject<List<NewCategory>>(json);

                    product.Variations?.ForEach(action => action.Slug = action.Name.UrlFriendly(stringLang) + "-" + action.ProductId);
                    product.Categories?.ForEach(action => action.Slug = action.Name.UrlFriendly(stringLang) + "-" + action.Id);

                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not GetProductModelByProductGuid...");
                Log.Error(ex);
                throw;
            }
            return product;
        }
        public List<VariationUpdDto> GetAllVariation(string tenantId)
        {
            string sql =
                @"SELECT
	                (
	                SELECT
		                P.PRODUCT_GUID Id,
		                P.GROUP_ID GroupId,
                        P.TENANT_ID TenantId,
		                P.NAME + ISNULL(
			                STUFF(
				                (
				                SELECT
					                ' ' + [VALUE] 
				                FROM
					                NEW_PRODUCT_VARIATION t1 
				                WHERE
					                t1.PRODUCT_ID = P.PRODUCT_ID 
					                AND ( SELECT V.[SHOW_IN_NAME] FROM NEW_VARIATION V WHERE V.VARIATION_ID = t1.VARIATION_ID AND V.IS_ACTIVE = 1 ) = 1 AND t1.IS_ACTIVE=1  FOR XML PATH ( '' ) 
				                ),
				                1,
				                0,
				                '' 
			                ),
			                '' 
		                ) Name,
		                P.PRICE Price,
		                P.DISCOUNTED_PRICE Discount,
		                P.BARCODE Barcode,
		                P.CODE Sku,
		                P.STOCK_QUANTITY Stock,
		                ( SELECT PV.PRODUCT_VAR_ID Id, PV.[VALUE] [Value] FROM NEW_PRODUCT_VARIATION PV WHERE PV.PRODUCT_ID = P.PRODUCT_ID and PV.IS_ACTIVE=1 FOR json path ) [Attributes],
		                (
		                SELECT
			                PF.[UPLOAD_FILE_IMAGE_ID] Id,
			                UF.PATH + UF.FILENAME + UF.EXTENSION FilePath 
		                FROM
			                NEW_PRODUCT_FILE PF
			                LEFT JOIN NEW_UPLOAD_FILE UF ON UF.UPLOAD_FILE_ID = PF.UPLOAD_FILE_IMAGE_ID 
		                WHERE
			                PF.PRODUCT_ID = P.PRODUCT_ID 
			                AND PF.IS_ACTIVE = 1 ORDER BY PF.WEIGHT FOR json path 
		                ) [Images],
		                JSON_QUERY ( P.IMAGES ) [AllImages] 
	                FROM
		                NEW_PRODUCT P 
	                WHERE
		                PRODUCT_GUID = PRD.PRODUCT_GUID  FOR json path,
		                without_array_wrapper 
	                ) Variation
                FROM
	                NEW_PRODUCT PRD WHERE IS_ACTIVE = 1  AND IS_VISIBLE=1 AND TENANT_ID = @tenantId";
            List<VariationUpdDto> productUpdDtoList = new List<VariationUpdDto>();
            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@tenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId)
                    });
                    while (reader.Read())
                    {
                        json = reader["Variation"].ToString();
                        //Nese elave edende evvelce check etmek lazimdir.
                        var variationUpdDto = JsonConvert.DeserializeObject<VariationUpdDto>(json);
                        productUpdDtoList.Add(variationUpdDto);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not GetAllVariation...");
                Log.Error(ex);
                throw;
            }

            return productUpdDtoList;

        }
        public List<VariationMainPageDto> GetAllVariationOnMainPageWithUser(int lang, string stringLang, string tenantId, int userId)
        {
            string sql =
                $@"SELECT
	                (
	                SELECT
                        P.PRODUCT_ID    ProductId,
                        P.PRODUCT_GUID    ProductGuid,
                        P.DESCRIPTION{(lang == 1 ? "" : lang.ToString())} Description,
                        ( SELECT AVG ( CAST ( RATE_SCORE AS DECIMAL ) ) FROM NEW_PRODUCT_COMMENT PC WHERE PC.GROUP_ID= P.GROUP_ID ) Rating,
                            {(userId != 0 ? "(SELECT CASE WHEN P.PRODUCT_ID IN(select PRODUCT_ID from NEW_USER_FAV_PRODUCT where IS_ACTIVE = 1 and USER_ID = @userId) THEN 1 ELSE 0 END AS IsFavorite)IsFavorite ," : "")} 
                        JSON_QUERY (
			                (
			                SELECT
				                S.TENANT_ID TenantId,
					                (
					                SELECT
						                UF.UPLOAD_FILE_ID Id,
						                UF.PATH+ UF.FILENAME+ UF.EXTENSION FilePath 
					                FROM
						                NEW_UPLOAD_FILE UF 
					                WHERE
						                UF.[UPLOAD_FILE_ID] = S.[LOGO_IMG_ID] FOR json path
					                ) LogoImage,
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
		                P.NAME{(lang == 1?"":lang.ToString())} + ISNULL(
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
                        P.STOCK_QUANTITY StockQuantity,
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
		                ) [Image] 
	                FROM
		                NEW_PRODUCT P 
	                WHERE
		                P.IS_ACTIVE= 1 
		                AND P.TENANT_ID= @TenantId
		                AND IS_VISIBLE = 1 
		                AND P.SHOW_ON_MAIN= 1
                        AND P.STOCK_QUANTITY > 0
	                ORDER BY
	                WEIGHT ASC FOR JSON PATH 
	                ) JSON";

            List<VariationMainPageDto> variationMainPages = new List<VariationMainPageDto>();

            try
            {
                string json = string.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@TenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@UserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                    //Nese elave edende evvelce check etmek lazimdir.
                    variationMainPages = JsonConvert.DeserializeObject<List<VariationMainPageDto>>(json);
                    variationMainPages = variationMainPages ?? new List<VariationMainPageDto>();
                    variationMainPages.ForEach(action => action.Slug = action.Name.UrlFriendly(stringLang) + "-" + action.ProductId);
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not getAllVariationOnMainPage...");
                Log.Error(ex);
                throw;
            }

            return variationMainPages;
        }

        public List<VariationMainPageDto> GetAllVariationOnMainPageWithAdmin(int lang, string tenantId, int userId)
        {
            string sql =
                $@"SELECT
	                (
	                SELECT
		                P.PRODUCT_GUID ProductGuid,
		                JSON_QUERY (
			                (
			                SELECT
				                S.TENANT_ID TenantId,
					                (
					                SELECT
						                UF.UPLOAD_FILE_ID Id,
						                UF.PATH+ UF.FILENAME+ UF.EXTENSION FilePath 
					                FROM
						                NEW_UPLOAD_FILE UF 
					                WHERE
						                UF.[UPLOAD_FILE_ID] = S.[LOGO_IMG_ID] FOR json path
					                ) LogoImage,
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
		                P.NAME{(lang == 1 ? "" : lang.ToString())} + ISNULL(
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
		                ) [Image] 
	                FROM
		                NEW_PRODUCT P 
	                WHERE
		                P.IS_ACTIVE= 1 
		                AND P.TENANT_ID= @TenantId
		                AND IS_VISIBLE = 1 
		                AND P.SHOW_ON_MAIN= 1
                        AND  EXISTS(select * from dbo.GetEmployeePermission(@UserId, @TenantId,'29'))
	                ORDER BY
	                WEIGHT ASC FOR JSON PATH 
	                ) JSON";

            List<VariationMainPageDto> variationMainPages = new List<VariationMainPageDto>();

            try
            {
                string json = string.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@TenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@UserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                    //Nese elave edende evvelce check etmek lazimdir.
                    variationMainPages = JsonConvert.DeserializeObject<List<VariationMainPageDto>>(json);
                    variationMainPages = variationMainPages ?? new List<VariationMainPageDto>();
                    variationMainPages.ForEach(action => action.Slug = action.Name.ToLower());
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not getAllVariationOnMainPage...");
                Log.Error(ex);
                throw;
            }

            return variationMainPages;
        }

        public List<VariationMainPageDto> GetAllVariationOnMainPageForShop(string tenantId, int userId)
        {
            string sql =
                $@"SELECT
	                (
	                SELECT
		                P.PRODUCT_GUID ProductGuid,
{(userId != 0 ? "(SELECT CASE WHEN P.PRODUCT_ID IN (select PRODUCT_ID from NEW_USER_FAV_PRODUCT where IS_ACTIVE=1 and USER_ID=@userId) THEN 1 ELSE 0 END AS IsFavorite)IsFavorite ," : " ")} 
                        JSON_QUERY (
			                (
			                SELECT
				                S.TENANT_ID TenantId,
					                (
					                SELECT
						                UF.UPLOAD_FILE_ID Id,
						                UF.PATH+ UF.FILENAME+ UF.EXTENSION FilePath 
					                FROM
						                NEW_UPLOAD_FILE UF 
					                WHERE
						                UF.[UPLOAD_FILE_ID] = S.[LOGO_IMG_ID] FOR json path
					                ) LogoImage,
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
		                P.NAME + ISNULL(
			                STUFF(
				                (
				                SELECT
					                ' ' + [VALUE] 
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
		                ) [Image] 
	                FROM
		                NEW_PRODUCT P 
	                WHERE
		                P.IS_ACTIVE= 1 
		                AND P.TENANT_ID= @TenantId
		                AND IS_VISIBLE = 1 
		                AND P.SHOW_ON_MAIN= 1
	                ORDER BY
	                LAST_SHOW_MAIN_DATE DESC FOR JSON PATH 
	                ) JSON";

            List<VariationMainPageDto> variationMainPages = new List<VariationMainPageDto>();

            try
            {
                string json = string.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@TenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@userId",SqlDbType.Int,10,ParameterDirection.Input,userId)

                    });
                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                    //Nese elave edende evvelce check etmek lazimdir.
                    variationMainPages = JsonConvert.DeserializeObject<List<VariationMainPageDto>>(json);
                    variationMainPages = variationMainPages ?? new List<VariationMainPageDto>();
                    variationMainPages.ForEach(action => action.Slug = action.Name.ToLower());
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not GetAllVariationOnMainPageForShop...");
                Log.Error(ex);
                throw;
            }

            return variationMainPages;
        }
        public List<VariationMainPageDto> SearchAllVariationFilter(int lang, bool onlyVisible, string tenantId = "")
        {
            string sql =
                $@"SELECT
              P.PRODUCT_GUID ProductGuid,
              P.NAME{(lang == 1 ? "" : lang.ToString())} + ISNULL(
               STUFF(
                (
                SELECT
                 ' ' + [VALUE{(lang == 1 ? "" : lang.ToString())}] 
                FROM
                 NEW_PRODUCT_VARIATION t1 
                WHERE
                 t1.PRODUCT_ID = P.PRODUCT_ID 
                 AND ( SELECT V.[SHOW_IN_NAME] FROM NEW_VARIATION V WHERE V.VARIATION_ID = t1.VARIATION_ID AND V.IS_ACTIVE=1 ) = 1 
                 AND t1.IS_ACTIVE= 1 FOR XML PATH ( '' ) 
                ),
                1,
                0,
                '' 
               ),
               '' 
              ) Name 
             FROM
              NEW_PRODUCT P 
             WHERE
              P.IS_ACTIVE = 1 {(!tenantId.IsNullOrEmpty() ? "AND P.TENANT_ID = @TenantId" : "")} {(onlyVisible ? " AND P.IS_VISIBLE = 1" : "")}";
            List<VariationMainPageDto> variationMainPages = new List<VariationMainPageDto>();
            try
            {
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@TenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId)
                    });

                    while (reader.Read())
                    {
                        VariationMainPageDto variationMainPage = new VariationMainPageDto()
                        {
                            ProductGuid = reader["ProductGuid"].ToString(),
                            Name = reader["Name"].ToString()
                        };
                        variationMainPages.Add(variationMainPage);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not SearchAllVariationFilter...");
                Log.Error(ex);
                throw;
            }

            return variationMainPages;
        }
        public ProductComment ProductComment(ProductComment productComment, int userId)
        {

            var commentId = 0;
            try
            {

                using (var con = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(productComment);
                    commentId = con.ExecStoredProcWithReturnIntValue("[ProductCommentInsert]", new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson",SqlDbType.NVarChar,-1,ParameterDirection.Input,json),
                        //DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@pLogUserId",SqlDbType.Int,10,ParameterDirection.Input,userId)
                    });
                }


            }
            catch (Exception ex)
            {
                Log.Warn("Could not ProductCommentInsert...");
                Log.Error(ex);
                throw;
            }
            //return category;
            return productComment;
        }
        public List<CommentResponse> GetProductComment(int userId, string groupId)
        {
            const string sql = @"Select(Select NPC.PR_COMMENT_ID AS Id, 
                                NPC.CREATE_DATE AS CreateDate,NPC.COMMENT AS Comment,NPC.RATE_SCORE AS Star,
                                (Select USER_GUID from NEW_USER Where NPC.[USER_ID]=[USER_ID]) AS UserGuid,NPC.SHOW_NAME AS ShowName, 
                                (Select ([FIRST_NAME]+' '+[LAST_NAME])   from NEW_USER WHERE [USER_ID]=@userId AND NPC.SHOW_NAME='true'  ) UserName
                                from [dbo].[NEW_PRODUCT_COMMENT] NPC  WHERE [USER_ID]=@userId AND GROUP_ID=@groupId Order by [CREATE_DATE] DESC FOR json path) Json ";
            List<CommentResponse> commentDto = null;

            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@userId",SqlDbType.Int,10,ParameterDirection.Input,userId),
                          DbHandler.SetParameter("@groupId",SqlDbType.VarChar,10,ParameterDirection.Input,groupId)
                    });

                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                }
                commentDto = JsonConvert.DeserializeObject<List<CommentResponse>>(json);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return commentDto;


        }
        public FavoriteDto AddFavorite(FavoriteDto favorite, int userId)
        {

            var commentId = 0;
            try
            {

                using (var con = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(favorite);
                    commentId = con.ExecStoredProcWithReturnIntValue("[AddFavorite]", new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson",SqlDbType.NVarChar,-1,ParameterDirection.Input,json),
                        //DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@pLogUserId",SqlDbType.Int,10,ParameterDirection.Input,userId)
                    });
                }

            }
            catch (Exception ex)
            {
                Log.Warn("Could not AddFavorite...");
                Log.Error(ex);
                throw;
            }
            //return category;
            return favorite;
        }
        public void DeleteFavoriteProduct(string productGuid, int userId)
        {


            try
            {
                using (var con = new DbHandler())
                {
                    con.ExecuteStoredProcedure("FavoriteProductDelete", new[]
                     {
                        DbHandler.SetParameter("@pProductGuid", SqlDbType.NVarChar, 50, ParameterDirection.Input,productGuid),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });

                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not DeleteCategoryById...");
                Log.Error(ex);
                throw;
            }
        }
        public void DeleteAllFavoriteProduct(int userId)
        {

            try
            {
                using (var con = new DbHandler())
                {
                    con.ExecuteStoredProcedure("FavoriteAllProductsDelete", new[]
                    {

                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });

                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not FavoriteAllProductsDelete...");
                Log.Error(ex);
                throw;
            }

        }
        public List<FavoriteResponse> GetFavoriteProduct(int lang, string stringLang, int userId)
        {
            string sql = $@"SELECT
	                        (
	                        SELECT
		                        NP.PRODUCT_GUID AS ProductGuid,
		                        NP.PRODUCT_ID AS ProductId,
		                        NP.NAME{(lang == 1 ? "":lang.ToString())} + ISNULL(
			                        STUFF(
				                        (
				                        SELECT
					                        ' ' + [VALUE{(lang == 1 ? "" : lang.ToString())}] 
				                        FROM
					                        NEW_PRODUCT_VARIATION t1 
				                        WHERE
					                        t1.PRODUCT_ID = NP.PRODUCT_ID 
					                        AND ( SELECT V.[SHOW_IN_NAME] FROM NEW_VARIATION V WHERE V.VARIATION_ID = t1.VARIATION_ID AND V.IS_ACTIVE = 1 ) = 1 
					                        AND t1.IS_ACTIVE= 1 FOR XML PATH ( '' ) 
				                        ),
				                        1,
				                        0,
				                        '' 
			                        ),
			                        '' 
		                        ) Name,
		                        json_query (
			                        (
			                        SELECT
				                        PC.PR_CAT_ID Id,
				                        PC.NAME{(lang == 1 ? "" : lang.ToString())} Name 
			                        FROM
				                        NEW_PRODUCT_CATEGORY PC 
			                        WHERE
				                        PC.PR_CAT_ID = NP.PR_CAT_ID FOR json path,
				                        without_array_wrapper 
			                        ) 
		                        ) Category,
		                        NP.PRICE AS Price,
                                NP.DESCRIPTION{(lang == 1 ? "" : lang.ToString())} Description,
                                (select AVG(CAST(RATE_SCORE as decimal)) from NEW_PRODUCT_COMMENT PC where PC.GROUP_ID=NP.GROUP_ID) Rating,
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
			                            PUC.PR_UNIT_ID = NP.UNIT_CODE_ID
			                            AND PUT.LANGUAGE_ID = ( SELECT SL.LANGUAGE_ID FROM NEW_STORE_LANGUAGE SL WHERE SL.IS_ACTIVE = 1 AND SL.TENANT_ID = NP.TENANT_ID AND SL.NUMBER = {lang} ) FOR json path,
			                            without_array_wrapper 
		                            ) 
	                            ) MeasureType,
		                        NP.DISCOUNTED_PRICE AS Discount,
		                        JSON_QUERY (
			                        (
			                        SELECT
				                        PF.[UPLOAD_FILE_IMAGE_ID] Id,
				                        UF.PATH + UF.FILENAME + UF.EXTENSION FilePath 
			                        FROM
				                        NEW_PRODUCT_FILE PF
				                        INNER JOIN NEW_UPLOAD_FILE UF ON UF.UPLOAD_FILE_ID = PF.UPLOAD_FILE_IMAGE_ID 
			                        WHERE
				                        PF.PRODUCT_ID = NP.PRODUCT_ID 
				                        AND PF.IS_ACTIVE = 1 
				                        AND PF.WEIGHT= 1 FOR json path,
				                        without_array_wrapper 
			                        ) 
		                        ) [Image],
		                        JSON_QUERY (
			                        (
			                        SELECT
				                        S.TENANT_ID TenantId,
				                        (
				                        SELECT
					                        UF.UPLOAD_FILE_ID Id,
					                        UF.PATH+ UF.FILENAME+ UF.EXTENSION FilePath 
				                        FROM
					                        NEW_UPLOAD_FILE UF 
				                        WHERE
					                        UF.[UPLOAD_FILE_ID] = S.[LOGO_IMG_ID] FOR json path 
				                        ) LogoImage,
				                        S.STORE_GUID StoreGuid,
				                        S.NAME Name,
				                        S.DOMAIN [Domain] 
			                        FROM
				                        NEW_STORE S 
			                        WHERE
				                        S.TENANT_ID = NP.TENANT_ID FOR json path,
				                        without_array_wrapper 
			                        ) 
		                        ) Store 
	                        FROM
		                        NEW_PRODUCT NP 
	                        WHERE
	                        NP.PRODUCT_ID IN ( SELECT PRODUCT_ID FROM NEW_USER_FAV_PRODUCT WHERE USER_ID = @userId AND IS_ACTIVE = 'true' ) FOR json path 
	                        ) Json";
            List<FavoriteResponse> favoriteDto = null;

            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@userId",SqlDbType.Int,10,ParameterDirection.Input,userId)
                    });

                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                }
                favoriteDto = JsonConvert.DeserializeObject<List<FavoriteResponse>>(json);
                favoriteDto = favoriteDto ?? new List<FavoriteResponse>();

                favoriteDto.ForEach(x =>
                {
                    x.Slug = x.Name.UrlFriendly(stringLang) + "-" + x.ProductId;
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return favoriteDto;
        }
        public List<VariationMainPageDto> SearchAllVariationForShop(int lang, string stringLang, bool isVisible, string tenantId, string data = "", string domain = "")
        {
            string sql =
                $@"SELECT
	                (
	                SELECT
                        SS.ProductId,
		                SS.ProductGuid,
                        SS.Description,
                        SS.Rating,
		                SS.Store,
                        SS.Category,
                        SS.MeasureType,
		                SS.Name,
                        SS.Price,
		                SS.Discount,
		                SS.Image 
	                FROM
		                (
		                SELECT
                            P.PRODUCT_ID ProductId,
			                P.PRODUCT_GUID ProductGuid,
                            P.DESCRIPTION{(lang == 1 ? "" : lang.ToString())} Description,
                            (select AVG(CAST(RATE_SCORE as decimal)) from NEW_PRODUCT_COMMENT PC where PC.GROUP_ID=P.GROUP_ID) Rating,
			                JSON_QUERY (
				                (
				                SELECT
					                S.TENANT_ID TenantId,
					                (
					                SELECT
						                UF.UPLOAD_FILE_ID Id,
						                UF.PATH+ UF.FILENAME+ UF.EXTENSION FilePath 
					                FROM
						                NEW_UPLOAD_FILE UF 
					                WHERE
						                UF.[UPLOAD_FILE_ID] = S.[LOGO_IMG_ID] FOR json path 
					                ) LogoImage,
					                S.STORE_GUID StoreGuid,
					                S.NAME Name,
                                    S.DOMAIN [Domain]
				                FROM
					                NEW_STORE S 
				                WHERE
					                S.TENANT_ID = P.TENANT_ID {((!domain.IsNullOrEmpty() && System.Configuration.ConfigurationManager.AppSettings["Note Home Page"] != domain) ? "AND S.DOMAIN= @domain" : "")} FOR  json path,
					                without_array_wrapper 
				                ) 
			                ) Store,
                                json_query((select PC.PR_CAT_ID Id, PC.NAME Name from NEW_PRODUCT_CATEGORY PC where PC.PR_CAT_ID = P.PR_CAT_ID for json path, without_array_wrapper)) Category,
			                P.NAME{(lang == 1 ? "" : lang.ToString())} + ISNULL(
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
			                ) [Image] 
		                FROM
			                NEW_PRODUCT P 
		                WHERE
			                P.IS_ACTIVE= 1 
                            {(!tenantId.IsNullOrEmpty() ? "AND P.TENANT_ID = @tenantId" : "")}
			                {(isVisible ? "AND P.IS_VISIBLE = @isVisible AND P.STOCK_QUANTITY>0" : "")}
                             
		                ) SS 
	                WHERE
	                SS.Name LIKE '%' +  @data + '%' FOR json path 
	                ) Json";
            List<VariationMainPageDto> variationMainPages = null;
            try
            {
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@tenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@isVisible", SqlDbType.Bit, -1, ParameterDirection.Input, isVisible),
                        DbHandler.SetParameter("@data", SqlDbType.NVarChar, 255, ParameterDirection.Input, data),
                        DbHandler.SetParameter("@domain", SqlDbType.NVarChar, 255, ParameterDirection.Input, domain)

                    });

                    if (reader.Read())
                    {
                        variationMainPages = JsonConvert.DeserializeObject<List<VariationMainPageDto>>(reader["Json"].ToString());
                        variationMainPages = variationMainPages ?? new List<VariationMainPageDto>();
                        variationMainPages.ForEach(x =>
                        {
                            x.Slug = x.Name.UrlFriendly(stringLang) + "-" + x.ProductId;
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not SearchAllVariationForShop...");
                Log.Error(ex);
                throw;
            }


            return variationMainPages;
        }
        public List<VariationMainPageDto> SearchAllVariationForNote(int lang, string stringLang, bool isVisible, string tenantId = "", string data = "", string domain = "")
        {
            string sql =
                $@"
                SELECT(
                    SELECT
	                P.PRODUCT_ID ProductId,
	                P.PRODUCT_GUID ProductGuid,
	                JSON_QUERY (
		                (
		                SELECT
			                S.TENANT_ID TenantId,
			                (
			                SELECT
				                UF.UPLOAD_FILE_ID Id,
				                UF.PATH+ UF.FILENAME+ UF.EXTENSION FilePath 
			                FROM
				                NEW_UPLOAD_FILE UF 
			                WHERE
				                UF.[UPLOAD_FILE_ID] = S.[LOGO_IMG_ID] FOR json path 
			                ) LogoImage,
			                S.STORE_GUID StoreGuid,
			                S.NAME Name,
			                S.DOMAIN [Domain] 
		                FROM
			                NEW_STORE S 
		                WHERE
			                S.TENANT_ID = P.TENANT_ID {((!domain.IsNullOrEmpty() && System.Configuration.ConfigurationManager.AppSettings["Note Home Page"] != domain) ? "AND S.DOMAIN= @domain" : "")} FOR json path,
			                without_array_wrapper 
		                ) 
	                ) Store,
	                json_query (
		                (
		                SELECT
			                PC.PR_CAT_ID Id,
			                (
			                CASE
					                SS.[Index] 
					                WHEN 1 THEN
					                PC.NAME 
					                WHEN 2 THEN
					                PC.NAME2 
					                WHEN 3 THEN
					                PC.NAME3 
					                WHEN 4 THEN
					                PC.NAME4 
				                END 
				                ) Name 
			                FROM
				                NEW_PRODUCT_CATEGORY PC 
			                WHERE
				                PC.PR_CAT_ID = P.PR_CAT_ID FOR json path,
				                without_array_wrapper 
			                ) 
		                ) Category,
		                SS.[Name],
		                SS.[Index],
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
		                ) [Image] 
	                FROM
		                NEW_PRODUCT P CROSS apply dbo.[GetProductNameAndIndex] ( P.PRODUCT_ID, @data) SS 
                WHERE
	                P.IS_ACTIVE= 1
                        {(!tenantId.IsNullOrEmpty() ? "AND P.TENANT_ID = @tenantId" : "")}
			            {(isVisible ? "AND P.IS_VISIBLE = @isVisible AND P.STOCK_QUANTITY>0" : "")} FOR json path 
               	) Json";
            List<VariationMainPageDto> variationMainPages = null;
            try
            {
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@tenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@isVisible", SqlDbType.Bit, -1, ParameterDirection.Input, isVisible),
                        DbHandler.SetParameter("@data", SqlDbType.NVarChar, 255, ParameterDirection.Input, data),
                        DbHandler.SetParameter("@domain", SqlDbType.NVarChar, 255, ParameterDirection.Input, domain)

                    });

                    if (reader.Read())
                    {
                        variationMainPages = JsonConvert.DeserializeObject<List<VariationMainPageDto>>(reader["Json"].ToString());
                        variationMainPages = variationMainPages ?? new List<VariationMainPageDto>();
                        variationMainPages.ForEach(x =>
                        {
                            x.Slug = x.Name.UrlFriendly(stringLang) + "-" + x.ProductId;
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not SearchAllVariationForShop...");
                Log.Error(ex);
                throw;
            }


            return variationMainPages;
        }
        public List<VariationMainPageDto> ProductShopFilter(string categoryId, int minPrice, int maxPrice,
            int differenceAttributeCount, bool isDiscount, List<KeyValuePair<string, string>> attributes)
        {
            StringBuilder sql = new StringBuilder(
                $@"select(
                SELECT
                    distinct P.PRODUCT_GUID ProductGuid,
                    JSON_QUERY (
		                (
                        SELECT
                            S.TENANT_ID TenantId,
                            (
                            SELECT
                                UF.UPLOAD_FILE_ID Id,
                                UF.PATH + UF.FILENAME + UF.EXTENSION FilePath
                            FROM
                                NEW_UPLOAD_FILE UF
                            WHERE
                                UF.[UPLOAD_FILE_ID] = S.[LOGO_IMG_ID] FOR json path
			                ) LogoImage,
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
	                json_query(
                        (SELECT PC.PR_CAT_ID Id, PC.NAME Name FROM NEW_PRODUCT_CATEGORY PC WHERE PC.PR_CAT_ID = P.PR_CAT_ID FOR json path, without_array_wrapper)
	                ) Category,
	                P.NAME + ISNULL(
                        STUFF(
                            (
                            SELECT
                                ' ' + [VALUE]
                            FROM
                                NEW_PRODUCT_VARIATION t1
                            WHERE
                                t1.PRODUCT_ID = P.PRODUCT_ID
                                AND(SELECT V.[SHOW_IN_NAME] FROM NEW_VARIATION V WHERE V.VARIATION_ID = t1.VARIATION_ID AND V.IS_ACTIVE = 1) = 1
                                AND t1.IS_ACTIVE = 1 FOR XML PATH('')
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
                            AND PF.WEIGHT = 1 FOR json path,
                            without_array_wrapper
		                ) 
	                ) [Image]
                            FROM
                    NEW_PRODUCT P
                    INNER JOIN NEW_PRODUCT_VARIATION PV ON PV.PRODUCT_ID = P.PRODUCT_ID
                WHERE
                    P.IS_ACTIVE = 1 AND P.IS_VISIBLE = 1 {(isDiscount ? " AND P.DISCOUNTED_PRICE > 0 " : "")}
                    AND P.TENANT_ID = (select TENANT_ID from NEW_PRODUCT_CATEGORY where PR_CAT_ID = @categoryId) 
	                AND(P.PRICE >= @minPrice AND P.PRICE <= @maxPrice) AND PV.IS_ACTIVE=1
                    AND P.PR_CAT_ID IN(
                    SELECT
                        PC.PR_CAT_ID
                    FROM
                        NEW_PRODUCT_CATEGORY PC
                    WHERE
                        (
                            PC.PR_CAT_ID = @categoryId
                            OR PC.P_CAT_ID = @categoryId
                            OR PC.PR_CAT_ID IN (SELECT PPC.PR_CAT_ID FROM NEW_PRODUCT_CATEGORY PPC WHERE PPC.P_CAT_ID IN (SELECT PPPC.PR_CAT_ID FROM NEW_PRODUCT_CATEGORY PPPC WHERE PPPC.P_CAT_ID = @categoryId) ) 
		                ) AND PC.IS_ACTIVE = 1) ");

            List<SqlParameter> whereList = new List<SqlParameter>();
            whereList.Add(DbHandler.SetParameter("@categoryId", SqlDbType.Int, 10, ParameterDirection.Input, int.Parse(categoryId)));
            whereList.Add(DbHandler.SetParameter("@minPrice", SqlDbType.Int, 10, ParameterDirection.Input, minPrice));
            whereList.Add(DbHandler.SetParameter("@maxPrice", SqlDbType.Int, 10, ParameterDirection.Input, maxPrice));
            if (differenceAttributeCount > 0 && attributes.Count > 0)
            {
                sql.Append(@"AND (
		                P.PRODUCT_ID IN (
		                SELECT
			                PV.PRODUCT_ID 
		                FROM
			                NEW_PRODUCT_VARIATION PV
			                INNER JOIN NEW_VARIATION V ON V.VARIATION_ID = PV.VARIATION_ID 
		                WHERE
			                (");
                int weight = 1;
                foreach (var attribute in attributes)
                {
                    sql.Append($"(V.NAME = @Name{weight} AND PV.[VALUE] = @Value{weight}) OR");
                    weight++;
                }

                weight = 1;
                sql.Append(@")
			                AND V.GROUP_ID = P.GROUP_ID AND PV.IS_ACTIVE=1
		                GROUP BY
			                PV.PRODUCT_ID 
		                HAVING
			                COUNT ( PV.PRODUCT_ID ) = @differenceAttributeCount
		                ) 
	                )");

                foreach (var attribute in attributes)
                {
                    whereList.Add(DbHandler.SetParameter($"@Name{weight}", SqlDbType.NVarChar, 150, ParameterDirection.Input, attribute.Key));
                    whereList.Add(DbHandler.SetParameter($"@Value{ weight}", SqlDbType.NVarChar, 150, ParameterDirection.Input, attribute.Value));
                    weight++;
                }
                whereList.Add(DbHandler.SetParameter("@differenceAttributeCount", SqlDbType.Int, 10, ParameterDirection.Input, differenceAttributeCount));
            }

            sql.Append(" for json path) Json");

            List<VariationMainPageDto> variationCategoryPage = null;
            try
            {
                using (var con = new DbHandler())
                {
                    string lastSql = sql.ToString();
                    if (differenceAttributeCount > 0 && attributes.Count > 0)
                    {
                        int index = lastSql.LastIndexOf("OR", StringComparison.Ordinal);
                        lastSql = lastSql.Remove(index, 2);
                    }
                    var reader = con.ExecuteSql(lastSql, whereList.ToArray());
                    if (reader.Read())
                    {
                        var json = reader["Json"].ToString();
                        variationCategoryPage = JsonConvert.DeserializeObject<List<VariationMainPageDto>>(json);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not ProductShopFilter...");
                Log.Error(ex);
                throw;
            }


            return variationCategoryPage;

        }

        public List<VariationMainPageDto> ProductShopFilterWithProcedure(DataForFilter filterData, int lang,string stringLang, string domain)
        {
            List<VariationMainPageDto> variationCategoryPage = null;
            string result = String.Empty;
            try
            {
                using (var con = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(filterData);
                    result = con.ExecStoredProcWithOutputValue("FilerProductByCategoryIdAndAttributeValue", "@pResult", SqlDbType.NVarChar, -1, new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson", SqlDbType.NVarChar, -1, ParameterDirection.Input, json),
                        DbHandler.SetParameter("@pNumber", SqlDbType.Int, 10, ParameterDirection.Input, lang),
                        DbHandler.SetParameter("@pDomain", SqlDbType.VarChar, 63, ParameterDirection.Input, domain)
                    });
                }

                variationCategoryPage = JsonConvert.DeserializeObject<List<VariationMainPageDto>>(result);
                variationCategoryPage = variationCategoryPage ?? new List<VariationMainPageDto>();
                variationCategoryPage.ForEach(action => action.Slug = action.Name.UrlFriendly(stringLang) + "-" + action.ProductId);
            }
            catch (Exception ex)
            {
                Log.Warn("Could not ProductVariationUpdate...");
                Log.Error(ex);
                throw;
            }

            return variationCategoryPage;
        }


        #region CategoryProductOperation

        public void DeleteCategoryProduct(string tenantId, int userId, params string[] productGuids)
        {
            try
            {
                var json = JsonConvert.SerializeObject(productGuids);
                using (var con = new DbHandler())
                {
                    con.ExecuteStoredProcedure("[CategoryProductDelete]", new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson", SqlDbType.NVarChar, -1, ParameterDirection.Input, json),
                        DbHandler.SetParameter("@pTenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not DeleteCategoryProduct...");
                Log.Error(ex);
                throw;
            }

        }

        #endregion

        public void UpdateProductWeight(IList<string> productGuids, string tenantId, int userId)
        {
            try
            {
                using (var con = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(productGuids);
                    con.ExecuteStoredProcedure("[MainPageProductSort]", new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson", SqlDbType.NVarChar, -1, ParameterDirection.Input, json),
                        DbHandler.SetParameter("@pTenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not UpdateProductWeight...");
                Log.Error(ex);
                throw;
            }

        }
    }
}