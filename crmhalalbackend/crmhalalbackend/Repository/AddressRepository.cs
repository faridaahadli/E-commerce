using System;
using System.Collections.Generic;
using System.Data;
using Castle.Core.Internal;
using CRMHalalBackEnd.DB;
using CRMHalalBackEnd.Models.Address;
using Newtonsoft.Json;

namespace CRMHalalBackEnd.Repository
{
    public class AddressRepository
    {
        private static readonly log4net.ILog Log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public int InsertUserAddress(NewAddress address, bool isDefault, int userId)
        {
            int addressId = 0;
            try
            {
                using (var con = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(address);
                    addressId = con.ExecStoredProcWithReturnIntValue("[UserAddressInsert]", new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson", SqlDbType.NVarChar, -1, ParameterDirection.Input, json),
                        DbHandler.SetParameter("@pIsDefault", SqlDbType.Bit, -1, ParameterDirection.Input, isDefault),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not InsertUserAddress...");
                Log.Error(ex);
                throw;
            }

            return addressId;
        }
        public int UpdateUserAddress(NewAddress address, bool isDefault, int userId)
        {
            int addressId = 0;
            try
            {
                using (var con = new DbHandler())
                {
                    var json = JsonConvert.SerializeObject(address);
                    addressId = con.ExecStoredProcWithReturnIntValue("[UserAddressUpdate]", new[]
                    {
                        DbHandler.SetParameter("@pRequestAsJson", SqlDbType.NVarChar, -1, ParameterDirection.Input, json),
                        DbHandler.SetParameter("@pIsDefault", SqlDbType.Bit, -1, ParameterDirection.Input, isDefault),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not UpdateUserAddress...");
                Log.Error(ex);
                throw;
            }

            return addressId;
        }
        public int DeleteUserAddress(int addressId, int userId)
        {
            try
            {
                using (var con = new DbHandler())
                {
                    addressId = con.ExecStoredProcWithReturnIntValue("[UserAddressDelete]", new[]
                    {
                        DbHandler.SetParameter("@pAddressId", SqlDbType.Int, 10, ParameterDirection.Input, addressId),
                        DbHandler.SetParameter("@pLogUserId", SqlDbType.Int, 10, ParameterDirection.Input, userId)
                    });
                }
            }
            catch (Exception ex)
            {
                Log.Warn("Could not DeleteUserAddress...");
                Log.Error(ex);
                throw;
            }

            return addressId;
        }
        public AddressResponse GetAddressByIdWithoutIsActive(int id, int userId)
        {
            const string sql =
                @"SELECT
	                A.[ADDRESS_ID] AddressId,
	                A.[ADDRESS] Address,
                CASE
		                ( SELECT DEFAULT_SHOPPING_ADDRESS FROM NEW_USER WHERE USER_ID = @UserId ) 
		                WHEN A.ADDRESS_ID THEN
		                CONVERT ( BIT, 1 ) ELSE CONVERT ( BIT, 0 ) 
	                END IsDefault,
	                A.[ADDRESS_TYPE_ID] AddressTypeId,
	                A.[COUNTRY] Country,
	                A.[CITY] City,
	                A.[POST_CODE] PostCode,
	                A.[LONGITUDE] Longitude,
	                A.[LATITUDE] Latitude,
	                A.[TITLE] Title 
                FROM
	                NEW_ADDRESS A 
                WHERE
	                A.ADDRESS_ID = @AddressId";
            AddressResponse userAddress = new AddressResponse();

            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@UserId",SqlDbType.Int,10,ParameterDirection.Input,userId),
                        DbHandler.SetParameter("@AddressId",SqlDbType.Int,10,ParameterDirection.Input,id)
                    });

                    if (reader.Read())
                    {
                        userAddress = new AddressResponse()
                        {
                            AddressId = reader.GetInt("AddressId"),
                            IsDefault = Boolean.Parse(reader["IsDefault"].ToString()),
                            Address = reader["Address"].ToString(),
                            AddressTypeId = reader.GetInt("AddressTypeId"),
                            Country = reader["Country"].ToString(),
                            City = reader["City"].ToString(),
                            Latitude = reader["Latitude"].ToString(),
                            Longitude = reader["Longitude"].ToString(),
                            PostCode = reader.GetInt("PostCode"),
                            Title = reader["Title"].ToString()
                        };

                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return userAddress;
        }

        public AddressResponse GetAddressById(int id, int userId)
        {
            const string sql =
                @"SELECT
	                A.[ADDRESS_ID] AddressId,
	                A.[ADDRESS] Address,
                CASE
		                ( SELECT DEFAULT_SHOPPING_ADDRESS FROM NEW_USER WHERE USER_ID = @UserId ) 
		                WHEN A.ADDRESS_ID THEN
		                CONVERT ( BIT, 1 ) ELSE CONVERT ( BIT, 0 ) 
	                END IsDefault,
	                A.[ADDRESS_TYPE_ID] AddressTypeId,
	                A.[COUNTRY] Country,
	                A.[CITY] City,
	                A.[POST_CODE] PostCode,
	                A.[LONGITUDE] Longitude,
	                A.[LATITUDE] Latitude,
	                A.[TITLE] Title 
                FROM
	                NEW_ADDRESS A 
                WHERE
	                A.ADDRESS_ID = @AddressId AND A.IS_ACTIVE = 1";
            AddressResponse userAddress = new AddressResponse();

            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@UserId",SqlDbType.Int,10,ParameterDirection.Input,userId),
                        DbHandler.SetParameter("@AddressId",SqlDbType.Int,10,ParameterDirection.Input,id)
                    });

                    if (reader.Read())
                    {
                        userAddress = new AddressResponse()
                        {
                            AddressId = reader.GetInt("AddressId"),
                            IsDefault = Boolean.Parse(reader["IsDefault"].ToString()),
                            Address = reader["Address"].ToString(),
                            AddressTypeId = reader.GetInt("AddressTypeId"),
                            Country = reader["Country"].ToString(),
                            City = reader["City"].ToString(),
                            Latitude = reader["Latitude"].ToString(),
                            Longitude = reader["Longitude"].ToString(),
                            PostCode = reader.GetInt("PostCode"),
                            Title = reader["Title"].ToString()
                        };

                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return userAddress;
        }

        public List<AddressResponse> GetAllAddressByUserId(string userId)
        {
            const string sql =
                @"SELECT 
                      (SELECT
	                        A.[ADDRESS_ID] AddressId,
	                        A.[ADDRESS] Address,
                        CASE
		                        ( SELECT DEFAULT_SHOPPING_ADDRESS FROM NEW_USER WHERE USER_ID = @UserId ) 
		                        WHEN A.ADDRESS_ID THEN
		                        CONVERT ( BIT, 1 ) ELSE CONVERT ( BIT, 0 ) 
	                        END IsDefault,
	                        A.[ADDRESS_TYPE_ID] AddressTypeId,
	                        A.[COUNTRY] Country,
	                        A.[CITY] City,
	                        A.[POST_CODE] PostCode,
	                        A.[LONGITUDE] Longitude,
	                        A.[LATITUDE] Latitude,
	                        A.[TITLE] Title 
                        FROM
	                        NEW_ADDRESS A 
                        WHERE
	                        A.USER_ID = @UserId 
	                        AND A.[IS_ACTIVE] =1 order by A.ADDRESS_ID DESC for json path) [Json];";
            List<AddressResponse> userAddresses;

            try
            {
                string json = String.Empty;
                using (var con = new DbHandler())
                {
                    var reader = con.ExecuteSql(sql, new[]
                    {
                        DbHandler.SetParameter("@UserId",SqlDbType.Int,10,ParameterDirection.Input,userId)
                    });

                    if (reader.Read())
                    {
                        json = reader["Json"].ToString();
                    }
                }

                userAddresses = JsonConvert.DeserializeObject<List<AddressResponse>>(json);
                userAddresses = userAddresses ?? new List<AddressResponse>();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }

            return userAddresses;
        }
    }
}