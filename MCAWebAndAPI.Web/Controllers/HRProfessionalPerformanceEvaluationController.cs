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
    public class HRProfessionalPerformanceEvaluationController : Controller
    {
        IHRProfessionalPerformanceEvaluationService _hRProfessionalPerformanceEvaluationService;
        const string SP_TRANSACTION_WORKFLOW_LIST_NAME = "Professional Performance Evaluation Workflow";
        const string SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME = "professionalperformanceevaluatio";

        public HRProfessionalPerformanceEvaluationController()
        {
            _hRProfessionalPerformanceEvaluationService = new HRProfessionalPerformanceEvaluationService();
        }

        public ActionResult EditPerformanceEvaluation(string siteUrl = null, int? ID = null, string requestor = null)
        {
            _hRProfessionalPerformanceEvaluationService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var viewModel = _hRProfessionalPerformanceEvaluationService.GetHeader(ID, requestor);
            viewModel.Requestor = requestor;
            viewModel.ID = ID;
            ViewBag.Action = "EditPerformanceEvaluation";

            // Used for Workflow Router
            ViewBag.ListName = "Professional%20Performance%20Evaluation";
            ViewBag.Requestor = requestor;

            return View("ProfessionalPerformanceEvaluation", viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> EditPerformanceEvaluation(FormCollection form, ProfessionalPerformanceEvaluationVM viewModel, string site)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _hRProfessionalPerformanceEvaluationService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

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
                Task createTransactionWorkflowItemsTask = WorkflowHelper.CreateTransactionWorkflowAsync(SP_TRANSACTION_WORKFLOW_LIST_NAME,
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
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            try
            {
                // Send to Level 1 Approver
                if (viewModel.StatusForm == "Initiated" || viewModel.StatusForm == "Draft")
                    _hRProfessionalPerformanceEvaluationService.SendEmail(viewModel, SP_TRANSACTION_WORKFLOW_LIST_NAME,
                    SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)viewModel.ID, 1,
                    string.Format(EmailResource.ProfessionalPerfromanceEvaluation, siteUrl, UrlResource.ProfessionalPerfromanceEvaluation, viewModel.ID), string.Format(""));

                // Send to Level 2 Approver and Requestor
                if (viewModel.StatusForm == "Pending Approval 1 of 2")
                    _hRProfessionalPerformanceEvaluationService.SendEmail(viewModel, SP_TRANSACTION_WORKFLOW_LIST_NAME,
                    SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)viewModel.ID, 2,
                    string.Format(EmailResource.ProfessionalPerfromanceEvaluation, siteUrl, UrlResource.ProfessionalPerfromanceEvaluation, viewModel.ID), string.Format("Approved By Level 1"));

                // Send to Requestor
                if (viewModel.StatusForm == "Pending Approval 2 of 2")
                    _hRProfessionalPerformanceEvaluationService.SendEmail(viewModel, SP_TRANSACTION_WORKFLOW_LIST_NAME,
                    SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)viewModel.ID, 2,
                    string.Format(""), string.Format("Approved by Level 2"));
            }
            catch (Exception e)
            {
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
        public ActionResult PrintProfessionalPerformanceEvaluation(FormCollection form, ProfessionalPerformancePlanVM viewModel)
        {
            const string RelativePath = "~/Views/HRProfessionalPerformanceEvaluation/PrintProfessionalPerformanceEvaluation.cshtml";
            var view = ViewEngines.Engines.FindView(ControllerContext, RelativePath, null);
            var fileName = viewModel.Name + "_ProfessionalPerformanceEvaluation.pdf";
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
    }
}