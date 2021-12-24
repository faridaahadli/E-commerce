using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Security.Claims;
using System.Web.Http;
using CRMHalalBackEnd.App_Code;
using CRMHalalBackEnd.Filters;
using CRMHalalBackEnd.Models;
using CRMHalalBackEnd.Models.Address;
using CRMHalalBackEnd.Repository;

namespace CRMHalalBackEnd.Controllers.Address
{
    [JwtRoleAuthentication(Actor = "User")]
    public class AddressController : ApiController
    {
        private readonly UtilsClass _controllerActions = new UtilsClass();
        private readonly AddressRepository _repository = new AddressRepository();

        [HttpPost]
        
        public IHttpActionResult InsertUserAddress(NewAddress address, bool isDefault = false)
        {
            Response<AddressResponse> response;
            try
            {
                var userId = _controllerActions.getUserId((ClaimsIdentity)User.Identity);
                int userAddressId = _repository.InsertUserAddress(address, isDefault, int.Parse(userId));
                response = new Response<AddressResponse>
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Data = _repository.GetAddressByIdWithoutIsActive(userAddressId, int.Parse(userId))
                };
            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<AddressResponse>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<AddressResponse>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception ex)
            {
                response = new Response<AddressResponse>
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }


            return Ok(response);
        }
        [HttpPost]
        public IHttpActionResult UpdateUserAddress(NewAddress address, bool isDefault = false)
        {
            Response<AddressResponse> response;
            try
            {
                var userId = _controllerActions.getUserId((ClaimsIdentity)User.Identity);
                int userAddressId = _repository.UpdateUserAddress(address, isDefault, int.Parse(userId));
                response = new Response<AddressResponse>
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _repository.GetAddressByIdWithoutIsActive(userAddressId, int.Parse(userId))
                };
            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<AddressResponse>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<AddressResponse>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception ex)
            {
                response = new Response<AddressResponse>
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            return Ok(response);
        }
        [HttpPost]
        public IHttpActionResult DeleteUserAddress(int addressId)
        {
            Response<AddressResponse> response;
            try
            {
                var userId = _controllerActions.getUserId((ClaimsIdentity)User.Identity);
                int userAddressId = _repository.DeleteUserAddress(addressId, int.Parse(userId));
                response = new Response<AddressResponse>
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _repository.GetAddressByIdWithoutIsActive(userAddressId, int.Parse(userId))
                };
            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<AddressResponse>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<AddressResponse>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception ex)
            {
                response = new Response<AddressResponse>
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }


            return Ok(response);
        }
        [HttpGet]
        public IHttpActionResult GetAllAddressByUser()
        {
            Response<List<AddressResponse>> response;
            try
            {
                var userid = _controllerActions.getUserId((ClaimsIdentity)User.Identity);
                response = new Response<List<AddressResponse>>
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _repository.GetAllAddressByUserId(userid)
                };
            }
            
            catch (Exception ex)
            {
                response = new Response<List<AddressResponse>>
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }


            return Ok(response);
        }
    }
}
