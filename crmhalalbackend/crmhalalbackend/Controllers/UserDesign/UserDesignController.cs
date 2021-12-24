using System.Web.Http;
using WebApi.Jwt.Filters;

namespace CRMHalalBackEnd.Controllers.UserDesign
{


    [JwtAuthentication]
    public class UserDesignController : ApiController
    {
        //private readonly UtilsClass _controllerActions = new UtilsClass();

        //[HttpGet]
        //[ResponseType(typeof(Response))]
        //public IHttpActionResult GetUserDesign()
        //{
        //    Response<Models.Common.UserDesign> response;
        //    try
        //    {
        //        var userid = _controllerActions.getUserId((ClaimsIdentity)User.Identity);
        //        var repository = new UserDesignRepository();
        //        response = new Response<Models.Common.UserDesign>()
        //        {
        //            Code = (int) HttpStatusCode.Created,
        //            Success = true,
        //            Data = repository.GetUserDesignByUserId(int.Parse(userid))
        //        };
        //    }
        //    catch (Exception e)
        //    {
        //        response = new Response<Models.Common.UserDesign>()
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
        //public IHttpActionResult GetUserDesignById(int id)
        //{
        //    Response<Models.Common.UserDesign> response;
        //    try
        //    {
        //        var repository = new UserDesignRepository();
        //        response = new Response<Models.Common.UserDesign>()
        //        {
        //            Code = (int) HttpStatusCode.Created,
        //            Success = true,
        //            Data = repository.GetUserDesignById(id)
        //        };
        //    }
        //    catch (Exception e)
        //    {
        //        response = new Response<Models.Common.UserDesign>()
        //        {
        //            Code = (int) HttpStatusCode.InternalServerError,
        //            Success = false,
        //            Message = e.Message,
        //            Data = null
        //        };
        //    }

        //    return Ok(response);
        //}


       

        //[HttpDelete]
        //[ResponseType(typeof(Response))]
        //public IHttpActionResult Delete(int id)
        //{
        //    var userid = _controllerActions.getUserId((ClaimsIdentity)User.Identity);
        //    Response<object> response;
        //    try
        //    {
        //        var repository = new UserDesignRepository();
        //        repository.Delete(id, int.Parse(userid));
        //        response = new Response<object>()
        //        {
        //            Code = (int) HttpStatusCode.Created,
        //            Success = true,
        //        };
        //    }
        //    catch (Exception e)
        //    {
        //        response = new Response<object>()
        //        {
        //            Code = (int) HttpStatusCode.InternalServerError,
        //            Success = false,
        //            Message = e.Message,
        //            Data = null
        //        };
        //    }

        //    return Ok(response);
        //}


        //[HttpPost]
        //[ResponseType(typeof(Response))]
        //public IHttpActionResult Upsert(Models.Common.UserDesign userDesign)
        //{
        //    var userid = _controllerActions.getUserId((ClaimsIdentity)User.Identity);
        //    Response<Models.Common.UserDesign> response;
        //    try
        //    {
        //        var repository = new UserDesignRepository();
        //        var newFaq = repository.Upsert(userDesign, int.Parse(userid));


        //        response = new Response<Models.Common.UserDesign>
        //        {
        //            Code = (int)HttpStatusCode.Created,
        //            Success = true,
        //            Data = newFaq
        //        };
        //    }
        //    catch (Exception e)
        //    {
        //        response = new Response<Models.Common.UserDesign>
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