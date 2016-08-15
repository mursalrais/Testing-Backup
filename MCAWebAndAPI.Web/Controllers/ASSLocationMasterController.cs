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
using System.Net;
using MCAWebAndAPI.Service.Resources;

namespace MCAWebAndAPI.Web.Controllers
{
    public class ASSLocationMasterController : Controller
    {
        ILocationMasterService _locationMasterService;

        public ASSLocationMasterController()
        {
            _locationMasterService = new LocationMasterService();
        }

        public ActionResult CreateLocationMaster(string siteUrl = null)
        {
            // MANDATORY: Set Site URL
            _locationMasterService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            ViewBag.Action = "CreateLocationMaster";

            // Get blank ViewModel
            var viewModel = _locationMasterService.GetPopulatedModel(siteUrl);

            // Return to the name of the view and parse the model
            return View("Create", viewModel);
        }

        public ActionResult Edit(int ID, string siteUrl = null)
        {
            // MANDATORY: Set Site URL
            _locationMasterService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            ViewBag.Action = "Edit";

            var viewModel = _locationMasterService.GetHeader(ID);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult CreateLocationMaster(FormCollection form, LocationMasterVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _locationMasterService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            int? headerID = null;
            try
            {
                headerID = _locationMasterService.CreateHeader(viewModel, viewModel.Province.Text, viewModel.OfficeName, viewModel.FloorName, viewModel.RoomName);
                if (headerID == 0)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return JsonHelper.GenerateJsonErrorResponse("This Location is Already Exist");
                }
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.MonthlyFee);
        }

        public JsonResult GetLocationMaster()
        {
            _locationMasterService.SetSiteUrl(SessionManager.Get<string>("SiteUrl"));

            var professionalmonthlyfee = GetFromLocationMasterExistingSession();
            return Json(professionalmonthlyfee.Select(e =>
                new
                {
                    e.ID,
                    e.Title
                }),
                JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<LocationMasterVM> GetFromLocationMasterExistingSession()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["LocationMaster"] as IEnumerable<LocationMasterVM>;
            var locationMaster = sessionVariable ?? _locationMasterService.GetLocationMaster();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["LocationMaster"] = locationMaster;
            return locationMaster;
        }

        public ActionResult UpdateProvince(string siteUrl = null)
        {
            // MANDATORY: Set Site URL
            _locationMasterService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            ViewBag.Action = "UpdateProvince";

            try
            {
                var province = _locationMasterService.UpdateProvince();
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.MonthlyFee);
        }

        [HttpPost]
        public ActionResult UpdateProvince(FormCollection form, LocationMasterVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _locationMasterService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            try
            {
                var province = _locationMasterService.UpdateProvince();
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.MonthlyFee);
        }
    }
}