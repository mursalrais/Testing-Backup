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
using MCAWebAndAPI.Service.Common;
using MCAWebAndAPI.Model.HR.DataMaster;

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
        private const string FirstPageUrl = "{0}/Lists/Requisition%20Note/All%20Items%20FIN.aspx";

        private IRequisitionNoteService reqNoteService;
        private IEventBudgetService eventBudgetService;
        
        public ActionResult Create(string siteUrl = null, string userEmail = "")
        {
            if (userEmail == string.Empty)
            {
                throw new InvalidOperationException("Invalid parameter: userEmail.");
            }

            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
            SessionManager.Set(SharedController.Session_SiteUrl, siteUrl);

            ViewBag.ListName = WORKFLOW_TITLE;
            
            reqNoteService = new RequisitionNoteService(siteUrl);

            var viewModel = reqNoteService.Get(null);
            viewModel.UserEmail = userEmail;
            ViewBag.CancelUrl = string.Format(FirstPageUrl, siteUrl);
            SetAdditionalSettingToViewModel(ref viewModel, true);
            return View(viewModel);
        }

        public ActionResult Edit(string siteUrl = null, int id = 0, string userEmail = "")
        {
            if (userEmail == string.Empty)
            {
                throw new InvalidOperationException("Invalid parameter: userEmail.");
            }

            if (id > 0)
            {
                siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
                SessionManager.Set(SharedController.Session_SiteUrl, siteUrl);

                ViewBag.ListName = WORKFLOW_TITLE;

                reqNoteService = new RequisitionNoteService(siteUrl);
                var viewModel = reqNoteService.Get(id);

                #region Check User

                var siteUrlHR = CommonService.GetSiteUrlFromCurrent(siteUrl, CommonService.Sites.HR);

                ProfessionalMaster professional = COMProfessionalController.GetFirstOrDefaultByOfficeEmail(siteUrl, userEmail);

                #endregion Check User

                ViewBag.CancelUrl = string.Format(FirstPageUrl, siteUrl);
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
        public async Task<ActionResult> CreateRequisitionNote(string actionType, FormCollection form, RequisitionNoteVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>(SharedController.Session_SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;

            reqNoteService = new RequisitionNoteService(siteUrl);
            eventBudgetService = new EventBudgetService(siteUrl);

            if (actionType != "Save")
            {
                return Redirect(string.Format(FirstPageUrl, siteUrl));
            }

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
        public async Task<ActionResult> EditRequisitionNote(string actionType, FormCollection form, RequisitionNoteVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>(SharedController.Session_SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;

            reqNoteService = new RequisitionNoteService(siteUrl);
            eventBudgetService = new EventBudgetService(siteUrl);

            if (actionType != "Save")
            {
                return Redirect(string.Format(FirstPageUrl, siteUrl));
            }

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
            var siteUrl = SessionManager.Get<string>(SharedController.Session_SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;

            eventBudgetService = new EventBudgetService(siteUrl);

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
            var siteUrl = SessionManager.Get<string>(SharedController.Session_SiteUrl);
           
            var glMasters = FinService.SharedService.GetGLMaster(siteUrl);

            return Json(glMasters.Select(e => new
            {
                Value = e.ID.HasValue ? Convert.ToString(e.ID) : string.Empty,
                Text = string.IsNullOrWhiteSpace(e.Title) ? string.Empty : (e.Title + "-" + e.GLDescription)
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWBSMaster(string activity=null)
        {
            var siteUrl = SessionManager.Get<string>(SharedController.Session_SiteUrl);

            reqNoteService = new RequisitionNoteService(siteUrl);
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
            string domain = new SharedFinanceController().GetImageLogoPrint(Request.IsSecureConnection, Request.Url.Authority);

            var siteUrl = SessionManager.Get<string>(SharedController.Session_SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;

            reqNoteService = new RequisitionNoteService(siteUrl);
            viewModel = reqNoteService.Get(viewModel.ID);

            ViewData.Model = viewModel;
            var view = ViewEngines.Engines.FindView(ControllerContext, RelativePath, null);
            var fileName = viewModel.Title + "_Application.pdf";
            byte[] pdfBuf = null;
            string content;

            string footer = string.Empty;

            //TODO: Resolve user name
            string userName = "xxxx";

            if (viewModel.Modified > viewModel.Created)
            {
                DateTime dt = DateTime.Now;
                footer = string.Format("This form was printed by {0}, {1:MM/dd/yyyy}, {2:HH:mm}", userName, dt, dt);
            }

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
                    pdfBuf = PDFConverter.Instance.ConvertFromHTML(fileName, content, footer);
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