using CRMHalalBackEnd.App_Code;
using CRMHalalBackEnd.Filters;
using CRMHalalBackEnd.Models;
using CRMHalalBackEnd.Models.ExcelImport;
using CRMHalalBackEnd.Repository;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Security.Claims;
using System.Web.Hosting;
using System.Web.Http;

namespace CRMHalalBackEnd.Controllers.Excel
{
    public class ExcelController : ApiController
    {
       private readonly UtilsClass _controllerActions = new UtilsClass();

 

        
        [HttpGet]
        [JwtRoleAuthentication(Actor = "Company", Permission = "21")]
        public IHttpActionResult GetAll(string path)
        {

            //List<AllData> allData = new List<AllData>();
            string fileName = HostingEnvironment.MapPath("~" + path);

            var userid = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);

            Response<List<AllData>> response;
            try
            {
                var repository = new ExcelRepository();
                

                response = new Response<List<AllData>>
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Message = "excel oxundu",
                    Data = repository.GetAllData(fileName, int.Parse(userid), tenantId)
                };
            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<List<AllData>>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message,
                        Data = null
                    };
                }
                else
                {
                    response = new Response<List<AllData>>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message,
                        Data = null
                    };
                }

            }
            catch (Exception e)
            {
                response = new Response<List<AllData>>
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = e.Message,
                    Data = null
                };
            }

            return Ok(response);

        }

    
        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "21")]
        public IHttpActionResult Insert(List<AllDataFront> allDataFront)
        {
            var userid = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);

            Response<string> response;
            try
            {
                var repository = new ExcelRepository();
                repository.InsertProducts(allDataFront, int.Parse(userid), tenantId);

                response = new Response<string>
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Message = "Məhsullar uğurla əlavə olundu!"
                };
            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<string>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<string>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception e)
            {
                response = new Response<string>
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = e.Message,
                    Data = "Xəta yarandı!"
                };
            }

            return Ok(response);
        }

    }

}