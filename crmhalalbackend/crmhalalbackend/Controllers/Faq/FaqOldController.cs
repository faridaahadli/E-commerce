using CRMHalalBackEnd.Models;
using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Description;
using CRMHalalBackEnd.App_Code;
using CRMHalalBackEnd.repository;
using WebApi.Jwt.Filters;

namespace CRMHalalBackEnd.Controllers.Faq
{


    [JwtAuthentication]
    public class FaqOldController : ApiController
    {
        private readonly UtilsClass _controllerActions = new UtilsClass();

        //[HttpGet]
        //[ResponseType(typeof(Response))]
        //public IHttpActionResult GetFaqList()
        //{
        //    Response<List<Models.Faq.FaqDto>> response;
        //    try
        //    {
        //        var repository = new FaqRepository();
        //        response = new Response<List<Models.Faq.FaqDto>>()
        //        {
        //            Code = (int) HttpStatusCode.Created,
        //            Success = true,
        //            Data = repository.GetFaqList()
        //        };
        //    }
        //    catch (Exception e)
        //    {
        //        response = new Response<List<Models.Faq.FaqDto>>()
        //        {
        //            Code = (int) HttpStatusCode.InternalServerError,
        //            Success = false,
        //            Message = e.Message,
        //            Data = null
        //        };
        //    }

        //    return Ok(response);
        //}

        //[HttpGet]
        //[ResponseType(typeof(Response))]        
        //public IHttpActionResult GetFaqById(int id)
        //{
        //    Response<Models.Faq.FaqDto> response;
        //    try
        //    {
        //        var repository = new FaqRepository();
        //        response = new Response<Models.Faq.FaqDto>()
        //        {
        //            Code = (int) HttpStatusCode.Created,
        //            Success = true,
        //            Data = repository.GetFaqById(id)
        //        };
        //    }
        //    catch (Exception e)
        //    {
        //        response = new Response<Models.Faq.FaqDto>()
        //        {
        //            Code = (int) HttpStatusCode.InternalServerError,
        //            Success = false,
        //            Message = e.Message,
        //            Data = null
        //        };
        //    }

        //    return Ok(response);
        //}


        [HttpGet]
        [ResponseType(typeof(Response))]
        public IHttpActionResult GetFaqByModuleId(int id)
        {
            Response<List<Models.Faq.FaqDto>> response;
            try
            {
                var repository = new FaqRepository();
                response = new Response<List<Models.Faq.FaqDto>>()
                {
                    Code = (int)HttpStatusCode.Created,
                    Success = true,
                    Data = repository.GetFaqListByModuleId(id)
                };
            }
            catch (Exception e)
            {
                response = new Response<List<Models.Faq.FaqDto>>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = e.Message,
                    Data = null
                };
            }

            return Ok(response);
        }

        //[HttpDelete]
        //[ResponseType(typeof(Response))]
        //public IHttpActionResult Delete(int id)
        //{
        //    var userid = _controllerActions.getUserId((ClaimsIdentity)User.Identity);
        //    Response<object> response;
        //    try
        //    {
        //        var repository = new FaqRepository();
        //        repository.Delete(id, int.Parse(userid));
        //        response = new Response<object>()
        //        {
        //            Code = (int)HttpStatusCode.Created,
        //            Success = true,
        //        };
        //    }
        //    catch (Exception e)
        //    {
        //        response = new Response<object>()
        //        {
        //            Code = (int)HttpStatusCode.InternalServerError,
        //            Success = false,
        //            Message = e.Message,
        //            Data = null
        //        };
        //    }

        //    return Ok(response);
        //}


        //[HttpPost]
        //[ResponseType(typeof(Response))]
        //public IHttpActionResult Upsert(Models.Faq.FaqDto faq)
        //{
        //    var userid = _controllerActions.getUserId((ClaimsIdentity)User.Identity);
        //    Response<Models.Faq.FaqDto> response;
        //    try
        //    {
        //        var repository = new FaqRepository();
        //        var newFaq = repository.Upsert(faq, int.Parse(userid));


        //        response = new Response<Models.Faq.FaqDto>
        //        {
        //            Code = (int)HttpStatusCode.Created,
        //            Success = true,
        //            Data = newFaq
        //        };
        //    }
        //    catch (Exception e)
        //    {
        //        response = new Response<Models.Faq.FaqDto>
        //        {
        //            Code = (int)HttpStatusCode.InternalServerError,
        //            Success = false,
        //            Message = e.Message,
        //            Data = null
        //        };
        //    }
        //    return Ok(response);
        //}

    }
}