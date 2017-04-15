using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using NTierUoWExampleApp.Core.BindingModels.Account;
using NTierUoWExampleApp.Core.BindingModels.JqGrid;
using NTierUoWExampleApp.Core.Services;
using NTierUoWExampleApp.Mvc.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NTierUoWExampleApp.Mvc.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class UserController : BaseController
    {
        private AccountService service;
        public UserController()
        {
            service = new AccountService(Startup.DataProtectionProvider);
        }


        public ActionResult Index()
        {
            ViewBag.AppUrl = GetAppUrl();
            ViewBag.UserId = GetUserId();
            return View();
        }

        [AllowAnonymous]
        public async Task<ActionResult> Register(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            IdentityResult result;
            try
            {
                result = await service.ConfirmUser(userId, code);
            }
            catch (InvalidOperationException ioe)
            {
                // ConfirmEmailAsync throws when the userId is not found.
                ViewBag.errorMessage = ioe.Message;
                return View("Error");

            }
            if (result.Succeeded)
            {
                // Return model to fill password and other missing information
                var model = await service.GetRegisterViewModel(userId);
                model.Code = code;
                return View(model);
            }
            else
            {
                ViewBag.errorMessage = result.Errors.FirstOrDefault();
                return View("Error");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await service.RegisterUser(model);
                if (result.Succeeded)
                {
                    var user = await service.GetUserViewModel(model.Id);
                    await SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AddErrors(result);
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }


        public ActionResult GetUsers(JqGridPostData jqGridPostData)
        {
            try
            {
                JqGridSearchModel search = new JqGridSearchModel();
                search.Search = jqGridPostData.Search;
                search.Filters = jqGridPostData.GetFilter();
                search.PageSize = jqGridPostData.PageSize;
                search.SortColumn = jqGridPostData.SortColumn;
                search.SortOrder = jqGridPostData.SortOrder;
                search.Page = jqGridPostData.Page;

                var result = service.GetUsers(search);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ActionResult Create()
        {
            CreateUserViewModel model = new CreateUserViewModel();
            model.UserAccessRoles = service.GetSystemRoles();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //check if currently sign in user is adding it self
                    if (GetUsername() == model.Email)
                    {
                        ShowInformation("Validation error. You can not create or add currently loged in user.");
                        model.UserAccessRoles = service.GetSystemRoles();
                        return View(model);
                    }

                    model.CreatedBy = GetUsername();
                    model.CreatedById = GetUserId();

                    IdentityResult result = await service.CreateUser(model);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error);
                        }
                    }
                }
                catch (ValidationException e)
                {
                    ShowInformation(e.Message);
                    model.UserAccessRoles = service.GetSystemRoles();
                    return View(model);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                }
            }
            model.UserAccessRoles = service.GetSystemRoles();
            return View(model);
        }

        public async Task<ActionResult> Edit(string userId)
        {
            //check if currently sign in user is adding it self
            if (GetUserId() == userId)
            {
                ShowInformation("You can not edit you own account. Please use Manage button.");
                return RedirectToAction("Index");
            }

            var model = await service.GetUserForEdit(userId);
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.ModifiedBy = GetUsername();
                    var result = await service.EditUser(model);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
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
            model.UserAccessRoles = service.GetSystemRoles();
            return View(model);
        }

        public async Task<ActionResult> Delete(string userId)
        {
            if (userId == GetUserId())
            {
                return Json(new { success = false, message = "You can not delete your own account." });
            }

            var message = string.Empty;
            List<string> errors = new List<string>();
            try
            {

                var result = await service.DeleteUser(userId, GetUserId(), GetUsername());

                if (result.Succeeded)
                {
                    return Json(new { success = true, message = "User is successfully deleted from the system." });
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        errors.Add(error);
                        message += error + ", ";
                    }
                    return Json(new { success = false, message = string.Format("Something went wrong. Contact your administrator. Error: {0}", message) });
                }

            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "Something went wrong. Contact your administrator." });
            }
        }

        public async Task<ActionResult> ForgotPassword(string userId)
        {
            try
            {
                if (GetUserId() == userId)
                {
                    return Json(new { success = false, message = "You can not reset your own password. Please use Change password option." });
                }

                await service.ForgotPassword(userId, GetUserId(), GetUsername());
                return Json(new { success = true, message = "Password reset email is sent to user." });

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

        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var result = await service.ResetPassword(model);

                if (result.Succeeded)
                {
                    return RedirectToAction("ResetPasswordConfirmation", "User");
                }

                AddErrors(result);
                return View();
            }
            catch (Exception e)
            {
                ShowError("Something went wrong. Contact your administrator.");
                return View();
            }
        }

        public async Task<ActionResult> ResendEmailConfirmationToken(string userId)
        {
            try
            {
                string adminUserId = GetUserId();
                string adminUsername = GetUsername();

                await service.ResendEmailConfirmationTokenAsync(userId, adminUserId, adminUsername);
                return Json(new { success = true, message = "Email confirmation token is sent to user." });

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

        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        #region Helpers
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
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
        #endregion
    }

}