using System;
using System.Collections.Generic;
using System.Data;
using CRMHalalBackEnd.DB;
using CRMHalalBackEnd.Helpers;
using CRMHalalBackEnd.Models.Category;
using CRMHalalBackEnd.Models.Variation;
using Newtonsoft.Json;

namespace CRMHalalBackEnd.Repository
{
    public class CategoryRepository
    {

        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public int Insert(CategoryDto category, int userId, string tenantId)
        {
            CategoryDto categoryDto;
            var categId = 0;
            try
            {
                using (var con = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(category);
                    categId = con.ExecStoredProcWithReturnIntValue("[CategoryInsert]", new[]
                    {
                            DbHandler.SetParameter("@pRequestAsJson",SqlDbType.NVarChar,-1,ParameterDirection.Input,json),
                            DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                            DbHandler.SetParameter("@pLogUserId",SqlDbType.Int,10,ParameterDirection.Input,userId)
                        });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not CategoryInsert...");
                Log.Error(ex);
                throw;
            }
            return categId;
        }

        public List<AllCategoriesForShop> GetParentCategory(string tenantId, int userId)
        {
            string sql =
                $@"SELECT
	                (
	                SELECT
		                PC.[PR_CAT_ID] Id,
		                PC.[NAME] AS Name,
                        PC.[NAME2] AS Name2,
                        PC.[NAME3] AS Name3,
                        PC.[NAME4] AS Name4,
                        ( dbo.GetPermissionDataForCategory ( @userId, @tenantId, 9, PC.PR_CAT_ID ) ) IsVisible,
		                (
		                SELECT
			                PPC.PR_CAT_ID Id,
			                PPC.NAME Name,
                            PPC.[NAME2] AS Name2,
                            PPC.[NAME3] AS Name3,
                            PPC.[NAME4] AS Name4,
                             ( dbo.GetPermissionDataForCategory ( @userId, @tenantId, 9, PPC.PR_CAT_ID ) ) IsVisible
		                FROM
			                NEW_PRODUCT_CATEGORY PPC 
		                WHERE
			                PPC.P_CAT_ID = PC.PR_CAT_ID 
			                AND PPC.IS_ACTIVE= 1 FOR json path 
		                ) SubCategory 
	                FROM
		                [dbo].[NEW_PRODUCT_CATEGORY] PC 
	                WHERE
		                PC.TENANT_ID = @tenantId 
		                AND IS_ACTIVE = 1 
		                AND P_CAT_ID IS NULL 
	                AND EXISTS ( SELECT * FROM dbo.GetEmployeePermission ( @userId, @tenantId, '8,9' ) ) FOR json path 
	                ) Json";
            List<AllCategoriesForShop> categories = null;

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
                    categories = new List<AllCategoriesForShop>();
                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                        categories = JsonConvert.DeserializeObject<List<AllCategoriesForShop>>(json);
                        categories = categories ?? new List<AllCategoriesForShop>();
                    }
                }
            }


            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
            return categories;
        }

        public CategoryDtoResponse GetCategoryById(int id,int userId,string tenantId)
        {

            string sql = @"SELECT
	                        (
	                        SELECT
		                        [PR_CAT_ID] AS Id,
		                        [P_CAT_ID] AS ParentCatId,
		                        [NAME] AS Name,
                                [NAME2] AS Name2,
                                [NAME3] AS Name3,
                                [NAME4] AS Name4,
                                (Select NAME from NEW_PRODUCT_CATEGORY Where PR_CAT_ID=PC.P_CAT_ID) AS ParentCatName,
                                (Select NAME2 from NEW_PRODUCT_CATEGORY Where PR_CAT_ID=PC.P_CAT_ID) AS ParentCatName2,
                                (Select NAME3 from NEW_PRODUCT_CATEGORY Where PR_CAT_ID=PC.P_CAT_ID) AS ParentCatName3,
                                (Select NAME4 from NEW_PRODUCT_CATEGORY Where PR_CAT_ID=PC.P_CAT_ID) AS ParentCatName4,
	                            ( SELECT NAME FROM NEW_PRODUCT_CATEGORY WHERE PR_CAT_ID = 
                                    ( SELECT P_CAT_ID FROM NEW_PRODUCT_CATEGORY WHERE PR_CAT_ID = PC.P_CAT_ID ) ) AS ParentParentCatName,
                                ( SELECT NAME2 FROM NEW_PRODUCT_CATEGORY WHERE PR_CAT_ID = 
                                                                    ( SELECT P_CAT_ID FROM NEW_PRODUCT_CATEGORY WHERE PR_CAT_ID = PC.P_CAT_ID ) ) AS ParentParentCatName2,
                                ( SELECT NAME3 FROM NEW_PRODUCT_CATEGORY WHERE PR_CAT_ID = 
                                                                    ( SELECT P_CAT_ID FROM NEW_PRODUCT_CATEGORY WHERE PR_CAT_ID = PC.P_CAT_ID ) ) AS ParentParentCatName3,
                                ( SELECT NAME4 FROM NEW_PRODUCT_CATEGORY WHERE PR_CAT_ID = 
                                                                    ( SELECT P_CAT_ID FROM NEW_PRODUCT_CATEGORY WHERE PR_CAT_ID = PC.P_CAT_ID ) ) AS ParentParentCatNam4,
		                        JSON_QUERY((
		                        SELECT 
			                        ICON Id,
			                        ( UF.PATH + UF.FILENAME + UF.EXTENSION ) FilePath 
		                        FROM
			                        NEW_UPLOAD_FILE UF 
		                        WHERE
			                        UF.UPLOAD_FILE_ID = ICON FOR json path,without_array_wrapper
		                        )) AS MenuIconId,
		                        [COLOR] AS Color,
		                        (select dbo.GetPermissionDataForCategory(@userId, @tenantId, 8, PC.PR_CAT_ID)) IsUpdating,
		                        JSON_QUERY ( ( SELECT UPLOAD_FILE_ID Id, PATH + FILENAME + EXTENSION FilePath FROM NEW_UPLOAD_FILE WHERE UPLOAD_FILE_ID = SLIDER FOR json path, without_array_wrapper ) ) AS MenuSliderId,
		                        JSON_QUERY((select SEO.SEO_ID Id, SEO.SEO_JSON Seo from NEW_SEO SEO where SEO.CATEGORY_ID = PC.PR_CAT_ID  AND SEO.IS_ACTIVE = 1 for json path, without_array_wrapper)) Seo
	                        FROM
		                        [dbo].[NEW_PRODUCT_CATEGORY] PC
	                        WHERE
		                        PC.TENANT_ID = @tenantId 
		                        AND PC.IS_ACTIVE = 1 AND PC.PR_CAT_ID = @id
	                        ORDER BY
	                        CREATE_DATE DESC, PR_CAT_ID FOR json path,without_array_wrapper
	                        ) Json";


            CategoryDtoResponse categoryDto = null;
            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@id",SqlDbType.Int,10,ParameterDirection.Input,id),
                        DbHandler.SetParameter("@tenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@userId",SqlDbType.Int,10,ParameterDirection.Input,userId)
                    });

                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                }
                //Nese elave edende evvelce check etmek lazimdir.
                categoryDto = JsonConvert.DeserializeObject<CategoryDtoResponse>(json);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return categoryDto;
        }

        public List<AllCategories> AllCategories(int lang, string tenantId, int userId)
        {
            string sql = $@"SELECT
	                        (
	                        SELECT
		                        [PR_CAT_ID] AS Id,
		                        [NAME{(lang == 1?"":lang.ToString())}] AS Name,
                                (Select NAME{(lang == 1 ? "" : lang.ToString())} from NEW_PRODUCT_CATEGORY Where PR_CAT_ID=PC.P_CAT_ID) AS ParentCatName,
	                            ( SELECT NAME{(lang == 1 ? "" : lang.ToString())} FROM NEW_PRODUCT_CATEGORY WHERE PR_CAT_ID = 
                                    ( SELECT P_CAT_ID FROM NEW_PRODUCT_CATEGORY WHERE PR_CAT_ID = PC.P_CAT_ID ) ) AS ParentParentCatName,
		                        (select dbo.GetPermissionDataForCategory(@userId, @tenantId, 10, PC.PR_CAT_ID)) IsDeleting,
                                (select dbo.GetPermissionDataForCategory(@userId, @tenantId, 6, PC.PR_CAT_ID)) IsShowProduct,
                                (select count(*) from NEW_PRODUCT P where (P.PR_CAT_ID = PC.PR_CAT_ID or P.PR_CAT_ID IN ( SELECT PR_CAT_ID FROM NEW_PRODUCT_CATEGORY WHERE P_CAT_ID = PC.PR_CAT_ID ) or P.PR_CAT_ID IN ( SELECT P_CAT_ID FROM NEW_PRODUCT_CATEGORY WHERE PR_CAT_ID IN ( SELECT PR_CAT_ID FROM NEW_PRODUCT_CATEGORY WHERE P_CAT_ID = PC.PR_CAT_ID ) ) ) and P.IS_ACTIVE = 1 
                                    AND EXISTS(select * from dbo.GetEmployeePermission(@userId,@tenantId, '6'))) ProductCount
	                        FROM
		                        [dbo].[NEW_PRODUCT_CATEGORY] PC
	                        WHERE
		                        TENANT_ID = @tenantId 
		                        AND IS_ACTIVE = 1 
                                AND  EXISTS(select * from dbo.GetEmployeePermission(@userId, @tenantId, '5'))
                                and dbo.GetPermissionDataForCategory(@userId, @tenantId, 5, PC.PR_CAT_ID)=1
	                        ORDER BY
	                        CREATE_DATE DESC, PR_CAT_ID FOR json path 
	                        ) Json";

            List<AllCategories> categoryDto = null;
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
                categoryDto = JsonConvert.DeserializeObject<List<AllCategories>>(json);
                categoryDto = categoryDto ?? new List<AllCategories>();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return categoryDto;
        }

        public List<AllCategoryForSelect> ShopCategory(int lang, string langString, string tenantId)
        {
            string sql =
				$@"SELECT
	                (
	                SELECT
		                PC.[PR_CAT_ID] AS Id,
		                PC.[NAME{(lang == 1?"":lang.ToString())}] AS Name,
		                JSON_QUERY (
			                (
			                SELECT
				                UF.UPLOAD_FILE_ID Id,
				                ( UF.[PATH] + UF.[FILENAME] + UF.[EXTENSION] ) FilePath 
			                FROM
				                NEW_UPLOAD_FILE UF 
			                WHERE
				                UF.[UPLOAD_FILE_ID] = PC.[SLIDER] FOR json path,
				                without_array_wrapper 
			                ) 
		                ) MenuSliderId,
		                PC.COLOR Color,
		                JSON_QUERY (
			                (
			                SELECT
				                UF.UPLOAD_FILE_ID Id,
				                ( UF.[PATH] + UF.[FILENAME] + UF.[EXTENSION] ) FilePath 
			                FROM
				                NEW_UPLOAD_FILE UF 
			                WHERE
				                UF.[UPLOAD_FILE_ID] = PC.[ICON] FOR json path,
				                without_array_wrapper 
			                ) 
		                ) MenuIconId,
                        JSON_QUERY (
			                (
			                SELECT
				                UF.UPLOAD_FILE_ID Id,
				                ( UF.[PATH] + UF.[FILENAME] + UF.[EXTENSION] ) FilePath 
			                FROM
				                NEW_UPLOAD_FILE UF 
			                WHERE
				                UF.[UPLOAD_FILE_ID] = PC.[GRID] FOR json path,
				                without_array_wrapper 
			                ) 
		                ) GridIconId,
		                (
		                SELECT
			                PPC.[PR_CAT_ID] AS Id,
			                PPC.[NAME{(lang == 1 ? "" : lang.ToString())}] AS Name,
			                JSON_QUERY (
				                (
				                SELECT
					                UF.UPLOAD_FILE_ID Id,
					                ( UF.[PATH] + UF.[FILENAME] + UF.[EXTENSION] ) FilePath 
				                FROM
					                NEW_UPLOAD_FILE UF 
				                WHERE
					                UF.[UPLOAD_FILE_ID] = PPC.[SLIDER] FOR json path,
					                without_array_wrapper 
				                ) 
			                ) MenuSliderId,
			                PPC.COLOR Color,
			                JSON_QUERY (
				                (
				                SELECT
					                UF.UPLOAD_FILE_ID Id,
					                ( UF.[PATH] + UF.[FILENAME] + UF.[EXTENSION] ) FilePath 
				                FROM
					                NEW_UPLOAD_FILE UF 
				                WHERE
					                UF.[UPLOAD_FILE_ID] = PPC.[ICON] FOR json path,
					                without_array_wrapper 
				                ) 
			                ) MenuIconId,
			                (
			                SELECT
				                PPPC.[PR_CAT_ID] AS Id,
				                PPPC.[NAME{(lang == 1 ? "" : lang.ToString())}] AS Name,
				                JSON_QUERY (
					                (
					                SELECT
						                UF.UPLOAD_FILE_ID Id,
						                ( UF.[PATH] + UF.[FILENAME] + UF.[EXTENSION] ) FilePath 
					                FROM
						                NEW_UPLOAD_FILE UF 
					                WHERE
						                UF.[UPLOAD_FILE_ID] = PPPC.[SLIDER] FOR json path,
						                without_array_wrapper 
					                ) 
				                ) MenuSliderId,
				                PPPC.COLOR Color,
				                JSON_QUERY (
					                (
					                SELECT
						                UF.UPLOAD_FILE_ID Id,
						                ( UF.[PATH] + UF.[FILENAME] + UF.[EXTENSION] ) FilePath 
					                FROM
						                NEW_UPLOAD_FILE UF 
					                WHERE
						                UF.[UPLOAD_FILE_ID] = PPPC.[ICON] FOR json path,
						                without_array_wrapper 
					                ) 
				                ) MenuIconId
			                FROM
				                NEW_PRODUCT_CATEGORY PPPC 
			                WHERE
				                PPPC.P_CAT_ID = PPC.PR_CAT_ID 
				                AND PPPC.IS_ACTIVE= 1 FOR json path 
			                ) SubCategory
		                FROM
			                NEW_PRODUCT_CATEGORY PPC 
		                WHERE
			                PPC.P_CAT_ID = PC.PR_CAT_ID 
			                AND PPC.IS_ACTIVE= 1 FOR json path 
		                ) SubCategory 
	                FROM
		                [dbo].[NEW_PRODUCT_CATEGORY] PC 
	                WHERE
		                PC.TENANT_ID = @tenantId 
		                AND IS_ACTIVE = 1 
	                AND P_CAT_ID IS NULL FOR json path 
	                ) Json";

            List<AllCategoryForSelect> categoryDto = null;
            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@tenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId)


                    });

                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                }
                categoryDto = JsonConvert.DeserializeObject<List<AllCategoryForSelect>>(json);
                categoryDto = categoryDto ?? new List<AllCategoryForSelect>();
                categoryDto.ForEach(x =>
                {
                    x.Slug = x.Name.UrlFriendly(langString) + "-" + x.Id;
                    x.SubCategory.ForEach(y =>
                    {
                        y.Slug = y.Name.UrlFriendly(langString) + "-" + y.Id;
                        y.SubCategory.ForEach(z =>
                        {
                            z.Slug = z.Name.UrlFriendly(langString) + "-" + z.Id;
                        });
                    });
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return categoryDto;
        }

        public List<CategoryForProductPageDto> GetCategoryTreeShowProduct(int lang, string tenantId, int userId)
        {
            string sql =
                $@"SELECT
	                (
	                SELECT
		                PC.[PR_CAT_ID] AS Id,
		                PC.[NAME{(lang == 1? "" : lang.ToString())}] AS Name,
		                (dbo.GetPermissionDataForCategory( @userId, @tenantId, 13, PC.PR_CAT_ID )) IsVisible,
		                (
		                SELECT
			                PPC.[PR_CAT_ID] AS Id,
			                PPC.[NAME{(lang == 1 ? "" : lang.ToString())}] AS Name,
			                (dbo.GetPermissionDataForCategory ( @userId, @tenantId, 13, PPC.PR_CAT_ID )) IsVisible,
			                (
			                SELECT
				                PPPC.[PR_CAT_ID] AS Id,
				                PPPC.[NAME{(lang == 1 ? "" : lang.ToString())}] AS Name,
				                (dbo.GetPermissionDataForCategory ( @userId, @tenantId, 13, PPPC.PR_CAT_ID )) IsVisible
			                FROM
				                NEW_PRODUCT_CATEGORY PPPC 
			                WHERE
				                PPPC.P_CAT_ID = PPC.PR_CAT_ID 
				                AND PPC.IS_ACTIVE= 1 FOR json path 
			                ) SubCategory 
		                FROM
			                NEW_PRODUCT_CATEGORY PPC 
		                WHERE
			                PPC.P_CAT_ID = PC.PR_CAT_ID 
			                AND PPC.IS_ACTIVE= 1 FOR json path 
		                ) SubCategory 
	                FROM
		                [dbo].[NEW_PRODUCT_CATEGORY] PC 
	                WHERE
		                PC.TENANT_ID = @tenantId 
		                AND IS_ACTIVE = 1 
		                AND EXISTS ( SELECT * FROM dbo.GetEmployeePermission ( @userId, @tenantId, '13' ) ) 
	                AND P_CAT_ID IS NULL FOR json path 
	                ) Json";

            List<CategoryForProductPageDto> categoryDto = null;
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
                categoryDto = JsonConvert.DeserializeObject<List<CategoryForProductPageDto>>(json);
                categoryDto = categoryDto ?? new List<CategoryForProductPageDto>();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return categoryDto;
        }

        public List<CategoryForProductPageDto> GetCategoryTreeInsertProduct(string tenantId, int userId)
        {
            string sql =
                @"SELECT
	                (
	                SELECT
		                PC.[PR_CAT_ID] AS Id,
		                PC.[NAME] AS Name,
                        PC.[NAME2] AS Name2,
                        PC.[NAME3] AS Name3,
                        PC.[NAME4] AS Name4,
		                (dbo.GetPermissionDataForCategory( @userId, @tenantId, 21, PC.PR_CAT_ID )) IsVisible,
		                (
		                SELECT
			                PPC.[PR_CAT_ID] AS Id,
			                PPC.[NAME] AS Name,
                            PPC.[NAME2] AS Name2,
                            PPC.[NAME3] AS Name3,
                            PPC.[NAME4] AS Name4,
			                (dbo.GetPermissionDataForCategory ( @userId, @tenantId, 21, PPC.PR_CAT_ID )) IsVisible,
			                (
			                SELECT
				                PPPC.[PR_CAT_ID] AS Id,
				                PPPC.[NAME] AS Name,
                                PPPC.[NAME2] AS Name2,
                                PPPC.[NAME3] AS Name3,
                                PPPC.[NAME4] AS Name4,
				                (dbo.GetPermissionDataForCategory ( @userId, @tenantId, 21, PPPC.PR_CAT_ID )) IsVisible
			                FROM
				                NEW_PRODUCT_CATEGORY PPPC 
			                WHERE
				                PPPC.P_CAT_ID = PPC.PR_CAT_ID 
				                AND PPC.IS_ACTIVE= 1 FOR json path 
			                ) SubCategory 
		                FROM
			                NEW_PRODUCT_CATEGORY PPC 
		                WHERE
			                PPC.P_CAT_ID = PC.PR_CAT_ID 
			                AND PPC.IS_ACTIVE= 1 FOR json path 
		                ) SubCategory 
	                FROM
		                [dbo].[NEW_PRODUCT_CATEGORY] PC 
	                WHERE
		                PC.TENANT_ID = @tenantId 
		                AND IS_ACTIVE = 1 
		                AND EXISTS ( SELECT * FROM dbo.GetEmployeePermission ( @userId, @tenantId, '21' ) ) 
	                AND P_CAT_ID IS NULL FOR json path 
	                ) Json";

            List<CategoryForProductPageDto> categoryDto = null;
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
                categoryDto = JsonConvert.DeserializeObject<List<CategoryForProductPageDto>>(json);
                categoryDto = categoryDto ?? new List<CategoryForProductPageDto>();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return categoryDto;
        }

        public void UpdateCategory(string tenantId, int userId, CategoryDto category)
        {
            try
            {
                var json = JsonConvert.SerializeObject(category);
                using (var conn = new DbHandler())
                {
                    conn.ExecuteStoredProcedure("CategoryUpdate", new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson",SqlDbType.NVarChar,-1,ParameterDirection.Input,json),
                        DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@pLogUserId",SqlDbType.Int,10,ParameterDirection.Input,userId)
                    });
                }

            }
            catch (Exception ex)
            {
                Log.Warn("Could not CategoryUpdate...");
                Log.Error(ex);
                throw;
            }
        }

        public List<DeleteCategory> ProductCheckForCategory(string tenantId, int categoryId)
        {

            string sql = $@"select (select PRODUCT_ID  from NEW_PRODUCT where PR_CAT_ID=@categoryId AND TENANT_ID=@tenantId and IS_ACTIVE=1 for json path) Json";
            
            List<DeleteCategory> categoryDto = null;
            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {

                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@tenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@categoryId",SqlDbType.Int,5,ParameterDirection.Input,categoryId),

                    });

                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                }
                categoryDto = JsonConvert.DeserializeObject<List<DeleteCategory>>(json);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return categoryDto;
        }

        public int DeleteCategory(int categoryId, int userId, string tenantId)
        {
            int returnStoreId = 0;
            try
            {
                using (var con = new DbHandler())
                {
                    returnStoreId = con.ExecStoredProcWithReturnIntValue("CategoryDelete", new[]
                    {
                        DbHandler.SetParameter("@pCategoryId", SqlDbType.Int, 10, ParameterDirection.Input, categoryId),
                        DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not DeleteCategory...");
                Log.Error(ex);
                throw;
            }

            return returnStoreId;

        }

        public CategorySideBar GetCategorySideBarData(int lang, string langString, string domain, int categoryId)
        {
            string result = "";
            CategorySideBar sideBar = null;
            try
            {

                using (var conn = new DbHandler())
                {

                    result = conn.ExecStoredProcWithOutputValue("[CategoryAttributeFilter]", "@pResult", SqlDbType.NVarChar, -1, new[]
                      {
                          DbHandler.SetParameter("@pDomain", SqlDbType.VarChar, 63, ParameterDirection.Input,domain.Replace('_','.')),
                          DbHandler.SetParameter("@pCategoryId", SqlDbType.NVarChar, -1, ParameterDirection.Input,categoryId),
                          DbHandler.SetParameter("@pNumber", SqlDbType.Int, 10, ParameterDirection.Input,lang)

                    });

                    sideBar = JsonConvert.DeserializeObject<CategorySideBar>(result);
                    sideBar = sideBar ?? new CategorySideBar();
                    sideBar.Slug = sideBar.Name.UrlFriendly(langString) + "-" + sideBar.CategoryId;
                    sideBar.SubCategories.ForEach(c =>
                    {
                        c.Slug = c.Name.UrlFriendly(langString) + "-" + sideBar.CategoryId;
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not Category Filter Get...");
                Log.Error(ex);
                throw;
            }



            return sideBar;
        }

        public List<VariationMainPageDto> LikeProducts(int lang, string langString, string productGuid, int userId)
        {
            string sql =
                $@"SELECT
	                (
	                SELECT TOP(15)
                        P.PRODUCT_ID	ProductId,
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
				                S.NAME Name 
			                FROM
				                NEW_STORE S 
			                WHERE
				                S.TENANT_ID = P.TENANT_ID FOR json path,
				                without_array_wrapper 
			                ) 
		                ) Store,
		                json_query (
			                ( SELECT PC.PR_CAT_ID Id, PC.NAME{(lang == 1 ? "" : lang.ToString())} Name FROM NEW_PRODUCT_CATEGORY PC WHERE PC.PR_CAT_ID = P.PR_CAT_ID FOR json path, without_array_wrapper ) 
		                ) Category,
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
                        P.DESCRIPTION{(lang == 1 ? "" : lang.ToString())} Description,
                        (select AVG(CAST(RATE_SCORE as decimal)) from NEW_PRODUCT_COMMENT PC where PC.GROUP_ID=P.GROUP_ID) Rating,
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
		                P.PR_CAT_ID IN ( SELECT NP.PR_CAT_ID FROM NEW_PRODUCT NP WHERE NP.PRODUCT_GUID = @productGuid ) AND P.STOCK_QUANTITY>0
		                AND P.IS_ACTIVE = 1 
	                AND P.IS_VISIBLE = 1 FOR json path 
	                ) JSON";
            string json = String.Empty;
            List<VariationMainPageDto> product = null;
            try
            {
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[] {

                        DbHandler.SetParameter("@productGuid", SqlDbType.VarChar, 50, ParameterDirection.Input, productGuid),
                         DbHandler.SetParameter("@userId", SqlDbType.Int, 50, ParameterDirection.Input,userId)


                    }
                    );
                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                    product = JsonConvert.DeserializeObject<List<VariationMainPageDto>>(json);
                    product = product ?? new List<VariationMainPageDto>();
                    product.ForEach(action => action.Slug = action.Name.UrlFriendly(langString) + "-" + action.ProductId);



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

    }
}