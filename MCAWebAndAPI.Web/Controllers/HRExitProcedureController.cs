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
using MCAWebAndAPI.Service.JobSchedulers.Schedulers;


namespace MCAWebAndAPI.Web.Controllers
{
    public class HRExitProcedureController : Controller
    {
        IExitProcedureService exitProcedureService;

        const string SP_TRANSACTION_WORKFLOW_LIST_NAME = "Exit Procedure Workflow";
        const string SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME = "exitprocedure";
        const string SP_EXP_CHECK_LIST = "Exit Procedure Checklist";

        public HRExitProcedureController()
        {
            exitProcedureService = new ExitProcedureService();
        }

        /// <summary>
        /// HTTP Get to View Create with specific argument
        /// </summary>
        /// <param name="siteUrl"></param>
        /// <returns></returns>
        public ActionResult CreateExitProcedure(int?ID, string user, string siteUrl = null, string requestor = null)
        {  
            // MANDATORY: Set Site URL
            exitProcedureService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            ViewBag.ListName = "Exit Procedure";

            var listName = ViewBag.ListName;

            ViewBag.RequestorUserLogin = requestor;

            SessionManager.Set("RequestorUserLogin", requestor);

            string projectUnit = exitProcedureService.GetProjectUnit(requestor);
            
                var viewModel = exitProcedureService.GetExitProcedure(null, siteUrl, requestor, listName, user);

                SessionManager.Set("UserLogin", requestor);
                SessionManager.Set("ExitProcedureChecklist", viewModel.ExitProcedureChecklist);
                SessionManager.Set("WorkflowRouterListName", viewModel.ListName);
                SessionManager.Set("WorkflowRouterRequestorUnit", viewModel.RequestorUnit);
                SessionManager.Set("WorkflowRouterRequestorPosition", viewModel.RequestorPosition);

                return View("CreateExitProcedure", viewModel);
        }

        public ActionResult CreateExitProcedureHR(int? ID, string siteUrl = null, string requestor = null)
        {
            // MANDATORY: Set Site URL
            exitProcedureService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            ViewBag.ListName = "Exit Procedure";

            var listName = ViewBag.ListName;

            ViewBag.RequestorUserLogin = requestor;

            SessionManager.Set("RequestorUserLogin", requestor);

            // Get blank ViewModel
            var viewModel = exitProcedureService.GetExitProcedureHR(null, siteUrl);

            return View("CreateExitProcedureHR", viewModel);

        }

        public ActionResult DisplayExitChecklistForHR(string professionalMail, string siteUrl = null, bool isPartial = true)
        {
            exitProcedureService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            ViewBag.ListName = "Exit Procedure";

            var listName = ViewBag.ListName;

            ViewBag.RequestorUserLogin = professionalMail;

            SessionManager.Set("RequestorUserLogin", professionalMail);

            var viewModel = exitProcedureService.GetExitChecklistForHR(null, siteUrl, professionalMail, listName);

            SessionManager.Set("ExitProcedureChecklist", viewModel.ExitProcedureChecklist);
            SessionManager.Set("WorkflowRouterListName", viewModel.ListName);
            SessionManager.Set("WorkflowRouterRequestorUnit", viewModel.RequestorUnit);
            SessionManager.Set("WorkflowRouterRequestorPosition", viewModel.RequestorPosition);

            if (isPartial)
                return PartialView("_ExitProcedureChecklistForHR", viewModel.ExitProcedureChecklist);
            return View("_ExitProcedureChecklistForHR", viewModel.ExitProcedureChecklist);

        }

        public ActionResult DisplayExitProcedure(string siteUrl = null, int? ID = null, string requestor = null)
        {
            // MANDATORY: Set Site URL
            exitProcedureService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var viewModel = exitProcedureService.GetExitProcedure(ID);


            if (viewModel.ID != null)
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

        public ActionResult DisplayExitProcedureHR(int? ID, string siteUrl = null, string requestor = null)
        {
            exitProcedureService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            ViewBag.ListName = "Exit Procedure";

            var listName = ViewBag.ListName;

            ViewBag.RequestorUserLogin = requestor;

            SessionManager.Set("RequestorUserLogin", requestor);

            // Get blank ViewModel
            var viewModel = exitProcedureService.GetExitProcedureHR(ID, siteUrl);

            return View("EditExitProcedureHR", viewModel);

        }

        public ActionResult ViewExitProcedureForApprover(int? ID, string siteUrl = null, string approver = null)
        {
            // MANDATORY: Set Site URL
            exitProcedureService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            ViewBag.ListName = "Exit Procedure";

            var listName = ViewBag.ListName;

            ViewBag.ApproverUserLogin = approver;

            var viewModel = exitProcedureService.GetExitProcedureForApprove(ID, siteUrl, approver);

            viewModel.ApproverMail = approver;

            return View("ViewExitProcedureForApprover", viewModel);
        }

        [HttpPost]
        public ActionResult UpdateChecklistItemApprover(FormCollection form, ExitProcedureVM viewModel)
        {
            // Check whether error is found
            if (!ModelState.IsValid)
            {
                RedirectToAction("Index", "Error");
            }

            var siteUrl = SessionManager.Get<string>("SiteUrl");
            exitProcedureService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            viewModel.ExitProcedureChecklist = BindExitProcedureChecklist(form, viewModel.ExitProcedureChecklist);
            bool updateExitChecklist = exitProcedureService.UpdateExitChecklist(viewModel, viewModel.ExitProcedureChecklist);
            
            string checklistStatusApproved = "Pending Approval";

            bool cekStatusExitProcedure = exitProcedureService.CheckPendingApproval(viewModel.ID, checklistStatusApproved);

            int psaID;
            
            if(cekStatusExitProcedure == false)
            {
                string statusExitProcedure = "Approved";
                bool statusExitProcedureApproved = exitProcedureService.UpdateExitProcedureStatus(viewModel.ID, statusExitProcedure);

                if(statusExitProcedureApproved == true)
                {
                    DateTime lastWorkingDate = exitProcedureService.GetLastWorkingDate(viewModel.ID);
                    string psaNumber = exitProcedureService.GetPSANumberOnExitProcedure(viewModel.ID);

                    if (psaNumber != null)
                    {
                        psaID = exitProcedureService.GetPSAId(psaNumber);

                        if (psaID != 0)
                        {
                            exitProcedureService.UpdateLastWorkingDateOnPSA(psaID, lastWorkingDate);
                        }
                    }
                }
            }
     
            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.ExitProcedure);

            //return RedirectToAction("Index",
            //    "Success",
            //    new { errorMessage = string.Format(MessageResource.SuccessCreateExitProcedureData, exitProcID) });
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
            
            string requestorposition = Convert.ToString(viewModel.Position);
            string requestorunit = Convert.ToString(viewModel.ProjectUnit);

            int? positionID = exitProcedureService.GetPositionID(requestorposition, requestorunit, 0, 0);

            viewModel.ExitProcedureChecklist = BindExitProcedureChecklist(form, viewModel.ExitProcedureChecklist);
            Task createExitProcedureChecklist = exitProcedureService.CreateExitProcedureChecklistAsync(viewModel, exitProcID, viewModel.ExitProcedureChecklist, requestorposition, requestorunit, positionID);

            try
            {
                exitProcedureService.CreateExitProcedureDocuments(exitProcID, viewModel.Documents, viewModel);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error");
            }

            //Cek Status Record
            string exitProcedureStatus = exitProcedureService.GetExitProcedureStatus(exitProcID);

            if (exitProcedureStatus == "Approved")
            {
                DateTime lastWorkingDate = exitProcedureService.GetLastWorkingDate(exitProcID);
                string psaNumber = exitProcedureService.GetPSANumberOnExitProcedure(exitProcID);
                int psaID = exitProcedureService.GetPSAId(psaNumber);
                exitProcedureService.UpdateLastWorkingDateOnPSA(psaID, lastWorkingDate);
            }

            try
            {
                if (viewModel.StatusForm == "Pending Approval")
                {
                    exitProcedureService.SendMailDocument(viewModel.RequestorMailAddress, string.Format("Thank You For Your Request, Please kindly download Non Disclosure Document on this url: {0}{1} and Exit Interview Form on this url: {2}{3}", siteUrl, UrlResource.ExitProcedureNonDisclosureAgreement, siteUrl, UrlResource.ExitProcedureExitInterviewForm));

                    exitProcedureService.SendEmail(viewModel, SP_EXP_CHECK_LIST,
                    SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)exitProcID,
                    string.Format("Dear Respective Approver : {0}{1}/ViewExitProcedureForApprover.aspx?ID={2}", siteUrl, UrlResource.ExitProcedure, viewModel.ID), string.Format("Message for Requestor"));
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

        

        public ActionResult UpdateExitProcedure(ExitProcedureVM exitProcedure, FormCollection form)
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

            var exitProcedureChecklist = exitProcedure.ExitProcedureChecklist;

            string requestorposition = Convert.ToString(exitProcedure.RequestorPosition);
            string requestorunit = Convert.ToString(exitProcedure.RequestorUnit);

            int? positionID = exitProcedureService.GetPositionID(requestorposition, requestorunit, 0, 0);

            exitProcedure.ExitProcedureChecklist = BindExitProcedureChecklist(form, exitProcedure.ExitProcedureChecklist);
            Task createExitProcedureChecklist = exitProcedureService.CreateExitProcedureChecklistAsync(exitProcedure, exitProcedure.ID, exitProcedure.ExitProcedureChecklist, requestorposition, requestorunit, positionID);

            try
            {
                if (exitProcedure.StatusForm == "Pending Approval")
                {
                    //exitProcedureService.SendMailDocument(exitProcedure.RequestorMailAddress, string.Format("Thank You For Your Request, Please kindly download Non Disclosure Document on this url: {0}{1} and Exit Interview Form on this url: {2}{3}", siteUrl, UrlResource.ExitProcedureNonDisclosureAgreement, siteUrl, UrlResource.ExitProcedureExitInterviewForm));

                    exitProcedureService.SendEmail(exitProcedure, SP_TRANSACTION_WORKFLOW_LIST_NAME,
                    SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)exitProcedure.ID,
                    string.Format("Dear Respective Approver : {0}{1}/ViewExitProcedureForApprover.aspx?ID={2}", siteUrl, UrlResource.ExitProcedure, exitProcedure.ID), string.Format("Message for Requestor"));

                }
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
        
        IEnumerable<ExitProcedureChecklistVM> BindExitProcedureChecklist(FormCollection form, IEnumerable<ExitProcedureChecklistVM> exitProcedureChecklist)
        {
            var array = exitProcedureChecklist.ToArray();

            for (int i = 0; i < array.Length; i++)
            {
                array[i].DateOfApproval = BindHelper.BindDateInGrid("ExitProcedureChecklist",
                    i, "DateOfApproval", form);
            }
            return array;
        }

        public ActionResult FiveDaysNotApproved(string siteUrl = null)
        {
            try
            {
                ExitProcedureManagementScheduler.DoNow_OnceEveryDay(siteUrl);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error");
            }
            return RedirectToAction("Index", "Success");
        }
    }
}