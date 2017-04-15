using Microsoft.AspNet.SignalR;
using NTierUoWExampleApp.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace NTierUoWExampleApp.Mvc.Hubs
{
    public class NotificationHub : Hub
    {
        AccountService service = new AccountService(Startup.DataProtectionProvider);
        public NotificationHub()
        {

        }

        public override async Task OnConnected()
        {
            await service.RegisterUserWebClient(Context.User.Identity.Name, Context.ConnectionId, Context.Request.Headers["Referer"], Context.Request.Headers["User-Agent"]);
            await service.SetUserConnectionStatus(Context.User.Identity.Name);
            await base.OnConnected();
        }

        public override async Task OnDisconnected(bool stopCalled)
        {
            await service.UnRegisterUserWebClient(Context.ConnectionId);
            await service.SetUserConnectionStatus(Context.User.Identity.Name);
            await base.OnDisconnected(stopCalled);
        }
    }
}