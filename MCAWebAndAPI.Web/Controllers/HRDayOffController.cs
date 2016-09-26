using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Model.ViewModel.Form.Common;
using MCAWebAndAPI.Service.HR.Leave;
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
    public class HRDayOffController : Controller
    {
        IDayOffService _hRDayOffService;
        const string SP_TRANSACTION_WORKFLOW_LIST_NAME = "Professional Performance Plan Workflow";
        const string SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME = "professionalperformanceplan";

        public HRDayOffController()
        {
            _hRDayOffService = new DayOffService();
        }

        public ActionResult CreateDayOff(string siteUrl = null, int? ID = null, string requestor = null)
        {
            // MANDATORY: Set Site URL
            _hRDayOffService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            // Get blank ViewModel
            var viewModel = _hRDayOffService.GetPopulatedModel(ID, requestor);
            viewModel.Requestor = requestor;
            viewModel.ID = ID;
            ViewBag.Action = "CreateDayOff";

            // Used for Workflow Router
            ViewBag.ListName = "Day-Off Request";
            ViewBag.Requestor = requestor;

            return View("CreateDayOffRequest", viewModel);
        }

        [HttpPost]
        public ActionResult CreateDayOffRequestData(FormCollection form, DayOffRequestVM viewModel)
        {
            // Check whether error is found
            if (!ModelState.IsValid)
            {
                RedirectToAction("Index", "Error");
            }

            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _hRDayOffService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            int? dayOffRequestHeaderID = null;
            try
            {
                dayOffRequestHeaderID = _hRDayOffService.CreateDayOffRequestHeader(viewModel);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error", new { errorMessage = e.Message });
            }

            var dayOffCurrentBalance = viewModel.DayOffBalanceDetails;
            var dayOffRequestDetailDisplay = viewModel.DayOffRequestDetailsDisplay;
            var dayOffNextBalance = viewModel.DayOffNextBalance;

            string requestorposition = Convert.ToString(viewModel.PositionName);
            string requestorunit = Convert.ToString(viewModel.ProjectUnit);

            int? positionID = _hRDayOffService.GetPositionID(requestorposition, requestorunit, 0, 0);

            viewModel.DayOffRequestDetailsDisplay = BindRequestStartDate(form, viewModel.DayOffRequestDetailsDisplay);
            viewModel.DayOffRequestDetailsDisplay = BindRequestEndDate(form, viewModel.DayOffRequestDetailsDisplay);
            Task createDayOffRequestDetail = _hRDayOffService.CreateDayOffRequestDetailAsync(viewModel, dayOffRequestHeaderID, dayOffRequestDetailDisplay, requestorposition, requestorunit, positionID, viewModel.StatusForm, dayOffCurrentBalance, dayOffNextBalance);

            //_hRDayOffService.UpdateCurrentAndNextBalance()

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.ExitProcedure);

        }

        public ActionResult ViewDayOffRequest(string siteUrl = null, int? ID = null)
        {
            // Clear Existing Session Variables if any

            // MANDATORY: Set Site URL
            _hRDayOffService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var viewModel = _hRDayOffService.ViewDayOffRequest(ID);
            return View("ViewDayOffRequest", viewModel);
        }

        IEnumerable<DayOffRequestDetailDisplayVM> BindRequestStartDate(FormCollection form, IEnumerable<DayOffRequestDetailDisplayVM> dayOffRequestDetailDisplay)
        {
            var array = dayOffRequestDetailDisplay.ToArray();

            for (int i = 0; i < array.Length; i++)
            {
                array[i].RqsStartDate = BindHelper.BindDateInGrid("DayOffRequestDetailsDisplay",
                    i, "RqsStartDate", form);
            }
            return array;
        }

        IEnumerable<DayOffRequestDetailDisplayVM> BindRequestEndDate(FormCollection form, IEnumerable<DayOffRequestDetailDisplayVM> dayOffRequestDetailDisplay)
        {
            var array = dayOffRequestDetailDisplay.ToArray();

            for (int i = 0; i < array.Length; i++)
            {
                array[i].RqsEndDate = BindHelper.BindDateInGrid("DayOffRequestDetailsDisplay",
                    i, "RqsEndDate", form);
            }
            return array;
        }

        public ActionResult DisplayRequestDetail(List<string> dayOffRequestDayOffType, List<string> dayOffRequestStarDate, List<string> dayOffRequestEndDate, List<string> dayOffRequestFullOrHalf, List<string> dayOffRequestRemarks, List<string> dayOffRewquestTotalDays, List<string> returnToWork, string siteUrl = null)
        {
            _hRDayOffService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var viewModel = _hRDayOffService.GetRequestData(dayOffRequestDayOffType, dayOffRequestStarDate, dayOffRequestEndDate, dayOffRequestFullOrHalf, dayOffRequestRemarks, dayOffRewquestTotalDays, returnToWork);

            return View("_DisplayRequestDetail", viewModel.DayOffRequestDetailsDisplay);
        }

        public ActionResult DisplayNextBalance(List<string> arrDayOffType, List<string> arrTotalDayOff, string professionalID, string siteUrl = null)
        {
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            _hRDayOffService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            
            var viewModel = _hRDayOffService.GetRequestDataByUser(arrDayOffType, arrTotalDayOff, professionalID);

            return View("_DayOffNextBalance", viewModel.DayOffNextBalance);
        }

        public ActionResult TestAction(int masterDayOffTypeValue, string siteUrl)
        {
            _hRDayOffService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateDayOff(FormCollection form, DayOffRequestVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _hRDayOffService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            //var Detail = viewModel.ProjectOrUnitGoalsDetails;
            //var Count = Detail.Count();
            //int sum = 0;
            //string project = null;
            //string individual = null;
            //foreach (var viewModelDetail in Detail)
            //{
            //    if (viewModelDetail.EditMode != -1 && Count != 0)
            //    {
            //        if (viewModelDetail.ProjectOrUnitGoals == null)
            //        {
            //            viewModelDetail.ProjectOrUnitGoals = "";
            //            project = "Empty";
            //        }

            //        if (viewModelDetail.IndividualGoalAndPlan == null)
            //        {
            //            viewModelDetail.IndividualGoalAndPlan = "";
            //            individual = "Empty";
            //        }

            //        if (viewModelDetail.Remarks == null)
            //        {
            //            viewModelDetail.Remarks = "";
            //        }

            //        sum = sum + viewModelDetail.Weight;
            //    }
            //}
            //if (project == "Empty")
            //{
            //    if (viewModel.StatusForm == "DraftInitiated")
            //    {
            //        viewModel.StatusForm = "Initiated";
            //    }
            //    if (viewModel.StatusForm == "DraftDraft")
            //    {
            //        viewModel.StatusForm = "Draft";
            //    }

            //    Response.StatusCode = (int)HttpStatusCode.BadRequest;
            //    return JsonHelper.GenerateJsonErrorResponse("Project Or Unit Goals is Required");
            //}

            //if (individual == "Empty")
            //{
            //    if (viewModel.StatusForm == "DraftInitiated")
            //    {
            //        viewModel.StatusForm = "Initiated";
            //    }
            //    if (viewModel.StatusForm == "DraftDraft")
            //    {
            //        viewModel.StatusForm = "Draft";
            //    }

            //    Response.StatusCode = (int)HttpStatusCode.BadRequest;
            //    return JsonHelper.GenerateJsonErrorResponse("Individual Goal And Plan is Required");
            //}

            //if (sum != 100 && Count != 0)
            //{
            //    if (viewModel.StatusForm == "DraftInitiated")
            //    {
            //        viewModel.StatusForm = "Initiated";
            //    }
            //    if (viewModel.StatusForm == "DraftDraft")
            //    {
            //        viewModel.StatusForm = "Draft";
            //    }

            //    Response.StatusCode = (int)HttpStatusCode.BadRequest;
            //    return JsonHelper.GenerateJsonErrorResponse("Weight must be total 100%");
            //}

            int? headerID = null;
            try
            {
                headerID = _hRDayOffService.CreateHeader(viewModel);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error", new { errorMessage = e.Message });
            }

            if (viewModel.StatusForm != "DraftInitiated")
            {
                Task createTransactionWorkflowItemsTask = WorkflowHelper.CreateTransactionWorkflowAsync(SP_TRANSACTION_WORKFLOW_LIST_NAME,
                    SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)headerID);
            }

            Task createDayOffDetailsTask = _hRDayOffService.CreateDayOffBalanceDetailsAsync(headerID, viewModel.DayOffBalanceDetails);

            Task allTasks = Task.WhenAll(createDayOffDetailsTask);

            try
            {
                await allTasks;
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            //try
            //{
            //    // Send to Level 1 Approver
            //    if (viewModel.StatusForm != "DraftInitiated")
            //        _hRDayOffService.SendEmail(viewModel, SP_TRANSACTION_WORKFLOW_LIST_NAME,
            //        SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)headerID, 1,
            //        string.Format("Dear Respective Approver,This email is sent to you to notify that there is a request which required your action to approve.Kindly check the link as per below to go to direct page accordingly.You may check your personal page in IMS(My Approval View).Thank you.Link: {0}{1}/EditFormApprover_Custom.aspx?ID={2}", siteUrl, UrlResource.ProfessionalDayOff, viewModel.ID), string.Format(""));
            //}
            //catch (Exception e)
            //{
            //    Response.StatusCode = (int)HttpStatusCode.BadRequest;
            //    return JsonHelper.GenerateJsonErrorResponse(e);
            //}

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.Professional);
        }

        public ActionResult EditDayOff(string siteUrl = null, int? ID = null, string requestor = null)
        {
            _hRDayOffService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var viewModel = _hRDayOffService.GetHeader(ID, requestor);
            viewModel.Requestor = requestor;
            viewModel.ID = ID;
            ViewBag.Action = "EditDayOff";

            // Used for Workflow Router
            ViewBag.ListName = "Professional%20Performance%20Plan";
            ViewBag.Requestor = requestor;

            return View("DayOff", viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> EditDayOff(FormCollection form, DayOffRequestVM viewModel, string site)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _hRDayOffService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            //var Detail = viewModel.ProjectOrUnitGoalsDetails;
            //var Count = Detail.Count();
            //int sum = 0;
            //string project = null;
            //string individual = null;
            //if (Count != 0)
            //{
            //    foreach (var viewModelDetail in Detail)
            //    {
            //        if (viewModelDetail.EditMode != -1)
            //        {
            //            if (viewModelDetail.ProjectOrUnitGoals == null)
            //            {
            //                viewModelDetail.ProjectOrUnitGoals = "";
            //                project = "Empty";
            //            }

            //            if (viewModelDetail.IndividualGoalAndPlan == null)
            //            {
            //                viewModelDetail.IndividualGoalAndPlan = "";
            //                individual = "Empty";
            //            }

            //            if (viewModelDetail.Remarks == null)
            //            {
            //                viewModelDetail.Remarks = "";
            //            }

            //            sum = sum + viewModelDetail.Weight;
            //        }
            //    }

            //    if (project == "Empty")
            //    {
            //        if (viewModel.StatusForm == "DraftInitiated")
            //        {
            //            viewModel.StatusForm = "Initiated";
            //        }
            //        if (viewModel.StatusForm == "DraftDraft")
            //        {
            //            viewModel.StatusForm = "Draft";
            //        }

            //        Response.StatusCode = (int)HttpStatusCode.BadRequest;
            //        return JsonHelper.GenerateJsonErrorResponse("Project Or Unit Goals is Required");
            //    }

            //    if (individual == "Empty")
            //    {
            //        if (viewModel.StatusForm == "DraftInitiated")
            //        {
            //            viewModel.StatusForm = "Initiated";
            //        }
            //        if (viewModel.StatusForm == "DraftDraft")
            //        {
            //            viewModel.StatusForm = "Draft";
            //        }

            //        Response.StatusCode = (int)HttpStatusCode.BadRequest;
            //        return JsonHelper.GenerateJsonErrorResponse("Individual Goal And Plan is Required");
            //    }

            //    if (sum != 100)
            //    {
            //        if (viewModel.StatusForm == "DraftInitiated")
            //        {
            //            viewModel.StatusForm = "Initiated";
            //        }
            //        if (viewModel.StatusForm == "DraftDraft")
            //        {
            //            viewModel.StatusForm = "Draft";
            //        }

            //        Response.StatusCode = (int)HttpStatusCode.BadRequest;
            //        return JsonHelper.GenerateJsonErrorResponse("Weight must be total 100%");
            //    }
            //}

            _hRDayOffService.UpdateHeader(viewModel);

            if (viewModel.StatusForm == "Initiated" || viewModel.StatusForm == "DraftInitiated")
            {
                Task createTransactionWorkflowItemsTask = WorkflowHelper.CreateTransactionWorkflowAsync(SP_TRANSACTION_WORKFLOW_LIST_NAME,
                    SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)viewModel.ID);
            }

            Task createDayOffDetailsTask = _hRDayOffService.CreateDayOffBalanceDetailsAsync(viewModel.ID, viewModel.DayOffBalanceDetails);

            Task allTasks = Task.WhenAll(createDayOffDetailsTask);

            try
            {
                await allTasks;
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            //try
            //{
            //    // Send to Level 1 Approver
            //    if (viewModel.StatusForm == "Initiated" || viewModel.StatusForm == "Draft")
            //        _hRDayOffService.SendEmail(viewModel, SP_TRANSACTION_WORKFLOW_LIST_NAME,
            //        SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)viewModel.ID, 1,
            //        string.Format(EmailResource.ProfessionalDayOff, siteUrl, UrlResource.ProfessionalDayOff, viewModel.ID), string.Format(""));

            //    // Send to Level 2 Approver and Requestor
            //    if (viewModel.Requestor == null && viewModel.StatusForm == "Pending Approval 1 of 2")
            //        _hRDayOffService.SendEmail(viewModel, SP_TRANSACTION_WORKFLOW_LIST_NAME,
            //        SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)viewModel.ID, 2,
            //        string.Format(EmailResource.ProfessionalDayOff, siteUrl, UrlResource.ProfessionalDayOff, viewModel.ID), string.Format("Approved By Level 1"));

            //    // Send to Level 1 Approver
            //    if (viewModel.Requestor != null && viewModel.StatusForm == "Pending Approval 1 of 2")
            //        _hRDayOffService.SendEmail(viewModel, SP_TRANSACTION_WORKFLOW_LIST_NAME,
            //        SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)viewModel.ID, 1,
            //        string.Format(EmailResource.ProfessionalDayOff, siteUrl, UrlResource.ProfessionalDayOff, viewModel.ID), string.Format(""));

            //    // Send to Level 2 Approver
            //    if (viewModel.Requestor != null && viewModel.StatusForm == "Pending Approval 2 of 2")
            //        _hRDayOffService.SendEmail(viewModel, SP_TRANSACTION_WORKFLOW_LIST_NAME,
            //        SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)viewModel.ID, 2,
            //        string.Format(EmailResource.ProfessionalDayOff, siteUrl, UrlResource.ProfessionalDayOff, viewModel.ID), string.Format(""));

            //    // Send to Requestor
            //    if (viewModel.Requestor == null && viewModel.StatusForm == "Pending Approval 2 of 2")
            //        _hRDayOffService.SendEmail(viewModel, SP_TRANSACTION_WORKFLOW_LIST_NAME,
            //        SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)viewModel.ID, 2,
            //        string.Format(""), string.Format("Approved by Level 2"));

            //    // Send to Requestor
            //    if (viewModel.StatusForm == "Reject1")
            //        _hRDayOffService.SendEmail(viewModel, SP_TRANSACTION_WORKFLOW_LIST_NAME,
            //        SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)viewModel.ID, 1,
            //        string.Format(""), string.Format("Rejected by Approver Level 1"));

            //    // Send to Requestor
            //    if (viewModel.StatusForm == "Reject2")
            //        _hRDayOffService.SendEmail(viewModel, SP_TRANSACTION_WORKFLOW_LIST_NAME,
            //        SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)viewModel.ID, 1,
            //        string.Format(""), string.Format("Rejected by Approver Level 2"));
            //}
            //catch (Exception e)
            //{
            //    Response.StatusCode = (int)HttpStatusCode.BadRequest;
            //    return JsonHelper.GenerateJsonErrorResponse(e);
            //}

            string url = null;
            //if (viewModel.TypeForm == "Professional")
            //{
            //    url = UrlResource.ProfessionalDayOff;
            //}
            //if (viewModel.TypeForm != "Professional")
            //{
            //    url = UrlResource.ProfessionalDayOffApprover;
            //}

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + url);
        }

        public ActionResult CalculateBalance(string requestor, string siteUrl = null, bool isPartial = true)
        {
            _hRDayOffService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            ViewBag.ListName = "Day-Off Balance";

            var listName = ViewBag.ListName;

            ViewBag.requestor = requestor;

            SessionManager.Set("RequestorUserLogin", requestor);

            var viewModel = _hRDayOffService.GetCalculateBalance(null, siteUrl, requestor, listName);

            SessionManager.Set("ExitProcedureChecklist", viewModel.ExitProcedureChecklist);
            SessionManager.Set("WorkflowRouterListName", viewModel.ListName);
            SessionManager.Set("WorkflowRouterRequestorUnit", viewModel.RequestorUnit);
            SessionManager.Set("WorkflowRouterRequestorPosition", viewModel.RequestorPosition);

            if (isPartial)
                return PartialView("_ExitProcedureChecklistForHR", viewModel.ExitProcedureChecklist);
            return View("_ExitProcedureChecklistForHR", viewModel.ExitProcedureChecklist);

        }

        public JsonResult GetMasterDayOffType()
        {
            _hRDayOffService.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);

            var dayOffTypeMaster = _hRDayOffService.GetDayOffType();

            //var positions = _hRDayOffService.GetDayOffType();
            dayOffTypeMaster = dayOffTypeMaster.OrderBy(e => e.DayOffTypeID);
            return Json(dayOffTypeMaster.Select(e =>
                new
                {
                    Value = Convert.ToInt32(e.DayOffTypeID),
                    Text = Convert.ToString(e.DayOffTypeName)
                }),
                JsonRequestBehavior.AllowGet);
        }

    }
}