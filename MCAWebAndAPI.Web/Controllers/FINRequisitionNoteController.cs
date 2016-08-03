using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Elmah;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Converter;
using MCAWebAndAPI.Service.Finance;
using MCAWebAndAPI.Service.Finance.RequisitionNote;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;

namespace MCAWebAndAPI.Web.Controllers.Finance
{
    [Filters.HandleError]
    public class FINRequisitionNoteController : Controller
    {
        readonly IRequisitionNote _service;

        private const string LIST_NAME = "requisitionnote";
        private const string SESSION_SITE_URL = "SiteUrl";
        private const string WORKFLOW_TITLE = "Requisition%20Note";
        private const string INDEX_PAGE = "Index";
        private const string ERROR = "Error";
        private const string DATA_NOT_EXISTS = "Data Does not exists!";
        private const string PRINT_PAGE_URL = "~/Views/FINRequisitionNote/PrintRequisitionNote.cshtml";

        public FINRequisitionNoteController()
        {
            _service = new RequisitionNoteService();
        }

        public ActionResult CreateRequisitionNote(string siteUrl = null)
        {
            _service.SetSiteUrl(siteUrl);
            SessionManager.Set(SESSION_SITE_URL, siteUrl);

            ViewBag.ListName = WORKFLOW_TITLE;
            
            var viewModel = _service.GetRequisitionNote(null);
            return View(viewModel);
        }

        public ActionResult EditRequisitionNote(string siteUrl = null, int ID = 0)
        {
            if (ID > 0)
            {
                _service.SetSiteUrl(siteUrl);
                SessionManager.Set(SESSION_SITE_URL, siteUrl);

                ViewBag.ListName = WORKFLOW_TITLE;

                var viewModel = _service.GetRequisitionNote(ID);
                return View(viewModel);
            }
            else
            {
                return RedirectToAction(INDEX_PAGE,ERROR, new { errorMessage = DATA_NOT_EXISTS });
            }
        }


        [HttpPost]
        public async Task<ActionResult> CreateRequisitionNote(FormCollection form, RequisitionNoteVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>(SESSION_SITE_URL);
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            int? headerID = null;
            try
            {
                headerID = _service.CreateRequisitionNote(viewModel);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction(INDEX_PAGE, ERROR, new { errorMessage = e.Message });
            }

            Task CreateDetailsTask = _service.CreateRequisitionNoteItemsAsync(headerID, viewModel.ItemDetails);
            Task CreateDocumentsTask = _service.CreateRequisitionNoteAttachmentsSync(headerID, viewModel.Documents);

            Task allTasks = Task.WhenAll(CreateDetailsTask, CreateDocumentsTask);

            try
            {
                await allTasks;
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction(INDEX_PAGE, ERROR, new { errorMessage = e.Message });
            }

            return RedirectToAction(INDEX_PAGE,
                "Success",
                new
                {
                    errorMessage =
                string.Format(MessageResource.SuccessCreateApplicationData, headerID)
                });
        }

        [HttpPost]
        public async Task<ActionResult> EditRequisitionNote(FormCollection form, RequisitionNoteVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>(SESSION_SITE_URL);
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            try
            {
                _service.UpdateRequisitionNote(viewModel);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction(INDEX_PAGE, ERROR, new { errorMessage = e.Message });
            }

            Task CreateDetailsTask = _service.EditRequisitionNoteItemsAsync(viewModel.ID, viewModel.ItemDetails);
            Task CreateDocumentsTask = _service.EditRequisitionNoteAttachmentsSync(viewModel.ID, viewModel.Documents);

            Task allTasks = Task.WhenAll(CreateDetailsTask, CreateDocumentsTask);

            try
            {
                await allTasks;
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction(INDEX_PAGE, ERROR, new { errorMessage = e.Message });
            }

            return RedirectToAction(INDEX_PAGE,
                "Success",
                new
                {
                    errorMessage =
                string.Format(MessageResource.SuccessCommon, viewModel.ID)
                });
        }
        [HttpPost]
        public JsonResult GetRequisitionNoteDetailsByEventBudgetId([DataSourceRequest] DataSourceRequest request, int? eventBudgetId)
        {
            var details = new List<RequisitionNoteItemVM>();

            if (eventBudgetId.HasValue && eventBudgetId.Value > 0)
            {
                //for (int i = 0; i < 5; i++)
                //{
                //    details.Add(new RequisitionNoteItemVM()
                //    {
                //        ID = i,
                //        Title = "FF",
                //        Activity = new Model.ViewModel.Control.AjaxComboBoxVM() { Value = i, Text = "2" },
                //        Specification = "TEST"

                //    });
                //}
            }
            
            
            // Convert to Kendo DataSource
            DataSourceResult result = details.ToDataSourceResult(request);
            // Convert to Json
            var json = Json(result, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }

        public JsonResult GetGLMaster()
        {
            var siteUrl = SessionManager.Get<string>(SESSION_SITE_URL);
            _service.SetSiteUrl(siteUrl);

            var glMasters = Shared.GetGLMaster(siteUrl);

            return Json(glMasters.Select(e => new
            {
                Value = e.ID.HasValue ? Convert.ToString(e.ID) : string.Empty,
                Text = e.Title
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWBSMaster()
        {
            _service.SetSiteUrl(SessionManager.Get<string>(SESSION_SITE_URL));

            var wbsMasters = _service.GetWBSMaster();

            return Json(wbsMasters.Select(e => new
            {
                Value = e.ID.HasValue ? Convert.ToString(e.ID) : string.Empty,
                Text = e.Title
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetActivity()
        {
            _service.SetSiteUrl(SessionManager.Get<string>(SESSION_SITE_URL));
      
            var activities = _service.GetActivity();

            return Json(activities.Select(e => new
            {
                Value = e.ID.HasValue ? Convert.ToString(e.ID) : string.Empty,
                Text = e.Title
            }), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PrintRequisitionNote(FormCollection form, RequisitionNoteVM viewModel)
        {
            string RelativePath = PRINT_PAGE_URL;

            ViewData.Model = viewModel;
            var view = ViewEngines.Engines.FindView(ControllerContext, RelativePath, null);
            var fileName = viewModel.Title + "_Application.pdf";
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