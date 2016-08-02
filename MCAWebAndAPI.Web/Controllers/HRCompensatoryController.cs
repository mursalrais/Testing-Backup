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

        public ActionResult InputCompensatoryUser(string siteurl = null, int? iD = null)
        {
            //mandatory: set site url
            _service.SetSiteUrl(siteurl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteurl ?? ConfigResource.DefaultHRSiteUrl);

            if (siteurl == "")
            {
                siteurl = SessionManager.Get<string>("SiteUrl");
            }
            _service.SetSiteUrl(siteurl ?? ConfigResource.DefaultHRSiteUrl);

            ViewBag.ListName = "Compensatory%20Request";

            var viewmodel = _service.GetComplistbyCmpid(iD);

            viewmodel.cmpID = iD;
            SessionManager.Set("IdComp", Convert.ToString(iD));

            if (viewmodel.cmpEmail != null)
            SessionManager.Set("RequestorUserLogin", viewmodel.cmpEmail);

            return View(viewmodel);
        }
         
        public ActionResult InputCompensatoryHR(string siteurl = null, int? iD = null, string userAccess = null)
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

            viewmodel = _service.GetComplistbyCmpid(iD);

            viewmodel.Requestor = userAccess;

            ViewBag.ListName = "Compensatory%20Request";

            viewmodel.cmpID = iD;
            if (viewmodel.cmpEmail != null)
                SessionManager.Set("RequestorUserLogin", viewmodel.cmpEmail);

            return View(viewmodel);
        }

        public ActionResult CompensatorylistUser(string siteurl = null, int? iD = null, string userAccess = null)
       {
            //mandatory: set site url
            _service.SetSiteUrl(siteurl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteurl ?? ConfigResource.DefaultHRSiteUrl);

            var viewmodel = _service.GetComplistbyCmpid(iD);
            viewmodel.Requestor = userAccess;

            //viewmodel.ID = id;
            return View(viewmodel);
        }

        public ActionResult CompensatorylistHR(string siteurl = null, int? iD = null)
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

            var viewmodel = _service.GetComplistbyCmpid(iD);

            //viewmodel.ID = id;
            return View(viewmodel);
        }

        [HttpPost]
        public async Task<ActionResult> CreateCompensatoryData(FormCollection form, CompensatoryVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var testget = form[""];

            int? cmpID = viewModel.cmpID;

            try
            {
                viewModel.CompensatoryDetails = BindCompensatorylistDateTime(form, viewModel.CompensatoryDetails);
                _service.CreateCompensatoryData(cmpID, viewModel);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            if (viewModel.StatusForm != "submit")
            {
                _service.UpdateHeader(viewModel);
            }

            if (viewModel.StatusForm != "DraftInitiated")
            {
                Task createTransactionWorkflowItemsTask = WorkflowHelper.CreateTransactionWorkflowAsync(SP_TRANSACTION_WORKFLOW_LIST_NAME,
                    SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME, (int)viewModel.cmpID);
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.Compensatory);
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

            if (idComp == null)
                return PartialView("_InputCompensantoryDetails", viewmodel.CompensatoryDetails);

            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            viewmodel = _service.GetComplistbyCmpid(idComp);

            return PartialView("_InputCompensantoryDetails", viewmodel.CompensatoryDetails);
        }

        public async Task<ActionResult> GetCompensatoryDetailsUser(int? idComp)
        {
            var viewmodel = new CompensatoryVM();

            if (idComp == null)
                return PartialView("_InputCompensantoryDetails", viewmodel.CompensatoryDetails);

            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            viewmodel = _service.GetComplistbyCmpid(idComp);

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