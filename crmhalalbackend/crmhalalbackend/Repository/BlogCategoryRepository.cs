using CRMHalalBackEnd.DB;
using CRMHalalBackEnd.Models.BlogCategory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CRMHalalBackEnd.Repository
{
    public class BlogCategoryRepository
    {
        private static readonly log4net.ILog Log =
           log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public int Insert(BlogCategoryInsert category, int userId, string tenantId)
        {        
            var categId = 0;
            try
            {
                using (var con = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(category);
                    categId = con.ExecStoredProcWithReturnIntValue("[BlogCategoryInsert]", new[]
                    {
                            DbHandler.SetParameter("@pRequestAsJson",SqlDbType.NVarChar,-1,ParameterDirection.Input,json),
                            DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                            DbHandler.SetParameter("@pLogUserId",SqlDbType.Int,10,ParameterDirection.Input,userId)
                        });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not BlogCategoryInsert...");
                Log.Error(ex);
                throw;
            }
            return categId;
        }


        public int Update(BlogCategoryUpd category, int userId, string tenantId)
        {
            var categId=0;
            try
            {
                using (var con = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(category);
                    categId = con.ExecStoredProcWithReturnIntValue("[BlogcategoryUpdate]", new[]
                    {
                            DbHandler.SetParameter("@pRequestAsJson",SqlDbType.NVarChar,-1,ParameterDirection.Input,json),
                            DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                            DbHandler.SetParameter("@pLogUserId",SqlDbType.Int,10,ParameterDirection.Input,userId)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not BlogcategoryUpdate...");
                Log.Error(ex);
                throw;
            }
            return categId;
        }

        public void Delete(int categoryId, int userId, string tenantId)
        {
           
            try
            {
                using (var con = new DbHandler())
                {
                    
                     con.ExecuteStoredProcedure("[BlogCategoryDelete]", new[]
                    {
                           
                            DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                            DbHandler.SetParameter("@pLogUserId",SqlDbType.Int,10,ParameterDirection.Input,userId),
                            DbHandler.SetParameter("@pBlogCategoryId",SqlDbType.Int,10,ParameterDirection.Input,categoryId),
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not BlogCategoryDelete...");
                Log.Error(ex);
                throw;
            }
         
        }




        public IEnumerable<BlogCategoryResponse> GetBlogCategories(string tenantId)
        {
            IEnumerable<BlogCategoryResponse> categories=new List<BlogCategoryResponse>();
            var sql = @"select(
                      select[Name][Name],
                      NAME2 Name2, NAME3 Name3,NAME4 Name4
                      from NEW_BLOG_CATEGORY where TENANT_ID = @pTenantId and IS_ACTIVE = 1 for json path) json";
            try
            {
                using (var con = new DbHandler())
                {
                    
                    var dr= con.ExecuteSql(sql, new[]
                    {                          
                            DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId)
                    });

                    if (dr.Read())
                    {

                        categories = JsonConvert.DeserializeObject<IEnumerable<BlogCategoryResponse>>(dr["json"].ToString());

                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not GetBlogCategories...");
                Log.Error(ex);
                throw;
            }
            return categories;
        }

        public BlogCategoryResponse GetBlogCategory(string tenantId,int categoryId)
        {
            BlogCategoryResponse category = new BlogCategoryResponse();
            var sql = @"select(
                        select BC.[Name] [Name],
                        BC.NAME2 Name2, BC.NAME3 Name3,BC.NAME4 Name4,
                        JSON_QUERY(
                        (select SEO.SEO_ID Id, SEO.SEO_JSON Seo from NEW_SEO SEO where SEO.CATEGORY_ID = BC.CATEGORY_ID
                        AND SEO.IS_ACTIVE=1 for json path, without_array_wrapper)
                        ) Seo
                        from NEW_BLOG_CATEGORY BC where TENANT_ID=@pTenantId and 
                        IS_ACTIVE=1 and CATEGORY_ID=@pCategoryId for json path, without_array_wrapper)json";
            try
            {
                using (var con = new DbHandler())
                {

                    var dr = con.ExecuteSql(sql, new[]
                    {
                            DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                            DbHandler.SetParameter("@pCategoryId",SqlDbType.Int,10,ParameterDirection.Input,categoryId)
                    });

                    if (dr.Read())
                    {

                        category = JsonConvert.DeserializeObject<BlogCategoryResponse>(dr["json"].ToString());

                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not GetBlogCategory...");
                Log.Error(ex);
                throw;
            }
            return category;
        }


    }
}