using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.DAL.Models.Account
{
    public class UserWebClientConnection
    {
        public Guid UserWebClientConnectionId { get; set; }
        public string SignalRConnectionId { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public string Username { get; set; }
        public string ConnectedUrl { get; set; }
        public string UserAgent { get; set; }
    }
}
