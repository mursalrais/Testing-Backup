using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.Utils;
using System.Web.Mvc;
using System;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using MCAWebAndAPI.Service.HR.Recruitment;
using Elmah;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using MCAWebAndAPI.Model.HR.DataMaster;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Collections.Generic;
using System.Data;
using MCAWebAndAPI.Service.Resources;


namespace MCAWebAndAPI.Web.Controllers
{
    public class HRExitProcedureController : Controller
    {
        IExitProcedureService exitProcedureService;

        const string SP_TRANSACTION_WORKFLOW_LIST_NAME = "Exit Procedure Workflow";
        const string SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME = "exitprocedure";

        public HRExitProcedureController()
        {
            exitProcedureService = new ExitProcedureService();
        }

        /// <summary>
        /// HTTP Get to View Create with specific argument
        /// </summary>
        /// <param name="siteUrl"></param>
        /// <returns></returns>
        public ActionResult CreateExitProcedure(int?ID, int? level, string siteUrl = null, string requestor = null)
        {  
            // MANDATORY: Set Site URL
            exitProcedureService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            ViewBag.ListName = "Exit Procedure";

            var listName = ViewBag.ListName;

            ViewBag.RequestorUserLogin = requestor;

                if(level == null)
                {
                    SessionManager.Set("RequestorUserLogin", requestor);

                    // Get blank ViewModel
                    var viewModel = exitProcedureService.GetExitProcedure(null, siteUrl, requestor, listName);

                    SessionManager.Set("ExitProcedureChecklist", viewModel.ExitProcedureChecklist);
                    SessionManager.Set("WorkflowRouterListName", viewModel.ListName);
                    SessionManager.Set("WorkflowRouterRequestorUnit", viewModel.RequestorUnit);
                    SessionManager.Set("WorkflowRouterRequestorPosition", viewModel.RequestorPosition);
                    SessionManager.Set("UserLogin", requestor);

                    return View("CreateExitProcedure", viewModel);
                }
                else
                {
                    var viewModel = exitProcedureService.GetExitProcedureApprover(ID, siteUrl, requestor, level);

                    return View("ViewExitProcedureForApprover", viewModel);
                }
            
        }

        [HttpPost]
        public ActionResult UpdateChecklistItemApprover(FormCollection form, ExitProcedureForApproverVM viewModel)
        {
            // Check whether error is found
            if (!ModelState.IsValid)
            {
                RedirectToAction("Index", "Error");
            }

            var siteUrl = SessionManager.Get<string>("SiteUrl");
            exitProcedureService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            try
            {
                var updateChecklist = exitProcedureService.UpdateExitChecklistStatus(viewModel);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return RedirectToAction("Index",
                "Success",
                new { errorMessage = string.Format(MessageResource.SuccessUpdateExitProcedure) });

        }

        //Submit every data in Exit Procedure to List
        [HttpPost]
        public ActionResult CreateExitProcedure(FormCollection form, ExitProcedureVM viewModel)
        {
            // Check whether error is found
            if (!ModelState.IsValid)
            {
                RedirectToAction("Index", "Error");
            }

            var siteUrl = SessionManager.Get<string>("SiteUrl");
            exitProcedureService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            int? exitProcID = null;
            try
            {
                exitProcID = exitProcedureService.CreateExitProcedure(viewModel);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error", new { errorMessage = e.Message });
            }

            var exitProcedureChecklist = viewModel.ExitProcedureChecklist;

            string requestorposition = Convert.ToString(viewModel.RequestorPosition);
            string requestorunit = Convert.ToString(viewModel.RequestorUnit);

            Task createExitProcedureChecklist = exitProcedureService.CreateExitProcedureChecklistAsync(exitProcID, viewModel.ExitProcedureChecklist, requestorposition, requestorunit);

            try
            {
                exitProcedureService.CreateExitProcedureDocuments(exitProcID, viewModel.Documents, viewModel);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error");
            }

            if (viewModel.StatusForm == "Draft")
            {
                Task createTransactionWorkflowItemsTask = WorkflowHelper.CreateExitProcedureWorkflowAsync(SP_TRANSACTION_WORKFLOW_LIST_NAME,
                    SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)exitProcID);
            }
            else if(viewModel.StatusForm == "Pending Approval")
            {
                Task createTransactionWorkflowItemsTask = WorkflowHelper.CreateExitProcedureWorkflowAsync(SP_TRANSACTION_WORKFLOW_LIST_NAME,
                    SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)exitProcID);
            }

            try
            {
                if (viewModel.StatusForm == "Pending Approval")
                {
                    //exitProcedureService.SendMailDocument(viewModel.RequestorMailAddress, string.Format("Thank You For Your Request, Please kindly download Non Disclosure Document on this url: {0}{1} and Exit Interview Form on this url: {2}{3}", siteUrl, UrlResource.ExitProcedureNonDisclosureAgreement, siteUrl, UrlResource.ExitProcedureExitInterviewForm));

                    exitProcedureService.SendEmail(viewModel, SP_TRANSACTION_WORKFLOW_LIST_NAME,
                    SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)exitProcID,
                    string.Format("Dear Respective Approver : {0}{1}/EditFormApprover_Custom.aspx?ID={2}", siteUrl, UrlResource.ExitProcedure, viewModel.ID), string.Format("Message for Requestor"));


                }
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.ExitProcedure);

            //return RedirectToAction("Index",
            //    "Success",
            //    new { errorMessage = string.Format(MessageResource.SuccessCreateExitProcedureData, exitProcID) });
        }

        public ActionResult DisplayExitProcedure(string siteUrl = null, int? ID = null)
        {
            // MANDATORY: Set Site URL
            exitProcedureService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var viewModel = exitProcedureService.GetExitProcedure(ID);


            if(viewModel.ID != null)
            {
                return View("EditExitProcedure", viewModel);
            }
            else
            {
                return RedirectToAction("Index",
                "Error",
                new { errorMessage = string.Format(MessageResource.ErrorEditExitProcedure) });
            }
        }

        public ActionResult UpdateExitProcedure(ExitProcedureVM exitProcedure, string site)
        {

            var siteUrl = SessionManager.Get<string>("SiteUrl");
            exitProcedureService.SetSiteUrl(siteUrl);

            try
            {
                var headerID = exitProcedureService.UpdateExitProcedure(exitProcedure);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return RedirectToAction("Index",
                "Success",
                new { errorMessage = string.Format(MessageResource.SuccessUpdateExitProcedure, exitProcedure.ID) });

        }

        public ActionResult ViewExitProcedure(string siteUrl = null, int? ID = null)
        {
            // Clear Existing Session Variables if any

            // MANDATORY: Set Site URL
            exitProcedureService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var viewModel = exitProcedureService.ViewExitProcedure(ID);
            return View("DisplayExitProcedure", viewModel);
        }

        

        public JsonResult GetApproverPositions(int approverUnit)
        {
            exitProcedureService.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);
            var listName = SessionManager.Get<string>("ExitProcedureListName");
            var requestorPosition = SessionManager.Get<string>("ExitProcedureRequestorPosition");
            var requestorUnitName = SessionManager.Get<string>("ExitProcedureRequestorUnit");
            var approverUnitName = ExitProcedureChecklistVM.GetUnitOptions().FirstOrDefault(e => e.Value == approverUnit).Text;

            var viewModel = exitProcedureService.GetPositionsInWorkflow(listName, approverUnitName,
                requestorUnitName, requestorPosition);
            return Json(viewModel.Select(e => new {
                e.ID,
                e.PositionName
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetApproverNames(int position)
        {
            exitProcedureService.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);
            var positionName = exitProcedureService.GetPositionName(position);
            var viewModel = SessionManager.Get<IEnumerable<ProfessionalMaster>>("WorkflowApprovers", "Position" + position)
                ?? exitProcedureService.GetApproverNames(positionName);
            SessionManager.Set("WorkflowApprovers", "Position" + position, viewModel);

            return Json(viewModel.Select(e => new
            {
                e.ID,
                e.Name
            }), JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public JsonResult Grid_Read([DataSourceRequest] DataSourceRequest request)
        //{
        //    // Get from existing session variable or create new if doesn't exist
        //    var exitProcedureChecklist = SessionManager.Get<IEnumerable<ExitProcedureChecklistVM>>("ExitProcedureChecklist");

        //    // Convert to Kendo DataSource
        //    DataSourceResult result = exitProcedureChecklist.ToDataSourceResult(request);

        //    // Convert to Json
        //    var json = Json(result, JsonRequestBehavior.AllowGet);
        //    json.MaxJsonLength = int.MaxValue;
        //    return json;
        //}

        //[HttpPost]
        //public ActionResult Grid_Update([DataSourceRequest] DataSourceRequest request,
        //    [Bind(Prefix = "models")]IEnumerable<ExitProcedureChecklistVM> viewModel)
        //{
        //    // Get existing session variable
        //    var sessionVariables = SessionManager.Get<IEnumerable<ExitProcedureChecklistVM>>("ExitProcedureChecklist");
            
        //    /*
        //    foreach (var item in viewModel)
        //    {
        //        var obj = sessionVariables.FirstOrDefault(e => e.ID != item.ID);
        //        obj = item;
        //    }
        //    */

        //    //DataSourceResult result = sessionVariables.ToDataSourceResult(request);

        //    // Overwrite existing session variable
        //    SessionManager.Set("ExitProcedureChecklist", sessionVariables);

        //    // Return JSON
        //    DataSourceResult result = sessionVariables.ToDataSourceResult(request);
        //    var json = Json(result, JsonRequestBehavior.AllowGet);
        //    json.MaxJsonLength = int.MaxValue;
        //    return json;
        //}

        //[HttpPost]
        //public JsonResult Grid_Create([DataSourceRequest] DataSourceRequest request,
        //    [Bind(Prefix = "models")]IEnumerable<ExitProcedureChecklistVM> viewModel)
        //{
        //    var sessionVariables = SessionManager.Get<IEnumerable<ExitProcedureChecklistVM>>("ExitProcedureChecklist");

        //    SessionManager.Set("ExitProcedureChecklist", sessionVariables);

        //    DataSourceResult result = sessionVariables.ToDataSourceResult(request);

        //    var json = Json(result, JsonRequestBehavior.AllowGet);
        //    json.MaxJsonLength = int.MaxValue;
        //    return json;
        //}
        
    }
}