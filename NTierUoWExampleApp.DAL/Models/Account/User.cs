using Microsoft.AspNet.Identity.EntityFramework;
using NTierUoWExampleApp.DAL.Models.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.DAL.Models.Account
{
    public class User : IdentityUser<string, IdentityUserLogin, UserRole, IdentityUserClaim>
    {
        public User()
        {
            UserNotifications = new List<UserNotification>();
            UserWebClientConnections = new List<UserWebClientConnection>();
            BrowsingHistory = new List<BrowsingHistory>();
            ClientUsers = new List<ClientUsers>();
        }
        //inherited properties

        //public virtual string Id { get; set; }
        //public virtual string UserName { get; set; }
        //public virtual string Email { get; set; }
        //public virtual bool EmailConfirmed { get; set; }
        //public virtual string PhoneNumber { get; set; }
        //public virtual bool PhoneNumberConfirmed { get; set; }
        //public virtual bool TwoFactorEnabled { get; set; }
        //public virtual DateTime LockoutEndDateUtc { get; set; }
        //public virtual bool LockoutEnabled { get; set; }
        //public virtual int AccessFailedCount { get; set; }
        //public virtual string PasswordHash { get; set; }
        //public virtual string SecurityStamp { get; set; }

        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual string Status { get; set; }
        public virtual bool NotificationEnabled { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual DateTime? LastLogin { get; set; }
        public virtual DateTime? LastLoginUtc { get; set; }
        public virtual string ModifiedBy { get; set; }
        public virtual DateTime? ModifiedDate { get; set; }
        public virtual bool PermanentLock { get; set; }
        public virtual DateTime? ValidThrough { get; set; }
        public virtual string OnlineStatus { get; set; }

        public virtual ICollection<UserNotification> UserNotifications { get; set; }
        public virtual ICollection<UserWebClientConnection> UserWebClientConnections { get; set; }
        public virtual ICollection<BrowsingHistory> BrowsingHistory { get; set; }
        public virtual ICollection<ClientUsers> ClientUsers { get; set; }
    }
}
