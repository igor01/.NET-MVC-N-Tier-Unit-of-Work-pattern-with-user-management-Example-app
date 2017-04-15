using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using NTierUoWExampleApp.DAL.DBInitialization;
using NTierUoWExampleApp.DAL.Models;
using NTierUoWExampleApp.DAL.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.DAL.IdentityManager
{
    public class ApplicationUserStore : UserStore<User, Role, string, IdentityUserLogin, UserRole, IdentityUserClaim>, IUserStore<User>
    {
        public ApplicationUserStore(ApplicationContext context)
           : base(context)
        {

        }
    }
}
