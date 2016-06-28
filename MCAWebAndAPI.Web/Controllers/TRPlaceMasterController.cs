using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.HR.Travel;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Web.Filters;
using MCAWebAndAPI.Web.Resources;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Service.Resources;
using System.Net;
using System;


namespace MCAWebAndAPI.Web.Controllers
{
    public class TRPlaceMasterController : Controller
    {
        ITRPlaceMasterService _tRPlaceMasterService;

        public TRPlaceMasterController()
        {
            _tRPlaceMasterService = new TRPlaceMasterService();
        }

        public ActionResult CreatePlaceMaster(string siteUrl = null)
        {



            // MANDATORY: Set Site URL
            _tRPlaceMasterService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            // Get blank ViewModel
            var viewModel = _tRPlaceMasterService.GetPopulatedModel();

            // Return to the name of the view and parse the model
            return View("CreatePlaceMaster", viewModel);
        }

        [HttpPost]
        public ActionResult SubmitPlaceMaster(PlaceMasterVM viewModel)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var errorMessages = BindHelper.GetErrorMessages(ModelState.Values);
                return JsonHelper.GenerateJsonErrorResponse(errorMessages);
            }
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _tRPlaceMasterService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            // Get Header ID after inster to SharePoint
            try
            {
                var headerID = _tRPlaceMasterService.CreateHeader(viewModel);
            }
            catch (Exception e)
            {

                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.PlaceMaster);
        }

        public ActionResult EditPlaceMaster(int ID, string siteUrl = null)
        {
            _tRPlaceMasterService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var viewModel = _tRPlaceMasterService.GetHeader(ID);

            return View(viewModel);
        }

        [HttpPost]

        public ActionResult UpdatePlaceMaster(PlaceMasterVM _data, string site)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var errorMessages = BindHelper.GetErrorMessages(ModelState.Values);
                return JsonHelper.GenerateJsonErrorResponse(errorMessages);
            }

            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _tRPlaceMasterService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            _tRPlaceMasterService.UpdateHeader(_data);

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.PlaceMaster);
        }

        // GET: TRPlaceMaster
        public ActionResult Index()
        {
            return View();
        }
    }
}