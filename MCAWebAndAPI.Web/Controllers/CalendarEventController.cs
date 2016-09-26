using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MCAWebAndAPI.Service.HR.Common;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MCAWebAndAPI.Web.Resources;
using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Web.Filters;

namespace MCAWebAndAPI.Web.Controllers
{
    public class CalendarEventController : Controller
    {
        ICalendarService calendarService;

        public CalendarEventController()
        {
            calendarService = new CalendarService();
        }


        public ActionResult CreateCalendarEvent(string siteUrl = null)
        {

            if (System.Web.HttpContext.Current.Session.Keys.Count > 0)
                System.Web.HttpContext.Current.Session.Clear();

            // MANDATORY: Set Site URL
            calendarService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            System.Web.HttpContext.Current.Session["SiteUrl"] = siteUrl ?? ConfigResource.DefaultHRSiteUrl;

            var viewModel = calendarService.GetPopulatedModel();

            return View("Create", viewModel);
        }

        [HttpPost]
        public ActionResult Create(CalendarEventVM viewModel)
        {
            // Check whether error is found
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("500", "Internal Server Error");
                return Json(new { success = false, urlToRedirect = "google.com" },
                JsonRequestBehavior.AllowGet);
            }

            calendarService.SetSiteUrl(System.Web.HttpContext.Current.Session["SiteUrl"] as string);

            
            calendarService.CreateHeader(viewModel);

            // Clear session variables
            System.Web.HttpContext.Current.Session.Clear();

            // Return JSON
            return Json(new { success = true, urlToRedirect = "google.com" },
                JsonRequestBehavior.AllowGet);
        }

    }
}