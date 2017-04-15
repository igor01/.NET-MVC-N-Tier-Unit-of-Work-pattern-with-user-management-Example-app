using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.DAL.Models.Account
{
    public class UserRole : IdentityUserRole
    {
        //inherited
        //public virtual Guid RoleId { get; set; }
        //public virtual Guid UserId { get; set; }

        public virtual Guid UserRoleId { get; set; }
        public virtual string RoleName { get; set; }
        public virtual string RoleType { get; set; }
    }
}
