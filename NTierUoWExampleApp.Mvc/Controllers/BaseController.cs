using Microsoft.AspNet.Identity;
using NTierUoWExampleApp.Common.Enum.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NTierUoWExampleApp.Mvc.Controllers
{
    public class BaseController : Controller
    {
        public void ShowInformation(string msg)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                TempData[MessageTypeEnum.Information.ToString()] = msg;
            }
        }

        public void ShowWarning(string msg)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                TempData[MessageTypeEnum.Warning.ToString()] = msg;
            }
        }

        public void ShowError(string msg)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                TempData[MessageTypeEnum.Error.ToString()] = msg;
            }
        }

        public void ShowSuccess(string msg)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                TempData[MessageTypeEnum.Success.ToString()] = msg;
            }
        }
        public string GetUsername()
        {
            return User.Identity.Name;
        }

        public string GetUserId()
        {
            return User.Identity.GetUserId();
        }

        public string GetAppUrl()
        {
            return Common.Utility.Config.AppURL;
        }

        public string GetUserAgent()
        {
            return Request.UserAgent;
        }

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