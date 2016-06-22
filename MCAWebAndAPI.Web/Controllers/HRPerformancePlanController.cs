using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.HR.Recruitment;
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
    public class HRPerformancePlanController : Controller
    {
        IHRPerformancePlanService _hRPerformancePlanService;

        public HRPerformancePlanController()
        {
            _hRPerformancePlanService = new HRPerformancePlanService();
        }

        public ActionResult CreatePerformancePlan(string siteUrl = null)
        {
            // Clear Existing Session Variables if any
            SessionManager.RemoveAll();

            // MANDATORY: Set Site URL
            _hRPerformancePlanService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            // Get blank ViewModel
            var viewModel = _hRPerformancePlanService.GetPopulatedModel();

            // Return to the name of the view and parse the model
            return View("CreatePerformancePlan", viewModel);
        }

        [HttpPost]
        public ActionResult SubmitPerformancePlan(FormCollection form, ProfessionalPerformancePlanVM viewModel)
        {
            //if (!ModelState.IsValid)
            //{
            //    Response.StatusCode = (int)HttpStatusCode.BadRequest;
            //    var errorMessages = BindHelper.GetErrorMessages(ModelState.Values);
            //    return JsonHelper.GenerateJsonErrorResponse(errorMessages);
            //}
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _hRPerformancePlanService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            int? headerID = null;
            try
            {
                headerID = _hRPerformancePlanService.CreateHeader(viewModel);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            try
            {
                _hRPerformancePlanService.CreatePerformancePlanDetails(headerID, viewModel.ProjectOrUnitGoalsDetails);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.ProfessionalPerformancePlan);
        }

        public ActionResult EditPerformancePlan(int ID, string siteUrl = null)
        {
            _hRPerformancePlanService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var viewModel = _hRPerformancePlanService.GetHeader(ID);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult UpdatePerformancePlan(FormCollection form, ProfessionalPerformancePlanVM viewModel, string site)
        {
            //if (!ModelState.IsValid)
            //{
            //    Response.StatusCode = (int)HttpStatusCode.BadRequest;
            //    var errorMessages = BindHelper.GetErrorMessages(ModelState.Values);
            //    return JsonHelper.GenerateJsonErrorResponse(errorMessages);
            //}

            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _hRPerformancePlanService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            _hRPerformancePlanService.UpdateHeader(viewModel);

            try
            {
                _hRPerformancePlanService.CreatePerformancePlanDetails(viewModel.ID, viewModel.ProjectOrUnitGoalsDetails);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.ProfessionalPerformancePlan);
        }

        public JsonResult GetCategoryGrid()
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _hRPerformancePlanService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var currency = ProjectOrUnitGoalsDetailVM.GetCategoryOptions();

            return Json(currency.Select(e =>
                new {
                    Value = Convert.ToString(e.Value),
                    Text = e.Text
                }),
                JsonRequestBehavior.AllowGet);
        }
    }
}