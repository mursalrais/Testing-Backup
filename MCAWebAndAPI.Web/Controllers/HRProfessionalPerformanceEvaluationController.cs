﻿using Kendo.Mvc.Extensions;
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
    public class HRProfessionalPerformanceEvaluationController : Controller
    {
        IHRProfessionalPerformanceEvaluationService _hRProfessionalPerformanceEvaluationService;
        const string SP_TRANSACTION_WORKFLOW_LIST_NAME = "Professional Performance Evaluation Workflow";
        const string SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME = "professionalperformanceevaluatio";

        public HRProfessionalPerformanceEvaluationController()
        {
            _hRProfessionalPerformanceEvaluationService = new HRProfessionalPerformanceEvaluationService();
        }

        public async Task<ActionResult> EditPerformanceEvaluation(string siteUrl = null, int? ID = null, string requestor = null)
        {
            _hRProfessionalPerformanceEvaluationService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var viewModel = await _hRProfessionalPerformanceEvaluationService.GetHeader(ID, requestor, "Professional Performance Evaluation", SP_TRANSACTION_WORKFLOW_LIST_NAME, SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME);
            viewModel.Requestor = requestor;
            viewModel.ID = ID;
            ViewBag.Action = "EditPerformanceEvaluation";
            SessionManager.Set("WorkflowItems", viewModel.WorkflowItems);

            return View("ProfessionalPerformanceEvaluation", viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> EditPerformanceEvaluation(FormCollection form, ProfessionalPerformanceEvaluationVM viewModel, string site)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _hRProfessionalPerformanceEvaluationService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            if (viewModel.CheckWorkflow == "False")
            {
                SessionManager.Set("WorkflowItems", viewModel.WorkflowItems);
                foreach (var item in viewModel.WorkflowItems)
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

                    if (lvl == "3")
                    {
                        viewModel.Approver3 = item.ApproverNameText;
                    }
                }
            }

            var Detail = viewModel.ProfessionalPerformanceEvaluationDetails;
            int sumPlanned = 0;
            int sumActual = 0;
            decimal totalTotalScore = 0;
            string outputTemp = "";
            var countData = Detail.Count();
            if (countData != 0)
            {
                foreach (var viewModelDetail in Detail)
                {
                    if (viewModelDetail.EditMode != -1)
                    {
                        if (viewModelDetail.Output == null)
                        {
                            outputTemp = "Empty";
                            viewModelDetail.Output = "";
                        }

                        totalTotalScore = totalTotalScore + viewModelDetail.TotalScore;
                        sumPlanned = sumPlanned + viewModelDetail.PlannedWeight;
                        sumActual = sumActual + viewModelDetail.ActualWeight;
                    }
                }

                viewModel.OverallTotalScore = totalTotalScore;

                if (sumPlanned != 100 || sumActual != 100)
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

                if (outputTemp == "Empty")
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
                    return JsonHelper.GenerateJsonErrorResponse("Output is Required");
                }
            }

            _hRProfessionalPerformanceEvaluationService.UpdateHeader(viewModel);

            if (viewModel.StatusForm == "Initiated" || viewModel.StatusForm == "DraftInitiated")
            {
                Task createTransactionWorkflowItemsTask = WorkflowHelper.CreateWorkflowAsync(SP_TRANSACTION_WORKFLOW_LIST_NAME,
                    SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)viewModel.ID);
            }
            Task createPerformancePlanDetailsTask = _hRProfessionalPerformanceEvaluationService.CreatePerformanceEvaluationDetailsAsync(viewModel.ID, viewModel.ProfessionalPerformanceEvaluationDetails);

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
                    _hRProfessionalPerformanceEvaluationService.SendEmail(viewModel, SP_TRANSACTION_WORKFLOW_LIST_NAME,
                    SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)viewModel.ID, 1,
                    string.Format(EmailResource.ProfessionalPerfromanceEvaluation, viewModel.Approver1, viewModel.PerformancePeriod, viewModel.Name, siteUrl, UrlResource.ProfessionalPerfromanceEvaluation, viewModel.ID), string.Format(EmailResource.ProfessionalPerformanceEvaluationRequestor, viewModel.Name, viewModel.Requestor));


                if (viewModel.ApproverCount == 1)
                {
                    // Send to Requestor
                    if (viewModel.TypeForm == "Approver1" && viewModel.StatusForm == "Pending Approval 1 of 1")
                        _hRProfessionalPerformanceEvaluationService.SendEmail(viewModel, SP_TRANSACTION_WORKFLOW_LIST_NAME,
                        SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)viewModel.ID, 2,
                        string.Format(EmailResource.ProfessionalPerfromanceEvaluation, viewModel.Approver1, viewModel.PerformancePeriod, viewModel.Name, siteUrl, UrlResource.ProfessionalPerfromanceEvaluation, viewModel.ID), string.Format(EmailResource.ProfessionalPerformanceEvaluationRequestor, viewModel.Name, viewModel.Requestor));
                }

                if (viewModel.ApproverCount == 2)
                {
                    // Send to Level 2 Approver and Requestor
                    if (viewModel.TypeForm == "Approver1" && viewModel.StatusForm == "Pending Approval 1 of 2")
                        _hRProfessionalPerformanceEvaluationService.SendEmail(viewModel, SP_TRANSACTION_WORKFLOW_LIST_NAME,
                        SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)viewModel.ID, 2,
                        string.Format(EmailResource.ProfessionalPerfromanceEvaluation, viewModel.Approver2, viewModel.PerformancePeriod, viewModel.Name, siteUrl, UrlResource.ProfessionalPerfromanceEvaluation, viewModel.ID), string.Format(EmailResource.ProfessionalPerformanceEvaluationRequestor, viewModel.Name, viewModel.Requestor));

                    // Send to Requestor
                    if (viewModel.TypeForm == "Approver2" && viewModel.StatusForm == "Pending Approval 2 of 2")
                        _hRProfessionalPerformanceEvaluationService.SendEmail(viewModel, SP_TRANSACTION_WORKFLOW_LIST_NAME,
                        SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)viewModel.ID, 2,
                        string.Format(EmailResource.ProfessionalPerfromanceEvaluation, viewModel.Approver1, viewModel.PerformancePeriod, viewModel.Name, siteUrl, UrlResource.ProfessionalPerfromanceEvaluation, viewModel.ID), string.Format(EmailResource.ProfessionalPerformanceEvaluationRequestor, viewModel.Name, viewModel.Requestor));
                }

                if (viewModel.ApproverCount == 3)
                {
                    // Send to Level 2 Approver and Requestor
                    if (viewModel.TypeForm == "Approver1" && viewModel.StatusForm == "Pending Approval 1 of 3")
                        _hRProfessionalPerformanceEvaluationService.SendEmail(viewModel, SP_TRANSACTION_WORKFLOW_LIST_NAME,
                        SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)viewModel.ID, 2,
                        string.Format(EmailResource.ProfessionalPerfromanceEvaluation, viewModel.Approver2, viewModel.PerformancePeriod, viewModel.Name, siteUrl, UrlResource.ProfessionalPerfromanceEvaluation, viewModel.ID), string.Format(EmailResource.ProfessionalPerformanceEvaluationRequestor, viewModel.Name, viewModel.Requestor));

                    // Send to Level 3 Approver and Requestor
                    if (viewModel.TypeForm == "Approver2" && viewModel.StatusForm == "Pending Approval 2 of 3")
                        _hRProfessionalPerformanceEvaluationService.SendEmail(viewModel, SP_TRANSACTION_WORKFLOW_LIST_NAME,
                        SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)viewModel.ID, 3,
                        string.Format(EmailResource.ProfessionalPerfromanceEvaluation, viewModel.Approver3, viewModel.PerformancePeriod, viewModel.Name, siteUrl, UrlResource.ProfessionalPerfromanceEvaluation, viewModel.ID), string.Format(EmailResource.ProfessionalPerformanceEvaluationRequestor, viewModel.Name, viewModel.Requestor));

                    // Send to Requestor
                    if (viewModel.TypeForm == "Approver3" && viewModel.StatusForm == "Pending Approval 3 of 3")
                        _hRProfessionalPerformanceEvaluationService.SendEmail(viewModel, SP_TRANSACTION_WORKFLOW_LIST_NAME,
                        SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)viewModel.ID, 3,
                        string.Format(EmailResource.ProfessionalPerfromanceEvaluation, viewModel.Approver3, viewModel.PerformancePeriod, viewModel.Name, siteUrl, UrlResource.ProfessionalPerfromanceEvaluation, viewModel.ID), string.Format(EmailResource.ProfessionalPerformanceEvaluationRequestor, viewModel.Name, viewModel.Requestor));
                }
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
                url = UrlResource.ProfessionalPerfromanceEvaluation;
            }
            if (viewModel.TypeForm != "Professional")
            {
                url = UrlResource.ProfessionalPerformanceEvaluationApprover;
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + url);
        }

        [HttpPost]
        public ActionResult PrintProfessionalPerformanceEvaluation(FormCollection form, ProfessionalPerformanceEvaluationVM viewModel)
        {

            const string RelativePath = "~/Views/HRProfessionalPerformanceEvaluation/PrintProfessionalPerformanceEvaluation.cshtml";
            var view = ViewEngines.Engines.FindView(ControllerContext, RelativePath, null);
            var fileName = viewModel.Name + "_ProfessionalPerformanceEvaluation.pdf";
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