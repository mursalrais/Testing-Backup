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
using System.Web;
using MCAWebAndAPI.Service.Utils;

namespace MCAWebAndAPI.Web.Controllers
{
    [Filters.HandleError]
    public class HRCompensatoryController : Controller
    {
        IHRCompensatoryService _service;
        const string SP_TRANSACTION_WORKFLOW_LIST_NAME = "Compensatory Request Workflow";
        const string SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME = "compensatoryrequest";

        public HRCompensatoryController()
        {
            _service = new HRCompensatoryService();
        }

        public async Task<ActionResult> AddCompensatoryHR(string siteurl = null, string userAccess = null)
        {
            var viewmodel = new CompensatoryVM();

            if (siteurl == "")
            {
                siteurl = SessionManager.Get<string>("SiteUrl");
                _service.SetSiteUrl(siteurl ?? ConfigResource.DefaultHRSiteUrl);
            }
            else
            {
                _service.SetSiteUrl(siteurl ?? ConfigResource.DefaultHRSiteUrl);
                SessionManager.Set("siteurl", siteurl ?? ConfigResource.DefaultHRSiteUrl);
            }
            
            if (userAccess != null)
                viewmodel = await _service.GetWorkflow(userAccess, "Compensatory Request");

            viewmodel.cmpEmail = userAccess;

            string position = _service.GetPosition(userAccess);

            if (position.Contains("HR"))
            {
                return View("AddCompensatoryHR", viewmodel);
            }
            else
            {
                viewmodel = _service.GetProfessional(userAccess, viewmodel);
                return View("AddCompensatoryUser", viewmodel);
            }
        }

        public ActionResult InputCompensatoryUser(string siteurl = null, int? iD = null)
        {
            var viewmodel = new CompensatoryVM();

            //mandatory: set site url
            _service.SetSiteUrl(siteurl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteurl ?? ConfigResource.DefaultHRSiteUrl);

            if (siteurl == "")
            {
                siteurl = SessionManager.Get<string>("SiteUrl");
            }
            _service.SetSiteUrl(siteurl ?? ConfigResource.DefaultHRSiteUrl);

            return View(viewmodel);
        }
         
        public async Task<ActionResult> InputCompensatoryHR(string siteurl = null, int? iD = null, string userAccess = null, string accesstype = null)
        {
            var viewmodel = new CompensatoryVM();

            if (siteurl == "")
            {
                siteurl = SessionManager.Get<string>("SiteUrl");
                _service.SetSiteUrl(siteurl ?? ConfigResource.DefaultHRSiteUrl);
            }
            else
            {
                _service.SetSiteUrl(siteurl ?? ConfigResource.DefaultHRSiteUrl);
                SessionManager.Set("siteurl", siteurl ?? ConfigResource.DefaultHRSiteUrl);
            }

            viewmodel.cmpEmail = userAccess;

            ViewBag.ListName = "Compensatory%20Request";

            viewmodel.cmpID = iD;

            if (viewmodel.cmpEmail != null)
            viewmodel = await _service.GetComplistbyCmpid(iD, viewmodel.cmpEmail, "Compensatory Request", SP_TRANSACTION_WORKFLOW_LIST_NAME, SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME);

            string position = _service.GetPosition(userAccess);

            if (position.Contains("HR") || accesstype == "app")
                return View("InputCompensatoryHR", viewmodel);


            return View("InputCompensatoryUser", viewmodel);
        }

        public ActionResult CompensatorylistUser(string siteurl = null, int? iD = null, string username = null)
       {
            if (siteurl == "")
            {
                siteurl = SessionManager.Get<string>("SiteUrl");
                _service.SetSiteUrl(siteurl ?? ConfigResource.DefaultHRSiteUrl);
            }
            else
            {
                _service.SetSiteUrl(siteurl ?? ConfigResource.DefaultHRSiteUrl);
                SessionManager.Set("siteurl", siteurl ?? ConfigResource.DefaultHRSiteUrl);
            }
            var viewmodel = _service.GetViewlistbyCmpid(iD);

            string position = _service.GetPosition(username);

            if (position.Contains("HR"))
            {
                ViewBag.IsHRView = true;
            }
            else
            {
                ViewBag.IsHRView = false;
            }

            //viewmodel.ID = id;
            return View(viewmodel);
        }

        public ActionResult CompensatorylistHR(string siteurl = null, int? iD = null, string username = null)
        {
            if (siteurl == "")
            {
                siteurl = SessionManager.Get<string>("SiteUrl");
                _service.SetSiteUrl(siteurl ?? ConfigResource.DefaultHRSiteUrl);
            }
            else
            {
                _service.SetSiteUrl(siteurl ?? ConfigResource.DefaultHRSiteUrl);
                SessionManager.Set("siteurl", siteurl ?? ConfigResource.DefaultHRSiteUrl);
            }

            var viewmodel = _service.GetViewlistbyCmpid(iD);

            string position = _service.GetPosition(username);

            if (position.Contains("HR"))
            {
                ViewBag.IsHRView = true;
            }
            else
            {
                ViewBag.IsHRView = false;
            }

            //viewmodel.ID = id; c
            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult CreateHeaderCompensatory(FormCollection form, CompensatoryVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("WorkflowItems", viewModel.WorkflowItems);

            int? cmpID = null;
            bool checkdate;

            viewModel.CompensatoryDetails = BindCompensatorylistDateTime(form, viewModel.CompensatoryDetails);
            checkdate = _service.CheckRequest(viewModel);

            if(checkdate == true)
                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse("date is already used at the previous transactions");

            try
            {
                cmpID = _service.CreateHeaderCompensatory(viewModel);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error");
            }

            viewModel.cmpID = cmpID;

            if (viewModel.StatusForm == "submithr")
            {
                _service.UpdateHeader(viewModel);
            }

            // BEGIN Workflow Demo 
            Task createTransactionWorkflowItemsTask = WorkflowHelper.CreateWorkflowAsync(SP_TRANSACTION_WORKFLOW_LIST_NAME, SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)cmpID);

            _service.SendEmail(SP_TRANSACTION_WORKFLOW_LIST_NAME, SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)cmpID, 1, string.Format(EmailResource.EmailCompensatoryApproval, siteUrl, cmpID));

            return RedirectToAction("Index",
               "Success",
               new { successMessage = string.Format(MessageResource.SuccessCreateCompensatoryData, viewModel.cmpName) });

        }

        [HttpPost]
        public ActionResult CreateCompensatoryData(FormCollection form, CompensatoryVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("WorkflowItems", viewModel.WorkflowItems);

            var testget = form[""];

            int? cmpID = viewModel.cmpID;

            bool checkdate;

            viewModel.CompensatoryDetails = BindCompensatorylistDateTime(form, viewModel.CompensatoryDetails);
            checkdate = _service.CheckRequest(viewModel);

            if (checkdate == true)
                Response.TrySkipIisCustomErrors = true;
            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return JsonHelper.GenerateJsonErrorResponse("date is already used at the previous transactions");

            try
            {
                viewModel.CompensatoryDetails = BindCompensatorylistDateTime(form, viewModel.CompensatoryDetails);
                _service.CreateCompensatoryData(cmpID, viewModel);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error");
            }

            _service.UpdateHeader(viewModel);
          
            if (viewModel.StatusForm != "Draft")
            {
                if (viewModel.StatusForm == "")
                {
                    // BEGIN Workflow Demo 
                    Task createTransactionWorkflowItemsTask = WorkflowHelper.CreateWorkflowAsync(SP_TRANSACTION_WORKFLOW_LIST_NAME, SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)cmpID);

                    // Send to Level 1 & 2 Approver
                    _service.SendEmail(SP_TRANSACTION_WORKFLOW_LIST_NAME, SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)cmpID, 1, string.Format(EmailResource.EmailCompensatoryApproval, siteUrl, cmpID));
                }
                else if (viewModel.StatusForm == "Pending Approval 1 of 2")
                {
                    EmailUtil.Send(viewModel.cmpEmail, "Ask for Approval", string.Format(EmailResource.EmailCompensatoryRequestor, siteUrl, cmpID));
                }
            }

            return RedirectToAction("Index",
                          "Success",
                          new { successMessage = string.Format(MessageResource.SuccessCreateCompensatoryData, viewModel.cmpName) });

        }

        private IEnumerable<CompensatoryDetailVM> BindCompensatorylistDateTime(FormCollection form, IEnumerable<CompensatoryDetailVM> compDetails)
        {
            var array = compDetails.ToArray();
            for (int i = 0; i < array.Length; i++)
            {
                array[i].CmpDate = BindHelper.BindDateInGrid("CompensatoryDetails",
                    i, "CmpDate", form);

                array[i].StartTime = array[i].CmpDate + BindHelper.BindTimeInGrid("CompensatoryDetails",
                    i, "StartTime", form);

                array[i].FinishTime = array[i].CmpDate + BindHelper.BindTimeInGrid("CompensatoryDetails",
                    i, "FinishTime", form);
            }
            return array;
        }

        public JsonResult GetStatusGrid()
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var positions = ShortlistDetailVM.GetStatusOptions();

            return Json(positions.Select(e =>
                new {
                    Value = Convert.ToString(e.Value),
                    Text = e.Text
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCompensatoryddl(int? idProf = null)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var comps = _service.GetCompensatoryIdbyProf(idProf);

            return Json(comps.Select(e =>
                new {
                    e.ID,
                    e.CompensatoryID,
                    e.CompensatoryDate,
                    e.CompensatoryTitle,
                    e.CompensatoryStatus,
                    Desc = string.Format("{0} {1}", e.CompensatoryDate, e.CompensatoryTitle)
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserCompensatoryddl()
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var getComp = SessionManager.Get<string>("IdComp");

            var comps = _service.GetCompensatoryId(Convert.ToInt32(getComp)); 

            return Json(comps.Select(e =>
                new {
                    e.ID,
                    e.CompensatoryID,
                    e.CompensatoryDate,
                    e.CompensatoryTitle,
                    e.CompensatoryStatus,
                    Desc = string.Format("{0} {1}", e.CompensatoryDate, e.CompensatoryTitle)
                }),
                JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetCompensatoryDetails(int? idComp)
        {
            var viewmodel = new CompensatoryVM();

            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            if (idComp == null)
                return PartialView("_InputCompensantoryDetails", viewmodel.CompensatoryDetails);

            viewmodel = _service.GetViewlistbyCmpid(idComp);

            return PartialView("_InputCompensantoryDetails", viewmodel.CompensatoryDetails);
        }

        public async Task<ActionResult> GetCompensatoryDetailsUser(int? idComp)
        {
            var viewmodel = new CompensatoryVM();

            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            if (idComp == null)
                return PartialView("_InputCompensantoryDetails", viewmodel.CompensatoryDetails);

            viewmodel = _service.GetViewlistbyCmpid(idComp);

            return PartialView("_InputCompensatoryDetailsUser", viewmodel.CompensatoryDetails);
        }

        [HttpPost]
        public ActionResult PrintCompensatoryRequest(FormCollection form, CompensatoryVM viewModel)
        {
            const string RelativePath = "~/Views/HRCompensatory/PrintCompensatoryRequest.cshtml";
            var view = ViewEngines.Engines.FindView(ControllerContext, RelativePath, null);
            var fileName = viewModel.cmpName + viewModel.cmpYearDate + "_CompensatoryRequest.pdf";
            byte[] pdfBuf = null;
            string content;


            // ControllerContext context = new ControllerContext();
            ControllerContext.Controller.ViewData.Model = viewModel;
            ViewData = ControllerContext.Controller.ViewData;
            TempData = ControllerContext.Controller.TempData;

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

        public async Task<ActionResult> GetWorkflowCompensatory(string idCmp, string listname, string cmpemail)
        {
            var viewmodel = new CompensatoryVM();

            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            if (cmpemail != null)
                    viewmodel = await _service.GetCheckWorkflow(Convert.ToInt32(idCmp), cmpemail, listname, SP_TRANSACTION_WORKFLOW_LIST_NAME, SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME);

            return PartialView("_WorkflowPathDetails", viewmodel);
        }

    }
}