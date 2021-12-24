using CRMHalalBackEnd.App_Code;
using CRMHalalBackEnd.Filters;
using CRMHalalBackEnd.Models;
using CRMHalalBackEnd.Models.CustomersCompany;
using CRMHalalBackEnd.Models.ExcelImport;
using CRMHalalBackEnd.repository;
using CRMHalalBackEnd.Repository;
using ExcelDataReader;
using ExcelDataReader.Log;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Hosting;
using System.Web.Http;

namespace CRMHalalBackEnd.Controllers.CompanyCustomers
{
    public class CustomersCompanyController : ApiController
    {

        private readonly UtilsClass _controllerActions = new UtilsClass();


        // 1. ReadCustomerData excelden gelen datalari key value formatinda oxuyur
        [HttpGet]
        [JwtRoleAuthentication(Actor = "Company", Permission = "109")]
        public IHttpActionResult ReadCustomerData(string path)
        {
            // string path = @"C:\Program Files (x86)\IIS Express\TestCust.xlsx";

            string fileName = HostingEnvironment.MapPath("~" + path);


            Response<List<AllData>> response;
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            var userId = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            try
            {
                var repository = new CustomersCompanyRepository();
                response = new Response<List<AllData>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = repository.ReadDataFromExcel(fileName)
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<AllData>>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }

            return Ok(response);
        }

        // 2. Insertden once gelen data repositoryde convert olunur ve daha sonra insert olunur
        [HttpPost]
        [JwtRoleAuthentication(Actor = "Company", Permission = "109")]
        public IHttpActionResult InsertCustomerdata(List<AllData> allDatas)
        {
            string tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            var userId = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            Response<string> response;

            var repository = new CustomersCompanyRepository();
            try
            {
                var custComp = repository.Insert(allDatas, tenantId, int.Parse(userId));
                response = new Response<string>
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Data = custComp,
                    Message = "Datalar uğurla yaradıldı!"
                };
                return Ok(response);

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
            catch (Exception ex)
            {

                response = new Response<string>
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message

                };
            }

            return Ok(response);

        }

        // 3. insert olunmus Butun datalari get edir
        [HttpGet]
        [JwtRoleAuthentication(Actor = "Company", Permission = "109")]
        public IHttpActionResult GetCustomerCompanyData()
        {
            Response<List<GetCustomers>> response;
            var tenantId = _controllerActions.getTenantId((ClaimsIdentity)User.Identity);
            var userId = _controllerActions.getActiveUserId((ClaimsIdentity)User.Identity);
            try
            {
                var repository = new CustomersCompanyRepository();
                response = new Response<List<GetCustomers>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = repository.AllCustomers(tenantId, int.Parse(userId))
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<GetCustomers>>()
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
