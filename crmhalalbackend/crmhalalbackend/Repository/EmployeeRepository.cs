using System;
using System.Collections.Generic;
using System.Data;
using CRMHalalBackEnd.DB;
using CRMHalalBackEnd.Models.Employee;
using CRMHalalBackEnd.Models.NewCompany;
using Newtonsoft.Json;

namespace CRMHalalBackEnd.Repository
{
    public class EmployeeRepository
    {

        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public EmployeeTokenData GetEmployeeData(int userId, string tenantId)
        {
            const string sql =
                @"SELECT
	                (
	                SELECT
		                E.TENANT_ID TenantId,
		                E.USER_ID ActiveUserId,
		                (
		                SELECT
			                NAME 
		                FROM
			                NEW_ROLE 
		                WHERE 
                            ROLE_ID in ( SELECT R.ROLE_ID FROM NEW_EMPLOYEE_ROLE R WHERE R.EMPLOYEE_ID = E.EMPLOYEE_ID AND IS_ACTIVE = 1 ) 
		                    AND ROLE_ID = 2
		                ) Role,
		                STUFF(
			                (
			                SELECT DISTINCT
				                ',' + CAST ( EP.PERMISSION_ID AS VARCHAR ) 
			                FROM
				                NEW_EMPLOYEE_PERMISSION EP 
			                WHERE
				                EP.EMPLOYEE_ID = E.EMPLOYEE_ID 
				                AND EP.IS_ACTIVE = 1 FOR XML PATH ( '' ) 
			                ),
			                1,
			                1,
			                '' 
		                ) Permission,
                        (
		                CASE
				                
				                WHEN EXISTS ( SELECT STORE_ID FROM NEW_STORE S WHERE S.TENANT_ID = @TenantId and IS_ACTIVE=1 ) THEN
				                CONVERT ( BIT, 1 ) ELSE CONVERT ( BIT, 0 ) 
			                END 
			                ) IsStore
	                FROM
		                NEW_EMPLOYEE E 
	                WHERE
		                E.TENANT_ID = @TenantId 
		                AND E.USER_ID = @UserId
		                AND E.IS_ACCEPTED = 1 
		                AND E.IS_ACTIVE= 1 FOR json path,
	                without_array_wrapper 
	                ) Json";
            EmployeeTokenData employeeData = null;

            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@UserId",SqlDbType.Int,-1,ParameterDirection.Input,userId),
                        DbHandler.SetParameter("@TenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId)
                    });

                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                }

                //Nese elave edende evvelce check etmek lazimdir.
                employeeData = JsonConvert.DeserializeObject<EmployeeTokenData>(json);
                employeeData = employeeData ?? new EmployeeTokenData();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return employeeData;
        }

        public int ConfirmEmployee(int userId, int companyId, bool confirm)
        {
            int result = 0;
            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    result = con.ExecStoredProcWithReturnIntValue("[EmployeeRoleConfirm]", new[]
                    {
                        DbHandler.SetParameter("@pLogUserId",SqlDbType.Int,10,ParameterDirection.Input,userId),
                        DbHandler.SetParameter("@pCompanyId",SqlDbType.Int,10,ParameterDirection.Input,companyId),
                        DbHandler.SetParameter("@pConfirm",SqlDbType.Bit,-1,ParameterDirection.Input,confirm),
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return result;
        }

        public List<EmployeeUserData> GetEmployeeEmailForOrder(int orderId)
        {
            const string sql =
                @"select tab.Email,tab.StoreName  from  (SELECT
	                ( SELECT U.EMAIL FROM NEW_USER U WHERE U.[USER_ID] = E.USER_ID ) Email,
                    ( SELECT S.NAME FROM NEW_STORE S WHERE S.TENANT_ID = E.TENANT_ID ) StoreName,
	                E.EMPLOYEE_ID,
	                ( SELECT ER.ROLE_ID FROM NEW_EMPLOYEE_ROLE ER WHERE ER.EMPLOYEE_ID = E.EMPLOYEE_ID AND ER.IS_ACTIVE= 1 ) Role,
	                ( SELECT EP.PERMISSION_ID Perm FROM NEW_EMPLOYEE_PERMISSION EP WHERE EP.EMPLOYEE_ID = E.EMPLOYEE_ID AND EP.IS_ACTIVE = 1 for json path) Permission
                FROM
	                NEW_EMPLOYEE E 
                WHERE
	                E.TENANT_ID IN ( select TENANT_ID from NEW_ORDER_PAYMENT_TYPE where ORDER_ID = @orderId ) 
	                AND E.IS_ACTIVE = 1 
	                AND E.IS_ACCEPTED = 1) tab 
	                where tab.Role = 2 or 107 in (select * from openjson(tab.Permission) with(Perm int))";
            List<EmployeeUserData> employeeEmail = new List<EmployeeUserData>();
            try
            {
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@orderId",SqlDbType.Int,10,ParameterDirection.Input,orderId)
                    });

                    while (reader.Read())
                    {
                        EmployeeUserData userData = new EmployeeUserData()
                        {
                            UserEmail = reader["Email"].ToString(),
                            StoreName = reader["StoreName"].ToString()
                        };
                        employeeEmail.Add(userData);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
            return employeeEmail;
        }
    }
}