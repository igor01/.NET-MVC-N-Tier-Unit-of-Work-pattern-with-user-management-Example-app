using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NTierUoWExampleApp.Mvc.API
{
    public class BaseController : ApiController
    {
        public List<string> CreateErrorResponse()
        {
            var allErrors = ModelState.Values.SelectMany(v => v.Errors).ToList();
            List<string> response = new List<string>();

            if (allErrors.Count > 0)
            {
                for (var i = 0; i < allErrors.Count; i++)
                {
                    response.Add(allErrors[i].ErrorMessage);
                }
            }
            return response;
        }
    }
}
