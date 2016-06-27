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
using System.IO;
using MCAWebAndAPI.Service.Converter;
using Elmah;

namespace MCAWebAndAPI.Web.Controllers
{
    public class HRPerformancePlanController : Controller
    {
        IHRPerformancePlanService _hRPerformancePlanService;

        public HRPerformancePlanController()
        {
            _hRPerformancePlanService = new HRPerformancePlanService();
        }

        public ActionResult CreatePerformancePlan(string siteUrl = null, int? ID = null, string position = null)
        {
            // Clear Existing Session Variables if any
            SessionManager.RemoveAll();

            // MANDATORY: Set Site URL
            _hRPerformancePlanService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            // Get blank ViewModel
            var viewModel = _hRPerformancePlanService.GetPopulatedModel();
            viewModel.Position = position;
            viewModel.ProfessionalID = ID;

            // Used for Workflow Router
            ViewBag.ListName = "Manpower%20Requisition";

            // This var should be taken from passing parameter
            ViewBag.RequestorUserLogin = "yunita.ajah@eceos.com";

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
                 _hRPerformancePlanService.CreateHeader(viewModel);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            try
            {
                _hRPerformancePlanService.CreatePerformancePlanDetails(viewModel.ID, viewModel.ProjectOrUnitGoalsDetails);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            try
            {
                if (viewModel.StatusDraft == "Submit")
                    _hRPerformancePlanService.GetProfessionalEmail(viewModel.ID);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.ProfessionalPerformancePlan);
        }

        [HttpPost]
        public ActionResult PrintPerformancePlan(FormCollection form, ProfessionalPerformancePlanVM viewModel)
        {
            const string RelativePath = "~/Views/HRPerformancePlan/PrintPerformancePlan.cshtml";
            var view = ViewEngines.Engines.FindView(ControllerContext, RelativePath, null);
            var fileName = viewModel.Name + "_PerformancePlan.pdf";
            byte[] pdfBuf = null;
            string content;

            using (var writer = new StringWriter())
            {
                var context = new ViewContext(ControllerContext, view.View, ViewData, TempData, writer);
                view.View.Render(context, writer);
                writer.Flush();
                content = writer.ToString();

                // Get PDF Bytes
                try
                {
                    pdfBuf = PDFConverter.Instance.ConvertFromHTML(fileName, content);
                }
                catch (Exception e)
                {
                    ErrorSignal.FromCurrentContext().Raise(e);
                    RedirectToAction("Index", "Error");
                }
            }
            if (pdfBuf == null)
                return HttpNotFound();
            return File(pdfBuf, "application/pdf");
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