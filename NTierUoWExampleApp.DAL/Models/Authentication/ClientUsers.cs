using NTierUoWExampleApp.DAL.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.DAL.Models.Authentication
{
    public class ClientUsers
    {
        public Guid ClientUsersId { get; set; }
        public string UserId { get; set; }
        public Guid ClientId { get; set; }
        public virtual User User { get; set; }
        public virtual Client Client { get; set; }
    }
}
