using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using NTierUoWExampleApp.Mvc.App_Start;
using Owin;
using System.Web.Http;
using static NTierUoWExampleApp.Mvc.MvcApplication;

[assembly: OwinStartupAttribute(typeof(NTierUoWExampleApp.Mvc.Startup))]
namespace NTierUoWExampleApp.Mvc
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);

            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);


            //map custom user provider for signalr
            GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => new CustomUserIdProvider());
            app.MapSignalR();
        }
    }
}
