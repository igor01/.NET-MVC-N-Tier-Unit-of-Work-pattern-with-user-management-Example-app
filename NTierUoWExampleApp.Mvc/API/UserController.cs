using NTierUoWExampleApp.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace NTierUoWExampleApp.Mvc.API
{
    public class UserController : BaseController
    {
        private WebApiService service;
        public UserController()
        {
            service = new WebApiService();
        }

       

        [Authorize]
        [HttpPost]
        [Route("api/user/getUsers")]
        public async Task<HttpResponseMessage> GetUsers()
        {
            try
            {
                ClaimsPrincipal principal = Request.GetRequestContext().Principal as ClaimsPrincipal;
                var username = principal.Identity.Name;
                var clientAppId = principal.Claims.Where(t => t.Type == "ClientAppId").FirstOrDefault().Value;

                //check if token is revoked
                if (service.IsUserTokenRevoked(username, clientAppId))
                {
                    return Request.CreateResponse(HttpStatusCode.Forbidden, "Access token not found.");
                }

                var users = await service.GetUsers();

                return Request.CreateResponse(HttpStatusCode.OK, users);
            }
            catch (ValidationException vex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, vex.Message);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}
