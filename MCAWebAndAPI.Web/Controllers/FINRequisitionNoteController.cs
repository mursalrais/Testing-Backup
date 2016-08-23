using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Elmah;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Converter;
using MCAWebAndAPI.Service.Finance;
using MCAWebAndAPI.Service.Finance.RequisitionNote;
using MCAWebAndAPI.Service.Resources;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using FinService = MCAWebAndAPI.Service.Finance;

namespace MCAWebAndAPI.Web.Controllers.Finance
{
    /// <summary>
    /// Wireframe FIN05: Requisition Note
    ///     i.e.: Purchase Requisition Note
    /// </summary>

    [Filters.HandleError]
    public class FINRequisitionNoteController : Controller 
    {
        private const string FIELD_ID = "ID";
        private const string FIELD_TITLE = "Title";
        private const string LIST_NAME = "requisitionnote";
        private const string WORKFLOW_TITLE = "Requisition%20Note";
        private const string INDEX_PAGE = "Index";
        private const string ERROR = "Error";
        private const string DATA_NOT_EXISTS = "Data Does not exists!";
        private const string PRINT_PAGE_URL = "~/Views/FINRequisitionNote/Print.cshtml";
        private const string CATEGORY_EVENT = "Event";
        private const string CATEGORY_NON_EVENT = "Non-event";
        private const string ONSELECTED_CATEGORYEVENT = "onSelectCategory";
        private const string PICKER_EVENTBUDGET_CONTROLLER = "FINEventBudget";
        private const string PICKER_EVENTBUDGET_ACTIONNAME = "GetEventBudgetList";
        private const string PICKER_EVENTBUDGET_ONSELECTCHANGE = "onSelectEventBudgetNo";

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
            SessionManager.Set(SharedFinanceController.Session_SiteUrl, siteUrl);

            ViewBag.ListName = WORKFLOW_TITLE;
            
            var viewModel = _service.GetRequisitionNote(null);
            SetAdditionalSettingToViewModel(ref viewModel, true);
            return View(viewModel);
        }

        public ActionResult Edit(string siteUrl = null, int id = 0, string user = null)
        {
            if (id > 0)
            {
                siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
                _service.SetSiteUrl(siteUrl);
                SessionManager.Set(SharedFinanceController.Session_SiteUrl, siteUrl);

                ViewBag.ListName = WORKFLOW_TITLE;

                var viewModel = _service.GetRequisitionNote(id);

                #region Check User

                user = user ?? ConfigResource.DefaultUser;

                var siteUrlHR = siteUrl.Replace("/bo", "/hr");
                var position = Service.Finance.SharedService.GetPosition(user, siteUrlHR);


                if (viewModel.Editor != user)
                {

                }

                #endregion Check User


                SetAdditionalSettingToViewModel(ref viewModel, false);

                return View(viewModel);
            }
            else
            {
                ErrorSignal.FromCurrentContext().Raise(new Exception(DATA_NOT_EXISTS));
                return JsonHelper.GenerateJsonErrorResponse(DATA_NOT_EXISTS);
            }
        }


        [HttpPost]
        public async Task<ActionResult> CreateRequisitionNote(FormCollection form, RequisitionNoteVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>(SharedFinanceController.Session_SiteUrl);
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            _eventBudgetService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            //set additional info for event budget no and project
            if (viewModel.EventBudgetNo.Value.HasValue && viewModel.EventBudgetNo.Value > 0)
            {
                var eventBdgt = _eventBudgetService.Get(viewModel.EventBudgetNo.Value);
                if (eventBdgt != null)
                {
                    viewModel.EventBudgetNo.Value = eventBdgt.ID;
                    viewModel.EventBudgetNo.Text = eventBdgt.No;
                    viewModel.Project.Value = eventBdgt.Project.Value;
                }
            }

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

         
            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.RequisitionNote);
        }

        [HttpPost]
        public async Task<ActionResult> EditRequisitionNote(FormCollection form, RequisitionNoteVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>(SharedFinanceController.Session_SiteUrl);
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            _eventBudgetService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            //set additional info for event budget no and project
            if (viewModel.EventBudgetNo.Value.HasValue && viewModel.EventBudgetNo.Value > 0)
            {
                var eventBdgt = _eventBudgetService.Get(viewModel.EventBudgetNo.Value);
                if (eventBdgt != null)
                {
                    viewModel.EventBudgetNo.Value = eventBdgt.ID;
                    viewModel.EventBudgetNo.Text = eventBdgt.No;
                    viewModel.Project.Value = eventBdgt.Project.Value;
                }
            } 

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

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.RequisitionNote);
        }


        [HttpPost]
        public JsonResult GetRequisitionNoteDetailsByEventBudgetId([DataSourceRequest] DataSourceRequest request, int? eventBudgetId)
        {
            var siteUrl = SessionManager.Get<string>(SharedFinanceController.Session_SiteUrl);
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

                        itemRNDetail.ID = null;
                        itemRNDetail.Activity = new Model.ViewModel.Control.AjaxComboBoxVM() { Value = Convert.ToInt32(eventbudget.Activity.Value), Text = eventbudget.Activity.Text };
                        itemRNDetail.WBS = new Model.ViewModel.Control.AjaxComboBoxVM() { Value = item.WBS.Value, Text = item.WBS.Text };
                        itemRNDetail.GL = new Model.ViewModel.Control.AjaxComboBoxVM() { Value = item.GL.Value, Text = item.GL.Text };
                        itemRNDetail.Specification = item.Title;
                        itemRNDetail.Quantity = item.Quantity;
                        itemRNDetail.Price = item.UnitPrice;
                        itemRNDetail.EditMode = (int)Item.Mode.CREATED;
                        itemRNDetail.IsFromEventBudget = true;
                        itemRNDetail.Frequency = item.Frequency;
                        itemRNDetail.Total = item.Frequency * itemRNDetail.Price * itemRNDetail.Quantity;
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


        public JsonResult GetGLMaster()
        {
            var siteUrl = SessionManager.Get<string>(SharedFinanceController.Session_SiteUrl);
           
            var glMasters = FinService.SharedService.GetGLMaster(siteUrl);

            return Json(glMasters.Select(e => new
            {
                Value = e.ID.HasValue ? Convert.ToString(e.ID) : string.Empty,
                Text = string.IsNullOrWhiteSpace(e.Title) ? string.Empty : (e.Title + "-" + e.GLDescription)
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWBSMaster(string activity=null)
        {
            _service.SetSiteUrl(SessionManager.Get<string>(SharedFinanceController.Session_SiteUrl));

            var wbsMasters = _service.GetWBSMaster(activity);

            return Json(wbsMasters.Select(e => new
            {
                Value = e.ID.HasValue ? Convert.ToString(e.ID) : string.Empty,
                Text = (e.Title + "-" + e.WBSDescription)
            }), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PrintRequisitionNote(FormCollection form, RequisitionNoteVM viewModel)
        {
            string RelativePath = PRINT_PAGE_URL;
             
            var siteUrl = SessionManager.Get<string>(SharedFinanceController.Session_SiteUrl);
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
            viewModel.Category.OnSelectEventName = ONSELECTED_CATEGORYEVENT;

            viewModel.EventBudgetNo.ControllerName = PICKER_EVENTBUDGET_CONTROLLER;
            viewModel.EventBudgetNo.ActionName = PICKER_EVENTBUDGET_ACTIONNAME;
            viewModel.EventBudgetNo.ValueField = FIELD_ID;
            viewModel.EventBudgetNo.TextField = FIELD_TITLE;
            viewModel.EventBudgetNo.OnSelectEventName = PICKER_EVENTBUDGET_ONSELECTCHANGE;

            if (create)
            {
                viewModel.Category.Value = CATEGORY_EVENT;
            }
            
        }
    }
}