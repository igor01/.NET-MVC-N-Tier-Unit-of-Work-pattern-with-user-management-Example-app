using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using NTierUoWExampleApp.DAL.DBInitialization;
using NTierUoWExampleApp.DAL.IdentityManager;
using NTierUoWExampleApp.DAL.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.Core.Utility.Identity
{
    public class ApplicationUserManager : UserManager<User>
    {
        public ApplicationUserManager(ApplicationUserStore store)
            : base(store)
        {

        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var appDbContext = context.Get<ApplicationContext>();
            var appUserManager = new ApplicationUserManager(new ApplicationUserStore(appDbContext));

            // Configure validation logic for usernames
            appUserManager.UserValidator = new UserValidator<User>(appUserManager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            appUserManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = false,
                RequireLowercase = true,
                RequireUppercase = true,
            };


            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                appUserManager.UserTokenProvider = new DataProtectorTokenProvider<User>(dataProtectionProvider.Create("UserToken"))
                {
                    //Code for email confirmation and reset password life time
                    TokenLifespan = TimeSpan.FromDays(7)
                };
            }

            return appUserManager;
        }
    }
}
