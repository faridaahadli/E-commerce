using CRMHalalBackEnd.App_Code;
using CRMHalalBackEnd.Models;
using CRMHalalBackEnd.Repository;
using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;

namespace CRMHalalBackEnd.Controllers.Attribute
{
    [AllowAnonymous]
    public class AttributeController : ApiController
    {
        private readonly UtilsClass _controllerActions = new UtilsClass();

        [HttpGet]
        [ResponseType(typeof(Response))]
        public IHttpActionResult GetAttributeByCategoryId(int id)
        {
            Response<List<Models.Attribute.Attribute>> content;
            try
            {
                AttributeRepository attributeRepository = new AttributeRepository();
                content = new Response<List<Models.Attribute.Attribute>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = attributeRepository.GetAttributesByCategoryId(id)
                };
            }
            catch (Exception ex)
            {
                content = new Response<List<Models.Attribute.Attribute>>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            return Ok(content);
        }
        public IHttpActionResult GetAllAttributeValueByAttribute(string attribute)
        {
            Response<List<string>> content;
            try
            {
                AttributeRepository attributeRepository = new AttributeRepository();
                content = new Response<List<string>>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Success = true,
                    Data = attributeRepository.GetAllValueByAttribute(attribute)
                };
            }
            catch (Exception ex)
            {
                content = new Response<List<string>>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
            }
            return Ok(content);
        }
    }
}