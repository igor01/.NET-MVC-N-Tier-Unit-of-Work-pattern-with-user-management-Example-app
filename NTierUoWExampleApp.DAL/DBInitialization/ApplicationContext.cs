using Microsoft.AspNet.Identity.EntityFramework;
using NTierUoWExampleApp.Common.Utility;
using NTierUoWExampleApp.DAL.Models.Account;
using NTierUoWExampleApp.DAL.Models.Global;
using NTierUoWExampleApp.DAL.ModelsConfiguration.Account;
using NTierUoWExampleApp.DAL.ModelsConfiguration.Global;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.DAL.DBInitialization
{
    public class ApplicationContext : IdentityDbContext<User, Role, string, IdentityUserLogin, UserRole, IdentityUserClaim>
    {
        public ApplicationContext()
            : base(Config.ConnectionStringName)
        {

        }

        //Account
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserNotification> UserNotifications { get; set; }
        public DbSet<UserWebClientConnection> UserWebClientConnections { get; set; }
        public DbSet<BrowsingHistory> BrowsingHistory { get; set; }

        //Logging
        public DbSet<ErrorLog> ErrorLogs { get; set; }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Account
            modelBuilder.Configurations.Add(new UserRoleConfig());
            modelBuilder.Entity<User>().ToTable("UserProfile");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaims");
            modelBuilder.Entity<Role>().ToTable("Roles");
            modelBuilder.Configurations.Add(new UserNotificationConfig());
            modelBuilder.Configurations.Add(new UserWebClientConnectionConfig());
            modelBuilder.Configurations.Add(new BrowsingHistoryConfig());

            //Logging
            modelBuilder.Configurations.Add(new ErrorLogConfig());

        }
    }
}
