using System;
using System.Collections.Generic;
using System.Data;
using CRMHalalBackEnd.DB;
using Attribute = CRMHalalBackEnd.Models.Attribute.Attribute;

namespace CRMHalalBackEnd.Repository
{
    public class AttributeRepository
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public List<Attribute> GetAttributesByCategoryId(int categoryId)
        {
            const string sql =
                @"SELECT
	                [NAME],
	                [NAME2],
	                [NAME3],
	                [NAME4] 
                FROM
	                [NEW_VARIATION] 
                WHERE
	                IS_ACTIVE = 1 
	                AND [GROUP_ID] IN (
	                SELECT
		                [GROUP_ID] 
	                FROM
		                [NEW_PRODUCT] 
	                WHERE
		                [PR_CAT_ID] IN ( SELECT [PR_CAT_ID] FROM [NEW_PRODUCT_CATEGORY] WHERE [PR_CAT_ID] = @CATEGORY_ID OR [P_CAT_ID] = @CATEGORY_ID OR [P_CAT_ID] IN ( SELECT [PR_CAT_ID] FROM [NEW_PRODUCT_CATEGORY] WHERE [P_CAT_ID] = @CATEGORY_ID ) ) 
	                GROUP BY
		                [GROUP_ID] 
	                ) 
                GROUP BY
	                [NAME],
	                [NAME2],
	                [NAME3],
	                [NAME4]";
            List<Attribute> attributes = new List<Attribute>();

            try
            {
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@CATEGORY_ID",SqlDbType.Int,10,ParameterDirection.Input,categoryId)
                    });

                    attributes = new List<Attribute>();
                    while (reader.Read())
                    {
                        Attribute attribute = new Attribute()
                        {
                            Name = reader["NAME"].ToString(),
                            Name2 = reader["NAME2"].ToString(),
                            Name3 = reader["NAME3"].ToString(),
                            Name4 = reader["NAME4"].ToString()
                        };
                        attributes.Add(attribute);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
            return attributes;
        }

        public List<string> GetAllValueByAttribute(string attribute)
        {
            const string sql =
                @"SELECT DISTINCT
	                    [VALUE] 
                    FROM
	                NEW_PRODUCT_VARIATION 
                    WHERE
	                VARIATION_ID IN ( SELECT VARIATION_ID FROM NEW_VARIATION WHERE NAME = @ATTRIBUTE )";
            List<string> values;
            try
            {
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@ATTRIBUTE",SqlDbType.VarChar,50,ParameterDirection.Input,attribute)
                    });

                    values = new List<string>();
                    while (reader.Read())
                    {
                        string value = reader["VALUE"].ToString();
                        values.Add(value);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
            return values;
        }
    }
}