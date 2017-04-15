using Microsoft.AspNet.Identity;
using NTierUoWExampleApp.DAL.IdentityManager;
using NTierUoWExampleApp.DAL.Models.Account;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.DAL.DBInitialization
{
    public class DatabaseInitialization : CreateDatabaseIfNotExists<ApplicationContext>
    {
        private ApplicationContext context;
        ApplicationUserManager UserManager { get; set; }
        ApplicationRoleManager RoleManager { get; set; }

        protected override void Seed(ApplicationContext context)
        {
            this.context = context;
            UserManager = new ApplicationUserManager(new ApplicationUserStore(context));
            RoleManager = new ApplicationRoleManager(new ApplicationRoleStore(context));

            UserManager.UserValidator = new UserValidator<User>(UserManager) { AllowOnlyAlphanumericUserNames = false };


            //create roles
            string administratorRoleId = Guid.NewGuid().ToString();
            string viewerRoleId = Guid.NewGuid().ToString();

            RoleManager.Create(new Role() { Name = "Administrator", Id = administratorRoleId });
            RoleManager.Create(new Role() { Name = "Viewer", Id = viewerRoleId });

            //create default user
            var user = CreateUser("name.lastname@gmail.com", "Password_123");


            if (!string.IsNullOrEmpty(user.Id))
            {
                context.UserRoles.Add(new UserRole() { RoleName = "Administrator", UserId = user.Id, RoleId = administratorRoleId });
            }

            base.Seed(context);
        }

        public User CreateUser(string username, string pass)
        {
            try
            {
                User user = new User();

                user.Id = Guid.NewGuid().ToString();
                user.UserName = username;
                user.Email = username;
                user.EmailConfirmed = true;
                user.FirstName = "Name";
                user.LastName = "Lastname";
                user.NotificationEnabled = true;
                user.CreatedDate = DateTime.UtcNow;
                user.LockoutEnabled = true;
                user.LastLogin = DateTime.Now;
                user.LastLoginUtc = DateTime.UtcNow;
                user.Status = Common.Enum.User.UserStatusEnum.Active.ToString();
                user.OnlineStatus = Common.Enum.User.UserOnlineStatusEnum.Offline.ToString();

                var result = UserManager.Create(user, pass);

                return user;
            }
            catch (Exception e)
            {
                return null;
            }

        }
        
    }
}
