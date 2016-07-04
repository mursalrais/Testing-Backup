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
using System.Threading.Tasks;

namespace MCAWebAndAPI.Web.Controllers
{
    public class HRPerformancePlanController : Controller
    {
        IHRPerformancePlanService _hRPerformancePlanService;
        const string SP_TRANSACTION_WORKFLOW_LIST_NAME = "Professional Performance Plan Workflow";
        const string SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME = "professionalperformanceplan";

        public HRPerformancePlanController()
        {
            _hRPerformancePlanService = new HRPerformancePlanService();
        }

        public ActionResult CreatePerformancePlan(string siteUrl = null, int? ID = null, string requestor = null)
        {
            // MANDATORY: Set Site URL
            _hRPerformancePlanService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            // Get blank ViewModel
            var viewModel = _hRPerformancePlanService.GetPopulatedModel(requestor);
            viewModel.Requestor = requestor;

                ViewBag.Action = "CreatePerformancePlan";

            // Used for Workflow Router
            ViewBag.ListName = "Professional%20Performance%20Plan";
            // This var should be taken from passing parameter
            if (requestor != null)
                SessionManager.Set("RequestorUserLogin", requestor);

            return View("PerformancePlan", viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> CreatePerformancePlan(FormCollection form, ProfessionalPerformancePlanVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _hRPerformancePlanService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            int? headerID = null;
            try
            {
                headerID = _hRPerformancePlanService.CreateHeader(viewModel);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error", new { errorMessage = e.Message });
            }

            var Detail = viewModel.ProjectOrUnitGoalsDetails;
            int sum = 0;
            foreach (var viewModelDetail in Detail)
            {
                if (viewModelDetail.EditMode != -1)
                {
                    sum = sum + viewModelDetail.Weight;
                }
            }

            if (sum != 100)
            {
                ModelState.AddModelError("ModelInvalid", "Weight must be total 100%");
                return View("PerformancePlan", viewModel);
            }

            if (viewModel.StatusForm != "DraftInitiated")
            {
                Task createTransactionWorkflowItemsTask = WorkflowHelper.CreateTransactionWorkflowAsync(SP_TRANSACTION_WORKFLOW_LIST_NAME,
                    SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)headerID);
            }

            Task createPerformancePlanDetailsTask = _hRPerformancePlanService.CreatePerformancePlanDetailsAsync(headerID, viewModel.ProjectOrUnitGoalsDetails);

            Task allTasks = Task.WhenAll(createPerformancePlanDetailsTask);

            try
            {
                await allTasks;
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            try
            {
                // Send to Level 1 Approver
                if (viewModel.StatusForm != "DraftInitiated")
                    _hRPerformancePlanService.SendEmail(viewModel, SP_TRANSACTION_WORKFLOW_LIST_NAME,
                    SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)headerID, 1,
                    string.Format("Dear Respective Approver,This email is sent to you to notify that there is a request which required your action to approve.Kindly check the link as per below to go to direct page accordingly.You may check your personal page in IMS(My Approval View).Thank you.Link: {0}{1}/EditFormApprover_Custom.aspx?ID={2}", siteUrl, UrlResource.ProfessionalPerformancePlan, viewModel.ID), string.Format(""));
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.ProfessionalPerformancePlan);
        }

        public ActionResult EditPerformancePlan(string siteUrl = null, int? ID = null, string requestor = null)
        {
            _hRPerformancePlanService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var viewModel = _hRPerformancePlanService.GetHeader(ID);
            viewModel.Requestor = requestor;
            viewModel.ID = ID;

            ViewBag.Action = "EditPerformancePlan";

            // Used for Workflow Router
            ViewBag.ListName = "Professional%20Performance%20Plan";

            // This var should be taken from passing parameter
            if (requestor != null)
                SessionManager.Set("RequestorUserLogin", requestor);

            return View("PerformancePlan", viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> EditPerformancePlan(FormCollection form, ProfessionalPerformancePlanVM viewModel, string site)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _hRPerformancePlanService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var Detail = viewModel.ProjectOrUnitGoalsDetails;
            int sum = 0;
            foreach (var viewModelDetail in Detail)
            {
                if (viewModelDetail.EditMode != -1)
                {
                    sum = sum + viewModelDetail.Weight;
                }
            }

            if (sum != 100)
            {
                ModelState.AddModelError("ModelInvalid", "Weight must be total 100%");
                return View("PerformancePlan", viewModel);
            }

            _hRPerformancePlanService.UpdateHeader(viewModel);

            if (viewModel.StatusForm == "Initiated" || viewModel.StatusForm == "DraftInitiated")
            {
                Task createTransactionWorkflowItemsTask = WorkflowHelper.CreateTransactionWorkflowAsync(SP_TRANSACTION_WORKFLOW_LIST_NAME,
                    SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)viewModel.ID);
            }
            if (viewModel.Requestor != null)
            {
                Task createPerformancePlanDetailsTask = _hRPerformancePlanService.CreatePerformancePlanDetailsAsync(viewModel.ID, viewModel.ProjectOrUnitGoalsDetails);

                Task allTasks = Task.WhenAll(createPerformancePlanDetailsTask);

                try
                {
                    await allTasks;
                }
                catch (Exception e)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return JsonHelper.GenerateJsonErrorResponse(e);
                }
            }

            try
            {
                // Send to Level 1 Approver
                if (viewModel.StatusForm == "Initiated" || viewModel.StatusForm == "Draft")
                    _hRPerformancePlanService.SendEmail(viewModel, SP_TRANSACTION_WORKFLOW_LIST_NAME,
                    SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)viewModel.ID, 1,
                    string.Format("Dear Respective Approver,This email is sent to you to notify that there is a request which required your action to approve.Kindly check the link as per below to go to direct page accordingly.You may check your personal page in IMS(My Approval View).Thank you.Link: {0}{1}/EditFormApprover_Custom.aspx?ID={2}", siteUrl, UrlResource.ProfessionalPerformancePlan, viewModel.ID), string.Format(""));

                // Send to Level 2 Approver and Requestor
                if (viewModel.StatusForm == "Pending Approval 1 of 2")
                    _hRPerformancePlanService.SendEmail(viewModel, SP_TRANSACTION_WORKFLOW_LIST_NAME,
                    SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)viewModel.ID, 2,
                    string.Format("Dear Respective Approver,This email is sent to you to notify that there is a request which required your action to approve.Kindly check the link as per below to go to direct page accordingly.You may check your personal page in IMS(My Approval View).Thank you.Link: {0}{1}/EditFormApprover_Custom.aspx?ID={2}", siteUrl, UrlResource.ProfessionalPerformancePlan, viewModel.ID), string.Format("Approved By Level 1"));

                // Send to Requestor
                if (viewModel.StatusForm == "Pending Approval 2 of 2")
                    _hRPerformancePlanService.SendEmail(viewModel, SP_TRANSACTION_WORKFLOW_LIST_NAME,
                    SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)viewModel.ID, 2,
                    string.Format(""), string.Format("Approved by Level 2"));

                // Send to Requestor
                if (viewModel.StatusForm == "Reject1")
                    _hRPerformancePlanService.SendEmail(viewModel, SP_TRANSACTION_WORKFLOW_LIST_NAME,
                    SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)viewModel.ID, 1,
                    string.Format(""), string.Format("Rejected by Approver Level 1"));

                // Send to Requestor
                if (viewModel.StatusForm == "Reject2")
                    _hRPerformancePlanService.SendEmail(viewModel, SP_TRANSACTION_WORKFLOW_LIST_NAME,
                    SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)viewModel.ID, 1,
                    string.Format(""), string.Format("Rejected by Approver Level 2"));
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
                new
                {
                    Value = Convert.ToString(e.Value),
                    Text = e.Text
                }),
                JsonRequestBehavior.AllowGet);
        }
    }
}