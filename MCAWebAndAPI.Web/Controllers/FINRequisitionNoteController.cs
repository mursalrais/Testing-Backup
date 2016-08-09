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
using FinService = MCAWebAndAPI.Service.Finance;
using MCAWebAndAPI.Service.Finance;
using MCAWebAndAPI.Service.Finance.RequisitionNote;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using MCAWebAndAPI.Service.Resources;

namespace MCAWebAndAPI.Web.Controllers.Finance
{
    [Filters.HandleError]
    public class FINRequisitionNoteController : Controller //FinSharedController
    {
        private const string LIST_NAME = "requisitionnote";
        private const string SESSION_SITE_URL = "SiteUrl";
        private const string WORKFLOW_TITLE = "Requisition%20Note";
        private const string INDEX_PAGE = "Index";
        private const string ERROR = "Error";
        private const string DATA_NOT_EXISTS = "Data Does not exists!";
        private const string PRINT_PAGE_URL = "~/Views/FINRequisitionNote/Print.cshtml";
        private const string CATEGORY_EVENT = "Event";
        private const string CATEGORY_NON_EVENT = "Non-event";

        readonly IRequisitionNote _service;
        readonly IEventBudgetService _eventBudgetService;
        
        public FINRequisitionNoteController()
        {
            _service = new RequisitionNoteService();
            _eventBudgetService = new EventBudgetService();
        }

        public ActionResult Create(string siteUrl = null)
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;

            _service.SetSiteUrl(siteUrl);
            SessionManager.Set(SESSION_SITE_URL, siteUrl);

            ViewBag.ListName = WORKFLOW_TITLE;
            
            var viewModel = _service.GetRequisitionNote(null);
            SetAdditionalSettingToViewModel(ref viewModel, true);
            return View(viewModel);
        }

        public ActionResult Edit(string siteUrl = null, int ID = 0)
        {
            if (ID > 0)
            {
                _service.SetSiteUrl(siteUrl);
                SessionManager.Set(SESSION_SITE_URL, siteUrl);

                ViewBag.ListName = WORKFLOW_TITLE;

                var viewModel = _service.GetRequisitionNote(ID);
                SetAdditionalSettingToViewModel(ref viewModel, false);

                return View(viewModel);
            }
            else
            {
                return JsonHelper.GenerateJsonErrorResponse(DATA_NOT_EXISTS);
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
                return JsonHelper.GenerateJsonErrorResponse(e);
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
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

         
            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.Compensatory);
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
                return JsonHelper.GenerateJsonErrorResponse(e);
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
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.Compensatory);
        }
        [HttpPost]
        public JsonResult GetRequisitionNoteDetailsByEventBudgetId([DataSourceRequest] DataSourceRequest request, int? eventBudgetId)
        {
            var siteUrl = SessionManager.Get<string>(SESSION_SITE_URL);
            _eventBudgetService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            var details = new List<RequisitionNoteItemVM>();
            
            if (eventBudgetId.HasValue && eventBudgetId.Value > 0)
            {
                var eventbudget =_eventBudgetService.Get(eventBudgetId.Value);
                if(eventbudget != null)
                {
                    foreach (var item in eventbudget.ItemDetails)
                    {
                        var itemRNDetail = new RequisitionNoteItemVM();

                        itemRNDetail.ID = -1;
                        itemRNDetail.Activity = new Model.ViewModel.Control.AjaxComboBoxVM() { Value = Convert.ToInt32(eventbudget.Activity.Value), Text = eventbudget.Activity.Text };
                        itemRNDetail.WBS = new Model.ViewModel.Control.AjaxComboBoxVM() { Value = null, Text = "" };
                        itemRNDetail.GL = new Model.ViewModel.Control.AjaxComboBoxVM() { Value = null, Text = "" };
                        itemRNDetail.Specification = item.Title;
                        itemRNDetail.Quantity = item.Quantity;
                        itemRNDetail.Price = item.UnitPrice.HasValue ? item.UnitPrice.Value : 0;
                        itemRNDetail.EditMode = (int)Model.Common.Item.Mode.CREATED;
                        itemRNDetail.IsFromEventBudget = true;
                        itemRNDetail.Frequency = item.Frequency;

                        details.Add(itemRNDetail);
                    }
                }
            }
                       
            // Convert to Kendo DataSource
            DataSourceResult result = details.ToDataSourceResult(request);
            // Convert to Json
            var json = Json(result, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }

        //public JsonResult GetWBSMaster()
        //{
        //    // TODO: kok jelek ya

        //    return this.GetWBSMaster(SessionManager.Get<string>(SESSION_SITE_URL));
        //}

        public JsonResult GetGLMaster()
        {
            var siteUrl = SessionManager.Get<string>(SESSION_SITE_URL);
           
            var glMasters = FinService.Shared.GetGLMaster(siteUrl);

            return Json(glMasters.Select(e => new
            {
                Value = e.ID.HasValue ? Convert.ToString(e.ID) : string.Empty,
                Text = string.IsNullOrWhiteSpace(e.Title) ? string.Empty : (e.Title + "-" + e.GLDescription)
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWBSMaster()
        {
            _service.SetSiteUrl(SessionManager.Get<string>(SESSION_SITE_URL));

            var wbsMasters = _service.GetWBSMaster();

            return Json(wbsMasters.Select(e => new
            {
                Value = e.ID.HasValue ? Convert.ToString(e.ID) : string.Empty,
                Text = (e.Title + "-" + e.WBSDescription)
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetActivity(string project=null)
        {
            _service.SetSiteUrl(SessionManager.Get<string>(SESSION_SITE_URL));
      
            var activities = _service.GetActivity(project);

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
             
            var siteUrl = SessionManager.Get<string>(SESSION_SITE_URL);
            _service.SetSiteUrl(siteUrl);
            viewModel = _service.GetRequisitionNote(viewModel.ID);

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
                    return JsonHelper.GenerateJsonErrorResponse(e);
                }
            }
            if (pdfBuf == null)
                return HttpNotFound();
            return File(pdfBuf, "application/pdf");
        }

        private void SetAdditionalSettingToViewModel(ref RequisitionNoteVM viewModel, bool create)
        {
            viewModel.Category.Choices = new string[] { CATEGORY_EVENT, CATEGORY_NON_EVENT };
            viewModel.Category.OnSelectEventName = "onSelectCategory";

            viewModel.EventBudgetNo.ControllerName = "FINEventBudget";
            viewModel.EventBudgetNo.ActionName = "GetEventBudgetList";
            viewModel.EventBudgetNo.ValueField = "ID";
            viewModel.EventBudgetNo.TextField = "Title";
            viewModel.EventBudgetNo.OnSelectEventName = "onSelectEventBudgetNo";

            if (create)
            {
                viewModel.Category.Value = CATEGORY_EVENT;
            }

        }
    }
}