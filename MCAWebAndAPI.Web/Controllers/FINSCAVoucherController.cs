using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Elmah;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Converter;
using MCAWebAndAPI.Service.Finance;
using MCAWebAndAPI.Service.Utils;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;

namespace MCAWebAndAPI.Web.Controllers.Finance
{
    /// <summary>
    /// Wireframe FIN06: SCA Voucher
    ///     i.e.: Special Cash Advance Voucher
    /// </summary>

    public class FINSCAVoucherController : Controller
    {
        ISCAVoucherService service;
        IEventBudgetService _eventBudgetService;

        private const string SiteUrl = "SiteUrl";
        private const string SCAVoucherIDSess = "SESS_SCAVoucherID";
        private const string EventBudgetIDSess = "SESS_EventBudgetID";
        private const string SubTitle = "Special Cash Advance Voucher";
        private const string PrintPageURL = "~/Views/FINSCAVoucher/Print.cshtml";
        private const string SuccessMsgFormatCreated = "SCA Voucher number {0} has been successfully created.";
        private const string SuccessMsgFormatUpdated = "SCA Voucher number {0} has been successfully updated.";
        private const string FirstPageUrl = "{0}/Lists/SCA%20Voucher/AllItems.aspx";

        public FINSCAVoucherController()
        {
            service = new SCAVoucherService();
            _eventBudgetService = new EventBudgetService();
        }

        public ActionResult Create(string siteUrl = null, string userEmail = "")
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);
            SessionManager.Set(SharedController.Session_SiteUrl, siteUrl);

            SCAVoucherVM model = new SCAVoucherVM();
            SetAdditionalSettingToVM(ref model);
            ViewBag.CancelUrl = string.Format(FirstPageUrl, siteUrl);

            return View(model);
        }

        public ActionResult Edit(string siteUrl = null, int? ID = null, string userEmail = "")
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);
            SessionManager.Set(SharedController.Session_SiteUrl, siteUrl);

            SCAVoucherVM model = new SCAVoucherVM();

            if (ID == null)
            {
                model.UserEmail = userEmail;
            }
            else
            {
                model = service.Get(ID);
                model.Action = SCAVoucherVM.ActionType.edit.ToString();
                SessionManager.Set(SCAVoucherIDSess, ID);
                SessionManager.Set(EventBudgetIDSess, model.EventBudgetID);
            }

            ViewBag.CancelUrl = string.Format(FirstPageUrl, siteUrl);

            SetAdditionalSettingToVM(ref model);
            return View(model);
        }

        public ActionResult Approve(string siteUrl = null, int? ID = null, string userEmail = "")
        {
            service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set(SiteUrl, siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            SCAVoucherVM model = new SCAVoucherVM();
            if (ID != null)
            {
                model = service.Get(ID);
                model.Action = SCAVoucherVM.ActionType.approve.ToString();
                model.UserEmail = userEmail;
                SessionManager.Set(SCAVoucherIDSess, ID);
            }

            ViewBag.CancelUrl = string.Format(FirstPageUrl, siteUrl);

            return View(model);
        }

        public ActionResult Display(string siteUrl = null, int? ID = null, string userEmail = "")
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);
            SessionManager.Set(SharedController.Session_SiteUrl, siteUrl);

            SCAVoucherVM model = new SCAVoucherVM();
            ProfessionalMaster professional = COMProfessionalController.GetFirstOrDefaultByOfficeEmail(userEmail); 

            model = service.Get(ID);
            
            if (model.UserEmail != userEmail || !COMProfessionalController.IsPositionFinance(professional.Position))
            {
                throw new InvalidOperationException("You have no right to see this data.");
            }

            ViewBag.SubTitle = SubTitle;
            return View(model);
        }

        public ActionResult GetEventBudget(string ID)
        {
            List<SCAVoucherVM> r = new List<SCAVoucherVM>();
            List<SCAVoucherItemsVM> d = new List<SCAVoucherItemsVM>();
            decimal? total = 0;
            string totalInWord = string.Empty;

            _eventBudgetService.SetSiteUrl(ConfigResource.DefaultBOSiteUrl);
            service.SetSiteUrl(ConfigResource.DefaultBOSiteUrl);

            var result = service.GetEventBudget(Convert.ToInt32(ID));
            var detail = service.GetEventBudgetItems(Convert.ToInt32(ID)).ToList();

            foreach (var item in detail)
            {
                total += item.Amount;
            }

            totalInWord = FormatUtil.ConvertToEnglishWords(Convert.ToInt32(total), result.Currency);
            r.Add(result);

            return Json(r.Select(m =>
                new
                {
                    TotalAmount = total,
                    TotalAmountInWord = totalInWord,
                    Purpose = m.Purpose,
                    Project = m.Project,
                    ActivityID = m.ActivityID,
                    ActivityName = m.ActivityName,
                    Fund = 3000,
                    Currency = m.Currency
                }),
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult Print(FormCollection form, RequisitionNoteVM model)
        {
            ViewBag.SubTitle = SubTitle;
            string domain = new SharedFinanceController().GetImageLogoPrint(Request.IsSecureConnection, Request.Url.Authority);

            var siteUrl = SessionManager.Get<string>(SiteUrl);
            service.SetSiteUrl(siteUrl);
            SessionManager.Set(SharedController.Session_SiteUrl, siteUrl);

            var viewModel = new SCAVoucherVM();
            viewModel = service.Get(model.ID);
            viewModel.SCAVoucherItems = service.GetSCAVoucherItems(Convert.ToInt32(model.ID)).ToList();

            ViewData.Model = viewModel;
            var view = ViewEngines.Engines.FindView(ControllerContext, PrintPageURL, null);
            var fileName = viewModel.SCAVoucherNo + "_Application.pdf";
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
                    RedirectToAction("Index", "Error");
                }
            }
            if (pdfBuf == null)
                return HttpNotFound();
            return File(pdfBuf, "application/pdf");
        }

        [HttpPost]
        public async Task<ActionResult> Create(FormCollection form, SCAVoucherVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>(SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);
            SessionManager.Set(SharedController.Session_SiteUrl, siteUrl);

            int? ID = null;
            ID = service.CreateSCAVoucher(ref viewModel);

            Task createSCAVoucherItemTask = service.CreateSCAVoucherItemAsync(ID, viewModel.SCAVoucherItems);
            Task createSCAVoucherDocumentTask = service.CreateSCAVoucherAttachmentAsync(ID, viewModel.Documents);
            Task allTasks = Task.WhenAll(createSCAVoucherItemTask);

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
                    successMessage = string.Format(SuccessMsgFormatCreated, viewModel.SCAVoucherNo),
                    previousUrl = string.Format(FirstPageUrl, siteUrl)
                });
        }

        [HttpPost]
        public async Task<ActionResult> Edit(FormCollection form, SCAVoucherVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>(SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);
            SessionManager.Set(SharedController.Session_SiteUrl, siteUrl);

            try
            {
                if (viewModel.Action == SCAVoucherVM.ActionType.approve.ToString())
                {
                    service.UpdateStatusSCAVoucher(viewModel);
                }
                else
                {
                    if (service.UpdateSCAVoucher(viewModel))
                    {
                        Task createSCAVoucherItemTask = service.UpdateSCAVoucherItem(viewModel.ID, viewModel.SCAVoucherItems);
                        Task createSCAVoucherDocumentTask = service.CreateSCAVoucherAttachmentAsync(viewModel.ID, viewModel.Documents);
                        Task allTasks = Task.WhenAll(createSCAVoucherItemTask);

                        await allTasks;
                    }
                }
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error", new { errorMessage = e.Message });
            }

            return RedirectToAction("Index", "Success",
                new
                {
                    successMessage = string.Format(SuccessMsgFormatUpdated, viewModel.SCAVoucherNo),
                    previousUrl = string.Format(FirstPageUrl, siteUrl)
                });
        }

        public JsonResult GetEventBudgetItem([DataSourceRequest] DataSourceRequest request, int? eventBudgetId)
        {
            var siteUrl = SessionManager.Get<string>(SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);
            SessionManager.Set(SharedController.Session_SiteUrl, siteUrl);

            List<SCAVoucherItemsVM> details = new List<SCAVoucherItemsVM>();
            details = service.GetEventBudgetItems(eventBudgetId.Value).ToList();

            DataSourceResult result = details.ToDataSourceResult(request);
            var json = Json(result, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;

            return json;
        }

        public JsonResult GetDisplayEventBudgetItem([DataSourceRequest] DataSourceRequest request, int? eventBudgetId)
        {
            var siteUrl = SessionManager.Get<string>(SiteUrl);
            var sess_scaVoucherID = SessionManager.Get<int>(SCAVoucherIDSess);
            var sess_eventBudgetId = SessionManager.Get<int>(EventBudgetIDSess);

            service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            List<SCAVoucherItemsVM> details = new List<SCAVoucherItemsVM>();
            if (eventBudgetId > 0 && eventBudgetId != sess_eventBudgetId)
            {
                details = service.GetEventBudgetItems(Convert.ToInt32(eventBudgetId)).ToList();
            }
            else
            {
                details = service.GetSCAVoucherItems(sess_scaVoucherID).ToList();
            }

            DataSourceResult result = details.ToDataSourceResult(request);
            var json = Json(result, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;

            return json;
        }

        public JsonResult GetAll()
        {
            service.SetSiteUrl(ConfigResource.DefaultBOSiteUrl);

            var result = service.GetAllAjaxComboBoxVM().ToList();
            result.Insert(0, new AjaxComboBoxVM() { Value = 0, Text = "" });

            return Json(result.Select(e =>
                new
                {
                    Value = e.Value,
                    Text = e.Text
                }),
                JsonRequestBehavior.AllowGet);
        }

        private void SetAdditionalSettingToVM(ref SCAVoucherVM viewModel)
        {
            viewModel.SDO.ControllerName = "ComboBox";
            viewModel.SDO.ActionName = "GetProfessionals";
            viewModel.SDO.ValueField = "ID";
            viewModel.SDO.TextField = "Desc1";
            viewModel.SDO.OnSelectEventName = "OnSelectProfessional";

            viewModel.EventBudget.ControllerName = "ComboBox";
            viewModel.EventBudget.ActionName = "GetEventBudgetsSCA";        // only display EB which has SCA amount > 0
            viewModel.EventBudget.ValueField = "Value";
            viewModel.EventBudget.TextField = "Text";
            viewModel.EventBudget.OnSelectEventName = "OnSelectEventBudgetNo";

            viewModel.SubActivity.ActionName = "GetSubActivitiesByEventBudgetID";
            viewModel.SubActivity.ControllerName = "ComboBox";
            viewModel.SubActivity.ValueField = "Value";
            viewModel.SubActivity.TextField = "Text";
            viewModel.SubActivity.Cascade = "EventBudget_Value";
            viewModel.SubActivity.Filter = "filterEventBudgetNo";
        }
    }
}