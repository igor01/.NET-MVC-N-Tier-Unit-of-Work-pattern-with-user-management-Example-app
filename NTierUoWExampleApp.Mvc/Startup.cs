using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
using static NTierUoWExampleApp.Mvc.MvcApplication;

[assembly: OwinStartupAttribute(typeof(NTierUoWExampleApp.Mvc.Startup))]
namespace NTierUoWExampleApp.Mvc
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            //map custom user provider for signalr
            GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => new CustomUserIdProvider());
            app.MapSignalR();
        }
    }
}
