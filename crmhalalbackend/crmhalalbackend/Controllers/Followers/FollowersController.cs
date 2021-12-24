using System;
using System.Net;
using System.Security.Claims;
using System.Web.Http;
using CRMHalalBackEnd.App_Code;
using CRMHalalBackEnd.Filters;
using CRMHalalBackEnd.Models;
using CRMHalalBackEnd.Repository;

namespace CRMHalalBackEnd.Controllers.Followers
{
    public class FollowersController : ApiController
    {
        private readonly UtilsClass _controllerActions = new UtilsClass();
        private readonly FollowersRepository _repository = new FollowersRepository();

        [JwtRoleAuthentication(Actor = "User")]
        [Route("note/api/Followers/{domain}/InsertFollower")]
        [HttpPost]
        public IHttpActionResult InsertFollowerUser(string domain)
        {
            int userId = int.Parse(_controllerActions.getDefaultUserId((ClaimsIdentity) User.Identity));
            Response<int> response;

            try
            {
                response = new Response<int>()
                {
                    Code = (int) HttpStatusCode.Created,
                    Data = _repository.FollowerInsert(domain.Replace('_','.'), userId),
                    Success = true,
                    Message = "Follower Create"

                };

            }
            catch (Exception ex)
            {
                response = new Response<int>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Data = 0,
                    Success = false,
                    Message = ex.Message

                };
            }

            return Ok(response);
        }

        [JwtRoleAuthentication(Actor = "User")]
        [Route("note/api/Followers/{domain}/DeleteFollowerUser")]
        [HttpPost]
        public IHttpActionResult DeleteFollowerUser(string domain)
        {
            int userId = int.Parse(_controllerActions.getDefaultUserId((ClaimsIdentity)User.Identity));
            Response<int> response;

            try
            {
                response = new Response<int>()
                {
                    Code = (int)HttpStatusCode.OK,
                    Data = _repository.FollowerDelete(domain.Replace('_', '.'), userId),
                    Success = true,
                    Message = "Follower Delete"

                };

            }
            catch (Exception ex)
            {
                response = new Response<int>()
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    Data = 0,
                    Success = false,
                    Message = ex.Message

                };
            }

            return Ok(response);
        }
    }
}
