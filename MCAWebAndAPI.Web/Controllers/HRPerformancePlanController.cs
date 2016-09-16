using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Model.ViewModel.Form.Common;
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

        public async Task<ActionResult> CreatePerformancePlan(string siteUrl = null, int? ID = null, string requestor = null)
        {
            // MANDATORY: Set Site URL
            _hRPerformancePlanService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            // Get blank ViewModel
            var viewModel = await _hRPerformancePlanService.GetPopulatedModel(requestor, "Professional Performance Plan");

            viewModel.Requestor = requestor;
            viewModel.ID = ID;
            viewModel.TypeForm = "Professional";
            ViewBag.Action = "CreatePerformancePlan";

            return View("PerformancePlan", viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> CreatePerformancePlan(FormCollection form, ProfessionalPerformancePlanVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _hRPerformancePlanService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("WorkflowItems", viewModel.WorkflowItems);

            var Detail = viewModel.ProjectOrUnitGoalsDetails;
            var Count = Detail.Count();
            int sum = 0;
            string project = null;
            string individual = null;
            var ApproverName = viewModel.WorkflowItems;

            foreach (var item in ApproverName)
            {
                var lvl = item.Level;
                if (lvl == "1")
                {
                    viewModel.Approver1 = item.ApproverNameText;
                }

                if (lvl == "2")
                {
                    viewModel.Approver2 = item.ApproverNameText;
                }
            }

            foreach (var viewModelDetail in Detail)
            {
                if (viewModelDetail.EditMode != -1 && Count != 0)
                {
                    if (viewModelDetail.ProjectOrUnitGoals == null)
                    {
                        viewModelDetail.ProjectOrUnitGoals = "";
                        project = "Empty";
                    }

                    if (viewModelDetail.IndividualGoalAndPlan == null)
                    {
                        viewModelDetail.IndividualGoalAndPlan = "";
                        individual = "Empty";
                    }

                    if (viewModelDetail.Remarks == null)
                    {
                        viewModelDetail.Remarks = "";
                    }

                    sum = sum + viewModelDetail.Weight;
                }
            }
            if (project == "Empty")
            {
                if (viewModel.StatusForm == "DraftInitiated")
                {
                    viewModel.StatusForm = "Initiated";
                }
                if (viewModel.StatusForm == "DraftDraft")
                {
                    viewModel.StatusForm = "Draft";
                }

                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse("Project Or Unit Goals is Required");
            }

            if (individual == "Empty")
            {
                if (viewModel.StatusForm == "DraftInitiated")
                {
                    viewModel.StatusForm = "Initiated";
                }
                if (viewModel.StatusForm == "DraftDraft")
                {
                    viewModel.StatusForm = "Draft";
                }

                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse("Individual Goal And Plan is Required");
            }

            if (sum != 100 && Count != 0)
            {
                if (viewModel.StatusForm == "DraftInitiated")
                {
                    viewModel.StatusForm = "Initiated";
                }
                if (viewModel.StatusForm == "DraftDraft")
                {
                    viewModel.StatusForm = "Draft";
                }

                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse("Weight must be total 100%");
            }

            int? headerID = null;
            try
            {
                headerID = _hRPerformancePlanService.CreateHeader(viewModel.Requestor, viewModel);
            }
            catch (Exception e)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            Task createTransactionWorkflowItemsTask = WorkflowHelper.CreateWorkflowAsync(SP_TRANSACTION_WORKFLOW_LIST_NAME,
                SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)headerID);

            Task createPerformancePlanDetailsTask = _hRPerformancePlanService.CreatePerformancePlanDetailsAsync(headerID, viewModel.PerformancePeriodID, viewModel.Requestor, viewModel.StatusForm, viewModel.TypeForm, viewModel.ProjectOrUnitGoalsDetails);

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
                    string.Format(EmailResource.ProfessionalPerformancePlan, viewModel.Approver1, viewModel.PerformancePeriod, viewModel.Name, siteUrl, UrlResource.ProfessionalPerformancePlan, headerID), string.Format(EmailResource.ProfessionalPerformancePlanApproved, viewModel.Name, viewModel.Requestor));
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.ProfessionalPerformancePlan);
        }

        public async Task<ActionResult> EditPerformancePlan(string siteUrl = null, int? ID = null, string requestor = null)
        {
            _hRPerformancePlanService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var viewModel = await _hRPerformancePlanService.GetHeader(ID, requestor, "Professional Performance Plan", SP_TRANSACTION_WORKFLOW_LIST_NAME, SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME);
            viewModel.Requestor = requestor;
            viewModel.ID = ID;
            ViewBag.Action = "EditPerformancePlan";

            // Used for Workflow Router
            ViewBag.ListName = "Professional%20Performance%20Plan";
            ViewBag.Requestor = requestor;

            return View("PerformancePlan", viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> EditPerformancePlan(FormCollection form, ProfessionalPerformancePlanVM viewModel, string site)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _hRPerformancePlanService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("WorkflowItems", viewModel.WorkflowItems);

            var Detail = viewModel.ProjectOrUnitGoalsDetails;
            var Count = Detail.Count();
            int sum = 0;
            string project = null;
            string individual = null;
            if (Count != 0)
            {
                foreach (var viewModelDetail in Detail)
                {
                    if (viewModelDetail.EditMode != -1)
                    {
                        if (viewModelDetail.ProjectOrUnitGoals == null)
                        {
                            viewModelDetail.ProjectOrUnitGoals = "";
                            project = "Empty";
                        }

                        if (viewModelDetail.IndividualGoalAndPlan == null)
                        {
                            viewModelDetail.IndividualGoalAndPlan = "";
                            individual = "Empty";
                        }

                        if (viewModelDetail.Remarks == null)
                        {
                            viewModelDetail.Remarks = "";
                        }

                        sum = sum + viewModelDetail.Weight;
                    }
                }

                if (project == "Empty")
                {
                    if (viewModel.StatusForm == "DraftInitiated")
                    {
                        viewModel.StatusForm = "Initiated";
                    }
                    if (viewModel.StatusForm == "DraftDraft")
                    {
                        viewModel.StatusForm = "Draft";
                    }

                    Response.TrySkipIisCustomErrors = true;
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return JsonHelper.GenerateJsonErrorResponse("Project Or Unit Goals is Required");
                }

                if (individual == "Empty")
                {
                    if (viewModel.StatusForm == "DraftInitiated")
                    {
                        viewModel.StatusForm = "Initiated";
                    }
                    if (viewModel.StatusForm == "DraftDraft")
                    {
                        viewModel.StatusForm = "Draft";
                    }

                    Response.TrySkipIisCustomErrors = true;
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return JsonHelper.GenerateJsonErrorResponse("Individual Goal And Plan is Required");
                }

                if (sum != 100)
                {
                    if (viewModel.StatusForm == "DraftInitiated")
                    {
                        viewModel.StatusForm = "Initiated";
                    }
                    if (viewModel.StatusForm == "DraftDraft")
                    {
                        viewModel.StatusForm = "Draft";
                    }

                    Response.TrySkipIisCustomErrors = true;
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return JsonHelper.GenerateJsonErrorResponse("Weight must be total 100%");
                }
            }

            _hRPerformancePlanService.UpdateHeader(viewModel);

            if (viewModel.StatusForm == "Initiated" || viewModel.StatusForm == "DraftInitiated")
            {
                Task createTransactionWorkflowItemsTask = WorkflowHelper.CreateTransactionWorkflowAsync(SP_TRANSACTION_WORKFLOW_LIST_NAME,
                    SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)viewModel.ID);
            }

            Task createPerformancePlanDetailsTask = _hRPerformancePlanService.CreatePerformancePlanDetailsAsync(viewModel.ID, viewModel.PerformancePeriodID, viewModel.Requestor, viewModel.StatusForm, viewModel.TypeForm, viewModel.ProjectOrUnitGoalsDetails);

            Task allTasks = Task.WhenAll(createPerformancePlanDetailsTask);

            try
            {
                await allTasks;
            }
            catch (Exception e)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            try
            {
                // Send to Level 1 Approver
                if (viewModel.StatusForm == "Initiated" || viewModel.StatusForm == "Draft")
                    _hRPerformancePlanService.SendEmail(viewModel, SP_TRANSACTION_WORKFLOW_LIST_NAME,
                    SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)viewModel.ID, 1,
                    string.Format(EmailResource.ProfessionalPerformancePlan, viewModel.Approver1, viewModel.PerformancePeriod, viewModel.Name, siteUrl, UrlResource.ProfessionalPerformancePlan, viewModel.ID), string.Format(EmailResource.ProfessionalPerformancePlanApproved, viewModel.Name, viewModel.Requestor));

                // Send to Level 2 Approver and Requestor
                if (viewModel.TypeForm == "Approver1" && viewModel.StatusForm == "Pending Approval 1 of 2")
                    _hRPerformancePlanService.SendEmail(viewModel, SP_TRANSACTION_WORKFLOW_LIST_NAME,
                    SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)viewModel.ID, 2,
                    string.Format(EmailResource.ProfessionalPerformancePlan, viewModel.Approver2, viewModel.PerformancePeriod, viewModel.Name, siteUrl, UrlResource.ProfessionalPerformancePlan, viewModel.ID), string.Format(EmailResource.ProfessionalPerformancePlanApproved, viewModel.Name, viewModel.Requestor));

                // Send to Level 1 Approver
                if (viewModel.TypeForm == "Professional" && viewModel.StatusForm == "Pending Approval 1 of 2")
                    _hRPerformancePlanService.SendEmail(viewModel, SP_TRANSACTION_WORKFLOW_LIST_NAME,
                    SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)viewModel.ID, 1,
                    string.Format(EmailResource.ProfessionalPerformancePlan, viewModel.Approver1, viewModel.PerformancePeriod, viewModel.Name, siteUrl, UrlResource.ProfessionalPerformancePlan, viewModel.ID), string.Format(EmailResource.ProfessionalPerformancePlanApproved, viewModel.Name, viewModel.Requestor));

                // Send to Level 2 Approver
                if (viewModel.TypeForm == "Professional" && viewModel.StatusForm == "Pending Approval 2 of 2")
                    _hRPerformancePlanService.SendEmail(viewModel, SP_TRANSACTION_WORKFLOW_LIST_NAME,
                    SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)viewModel.ID, 2,
                    string.Format(EmailResource.ProfessionalPerformancePlan, viewModel.Approver2, viewModel.PerformancePeriod, viewModel.Name, siteUrl, UrlResource.ProfessionalPerformancePlan, viewModel.ID), string.Format(EmailResource.ProfessionalPerformancePlanApproved, viewModel.Name, viewModel.Requestor));

                // Send to Requestor
                if (viewModel.TypeForm == "Approver2" && viewModel.StatusForm == "Pending Approval 2 of 2")
                    _hRPerformancePlanService.SendEmail(viewModel, SP_TRANSACTION_WORKFLOW_LIST_NAME,
                    SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)viewModel.ID, 2,
                    string.Format(EmailResource.ProfessionalPerformancePlan, viewModel.Approver2, viewModel.PerformancePeriod, viewModel.Name, siteUrl, UrlResource.ProfessionalPerformancePlan, viewModel.ID), string.Format(EmailResource.ProfessionalPerformancePlanApproved, viewModel.Name, viewModel.Requestor));

                // Send to Requestor
                if (viewModel.StatusForm == "Reject1")
                    _hRPerformancePlanService.SendEmail(viewModel, SP_TRANSACTION_WORKFLOW_LIST_NAME,
                    SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)viewModel.ID, 1,
                    string.Format(EmailResource.ProfessionalPerformancePlan, viewModel.Approver1, viewModel.PerformancePeriod, viewModel.Name, siteUrl, UrlResource.ProfessionalPerformancePlan, viewModel.ID), string.Format(EmailResource.ProfessionalPerformancePlanRejected, viewModel.Name, viewModel.Requestor));

                // Send to Requestor
                if (viewModel.StatusForm == "Reject2")
                    _hRPerformancePlanService.SendEmail(viewModel, SP_TRANSACTION_WORKFLOW_LIST_NAME,
                    SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)viewModel.ID, 1,
                    string.Format(EmailResource.ProfessionalPerformancePlan, viewModel.Approver2, viewModel.PerformancePeriod, viewModel.Name, siteUrl, UrlResource.ProfessionalPerformancePlan, viewModel.ID), string.Format(EmailResource.ProfessionalPerformancePlanRejected, viewModel.Name, viewModel.Requestor));
            }
            catch (Exception e)
            {
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            string url = null;
            if (viewModel.TypeForm == "Professional")
            {
                url = UrlResource.ProfessionalPerformancePlan;
            }
            if (viewModel.TypeForm != "Professional")
            {
                url = UrlResource.ProfessionalPerformancePlanApprover;
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + url);
        }

        [HttpPost]
        public ActionResult PrintPerformancePlan(FormCollection form, ProfessionalPerformancePlanVM viewModel)
        {
            const string RelativePath = "~/Views/HRPerformancePlan/PrintPerformancePlan.cshtml";
            var view = ViewEngines.Engines.FindView(ControllerContext, RelativePath, null);
            var fileName = viewModel.Name + "_PerformancePlan.pdf";
            byte[] pdfBuf = null;
            string content;

            // ControllerContext context = new ControllerContext();
            ControllerContext.Controller.ViewData.Model = viewModel;
            ViewData = ControllerContext.Controller.ViewData;
            TempData = ControllerContext.Controller.TempData;

            using (var writer = new StringWriter())
            {
                //var contextviewContext = new ViewContext(ControllerContext, view.View, ViewData, TempData, writer);
                var contextviewContext = new ViewContext(ControllerContext, view.View, ViewData, TempData, writer);
                view.View.Render(contextviewContext, writer);
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
    }
}