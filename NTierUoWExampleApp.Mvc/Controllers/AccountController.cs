using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.ComponentModel.DataAnnotations;
using NTierUoWExampleApp.Core.Utility.Identity;
using NTierUoWExampleApp.Core.Services;
using NTierUoWExampleApp.Core.BindingModels.Account;

namespace NTierUoWExampleApp.Mvc.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private ApplicationUserManager _userManager;
        private AccountService service = null;

        public AccountController()
        {
            service = new AccountService(Startup.DataProtectionProvider);
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await service.Authenticate(model.Email, model.Password);

                    if (user != null)
                    {
                        await SignInAsync(user, false);

                        if (returnUrl != null && returnUrl.ToLowerInvariant().StartsWith("/account/logoff"))
                        {
                            return RedirectToLocal("/Home"); // Redirect to your default account page
                        }
                        return RedirectToLocal(returnUrl);
                    }
                }
                catch (ValidationException e)
                {
                    ModelState.AddModelError("", e.Message);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                }
            }

            // If we got this far, something failed, redisplay form

            return View(model);
        }

        public async Task<ActionResult> Manage(string userId)
        {
            var model = await service.GetUserForManage(userId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.ModifiedBy = User.Identity.GetUserName();
                    var result = await service.ManageUser(model);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error);
                        }
                    }

                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                }
            }

            return View(model);
        }

        public async Task<ActionResult> ChangePassword(string userId)
        {
            var model = await service.GetUserForChangePassword(userId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await service.ChangePassword(model);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error);
                        }
                    }

                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                }
            }

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ForgotPassword(string Email)
        {
            try
            {
                if (string.IsNullOrEmpty(Email))
                {
                    return Json(new { success = false, message = "Email address is required." });
                }

                await service.ForgotPassword(Email);
                return Json(new { success = true, message = "Password reset email is sent to your email address." });

            }
            catch (ValidationException e)
            {
                return Json(new { success = false, message = e.Message });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "Something went wrong. Contact your administrator." });
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LogOff()
        {
            try
            {
                var userId = GetUserId();
                var username = GetUsername();
                var userAgent = GetUserAgent();

                AuthenticationManager.SignOut();

                await service.UnRegisterAllUserWebClients(userId, username, userAgent);

                return RedirectToAction("Login", "Account");
            }
            catch (Exception e)
            {
                return RedirectToAction("Login", "Account");
            }          
        }

        #region helpers
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        private async Task SignInAsync(UserViewModel user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await service.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
            await service.SetLastLoginDate(user);
        }
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        #endregion
    }
}