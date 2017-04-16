using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;
using NTierUoWExampleApp.Common.Enum.Email;
using NTierUoWExampleApp.Common.Enum.User;
using NTierUoWExampleApp.Common.Utility;
using NTierUoWExampleApp.Core.BindingModels.Account;
using NTierUoWExampleApp.Core.BindingModels.Common;
using NTierUoWExampleApp.Core.BindingModels.JqGrid;
using NTierUoWExampleApp.Core.Notification.Email;
using NTierUoWExampleApp.Core.Notification.Email.Config;
using NTierUoWExampleApp.Core.Utility.JqGrid;
using NTierUoWExampleApp.Core.Utility.Validation;
using NTierUoWExampleApp.DAL.IdentityManager;
using NTierUoWExampleApp.DAL.Models.Account;
using NTierUoWExampleApp.DAL.UnitOfWork;
using NTierUoWExampleApp.DAL.UnitOfWork.IUoW;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NTierUoWExampleApp.Core.Services
{
    public class AccountService
    {
        private IUnitOfWork unitOfWork;
        private ApplicationUserManager UserManager = null;
        private ApplicationRoleManager RoleManager = null;
        private ApplicationLogger logger;
        private IDataProtectionProvider dataProtectionProvider { get; set; }
        public AccountService(IDataProtectionProvider dataProtectionProvider)
        {
            unitOfWork = new AppUnitOfWork();
            logger = new ApplicationLogger(unitOfWork);
            this.dataProtectionProvider = dataProtectionProvider;
            UserManagerInit();
        }

        public AccountService(IDataProtectionProvider dataProtectionProvider, IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            logger = new ApplicationLogger(unitOfWork);
            this.dataProtectionProvider = dataProtectionProvider;
            UserManagerInit();
        }

        private void UserManagerInit()
        {
            UserManager = new ApplicationUserManager(new ApplicationUserStore(new DAL.DBInitialization.ApplicationContext()));
            RoleManager = new ApplicationRoleManager(new ApplicationRoleStore(new DAL.DBInitialization.ApplicationContext()));


            UserManager.UserValidator = new UserValidator<User>(UserManager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            UserManager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            UserManager.UserLockoutEnabledByDefault = Config.UserLockoutEnabledByDefault;
            UserManager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(Config.DefaultAccountLockoutTimeSpan);
            UserManager.MaxFailedAccessAttemptsBeforeLockout = Config.MaxFailedAccessAttemptsBeforeLockout;

            UserManager.EmailService = new EmailService();

            UserManager.UserTokenProvider = new DataProtectorTokenProvider<User>(this.dataProtectionProvider.Create("UserToken"))
            {
                TokenLifespan = TimeSpan.FromDays(7)
            };
        }

        public IUnitOfWork UnitOfWork
        {
            get { return this.unitOfWork; }
        }

        #region user managment

        public async Task<UserViewModel> Authenticate(string username, string password)
        {
            try
            {
                string message = null;
                User user = await this.UserManager.FindByNameAsync(username);
                if (user != null)
                {
                    var tempUser = await UserManager.FindAsync(username, password);
                    UserViewModel validCredentials = null;

                    if (tempUser != null)
                    {
                        validCredentials = new UserViewModel(tempUser);
                    }


                    //account can be locked by administrator
                    if (user.PermanentLock)
                    {
                        message = "This account has been locked.";
                        throw new ValidationException(message);
                    }


                    // When user is locked, this check is done to ensure that even if the credentials are valid
                    // the user can not login until the lockout duration has passed
                    if (await UserManager.IsLockedOutAsync(user.Id))
                    {
                        message = string.Format("Your account has been locked out for {0} minutes due to multiple failed login attempts.", ((int)(user.LockoutEndDateUtc.Value - DateTime.UtcNow).TotalMinutes).ToString());
                        throw new ValidationException(message);
                    }

                    // if user is subject to lockouts and the credentials are invalid
                    // record the failure and check if user is lockedout and display message, otherwise,
                    // display the number of attempts remaining before lockout
                    else if (await UserManager.GetLockoutEnabledAsync(user.Id) && validCredentials == null)
                    {
                        // Record the failure which also may cause the user to be locked out
                        await UserManager.AccessFailedAsync(user.Id);

                        if (await UserManager.IsLockedOutAsync(user.Id))
                        {
                            message = string.Format("Your account has been locked out for {0} minutes due to multiple failed login attempts.", Config.DefaultAccountLockoutTimeSpan.ToString());
                        }
                        else
                        {
                            int accessFailedCount = await UserManager.GetAccessFailedCountAsync(user.Id);

                            int attemptsLeft = Config.MaxFailedAccessAttemptsBeforeLockout - accessFailedCount;
                            message = string.Format("Invalid credentials. You have {0} more attempt(s) before your account gets locked out.", attemptsLeft);

                        }

                        throw new ValidationException(message);

                    }
                    else if (validCredentials == null)
                    {
                        throw new ValidationException("Invalid credentials. Please try again.");
                    }

                    // All is ok
                    await UserManager.ResetAccessFailedCountAsync(validCredentials.Id);

                    return validCredentials; // Return user object

                }
                else
                {
                    throw new ValidationException("Invalid username !");
                }

            }
            catch (ValidationException exe)
            {
                throw exe;
            }
            catch (Exception e)
            {
                await LogError("Authenticate", e);
                Trace.TraceError(string.Format("Authenticate in account service error: {0}", e.Message));
                throw e;
            }
        }
        public async Task ResendEmailConfirmationTokenAsync(string userId, string adminUserId, string adminUsername)
        {
            try
            {
                var user = await GetUserById(userId);
                if (await UserManager.IsEmailConfirmedAsync(user.Id))
                {
                    throw new ValidationException("Email not sent. User account is already confirmed.");
                }

                string code = Uri.EscapeDataString(await UserManager.GenerateEmailConfirmationTokenAsync(user.Id));

                MessageConfig mailConfig = new MessageConfig();
                mailConfig.Token = code;
                mailConfig.UserId = user.Id;
                mailConfig.MailType = EmailTypeEnum.AccountCreated;
                mailConfig.UserName = user.UserName;
                mailConfig.Email = user.Email;

                Message msg = EmailService.MessageFromTemplate(mailConfig);

                await UserManager.SendEmailAsync(user.Id, msg.Subject, msg.Body);

            }
            catch (Exception e)
            {
                await LogError("ResendEmailConfirmationTokenAsync", e);
                Trace.TraceError(string.Format("ResendEmailConfirmationTokenAsync error: {0}", e.Message));
                throw e;
            }
        }
        public JqGridResultViewModel<UserViewModel> GetUsers(JqGridSearchModel jqGridSearchModel)
        {
            try
            {
                //TODO add support for join query
                if (jqGridSearchModel != null)
                {
                    List<UserViewModel> model = new List<UserViewModel>();


                    JqGridPaging<User, UserViewModel> JqGridPaging = new JqGridPaging<User, UserViewModel>(unitOfWork.UserRepository.GetAllQueryable(), "Id", jqGridSearchModel);
                    var users = JqGridPaging.GetResult();
                    var test = UserManager.Users.ToList();

                    if (users != null)
                    {
                        foreach (var user in users)
                        {
                            model.Add(new UserViewModel(user));
                        }
                    }

                    return JqGridPaging.GetJqGridResult(model);
                }
                return null;
            }
            catch (Exception e)
            {
                LogError("GetUsers", e);

                Trace.TraceError("GetUsers in user service error : {0}", e.Message);
                throw e;
            }
        }
        public async Task<ClaimsIdentity> CreateIdentityAsync(UserViewModel user, string authType)
        {
            try
            {
                User us = new User()
                {
                    AccessFailedCount = user.AccessFailedCount,
                    CreatedBy = user.CreatedBy,
                    CreatedDate = user.CreatedDate,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    FirstName = user.FirstName,
                    Id = user.Id,
                    LastLogin = user.LastLogin,
                    LastLoginUtc = user.LastLoginUtc,
                    LastName = user.LastName,
                    LockoutEnabled = user.LockoutEnabled,
                    LockoutEndDateUtc = user.LockoutEndDateUtc,
                    ModifiedBy = user.ModifiedBy,
                    ModifiedDate = user.ModifiedDate,
                    NotificationEnabled = user.NotificationEnabled,
                    PasswordHash = user.PasswordHash,
                    PermanentLock = user.PermanentLock,
                    PhoneNumber = user.PhoneNumber,
                    PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                    SecurityStamp = user.SecurityStamp,
                    Status = user.Status,
                    TwoFactorEnabled = user.TwoFactorEnabled,
                    UserName = user.UserName
                };
                return await UserManager.CreateIdentityAsync(us, authType);
            }
            catch (Exception e)
            {
                await LogError("CreateIdentityAsync", e);
                Trace.TraceError(string.Format("CreateIdentityAsync in account service error: {0}", e.Message));
                throw e;
            }
        }
        public async Task<IdentityResult> CreateUser(CreateUserViewModel model)
        {
            User user = null;
            try
            {
                //check if user already exist
                user = await UserManager.FindByNameAsync(model.Email);
                if (user != null) throw new ValidationException("User with this email address is already in the system.");

                user = new User();

                user.Id = Guid.NewGuid().ToString();
                user.CreatedDate = DateTime.UtcNow;
                user.CreatedBy = model.CreatedBy;
                user.Email = model.Email;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.LockoutEnabled = true;
                user.PhoneNumber = model.PhoneNumber;
                user.UserName = model.Email;

                IdentityResult createUserResult = await UserManager.CreateAsync(user);

                if (createUserResult.Succeeded)
                {
                    MethodActionResult addToRoleResult = null;

                    try
                    {
                        addToRoleResult = await AddUserToRole(user.Id, (model.UserAccessRole).Replace(" ", ""));
                    }
                    catch (Exception e)
                    {
                        await UserManager.DeleteAsync(user);
                        throw new ValidationException("User role does not exist.");
                    }


                    if (addToRoleResult.Succeeded)
                    {

                        //notify user by email
                        string code = Uri.EscapeDataString(await UserManager.GenerateEmailConfirmationTokenAsync(user.Id));

                        MessageConfig mailConfig = new MessageConfig();
                        mailConfig.Token = code;
                        mailConfig.UserId = user.Id;
                        mailConfig.MailType = EmailTypeEnum.AccountCreated;
                        mailConfig.UserName = user.UserName;
                        mailConfig.Email = user.Email;

                        Message msg = EmailService.MessageFromTemplate(mailConfig);
                        try
                        {
                            await UserManager.SendEmailAsync(user.Id, msg.Subject, msg.Body);
                        }
                        catch (Exception e)
                        {
                            //if sending email fails do rollback
                            if (addToRoleResult.Succeeded)
                            {
                                await UserManager.RemoveFromRoleAsync(user.Id, model.UserAccessRole.Replace(" ", ""));
                            }

                            if (createUserResult.Succeeded)
                            {
                                await UserManager.DeleteAsync(user);
                            }

                            await LogError("CreateUser, Send email for create user", e);

                            Trace.TraceError(string.Format("Send email for create user error: {0}", e.Message));
                            throw new ValidationException("Email notification sending problem.");
                        }
                    }
                    else
                    {
                        await UserManager.DeleteAsync(user);
                        throw new ValidationException("Add user role problem.");
                    }
                }
                return createUserResult;
            }
            catch (Exception e)
            {
                await LogError("CreateUser", e);
                Trace.TraceError(string.Format("CreateUser in account service error: {0}", e.Message));
                throw e;
            }
        }
        public async Task<EditUserViewModel> GetUserForEdit(string userId)
        {
            try
            {
                EditUserViewModel user = new EditUserViewModel(await GetUserById(userId));
                user.UserAccessRole = GetSystemRoleForUser(userId, true);
                user.UserAccessRoles = GetSystemRoles();
                return user;
            }
            catch (Exception e)
            {
                await LogError("GetUserForEdit", e);
                Trace.TraceError(string.Format("GetUserForEdit in account service error: {0}", e.Message));
                throw e;
            }
        }
        public async Task<IdentityResult> EditUser(EditUserViewModel model)
        {
            try
            {
                var user = await GetUserById(model.Id);
                var prevLock = user.LockoutEndDateUtc;

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.ModifiedBy = model.ModifiedBy;
                user.ModifiedDate = DateTime.UtcNow;
                user.PhoneNumber = model.PhoneNumber;

                string currentRole = GetSystemRoleForUser(model.Id, false);

                if (model.UserAccessRole != GetSystemRoleForUser(model.Id, true))
                {
                    var systemRole = (from s in unitOfWork.RoleRepository.GetAllQueryable()
                                          .Where(x => x.Name == model.UserAccessRole.Replace(" ", ""))
                                      select s).ToList();

                    if (systemRole != null && systemRole.Count > 0)
                    {
                        //remove current role
                        var result = await UserManager.RemoveFromRoleAsync(user.Id, currentRole);

                        if (result.Succeeded)
                        {
                            var addToRoleResult = await AddUserToRole(user.Id, (model.UserAccessRole).Replace(" ", ""));

                            if (!addToRoleResult.Succeeded)
                            {
                                throw new ValidationException("Could not update role.");
                            }
                        }
                    }
                    else
                    {
                        throw new ValidationException("Selected role not found.");
                    }
                }


                if (model.Locked)
                {
                    user.LockoutEndDateUtc = DateTime.UtcNow.AddYears(100);
                    user.PermanentLock = true;
                }
                else
                {
                    user.LockoutEndDateUtc = null;
                    user.PermanentLock = false;
                }

                var res = await UserManager.UpdateAsync(user);
                return res;
            }
            catch (Exception e)
            {
                await LogError("EditUser", e);
                Trace.TraceError(string.Format("EditUser in account service error: {0}", e.Message));
                throw e;
            }
        }
        public async Task<IdentityResult> DeleteUser(string userId, string deletedById, string deletedByUsername)
        {
            try
            {
                var user = await UserManager.FindByIdAsync(userId);
                if (user == null)
                {
                    throw new ValidationException("User does not exist.");
                }

                //do not delete initial user
                if (user.UserName == "osls.platform@ericsson.com")
                {
                    throw new ValidationException("You can not delete this account.");
                }

                var rolesForUser = await UserManager.GetRolesAsync(userId);


                var logins = user.Logins;

                foreach (var login in logins.ToList())
                {
                    await UserManager.RemoveLoginAsync(login.UserId, new UserLoginInfo(login.LoginProvider, login.ProviderKey));
                }

                if (rolesForUser.Count() > 0)
                {
                    foreach (var item in rolesForUser.ToList())
                    {
                        // item should be the name of the role
                        var result = await UserManager.RemoveFromRoleAsync(user.Id, item);
                    }
                }

                var res = await UserManager.DeleteAsync(user);
                return res;
            }
            catch (Exception e)
            {
                await LogError("DeleteUser", e);
                Trace.TraceError(string.Format("DeleteUser in account service error: {0}", e.Message));
                throw e;
            }
        }
        public async Task ForgotPassword(string userId, string adminId, string adminUsername)
        {
            try
            {
                var user = await UserManager.FindByIdAsync(userId);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    throw new ValidationException("User does not exist or is not confirmed.");
                }


                //notify user by email
                string code = Uri.EscapeDataString(await UserManager.GeneratePasswordResetTokenAsync(user.Id));

                MessageConfig mailConfig = new MessageConfig();
                mailConfig.Token = code;
                mailConfig.UserId = user.Id;
                mailConfig.MailType = EmailTypeEnum.PasswordReset;
                mailConfig.UserName = user.UserName;
                mailConfig.Email = user.Email;

                Message msg = EmailService.MessageFromTemplate(mailConfig);
                try
                {
                    await UserManager.SendEmailAsync(user.Id, msg.Subject, msg.Body);
                }
                catch (Exception e)
                {
                    Trace.TraceError(string.Format("Send email for password reset error: {0}", e.Message));
                    throw new ValidationException("Email notification sending problem.");
                }
            }
            catch (Exception e)
            {
                await LogError("ForgotPassword", e);
                Trace.TraceError(string.Format("ForgotPassword in account service error: {0}", e.Message));
                throw e;
            }
        }
        public async Task<MethodActionResult> AddUserToRole(string userId, string roleName)
        {
            try
            {
                var role = unitOfWork.RoleRepository.GetAllQueryable()
                    .Where(x => x.Name == roleName.Replace(" ", "")).ToList();

                if (role == null || role.Count == 0)
                {
                    return new MethodActionResult() { Succeeded = false, Error = "Role not found" };
                }

                unitOfWork.UserRoleRepository.Create(new UserRole()
                {
                    RoleId = role.FirstOrDefault().Id,
                    UserId = userId,
                    RoleName = roleName
                });

                await unitOfWork.SaveChangesAsync();

                return new MethodActionResult() { Succeeded = true };
            }
            catch (Exception e)
            {
                await LogError("AddUserToRole", e);
                Trace.TraceError(string.Format("AddUserToRole in account service error: {0}", e.Message));
                return new MethodActionResult() { Succeeded = false, Error = e.Message };
            }
        }
        public async Task<IdentityResult> ConfirmUser(string userId, string code)
        {
            try
            {
                return await UserManager.ConfirmEmailAsync(userId, code);
            }
            catch (Exception e)
            {
                await LogError("ConfirmUser", e);
                Trace.TraceError(string.Format("ConfirmUser in account service error: {0}", e.Message));
                throw e;
            }
        }
        public async Task<User> GetUserById(string userId)
        {
            try
            {
                var user = await UserManager.FindByIdAsync(userId);
                if (user == null) throw new ValidationException("User not found.");
                return user;
            }
            catch (Exception e)
            {
                await LogError("GetUserById", e);
                Trace.TraceError(string.Format("GetUserById in account service error: {0}", e.Message));
                throw e;
            }
        }
        public async Task<User> GetUserByUsernameAsync(string username)
        {
            try
            {
                var user = await unitOfWork.UserRepository.FindByUserNameAsync(username);
                //var user = await UserManager.FindByNameAsync(username);
                if (user == null) throw new ValidationException("User not found.");
                return user;
            }
            catch (Exception e)
            {
                await LogError("GetUserByUsername", e);
                Trace.TraceError(string.Format("GetUserByUsername in account service error: {0}", e.Message));
                throw e;
            }
        }
        public async Task<IdentityResult> RegisterUser(RegisterUserViewModel model)
        {
            try
            {
                User user = await UserManager.FindByIdAsync(model.Id);
                if (user == null) throw new ValidationException("User not found");

                // Map data

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.PhoneNumber = model.PhoneNumber;


                IdentityResult rez = await UserManager.UpdateAsync(user);
                if (rez.Succeeded)
                {
                    if (user.PasswordHash == null)
                    {
                        // User don't have password becasue it's registration process
                        rez = await UserManager.AddPasswordAsync(model.Id, model.Password);
                    }
                }
                return rez;
            }
            catch (Exception e)
            {
                await LogError("RegisterUser", e);
                Trace.TraceError(string.Format("RegisterUser in account service error: {0}", e.Message));
                throw e;
            }
        }
        public async Task<IdentityResult> ResetPassword(ResetPasswordViewModel model)
        {
            try
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    throw new ValidationException("User not found");
                }
                var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
                return result;
            }
            catch (Exception e)
            {
                await LogError("ResetPassword", e);
                Trace.TraceError(string.Format("ResetPassword in account service error: {0}", e.Message));
                throw e;
            }
        }
        public async Task<ManageAccountViewModel> GetUserForManage(string userId)
        {
            try
            {
                return new ManageAccountViewModel(await GetUserById(userId));
            }
            catch (Exception e)
            {
                await LogError("GetUserForManage", e);
                Trace.TraceError(string.Format("GetUserForManage in account service error: {0}", e.Message));
                throw e;
            }
        }
        public async Task<IdentityResult> ManageUser(ManageAccountViewModel model)
        {
            try
            {
                var user = await GetUserById(model.Id);

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.ModifiedBy = model.ModifiedBy;
                user.ModifiedDate = DateTime.UtcNow;
                user.PhoneNumber = model.PhoneNumber;

                var res = await UserManager.UpdateAsync(user);
                return res;
            }
            catch (Exception e)
            {
                await LogError("ManageUser", e);
                Trace.TraceError(string.Format("ManageUser in account service error: {0}", e.Message));
                throw e;
            }
        }
        public async Task<ChangePasswordViewModel> GetUserForChangePassword(string userId)
        {
            try
            {
                return new ChangePasswordViewModel(await GetUserById(userId));
            }
            catch (Exception e)
            {
                await LogError("GetUserForChangePassword", e);
                Trace.TraceError(string.Format("GetUserForChangePassword in account service error: {0}", e.Message));
                throw e;
            }
        }
        public async Task<IdentityResult> ChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                return await UserManager.ChangePasswordAsync(model.Id, model.OldPassword, model.NewPassword);
            }
            catch (Exception e)
            {
                await LogError("ChangePassword", e);
                Trace.TraceError(string.Format("ChangePassword in account service error: {0}", e.Message));
                throw e;
            }
        }
        public async Task ForgotPassword(string email)
        {
            try
            {
                //validate email input
                ValidationHelper validationHelper = new ValidationHelper();
                if (!validationHelper.IsValidEmail(email))
                {
                    throw new ValidationException("Email is not in correct format.");
                }

                //check if user exist in the system
                var user = await UserManager.FindByEmailAsync(email);
                if (user == null)
                {
                    throw new ValidationException("User does not exist.");
                }


                //notify user by email
                string code = Uri.EscapeDataString(await UserManager.GeneratePasswordResetTokenAsync(user.Id));

                MessageConfig mailConfig = new MessageConfig();
                mailConfig.Token = code;
                mailConfig.UserId = user.Id;
                mailConfig.MailType = EmailTypeEnum.PasswordReset;
                mailConfig.UserName = user.UserName;
                mailConfig.Email = user.Email;

                Message msg = EmailService.MessageFromTemplate(mailConfig);
                try
                {
                    await UserManager.SendEmailAsync(user.Id, msg.Subject, msg.Body);
                }
                catch (Exception e)
                {
                    await LogError("ForgotPassword, Send email for password reset", e);
                    Trace.TraceError(string.Format("Send email for password reset error: {0}", e.Message));
                    throw new ValidationException("Email notification sending problem.");
                }

            }
            catch (Exception e)
            {
                await LogError("ForgotPassword", e);
                Trace.TraceError(string.Format("ForgotPassword in account service error: {0}", e.Message));
                throw e;
            }
        }
        public async Task SetLastLoginDate(UserViewModel user)
        {
            try
            {
                var u = await GetUserById(user.Id);
                u.LastLogin = DateTime.Now;
                u.LastLoginUtc = DateTime.UtcNow;

                await UserManager.UpdateAsync(u);
            }
            catch (Exception e)
            {
                await LogError("SetLastLoginDate", e);
                Trace.TraceError(string.Format("SetLastLoginDate in account service error: {0}", e.Message));
                //throw e;
            }
        }
        public async Task<RegisterUserViewModel> GetRegisterViewModel(string userId)
        {
            try
            {
                var user = await GetUserById(userId);
                return new RegisterUserViewModel(user);
            }
            catch (Exception e)
            {
                await LogError("GetRegisterViewModel", e);
                Trace.TraceError(string.Format("GetRegisterViewModel in account service error: {0}", e.Message));
                throw e;
            }
        }
        public async Task<UserViewModel> GetUserViewModel(string userId)
        {
            try
            {
                var user = await GetUserById(userId);
                return new UserViewModel(user);
            }
            catch (Exception e)
            {
                await LogError("GetUserViewModel", e);
                Trace.TraceError(string.Format("GetUserViewModel in account service error: {0}", e.Message));
                throw e;
            }
        }
        public List<string> GetSystemRoles()
        {
            return new List<string> { "Administrator", "Viewer" };
        }
        public string GetSystemRoleForUser(string userId, bool splitCamelCase)
        {
            try
            {
                var role = (from r in unitOfWork.UserRoleRepository.GetAllQueryable()
                    .Where(x => x.UserId == userId)
                            select r).ToList();


                if (role != null && role.Count > 0)
                {
                    if (splitCamelCase)
                    {
                        return Utility.Converter.AppConverter.SplitCamelCase(role.FirstOrDefault().RoleName);
                    }
                    else
                    {
                        return role.FirstOrDefault().RoleName;
                    }
                }

                return "N/A";

            }
            catch (Exception e)
            {
                LogError("GetSystemRoleForUser", e);
                Trace.TraceError(string.Format("GetSystemRoleForUser error: {0}", e.Message));
                throw e;
            }
        }
        public List<BrowsingHistoryViewModel> GetUserBrowsingHistory(string userId)
        {
            try
            {
                List<BrowsingHistoryViewModel> model = new List<BrowsingHistoryViewModel>();

                //get user history for last 3 months
                DateTime time = DateTime.UtcNow.AddMonths(-3);
                var history = (from h in unitOfWork.BrowsingHistoryRepository.GetAllQueryable()
                               .Where(x => x.UserId == userId && x.DateTimeUtc > time)
                               .OrderByDescending(x => x.DateTimeUtc)
                               select h).ToList();

                if (history != null && history.Count > 0)
                {
                    foreach (var hist in history)
                    {
                        model.Add(new BrowsingHistoryViewModel(hist));
                    }
                }

                return model;
            }
            catch (Exception e)
            {
                LogError("BrowsingHistoryViewModel", e);
                Trace.TraceError(string.Format("GetUserBrowsingHistory in account service error: {0}", e.Message));
                throw e;
            }
        }

        #endregion

        #region user connections
        public async Task RegisterUserWebClient(string username, string signalRconnectionId, string connectedUrl, string userAgent)
        {
            try
            {
                var user = await GetUserByUsernameAsync(username);
                UserWebClientConnection connection = new UserWebClientConnection()
                {
                    ConnectedUrl = connectedUrl,
                    SignalRConnectionId = signalRconnectionId,
                    UserId = user.Id,
                    Username = username,
                    UserAgent = userAgent
                };

                unitOfWork.UserWebClientConnectionRepository.Create(connection);
                await unitOfWork.SaveChangesAsync();
                await AddPageToWebBrowsingHistory(username, user.Id, connectedUrl, userAgent);
            }
            catch (Exception e)
            {
                await LogError("RegisterUserWebClient", e);
                Trace.TraceError(string.Format("RegisterUserWebClient error: {0}", e.Message));
                throw e;
            }
        }
        public async Task UnRegisterUserWebClient(string signalRconnectionId)
        {
            try
            {
                var connections = (from c in unitOfWork.UserWebClientConnectionRepository.GetAllQueryable()
                                   .Where(x => x.SignalRConnectionId == signalRconnectionId)
                                   select c).ToList();

                if (connections == null || connections.Count == 0) throw new ValidationException("Connection not found.");

                unitOfWork.UserWebClientConnectionRepository.Delete(connections.FirstOrDefault());
                await unitOfWork.SaveChangesAsync();
            }
            catch (Exception e)
            {
                await LogError("UnRegisterUserWebClient", e);
                Trace.TraceError(string.Format("UnRegisterUserWebClient error: {0}", e.Message));
                throw e;
            }
        }
        public async Task UnRegisterAllUserWebClients(string userId, string username, string userAgent)
        {
            try
            {
                var connections = (from c in unitOfWork.UserWebClientConnectionRepository.GetAllQueryable()
                                   .Where(x => x.UserId == userId && x.UserAgent == userAgent)
                                   select c).ToList();

                if (connections != null && connections.Count > 0)
                {
                    foreach (var connection in connections)
                    {
                        unitOfWork.UserWebClientConnectionRepository.Delete(connection);
                    }

                    await unitOfWork.SaveChangesAsync();
                }

                await SetUserConnectionStatus(username);
            }
            catch (Exception e)
            {
                await LogError("UnRegisterAllUserWebClients", e);
                Trace.TraceError(string.Format("UnRegisterAllUserWebClients error: {0}", e.Message));
                throw e;
            }
        }
        public async Task SetUserConnectionStatus(string username)
        {
            try
            {
                var user = await GetUserByUsernameAsync(username);

                //check if user has any connected pages
                var connections = (from c in unitOfWork.UserWebClientConnectionRepository.GetAllQueryable()
                                   .Where(x => x.UserId == user.Id)
                                   select c).ToList();

                if (connections == null || connections.Count == 0)
                {
                    user.OnlineStatus = UserOnlineStatusEnum.Offline.ToString();
                    unitOfWork.UserRepository.Update(user);
                    await unitOfWork.SaveChangesAsync();
                }
                else
                {
                    if (user.OnlineStatus == UserOnlineStatusEnum.Offline.ToString())
                    {
                        user.OnlineStatus = UserOnlineStatusEnum.Online.ToString();
                        unitOfWork.UserRepository.Update(user);
                        await unitOfWork.SaveChangesAsync();
                    }
                }
            }
            catch (Exception e)
            {
                await LogError("SetUserConnectionStatus", e);
                Trace.TraceError(string.Format("SetUserConnectionStatus error: {0}", e.Message));
                throw e;
            }
        }
        public async Task AddPageToWebBrowsingHistory(string username, string userId, string pageUrl, string userAgent)
        {
            try
            {
                BrowsingHistory browsingHistory = new BrowsingHistory()
                {
                    DateTimeServer = DateTime.Now,
                    DateTimeUtc = DateTime.UtcNow,
                    PageUrl = pageUrl,
                    UserAgent = userAgent,
                    UserId = userId,
                    Username = username
                };

                unitOfWork.BrowsingHistoryRepository.Create(browsingHistory);
                await unitOfWork.SaveChangesAsync();
            }
            catch (Exception e)
            {
                await LogError("AddPageToWebBrowsingHistory", e);
                Trace.TraceError(string.Format("AddPageToWebBrowsingHistory error: {0}", e.Message));
            }
        }
        
        #endregion

        #region logger
        public async Task LogError(string errorMethodName, Exception e)
        {
            try
            {
                string innerExceptionMessage = e.InnerException != null ? e.InnerException.Message : null;
                await logger.LogError(errorMethodName, Common.Enum.Logger.LogTypeEnum.Error.ToString(), e.Message, innerExceptionMessage, e.StackTrace);
            }
            catch (Exception ex)
            {

            }
        }
        #endregion
    }
}
