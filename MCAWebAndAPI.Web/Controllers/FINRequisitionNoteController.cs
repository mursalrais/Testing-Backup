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
using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Converter;
using MCAWebAndAPI.Service.Finance;
using MCAWebAndAPI.Service.Finance.RequisitionNote;
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
        public const string Action_UpdateFromEBChanged = "ufebc";

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

        private const string SuccessMsgFormatUpdated = "Requisition Note number {0} has been successfully updated.";
        private const string FirstPageUrl = "{0}/Lists/Requisition%20Note/AllItems.aspx";

        readonly IRequisitionNoteService reqNoteService;
        readonly IEventBudgetService eventBudgetService;
        
        public FINRequisitionNoteController()
        {
            reqNoteService = new RequisitionNoteService();
            eventBudgetService = new EventBudgetService();
        }

        public ActionResult Create(string siteUrl = null)
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
            reqNoteService.SetSiteUrl(siteUrl);
            SessionManager.Set(SharedFinanceController.Session_SiteUrl, siteUrl);

            ViewBag.ListName = WORKFLOW_TITLE;
            
            var viewModel = reqNoteService.Get(null);
            SetAdditionalSettingToViewModel(ref viewModel, true);
            return View(viewModel);
        }

        public ActionResult Edit(string siteUrl = null, int id = 0, string user = null)
        {
            if (id > 0)
            {
                siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
                reqNoteService.SetSiteUrl(siteUrl);
                SessionManager.Set(SharedFinanceController.Session_SiteUrl, siteUrl);

                ViewBag.ListName = WORKFLOW_TITLE;

                var viewModel = reqNoteService.Get(id);

                #region Check User

                var siteUrlHR = siteUrl.Replace("/bo", "/hr");
                var position = FinService.SharedService.GetPosition(user, siteUrlHR);


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
            reqNoteService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            eventBudgetService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            //set additional info for event budget no and project
            if (viewModel.EventBudgetNo.Value.HasValue && viewModel.EventBudgetNo.Value > 0)
            {
                var eventBdgt = eventBudgetService.Get(viewModel.EventBudgetNo.Value);
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
                headerID = reqNoteService.CreateRequisitionNote(viewModel);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            Task CreateDetailsTask = reqNoteService.CreateRequisitionNoteItemsAsync(headerID, viewModel.ItemDetails);
            Task CreateDocumentsTask = reqNoteService.CreateRequisitionNoteAttachmentsSync(headerID, viewModel.Documents);

            Task allTasks = Task.WhenAll(CreateDetailsTask, CreateDocumentsTask);

            try
            {
                await allTasks;
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error", new { errorMessage = e.Message });
            }

            return RedirectToAction("Index", "Success",
                new
                {
                    successMessage = string.Format(SuccessMsgFormatUpdated, viewModel.Title),
                    previousUrl = string.Format(FirstPageUrl, siteUrl)
                });
        }

        [HttpPost]
        public async Task<ActionResult> EditRequisitionNote(FormCollection form, RequisitionNoteVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>(SharedFinanceController.Session_SiteUrl);
            reqNoteService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            eventBudgetService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            //set additional info for event budget no and project
            if (viewModel.EventBudgetNo.Value.HasValue && viewModel.EventBudgetNo.Value > 0)
            {
                var eventBdgt = eventBudgetService.Get(viewModel.EventBudgetNo.Value);
                if (eventBdgt != null)
                {
                    viewModel.EventBudgetNo.Value = eventBdgt.ID;
                    viewModel.EventBudgetNo.Text = eventBdgt.No;
                    viewModel.Project.Value = eventBdgt.Project.Value;
                }
            } 

            try
            {
                reqNoteService.UpdateRequisitionNote(viewModel);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            Task CreateDetailsTask = reqNoteService.EditRequisitionNoteItemsAsync(viewModel.ID, viewModel.ItemDetails);
            Task CreateDocumentsTask = reqNoteService.EditRequisitionNoteAttachmentsSync(viewModel.ID, viewModel.Documents);

            Task allTasks = Task.WhenAll(CreateDetailsTask, CreateDocumentsTask);

            try
            {
                await allTasks;
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error", new { errorMessage = e.Message });
            }

            return RedirectToAction("Index", "Success",
                new
                {
                    successMessage = string.Format(SuccessMsgFormatUpdated, viewModel.Title),
                    previousUrl = string.Format(FirstPageUrl, siteUrl)
                });
        }


        [HttpPost]
        public JsonResult GetRequisitionNoteDetailsByEventBudgetId([DataSourceRequest] DataSourceRequest request, int? eventBudgetId)
        {
            var siteUrl = SessionManager.Get<string>(SharedFinanceController.Session_SiteUrl);
            eventBudgetService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            var details = new List<RequisitionNoteItemVM>();
            
            if (eventBudgetId.HasValue && eventBudgetId.Value > 0)
            {
                var eventbudget =eventBudgetService.Get(eventBudgetId.Value);
                if(eventbudget != null)
                {
                    foreach (var item in eventbudget.ItemDetails)
                    {
                        var itemRNDetail = new RequisitionNoteItemVM();

                        itemRNDetail.ID = null;
                        itemRNDetail.Activity = new AjaxComboBoxVM() { Value = Convert.ToInt32(eventbudget.Activity.Value), Text = eventbudget.Activity.Text };
                        itemRNDetail.WBS = new AjaxComboBoxVM() { Value = item.WBS.Value, Text = item.WBS.Text };
                        itemRNDetail.GL = new AjaxComboBoxVM() { Value = item.GL.Value, Text = item.GL.Text };
                        itemRNDetail.Specification = item.Description;
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
            reqNoteService.SetSiteUrl(SessionManager.Get<string>(SharedFinanceController.Session_SiteUrl));

            var wbsMasters = reqNoteService.GetWBSMaster(activity);

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
            string domain = "http://" + Request.Url.Authority + "/img/logo.png";

            var siteUrl = SessionManager.Get<string>(SharedFinanceController.Session_SiteUrl);
            reqNoteService.SetSiteUrl(siteUrl);
            viewModel = reqNoteService.Get(viewModel.ID);

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
                content = content.Replace("{XIMGPATHX}", domain);
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

        //public ActionResult UpdateFromEBChanged(string siteUrl = null, int id = 0, string user = null, string a = null)
        //{
        //    siteUrl = siteUrl ?? SessionManager.Get<string>(SharedFinanceController.Session_SiteUrl);
        //    siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;

        //    if (id == 0 || a != Action_UpdateFromEBChanged)
        //    {
        //        throw new Exception("Invalid parameters.");
        //    }

        //    reqNoteService.SetSiteUrl(siteUrl);
        //    eventBudgetService.SetSiteUrl(siteUrl);

        //    RequisitionNoteVM rnHeader = reqNoteService.Get(id);
        //    EventBudgetVM  ebHeader = eventBudgetService.Get(rnHeader.EventBudgetNo.Value);

        //    if (ebHeader.TransactionStatus.Value == TransactionStatusComboBoxVM.Locked)
        //        throw new Exception("Cannot update Requisition Note for a Locked Event Budget");

        //    // copy EB updated values to RN
        //    rnHeader.Project.Value = ebHeader.Project.Value;
        //    rnHeader.Total = ebHeader.TotalIDR;

        //    // delete all existing details in RN
        //    foreach (var rnDetail in rnHeader.ItemDetails)
        //    {
        //        reqNoteService.DeleteDetail((int)rnDetail.ID);
        //    }

        //    //copy all new details from EB
        //    List<RequisitionNoteItemVM> d = new List<RequisitionNoteItemVM>();
        //    foreach (var ebDetail in ebHeader.ItemDetails)
        //    {
        //        d.Add(new RequisitionNoteItemVM()
        //        {
        //            Activity = new AjaxComboBoxVM() { Value = Convert.ToInt32(ebHeader.Activity.Value), Text = ebHeader.Activity.Text },
        //            WBS = new AjaxComboBoxVM() { Value = ebDetail.WBS.Value, Text = ebDetail.WBS.Text },
        //            GL = new AjaxComboBoxVM() { Value = ebDetail.GL.Value, Text = ebDetail.GL.Text },
        //            Specification = ebDetail.Title,
        //            Quantity = ebDetail.Quantity,
        //            Frequency = ebDetail.Frequency,
        //            Price = ebDetail.UnitPrice,
        //            EditMode = (int)Item.Mode.CREATED,
        //            IsFromEventBudget = true,
        //            Total = ebDetail.Frequency * ebDetail.UnitPrice * ebDetail.Quantity
        //        });
        //    }

        //    reqNoteService.CreateRequisitionNoteItems(rnHeader.ID, d);

        //    // attachment?

        //    return RedirectToAction("Edit", "FINRequisitionNote",
        //        new { siteUrl = siteUrl, id = id, user = user });
        //}

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