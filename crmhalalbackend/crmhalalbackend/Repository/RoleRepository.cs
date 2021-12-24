using CRMHalalBackEnd.DB;
using CRMHalalBackEnd.Models.Permission;
using CRMHalalBackEnd.Models.Role;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Data;
using CRMHalalBackEnd.Models.NewCompany;

namespace CRMHalalBackEnd.Repository
{
    public class RoleRepository
    {

        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string GetRoleByIdForUser(int roleId)
        {
            var sql = "select NAME from NEW_ROLE where ROLE_ID=@pRoleId";
            var role = "";
            try
            {
                using (var conn = new DbHandler())
                {
                    var dr = conn.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@pRoleId",SqlDbType.Int,10,ParameterDirection.Input,roleId)
                    });
                    if (dr.Read())
                    {
                        role = dr.GetString("NAME");
                    }
                }

            }
            catch
            {

            }
            return role;
        }
        public string GetRoleByUserId(int userId)
        {
            var sql = "select ROLE_ID from NEW_USER_ROLE where USER_ID=@pUserId";
            int roleId = 0;
            string role = "";
            try
            {
                using (var conn = new DbHandler())
                {
                    var dr = conn.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@pUserId",SqlDbType.Int,10,ParameterDirection.Input,userId)
                    });
                    if (dr.Read())
                    {
                        roleId = dr.GetInt("ROLE_ID");
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            role = GetRoleByIdForUser(roleId);
            return role;
        }

        public int InsertRole(RoleInsDto role, string tenantId, int userId)
        {
            int roleId = 0;
            try
            {
                using (var con = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(role);
                    roleId = con.ExecStoredProcWithReturnIntValue("[RoleInsert]", new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson", SqlDbType.NVarChar, -1, ParameterDirection.Input, json),
                        DbHandler.SetParameter("@pTenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not RoleInsert...");
                Log.Error(ex);
                throw;
            }

            return roleId;
        }
        public string UpdateRole(RoleUpdDto role, string tenantId, int userId)
        {
            try
            {
                using (var con = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(role);
                    con.ExecuteStoredProcedure("[RoleUpdate]", new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson", SqlDbType.NVarChar, -1, ParameterDirection.Input, json),
                        DbHandler.SetParameter("@pTenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not UpdateRole...");
                Log.Error(ex);
                throw;
            }

            return "Dəyişiklik uğurla tamamlandı. Silinən icazənin xüsusi məlumantları istifadəçilərdən də silindi. Əlavə onunan icazə isə bütün məlumatları özündə əks etdirir. Dəyişdirmək üçün istifadəçilər panelinə keçin.";
        }

        public string DeleteRole(int roleId, string tenantId, int userId)
        {
            try
            {
                using (var con = new DbHandler())
                {
                    con.ExecuteStoredProcedure("[RoleDelete]", new[]
                    {
                        DbHandler.SetParameter("@pRoleId", SqlDbType.Int, 10, ParameterDirection.Input, roleId),
                        DbHandler.SetParameter("@pTenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not DeleteRole...");
                Log.Error(ex);
                throw;
            }

            return "Rolun silinməsi uğurla tamamlandı";
        }
        public RoleResponse GetRoleById(int lang, int roleId, string tenantId)
        {
            string sql = $@"SELECT
	                        (
	                        SELECT
		                        R.ROLE_ID RoleId,
		                        R.NAME RoleName,
		                        (
		                        SELECT
			                        P.PERMISSION_ID PermissionId,
			                        PT.FRONT_NAME PermissionName,
			                        JSON_QUERY (
				                        (
				                        SELECT
					                        PC.PERMISSION_CATEGORY_ID PermissionCategoryId,
					                        PCT.CATEGORY_NAME PermissionCategoryName 
				                        FROM
					                        NEW_PERMISSION_CATEGORY PC
					                        INNER JOIN NEW_PERM_CAT_TRANSLATE PCT ON PCT.PERMISSION_CATEGORY_ID = PC.PERMISSION_CATEGORY_ID 
				                        WHERE
					                        PC.PERMISSION_CATEGORY_ID = P.CATEGORY_ID 
					                        AND PCT.LANGUAGE_ID = ( SELECT SL.LANGUAGE_ID FROM NEW_STORE_LANGUAGE SL WHERE SL.TENANT_ID= @tenantId AND SL.IS_ACTIVE= 1 AND SL.NUMBER= { lang } ) FOR json path,
					                        without_array_wrapper 
				                        ) 
			                        ) PermissionCategory 
		                        FROM
			                        NEW_PERMISSION P
			                        INNER JOIN NEW_PERMISSION_TRANSLATE PT ON PT.PERMISSION_ID = P.PERMISSION_ID 
		                        WHERE
			                        P.PERMISSION_ID IN ( SELECT RP.PERMISSION_ID FROM NEW_ROLE_PERMISSION RP WHERE RP.ROLE_ID = R.ROLE_ID AND RP.IS_ACTIVE = 1 ) 
			                        AND IS_ACTIVE = 1 
			                        AND PT.LANGUAGE_ID = ( SELECT SL.LANGUAGE_ID FROM NEW_STORE_LANGUAGE SL WHERE SL.TENANT_ID= @tenantId AND SL.IS_ACTIVE= 1 AND SL.NUMBER= { lang } ) FOR json path 
		                        ) Permissions 
	                        FROM
		                        NEW_ROLE R 
	                        WHERE
		                        R.TENANT_ID = @tenantId 
		                        AND R.ROLE_ID = @roleId 
		                        AND IS_ACTIVE = 1 FOR json path,
	                        without_array_wrapper 
	                        ) Json";
            RoleResponse roleResponse;
            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@tenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@roleId",SqlDbType.Int,10,ParameterDirection.Input,roleId)
                    });

                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                }
                roleResponse = JsonConvert.DeserializeObject<RoleResponse>(json);
                roleResponse = roleResponse ?? new RoleResponse();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return roleResponse;
        }

        public List<RoleResponse> GetAllRole(int lang, string tenantId, int userId)
        {
            string sql = $@"
                        SELECT
	                        (
	                        SELECT
		                        R.ROLE_ID RoleId,
		                        R.NAME RoleName,
		                        (
		                        SELECT
			                        P.PERMISSION_ID PermissionId,
			                        PT.FRONT_NAME PermissionName,
			                        JSON_QUERY (
				                        (
				                        SELECT
					                        PC.PERMISSION_CATEGORY_ID PermissionCategoryId,
					                        PCT.CATEGORY_NAME PermissionCategoryName 
				                        FROM
					                        NEW_PERMISSION_CATEGORY PC
					                        INNER JOIN NEW_PERM_CAT_TRANSLATE PCT ON PCT.PERMISSION_CATEGORY_ID = PC.PERMISSION_CATEGORY_ID 
				                        WHERE
					                        PC.PERMISSION_CATEGORY_ID = P.CATEGORY_ID 
					                        AND PCT.LANGUAGE_ID = {lang} FOR json path,
					                        without_array_wrapper 
				                        ) 
			                        ) PermissionCategory 
		                        FROM
			                        NEW_PERMISSION P
			                        INNER JOIN NEW_PERMISSION_TRANSLATE PT ON PT.PERMISSION_ID = P.PERMISSION_ID 
		                        WHERE
			                        P.PERMISSION_ID IN ( SELECT RP.PERMISSION_ID FROM NEW_ROLE_PERMISSION RP WHERE RP.ROLE_ID = R.ROLE_ID AND RP.IS_ACTIVE = 1 ) 
			                        AND IS_ACTIVE = 1 
			                        AND PT.LANGUAGE_ID = {lang} FOR json path 
		                        ) Permissions 
	                        FROM
		                        NEW_ROLE R 
	                        WHERE
		                        R.TENANT_ID = @tenantId 
		                        AND R.IS_ACTIVE = 1 FOR json path
	                        ) Json";
            List<RoleResponse> roleResponse;
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
                roleResponse = JsonConvert.DeserializeObject<List<RoleResponse>>(json);
                roleResponse = roleResponse ?? new List<RoleResponse>();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return roleResponse;
        }
        //Send Data about Permission Category and Permision
        public RoleAddSendingData GetCategoryAndPermission(int lang, string tenantId, int userId)
        {

            var sql1 = $@"SELECT (SELECT DISTINCT
	                        PC.PERMISSION_CATEGORY_ID PermissionCategoryId,
	                        PCT.CATEGORY_NAME PermissionCategoryName 
                        FROM
	                        NEW_PERMISSION_CATEGORY PC 
                            INNER JOIN NEW_PERM_CAT_TRANSLATE PCT ON PCT.PERMISSION_CATEGORY_ID = PC.PERMISSION_CATEGORY_ID 
                        WHERE
	                        (
		                        (
			                        exists( SELECT *  FROM NEW_STORE WHERE TENANT_ID = @tenantId ) 
			                        AND PC.PERMISSION_CATEGORY_ID IN ( SELECT CATEGORY_ID FROM NEW_PERMISSION WHERE TYPE_ID IN ( 2,3 ) ) 
		                        ) or 
		                        (
			                        not exists( SELECT *  FROM NEW_STORE WHERE TENANT_ID = @tenantId ) 
			                        AND PC.PERMISSION_CATEGORY_ID IN ( SELECT CATEGORY_ID FROM NEW_PERMISSION WHERE TYPE_ID IN ( 1,3 ) ) 
		                        )
	                        ) and EXISTS(select * from dbo.GetEmployeePermission(@userId,@tenantId, '57'))
                            AND PCT.LANGUAGE_ID = {lang} for json path) JSON";


            var sql2 = $@"SELECT
	                        (
	                        SELECT
		                        PR.PERMISSION_ID PermissionId,
		                        PT.FRONT_NAME PermissionName,
		                        JSON_QUERY (
			                        (
			                        SELECT
				                        PC.PERMISSION_CATEGORY_ID PermissionCategoryId,
				                        PCT.CATEGORY_NAME PermissionCategoryName 
			                        FROM
				                        NEW_PERMISSION_CATEGORY PC
				                        INNER JOIN NEW_PERM_CAT_TRANSLATE PCT ON PCT.PERMISSION_CATEGORY_ID = PC.PERMISSION_CATEGORY_ID 
			                        WHERE
				                        PC.PERMISSION_CATEGORY_ID = PR.CATEGORY_ID 
				                        AND PCT.LANGUAGE_ID = {lang} FOR json path,
				                        without_array_wrapper 
			                        ) 
		                        ) PermissionCategory 
	                        FROM
		                        NEW_PERMISSION PR
		                        INNER JOIN NEW_PERMISSION_TRANSLATE PT ON PT.PERMISSION_ID = PR.PERMISSION_ID 
	                        WHERE
		                        PR.CATEGORY_ID IN ( SELECT PERMISSION_CATEGORY_ID FROM NEW_PERMISSION_CATEGORY ) 
		                        AND (
			                        ( EXISTS ( SELECT * FROM NEW_STORE WHERE TENANT_ID = @tenantId ) AND PR.TYPE_ID IN ( 2, 3 ) ) 
			                        OR ( NOT EXISTS ( SELECT * FROM NEW_STORE WHERE TENANT_ID = @tenantId ) AND PR.TYPE_ID IN ( 1, 3 ) ) 
		                        ) 
		                        AND EXISTS ( SELECT * FROM dbo.GetEmployeePermission ( @userId, @tenantId, '57' ) ) 
	                        AND PT.LANGUAGE_ID = {lang} FOR json path 
	                        ) JSON";
            RoleAddSendingData roleData = new RoleAddSendingData();
            List<PermissionCategoryDto> permissionCategories = new List<PermissionCategoryDto>();
            List<PermissionDto> permissions = new List<PermissionDto>();
            string json1 = String.Empty;
            string json2 = String.Empty;
            try
            {
                using (var conn = new DbHandler())
                {
                    var dr1 = conn.ExecuteSql(sql1, new[]
                    {
                        DbHandler.SetParameter("@tenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@userId",SqlDbType.Int,10,ParameterDirection.Input,userId)
                    });
                    if (dr1.Read())
                    {
                        json1 = dr1["JSON"].ToString();

                    }
                    permissionCategories = JsonConvert.DeserializeObject<List<PermissionCategoryDto>>(json1);
                    dr1.Close();
                    var dr2 = conn.ExecuteSql(sql2, new[]
                    {
                        DbHandler.SetParameter("@tenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@userId",SqlDbType.Int,10,ParameterDirection.Input,userId)
                    });
                    if (dr2.Read())
                    {
                        json2 = dr2["JSON"].ToString();
                    }

                    permissions = JsonConvert.DeserializeObject<List<PermissionDto>>(json2);
                    dr2.Close();
                    roleData.PermissionCategories = permissionCategories;
                    roleData.Permissions = permissions;

                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not GetCategoryAndPermission...");
                Log.Error(ex);
                throw;
            }
            return roleData;
        }

        public CompanyRoleAddSendingData GetRoleAndPermission(int lang, string tenantId, int userId)
        {


            CompanyRoleAddSendingData data = new CompanyRoleAddSendingData();

            string json = String.Empty;
            string result = String.Empty;

            try
            {
                using (var conn = new DbHandler())
                {

                    result = conn.ExecStoredProcWithOutputValue("DataForAddEmployee", "@pResult", SqlDbType.NVarChar, -1, new[] {
                    DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                    DbHandler.SetParameter("@pLang",SqlDbType.Int,10,ParameterDirection.Input,lang),
                    DbHandler.SetParameter("@pLogUserId",SqlDbType.Int,10,ParameterDirection.Input,userId)
                    });
                }
                data = JsonConvert.DeserializeObject<CompanyRoleAddSendingData>(result);
            }
            catch (Exception ex)
            {
                Log.Warn("Could not GetRoleAndPermission...");
                Log.Error(ex);
                throw;
            }
            return data;
        }

        public CompanyEmployeeUpdatePackage GetRoleAndPermissionForUpdate(int lang, int employeeId, string tenantId, int userId)
        {


            CompanyEmployeeUpdatePackage data = new CompanyEmployeeUpdatePackage();

            CompanyRoleAddSendingData json1 = new CompanyRoleAddSendingData();
            string result1 = String.Empty;

            EmployeeUpdateSendData json2 = new EmployeeUpdateSendData();
            string result2 = String.Empty;

            try
            {
                using (var conn = new DbHandler())
                {

                    result1 = conn.ExecStoredProcWithOutputValue("DataForAddEmployee", "@pResult", SqlDbType.NVarChar, -1, new[] {
                    DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                    DbHandler.SetParameter("@pLang",SqlDbType.Int,10,ParameterDirection.Input,lang),
                    DbHandler.SetParameter("@pLogUserId",SqlDbType.Int,50,ParameterDirection.Input,userId)
                    });
                }
                using (var conn = new DbHandler())
                {

                    result2 = conn.ExecStoredProcWithOutputValue("DataForUpdateEmployee", "@pResult", SqlDbType.NVarChar, -1, new[] {
                    DbHandler.SetParameter("@pEmployeeId",SqlDbType.Int,50,ParameterDirection.Input,employeeId),
                    DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                    DbHandler.SetParameter("@pLang",SqlDbType.Int,10,ParameterDirection.Input,lang),
                    DbHandler.SetParameter("@pLogUserId",SqlDbType.Int,50,ParameterDirection.Input,userId)
                    });

                }
                json1 = JsonConvert.DeserializeObject<CompanyRoleAddSendingData>(result1);
                json2 = JsonConvert.DeserializeObject<EmployeeUpdateSendData>(result2);


                data.WholeData = json1;
                data.EmployeeData = json2;
            }
            catch (Exception ex)
            {
                Log.Warn("Could not GetRoleAndPermissionForUpdate...");
                Log.Error(ex);
                throw;
            }
            return data;
        }




    }
}