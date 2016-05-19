using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Service.Asset;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Web.Filters;
using MCAWebAndAPI.Web.Resources;

namespace MCAWebAndAPI.Web.Controllers
{
    public class ASSLocationMasterController : Controller
    {
        ILocationMasterService _locationMasterService;

        public ASSLocationMasterController()
        {
            _locationMasterService = new LocationMasterService();
        }

        // GET: ASSLocationMaster
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Edit()
        {
            return View();
        }

        public ActionResult IFrame()
        {
            return View();
        }

        public ActionResult Create(string siteUrl = null, string userId = null)
        {
            // Clear Existing Session Variables if any
            if (System.Web.HttpContext.Current.Session.Keys.Count > 0)
                System.Web.HttpContext.Current.Session.Clear();

            // MANDATORY: Set Site URL
            _locationMasterService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            System.Web.HttpContext.Current.Session["SiteUrl"] = siteUrl ?? ConfigResource.DefaultBOSiteUrl;

            var viewModel = _locationMasterService.GetLocationMaster();

            return View(viewModel);
        }

        //public ActionResult SubmitEvent(LocationMasterVM[] Options)
        //{
        //    return Json(new { status = "Success", message = "Success" });
        //}

        [HttpPost]
        public JsonResult Submit(LocationMasterVM _data)
        {
            _locationMasterService.CreateLocationMaster(_data);
            if (!ModelState.IsValid)
            {
                return new JsonResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new { result = "Error" }
                };
            }

            //add to database

            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new { result = "Success" }
            };
        }
    }
}