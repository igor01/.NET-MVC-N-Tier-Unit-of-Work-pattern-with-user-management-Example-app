using Microsoft.AspNet.Identity;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace NTierUoWExampleApp.Mvc
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Core.Configuration.InitDatabase InitDatabase = new Core.Configuration.InitDatabase();
            InitDatabase.Init();

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        //signal R custom user provider
        public class CustomUserIdProvider : IUserIdProvider
        {
            public string GetUserId(IRequest request)
            {
                //implement your own user provider here
                var userId = HttpContext.Current.User.Identity.GetUserId();
                return userId;
            }
        }
    }
}
