using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using CRMHalalBackEnd.App_Code;
using CRMHalalBackEnd.Filters;
using CRMHalalBackEnd.Helpers;
using CRMHalalBackEnd.Models;
using CRMHalalBackEnd.Models.Employee;
using CRMHalalBackEnd.Models.NewCompany;
using CRMHalalBackEnd.Repository;
using WebApi.Jwt;

namespace CRMHalalBackEnd.Controllers.Company
{
    [JwtRoleAuthentication(Actor = "User")]
    public class CompanyController : ApiController
    {
        private readonly UtilsClass _controllerActions = new UtilsClass();
        private readonly CompanyRepository _repository = new CompanyRepository();
        private readonly EmployeeRepository _employeeRepository = new EmployeeRepository();

        [HttpPost]
        public IHttpActionResult Registration(CompanyRegDto companyRegDto)
        {
            var userid = _controllerActions.getUserId((ClaimsIdentity)User.Identity);
            Response<EmployeeTokenData> response;
            try
            {
                if (!Regex.IsMatch(companyRegDto.Contacts.Where(k => k.ContactTypeId == 2).Select(k => k.Text).First(),@"^\+[1-9]{1}[0-9]{3,14}$"))
                {
                    throw new Exception("Nömrənin formatı düzgün deyil!");
                }

                var tenantId = _repository.Insert(companyRegDto, userid);
                var company = _repository.GetCompanyByTenant(tenantId, userid);
                string html = HtmlFileSend.HtmlFileSender("~/HtmlFiles/Sirket qeydiyyati/index.html");
                company.Contacts.ForEach(k =>
                {
                    Task.Run(async () =>
                    {
                        if (k.ContactTypeId == 1)
                        {
                            await EmailSend.SendEmailAsync(k.Text, "Note - Şirkət qeydiyyatı", html);

                        }

                    });
                });
                response = new Response<EmployeeTokenData>
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Data = JwtManager.NewGenerateTokenForCompany(_employeeRepository.GetEmployeeData(int.Parse(userid), tenantId))
                };
            }
            catch (SqlException ex)
            {

                if (ex.Number == 51000)
                {
                    response = new Response<EmployeeTokenData>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Success = false,
                        Message = ex.Message
                    };
                }
                else
                {
                    response = new Response<EmployeeTokenData>
                    {
                        Code = (int)HttpStatusCode.InternalServerError,
                        Success = false,
                        Message = ex.Message
                    };
                }

            }
            catch (Exception ex)
            {
                response = new Response<EmployeeTokenData>
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
        public IHttpActionResult GetAllCompanyByUser()
        {
            var userId = _controllerActions.getUserId((ClaimsIdentity)User.Identity);
            Response<List<CompanyMainPageDto>> response;
            try
            {
                response = new Response<List<CompanyMainPageDto>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _repository.GetAllCompanyByUser(userId)
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<CompanyMainPageDto>>
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

        public IHttpActionResult Login(string tenantId)
        {
            var userid = _controllerActions.getUserId((ClaimsIdentity)User.Identity);
            Response<EmployeeTokenData> response;
            try
            {
                response = new Response<EmployeeTokenData>
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Data = JwtManager.NewGenerateTokenForCompany(_employeeRepository.GetEmployeeData(int.Parse(userid), tenantId))
                };
            }
            catch (Exception ex)
            {
                response = new Response<EmployeeTokenData>
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            return Ok(response);
        }

        #region CompanyCategory
        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult GetAllCompanyCategory()
        {
            Response<List<CompanyCategory>> response;
            try
            {
                response = new Response<List<CompanyCategory>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = _repository.GetCompanyCategory()
                };
            }
            catch (Exception ex)
            {
                response = new Response<List<CompanyCategory>>
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            return Ok(response);
        }

        #endregion

    }
}
