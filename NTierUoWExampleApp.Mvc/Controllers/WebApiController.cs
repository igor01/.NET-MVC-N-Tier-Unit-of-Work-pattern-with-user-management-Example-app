using NTierUoWExampleApp.Core.BindingModels.JqGrid;
using NTierUoWExampleApp.Core.BindingModels.WebApi;
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
    public class WebApiController : BaseController
    {
        private WebApiService service;
        public WebApiController()
        {
            service = new WebApiService();
        }


        public ActionResult Index()
        {
            ViewBag.AppUrl = GetAppUrl();
            ViewBag.UserId = GetUserId();
            return View();
        }

        public ActionResult GetClients(JqGridPostData jqGridPostData)
        {
            try
            {
                var userId = GetUserId();

                JqGridSearchModel search = new JqGridSearchModel();
                search.Search = jqGridPostData.Search;
                search.Filters = jqGridPostData.GetFilter();
                search.PageSize = jqGridPostData.PageSize;
                search.SortColumn = jqGridPostData.SortColumn;
                search.SortOrder = jqGridPostData.SortOrder;
                search.Page = jqGridPostData.Page;

                var result = service.GetWebApiClients(search);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpGet]
        public ActionResult CreateClient()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateClient(CreateClientViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await service.CreateClient(model, GetUserId(), GetUsername());
                    return RedirectToAction("Index");
                }
                catch (ValidationException ve)
                {
                    ModelState.AddModelError("", ve.Message);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", "Something went wrong. Contact your administrator.");
                }
            }

            return View(model);
        }

        public async Task<ActionResult> DeleteClient(Guid clientId)
        {
            var message = string.Empty;
            try
            {
                await service.DeleteClient(clientId, GetUserId(), GetUsername());
                return Json(new { success = true, message = "Client is successfully deleted." });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "Something went wrong. Contact your administrator." });
            }
        }

        [HttpGet]
        public async Task<ActionResult> EditClient(Guid clientId)
        {
            EditClientViewModel model = new EditClientViewModel(await service.GetClientByIdAsync(clientId));
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> EditClient(EditClientViewModel model)
        {
            try
            {
                await service.EditClient(model, GetUserId(), GetUsername());
                return RedirectToAction("Index", "WebApi");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                return View(model);
            }

        }

        public ActionResult ClientUsers(Guid clientId)
        {
            ViewBag.AppUrl = GetAppUrl();
            ViewBag.ClientId = clientId;
            return View();
        }

        public ActionResult GetClientUsers(Guid id, JqGridPostData jqGridPostData)
        {
            try
            {
                var userId = GetUserId();

                JqGridSearchModel search = new JqGridSearchModel();
                search.Search = jqGridPostData.Search;
                search.Filters = jqGridPostData.GetFilter();
                search.PageSize = jqGridPostData.PageSize;
                search.SortColumn = jqGridPostData.SortColumn;
                search.SortOrder = jqGridPostData.SortOrder;
                search.Page = jqGridPostData.Page;

                var result = service.GetClientUsersRefreshTokens(id, search);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<ActionResult> RevokeUser(string refreshTokenId)
        {
            var message = string.Empty;
            try
            {
                await service.RevokeUser(refreshTokenId, GetUserId(), GetUsername());
                return Json(new { success = true, message = "User is successfully revoked." });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = "Something went wrong. Contact your administrator." });
            }
        }
    }
}