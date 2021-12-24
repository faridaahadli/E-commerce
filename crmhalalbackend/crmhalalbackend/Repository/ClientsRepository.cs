using System;
using System.Collections.Generic;
using System.Data;
using Castle.Core.Internal;
using CRMHalalBackEnd.DB;
using CRMHalalBackEnd.Models.Clients;
using Newtonsoft.Json;

namespace CRMHalalBackEnd.Repository
{
    public class ClientsRepository
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public List<ClientResponse> GetAllClient(string tenantId,int userId)
        {

            string sql = $@"SELECT
						(
						SELECT DISTINCT
							T.USER_GUID Guid,
							T.FIRST_NAME Name,
							T.LAST_NAME Surname,
							T.BIRTH_DATE Birthdate,
							T.EMAIL,
							JSON_QUERY (
								(
								SELECT
									A.ADDRESS_ID AddressId,
									A.ADDRESS_TYPE_ID AddressTypeId,
									A.ADDRESS Address,
									A.LATITUDE Latitude,
									A.LONGITUDE Longitude,
									A.COUNTRY Country,
									A.CITY City,
									A.TITLE Title,
									A.POST_CODE PostCode 
								FROM
									NEW_ADDRESS A 
								WHERE
									ADDRESS_ID = T.DEFAULT_SHOPPING_ADDRESS 
									AND IS_ACTIVE = 1 FOR json path,
									without_array_wrapper 
								) 
							) Address,
							JSON_QUERY (
								(
								SELECT
									C.CONTACT_ID ContactId,
									[TEXT] Text,
									C.NOTE Note,
									C.CONTACT_TYPE_ID ContactTypeId 
								FROM
									NEW_CONTACT C 
								WHERE
									C.USER_ID = T.USER_ID 
									AND C.CONTACT_TYPE_ID = 2 
									AND C.IS_ACTIVE= 1 FOR json path,
									without_array_wrapper 
								) 
							) Contact 
						FROM
							(
							SELECT
								U.* 
							FROM
								NEW_USER U 
							WHERE
								U.TENANT_ID = @tenantId
								AND U.IS_ACTIVE= 1 UNION ALL
							SELECT
								U.* 
							FROM
								NEW_USER U
								INNER JOIN NEW_FOLLOWERS F ON F.USER_ID = U.USER_ID 
							WHERE
								F.TENANT_ID= @tenantId
								AND F.IS_ACTIVE= 1 UNION ALL
							SELECT
								U.* 
							FROM
								NEW_USER U
								INNER JOIN NEW_SALES_ORDER SO ON SO.USER_ID = U.USER_ID 
							WHERE
								EXISTS( SELECT * FROM NEW_SALES_ORDER_LINE SOL WHERE SOl.SO_ID = SO.SO_ID AND SOL.TENANT_ID = @tenantId) 
								AND SO.IS_ACTIVE = 1 
							) T 
						WHERE
						EXISTS ( SELECT * FROM dbo.GetEmployeePermission ( @userId, @tenantId, '100' ) ) FOR json path 
						) Json";

			List<ClientResponse> clientList;

            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@tenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@userId",SqlDbType.Int,-1,ParameterDirection.Input,userId)

					});
                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                }

                clientList = JsonConvert.DeserializeObject<List<ClientResponse>>(json);
                clientList = clientList ?? new List<ClientResponse>();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
            return clientList;
        }

        public List<ClientResponse> GetAllClientByData(string tenantId, int userId,string data)
        {
            string sqlRegister = @"SELECT(SELECT 
	                                U.USER_GUID Guid,
	                                U.FIRST_NAME Name,
	                                U.LAST_NAME Surname,
	                                U.BIRTH_DATE Birthdate,
	                                U.EMAIL,
	                                JSON_QUERY (
		                                (
		                                SELECT
			                                A.ADDRESS_ID AddressId,
			                                A.ADDRESS_TYPE_ID AddressTypeId,
			                                A.ADDRESS Address,
			                                A.LATITUDE Latitude,
			                                A.LONGITUDE Longitude,
			                                A.COUNTRY Country,
			                                A.CITY City,
			                                A.TITLE Title,
			                                A.POST_CODE PostCode 
		                                FROM
			                                NEW_ADDRESS A 
		                                WHERE
			                                ADDRESS_ID = U.DEFAULT_SHOPPING_ADDRESS 
			                                AND A.IS_ACTIVE = 1 FOR json path,
			                                without_array_wrapper 
		                                ) 
	                                ) Address,
	                                JSON_QUERY (
		                                (
		                                SELECT
			                                C.CONTACT_ID ContactId,
			                                [TEXT] Text,
			                                C.NOTE Note,
			                                C.CONTACT_TYPE_ID ContactTypeId 
		                                FROM
			                                NEW_CONTACT C 
		                                WHERE
			                                C.USER_ID = U.USER_ID 
			                                AND C.CONTACT_TYPE_ID = 2 
			                                AND C.IS_ACTIVE= 1 FOR json path,
			                                without_array_wrapper 
		                                ) 
	                                ) Contact 
                                FROM
	                                NEW_USER U 
                                WHERE
	                                U.TENANT_ID = @tenantId 
	                                AND U.IS_ACTIVE= 1 
	                                AND EXISTS ( SELECT * FROM dbo.GetEmployeePermission ( @userId, @tenantId, '100' ) ) for json path) Json";

            string sqlFollowers = @"SELECT(SELECT 
	                                    U.USER_GUID Guid,
	                                    U.FIRST_NAME Name,
	                                    U.LAST_NAME Surname,
	                                    U.BIRTH_DATE Birthdate,
	                                    U.EMAIL,
	                                    JSON_QUERY (
		                                    (
		                                    SELECT
			                                    A.ADDRESS_ID AddressId,
			                                    A.ADDRESS_TYPE_ID AddressTypeId,
			                                    A.ADDRESS Address,
			                                    A.LATITUDE Latitude,
			                                    A.LONGITUDE Longitude,
			                                    A.COUNTRY Country,
			                                    A.CITY City,
			                                    A.TITLE Title,
			                                    A.POST_CODE PostCode 
		                                    FROM
			                                    NEW_ADDRESS A 
		                                    WHERE
			                                    ADDRESS_ID = U.DEFAULT_SHOPPING_ADDRESS 
			                                    AND A.IS_ACTIVE = 1 FOR json path,
			                                    without_array_wrapper 
		                                    ) 
	                                    ) Address,
	                                    JSON_QUERY (
		                                    (
		                                    SELECT
			                                    C.CONTACT_ID ContactId,
			                                    [TEXT] Text,
			                                    C.NOTE Note,
			                                    C.CONTACT_TYPE_ID ContactTypeId 
		                                    FROM
			                                    NEW_CONTACT C 
		                                    WHERE
			                                    C.USER_ID = U.USER_ID 
			                                    AND C.CONTACT_TYPE_ID = 2 
			                                    AND C.IS_ACTIVE= 1 FOR json path,
			                                    without_array_wrapper 
		                                    ) 
	                                    ) Contact 
                                    FROM
	                                    NEW_USER U
	                                    inner join NEW_FOLLOWERS F on  F.USER_ID = U.USER_ID
                                    WHERE
	                                    F.TENANT_ID = @tenantId 
	                                    AND U.IS_ACTIVE= 1 
	                                    AND F.IS_ACTIVE = 1
	                                    AND EXISTS ( SELECT * FROM dbo.GetEmployeePermission ( @userId, @tenantId, '100' ) ) for json path)Json";
			string sqlOrder = @"SELECT
								(
								SELECT DISTINCT
									U.USER_GUID Guid,
									U.FIRST_NAME Name,
									U.LAST_NAME Surname,
									U.BIRTH_DATE Birthdate,
									U.EMAIL,
									JSON_QUERY (
										(
										SELECT
											A.ADDRESS_ID AddressId,
											A.ADDRESS_TYPE_ID AddressTypeId,
											A.ADDRESS Address,
											A.LATITUDE Latitude,
											A.LONGITUDE Longitude,
											A.COUNTRY Country,
											A.CITY City,
											A.TITLE Title,
											A.POST_CODE PostCode 
										FROM
											NEW_ADDRESS A 
										WHERE
											ADDRESS_ID = U.DEFAULT_SHOPPING_ADDRESS 
											AND A.IS_ACTIVE = 1 FOR json path,
											without_array_wrapper 
										) 
									) Address,
									JSON_QUERY (
										(
										SELECT
											C.CONTACT_ID ContactId,
											[TEXT] Text,
											C.NOTE Note,
											C.CONTACT_TYPE_ID ContactTypeId 
										FROM
											NEW_CONTACT C 
										WHERE
											C.USER_ID = U.USER_ID 
											AND C.CONTACT_TYPE_ID = 2 
											AND C.IS_ACTIVE= 1 FOR json path,
											without_array_wrapper 
										) 
									) Contact 
								FROM
									NEW_USER U
									INNER JOIN NEW_SALES_ORDER SO ON SO.USER_ID = U.USER_ID 
								WHERE
									EXISTS ( SELECT * FROM NEW_SALES_ORDER_LINE SOL WHERE SOL.SO_ID= SO.SO_ID AND SOL.TENANT_ID = @tenantId ) 
									AND U.IS_ACTIVE= 1 
									AND SO.IS_ACTIVE = 1 
								AND EXISTS ( SELECT * FROM dbo.GetEmployeePermission ( @userId, @tenantId, '100' ) ) FOR json path 
								) Json";


            List<ClientResponse> clientList;

            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(data.ToLower().Equals("follower") ? sqlFollowers : data.ToLower().Equals("order")?sqlOrder:sqlRegister, new[]
                    {
                        DbHandler.SetParameter("@tenantId",SqlDbType.VarChar,5,ParameterDirection.Input,tenantId),
                        DbHandler.SetParameter("@userId",SqlDbType.Int,10,ParameterDirection.Input,userId)

                    });
                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                }

                clientList = JsonConvert.DeserializeObject<List<ClientResponse>>(json);
                clientList = clientList ?? new List<ClientResponse>();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
            return clientList;
        }
    }
}