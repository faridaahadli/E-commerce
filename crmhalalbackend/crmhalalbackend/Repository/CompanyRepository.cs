using System;
using System.Collections.Generic;
using System.Data;
using CRMHalalBackEnd.DB;
using CRMHalalBackEnd.Models.NewCompany;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CRMHalalBackEnd.Repository
{
    public class CompanyRepository
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public string Insert(CompanyRegDto companyRegDto, string userId)
        {
            string tenantId = String.Empty;
            try
            {
                using (var con = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(companyRegDto);
                    tenantId = con.ExecStoredProcWithOutputValue("[CompanyInsert]", "@pTenantId", SqlDbType.VarChar, 5, new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson", SqlDbType.NVarChar, -1, ParameterDirection.Input, json),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not CompanyInsert...");
                Log.Error(ex);
                throw;
            }
            return tenantId;
        }

        public CompanyDto Update(CompanyRegDto companyRegDto, string tenantId, string userId)
        {
            try
            {
                using (var con = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(companyRegDto);
                    con.ExecuteStoredProcedure("[CompanyUpdate]", new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson", SqlDbType.NVarChar, -1, ParameterDirection.Input, json),
                        DbHandler.SetParameter("@pTenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not CompanyUpdate...");
                Log.Error(ex);
                throw;
            }
            return GetCompanyByTenant(tenantId, userId);
        }

        public CompanyDto GetCompanyByTenant(string tenantId, string userId)
        {
            const string sql =
                @"SELECT
	                (
	                SELECT
		                C.[NAME] [Name],
		                (
		                SELECT
			                CA.CONTACT_ID ContactId,
			                CA.CONTACT_TYPE_ID ContactTypeId,
			                CA.[TEXT] [Text],
			                CA.NOTE Note 
		                FROM
			                NEW_CONTACT CA 
		                WHERE
			                CA.COMPANY_ID = C.COMPANY_ID FOR json path 
		                ) Contacts,
		                (
		                SELECT
			                AD.ADDRESS_ID AddressId,
			                AD.ADDRESS_TYPE_ID AddressTypeId,
			                AD.[ADDRESS] [Address],
			                AD.COUNTRY Country,
			                AD.CITY City,
			                AD.POST_CODE PostCode,
			                AD.LATITUDE Latitude,
			                AD.LONGITUDE Longitude 
		                FROM
			                NEW_ADDRESS AD 
		                WHERE
			                AD.COMPANY_ID = C.COMPANY_ID FOR json path 
		                ) Addresses
	                FROM
		                NEW_TENANT T
		                INNER JOIN NEW_COMPANY C ON C.COMPANY_ID = T.COMPANY_ID 
	                WHERE
		                T.TENANT_ID=@TenantId 
		                AND T.IS_ACTIVE = 1 and exists(select * from dbo.GetEmployeePermission(@UserId, T.TENANT_ID, '61')) FOR json path,
	                without_array_wrapper 
	                ) [Json]";
            CompanyDto companyDto = null;

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
                companyDto = JsonConvert.DeserializeObject<CompanyDto>(json);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return companyDto;
        }


        public List<CompanyCategory> GetCompanyCategory()
        {
            const string sql =
                                    @"SELECT
                        (
                        SELECT
	                        MC.COMPANY_CATEGORY_ID Id,
	                        MC.NAME Name,
	                        (
	                        SELECT
		                        SC.COMPANY_CATEGORY_ID Id,
		                        SC.NAME Name,
		                        ( SELECT COMPANY_CATEGORY_ID Id, NAME Name FROM NEW_COMPANY_CATEGORY SSC WHERE SSC.C_CAT_ID= SC.COMPANY_CATEGORY_ID FOR json path ) SubCategory 
	                        FROM
		                        NEW_COMPANY_CATEGORY SC 
	                        WHERE
		                        SC.C_CAT_ID= MC.COMPANY_CATEGORY_ID FOR json path 
	                        ) SubCategory 
                        FROM
	                        NEW_COMPANY_CATEGORY MC 
                        WHERE
                        MC.C_CAT_ID IS NULL FOR json path 
                        ) Json";
            List<CompanyCategory> categoryDto = null;

            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql);

                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                }

                categoryDto = JsonConvert.DeserializeObject<List<CompanyCategory>>(json);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return categoryDto;
        }


        public List<CompanyMainPageDto> GetAllCompanyByUser(string userId)
        {
            const string sql =
                @"SELECT
	                C.COMPANY_ID,
	                C.NAME,
	                T.TENANT_ID 
                FROM
	                NEW_EMPLOYEE E
	                INNER JOIN NEW_TENANT T ON T.TENANT_ID = E.TENANT_ID
	                INNER JOIN NEW_COMPANY C ON C.COMPANY_ID = T.COMPANY_ID 
                WHERE
	                E.USER_ID = @UserID AND E.IS_ACTIVE=1 AND E.IS_ACCEPTED=1";
            List<CompanyMainPageDto> companyMainPageDto = new List<CompanyMainPageDto>();
            try
            {
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@UserID", SqlDbType.VarChar, 10, ParameterDirection.Input, userId)
                    });

                    while (reader.Read())
                    {
                        CompanyMainPageDto dto = new CompanyMainPageDto()
                        {
                            CompanyId = reader.GetInt("COMPANY_ID"),
                            Name = reader["NAME"].ToString(),
                            TenantId = reader["TENANT_ID"].ToString()
                        };
                        companyMainPageDto.Add(dto);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
            return companyMainPageDto;
        }


        public string CompanyEmployeeInsert(CompanyEmployeeInsDto companyEmployee, int userId, string tenantId)
        {

            try
            {
                using (var con = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(companyEmployee);
                    con.ExecuteStoredProcedure("[CompanyEmployeeInsert]", new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson", SqlDbType.NVarChar, -1, ParameterDirection.Input, json),
                        DbHandler.SetParameter("@pTenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not CompanyEmployeeInsert...");
                Log.Error(ex);
                throw;
            }
            return "okay";
        }
        public string CompanyEmployeeUpdate(CompanyEmployeeUpdateDto companyEmployee, string tenantId, int userId)
        {

            try
            {

                using (var con = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(companyEmployee);
                    con.ExecuteStoredProcedure("[CompanyEmployeeUpdate]", new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson", SqlDbType.NVarChar, -1, ParameterDirection.Input, json),
                        DbHandler.SetParameter("@pTenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 50, ParameterDirection.Input, userId)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not CompanyEmployeeUpdate...");
                Log.Error(ex);
                throw;
            }
            return "okay";
        }

        public string CompanyEmployeeDelete(int employeeId, string tenantId, int userId)
        {

            try
            {
                using (var con = new DbHandler())
                {

                    con.ExecuteStoredProcedure("[CompanyEmployeeDelete]", new[]
                    {
                        DbHandler.SetParameter("@pEmployeeId", SqlDbType.Int, 10, ParameterDirection.Input, employeeId),
                        DbHandler.SetParameter("@pTenantId", SqlDbType.VarChar, 5, ParameterDirection.Input, tenantId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)

                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not CompanyEmployeeUpdate...");
                Log.Error(ex);
                throw;
            }
            return "okay";
        }

        public int GetTotalPage(string tenantId, int perPage, int? roleId)
        {
            int totalPage = 0;
            string sql = $@"select ceiling(cast((select count(EMPLOYEE_ID) from NEW_EMPLOYEE
                          where TENANT_ID=@pTenantId  and IS_ACTIVE=1 and IS_ACCEPTED=1
                          {(roleId != null ? "and EMPLOYEE_ID in (select EMPLOYEE_ID from NEW_EMPLOYEE_ROLE where ROLE_ID=@pRoleId)" : String.Empty)}
                          ) as float)/cast(@pPerPage as float))totalpage";
            try
            {
                using (var conn = new DbHandler())
                {
                    var dr = conn.ExecuteSql(sql, new[]
                     {
                        DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@pPerPage",SqlDbType.Int,50,ParameterDirection.Input,perPage),
                        ((roleId!=null))? DbHandler.SetParameter("@pRoleId",SqlDbType.Int,10,ParameterDirection.Input,roleId)
                        :DbHandler.SetParameter("@pRoleId",SqlDbType.Int,10,ParameterDirection.Input,DBNull.Value)
                    });

                    if (dr.Read())
                    {
                        totalPage = int.Parse(dr["totalpage"].ToString());
                    }


                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not GetAllEmployee...");
                Log.Error(ex);
                throw;
            }
            return totalPage;
        }

        public ICollection<CompanyEmployeeResponse> GetAllEmployee(string tenantId, int currentPage, int perPage, int userId, int? roleId)
        {
            ICollection<CompanyEmployeeResponse> employees;
            var json = String.Empty;
            string sql = $@"select(select emp.EMPLOYEE_ID EmployeeId,emp.IS_ACCEPTED IsAccepted,
                          STUFF(
					                (
					                SELECT 
						                ','+' ' + [NAME] 
					                FROM
						              NEW_ROLE where ROLE_ID in(select ROLE_ID from NEW_EMPLOYEE_ROLE
									  where EMPLOYEE_ID=emp.EMPLOYEE_ID and IS_ACTIVE=1)
					                 FOR XML PATH ( '' ) 
					                ),
					                1,
					                1,'')Roles,
                           nu.USER_GUID UserGuid,nu.FIRST_NAME FirstName,nu.LAST_NAME LastName,
                           nu.BIRTH_DATE BirthDate,nu.EMAIL Email,nu.CODE Code,
                           json_query((select [TEXT] [Text],CONTACT_TYPE_ID ContactTypeId,
                           NOTE Note from NEW_CONTACT where [USER_ID]=nu.[USER_ID] and IS_ACTIVE=1
                           for json path))Contacts
                           from NEW_USER nu
						   inner join NEW_EMPLOYEE emp on emp.USER_ID=nu.USER_ID    
                           where
                           emp.TENANT_ID=@pTenantId and emp.IS_ACTIVE=1 
                           and exists(select * from dbo.GetEmployeePermission(@pUserId,@pTenantId , '73'))
                            {(roleId != null ? "and exists(select * from NEW_EMPLOYEE_ROLE where ROLE_ID = @pRoleId and EMPLOYEE_ID = emp.EMPLOYEE_ID and IS_ACTIVE=1)" : String.Empty)}
						   ORDER BY emp.EMPLOYEE_ID OFFSET (@pCurrentPage-1)*@pPerPage ROWS FETCH NEXT @pPerPage ROWS ONLY for json path)json";
            try
            {
                using (var conn = new DbHandler())
                {
                    var dr = conn.ExecuteSql(sql, new[]
                     {
                        DbHandler.SetParameter("@pTenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@pCurrentPage",SqlDbType.Int,50,ParameterDirection.Input,currentPage),
                        DbHandler.SetParameter("@pPerPage",SqlDbType.Int,50,ParameterDirection.Input,perPage),
                        DbHandler.SetParameter("@pUserId",SqlDbType.Int,50,ParameterDirection.Input,userId),
                        ((roleId!=null))? DbHandler.SetParameter("@pRoleId",SqlDbType.Int,10,ParameterDirection.Input,roleId)
                        :DbHandler.SetParameter("@pRoleId",SqlDbType.Int,10,ParameterDirection.Input,DBNull.Value)
                    });

                    if (dr.Read())
                    {
                        json = dr["json"].ToString();
                    }
                    employees = JsonConvert.DeserializeObject<ICollection<CompanyEmployeeResponse>>(json);

                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not GetAllEmployee...");
                Log.Error(ex);
                throw;
            }
            return employees;
        }



    }
}