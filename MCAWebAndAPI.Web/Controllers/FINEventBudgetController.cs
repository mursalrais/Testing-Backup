using MCAWebAndAPI.Service.Finance;
using System.Web.Mvc;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using System.Linq;
using System;

namespace MCAWebAndAPI.Web.Controllers
{
    public class FINEventBudgetController : Controller
    {
        private const string SESSION_SITE_URL = "SiteUrl";
        IEventBudgetService service;

        public FINEventBudgetController()
        {
            service = new EventBudgetService();
        }


        public ActionResult Create(string siteUrl = null, int? id = null)
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;

            service.SetSiteUrl(siteUrl);
            SessionManager.Set(SESSION_SITE_URL, siteUrl);

            var viewModel = service.GetEventBudget(id);
            return View(viewModel);
        }

        public JsonResult GetGLMaster()
        {
            var siteUrl = SessionManager.Get<string>(SESSION_SITE_URL);
            service.SetSiteUrl(siteUrl);

            var glMasters = Shared.GetGLMaster(siteUrl);

            return Json(glMasters.Select(e => new
            {
                Value = e.ID.HasValue ? Convert.ToString(e.ID) : string.Empty,
                Text = e.Title
            }), JsonRequestBehavior.AllowGet);
        }

    }
}