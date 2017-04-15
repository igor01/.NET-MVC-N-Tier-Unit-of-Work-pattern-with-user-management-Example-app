using Microsoft.AspNet.Identity.EntityFramework;
using NTierUoWExampleApp.DAL.DBInitialization;
using NTierUoWExampleApp.DAL.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.DAL.IdentityManager
{
    public class ApplicationRoleStore : RoleStore<Role, string, UserRole>
    {
        public ApplicationRoleStore(ApplicationContext context)
            : base(context)
        {

        }
    }
}
