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
        private const string EventBudgetDetailSess = "EventBudgetDetail";
        private const string SCAVoucherIDSess = "SESS_SCAVoucherID";
        private const string EventBudgetIDSess = "SESS_EventBudgetID";
        private const string SubTitle = "Special Cash Advance Voucher";
        private const string PrintPageURL = "~/Views/FINSCAVoucher/Print.cshtml";

        public FINSCAVoucherController()
        {
            service = new SCAVoucherService();
            _eventBudgetService = new EventBudgetService();
        }

        public ActionResult Create(string siteUrl = null, string userAccess = null)
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);
            SessionManager.Set(SharedFinanceController.Session_SiteUrl, siteUrl);

            SessionManager.Remove(EventBudgetDetailSess);

            SCAVoucherVM model = new SCAVoucherVM();
            SetAdditionalSettingToVM(ref model);

            return View(model);
        }

        public ActionResult Edit(string siteUrl = null, int? ID = null, string userAccess = null)
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);
            SessionManager.Set(SharedFinanceController.Session_SiteUrl, siteUrl);

            SCAVoucherVM model = new SCAVoucherVM();
            if (ID != null)
            {
                model = service.Get(ID);
                model.Action = SCAVoucherVM.ActionType.edit.ToString();
                SessionManager.Set(SCAVoucherIDSess, ID);
                SessionManager.Set(EventBudgetIDSess, model.EventBudgetID);
            }

            SetAdditionalSettingToVM(ref model);
            return View(model);
        }

        public ActionResult Approve(string siteUrl = null, int? ID = null, string userAccess = null)
        {
            service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set(SiteUrl, siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            SCAVoucherVM model = new SCAVoucherVM();
            if (ID != null)
            {
                model = service.Get(ID);
                model.Action = SCAVoucherVM.ActionType.approve.ToString();
                SessionManager.Set(SCAVoucherIDSess, ID);
            }

            return View(model);
        }

        public ActionResult Display(string siteUrl = null, int? ID = null, string userAccess = null)
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);
            SessionManager.Set(SharedFinanceController.Session_SiteUrl, siteUrl);

            SCAVoucherVM model = new SCAVoucherVM();

            model = service.Get(ID);

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

            totalInWord = FormatUtil.ConvertToEnglishWords(Convert.ToInt32(total));
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

        public ActionResult Print(int? ID, string userAccess = null)
        {
            ViewBag.SubTitle = SubTitle;

            var siteUrl = SessionManager.Get<string>(SiteUrl);
            service.SetSiteUrl(siteUrl);
            SessionManager.Set(SharedFinanceController.Session_SiteUrl, siteUrl);

            var viewModel = new SCAVoucherVM();
            viewModel = service.Get(ID);
            viewModel.SCAVoucherItems = service.GetSCAVoucherItems(Convert.ToInt32(ID)).ToList();

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
        public async Task<ActionResult> Create(FormCollection form, SCAVoucherVM viewModel, string userAccess = null)
        {
            var siteUrl = SessionManager.Get<string>(SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);
            SessionManager.Set(SharedFinanceController.Session_SiteUrl, siteUrl);

            int? ID = null;
            ID = service.CreateSCAVoucher(viewModel);
            viewModel.SCAVoucherItems = SessionManager.Get<List<SCAVoucherItemsVM>>(EventBudgetDetailSess);

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

            return RedirectToAction("Print",
                "FINSCAVoucher",
                new
                {
                    ID = ID
                });
        }

        [HttpPost]
        public async Task<ActionResult> Edit(FormCollection form, SCAVoucherVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>(SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);
            SessionManager.Set(SharedFinanceController.Session_SiteUrl, siteUrl);

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
                        viewModel.SCAVoucherItems = SessionManager.Get<List<SCAVoucherItemsVM>>(EventBudgetDetailSess);

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

            return RedirectToAction("Print",
                "FINSCAVoucher",
                new
                {
                    ID = viewModel.ID
                });
        }

        public JsonResult GetEventBudgetItem([DataSourceRequest] DataSourceRequest request, int? eventBudgetId)
        {
            var siteUrl = SessionManager.Get<string>(SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);
            SessionManager.Set(SharedFinanceController.Session_SiteUrl, siteUrl);

            List<SCAVoucherItemsVM> details = new List<SCAVoucherItemsVM>();
            details = service.GetEventBudgetItems(eventBudgetId.Value).ToList();
            SessionManager.Set(EventBudgetDetailSess, details);
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
            SessionManager.Set(EventBudgetDetailSess, details);
            DataSourceResult result = details.ToDataSourceResult(request);
            var json = Json(result, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;

            return json;
        }

        private void SetAdditionalSettingToVM(ref SCAVoucherVM viewModel)
        {
            viewModel.SDO.ControllerName = "ComboBox";
            viewModel.SDO.ActionName = "GetProfessionals";
            viewModel.SDO.ValueField = "ID";
            viewModel.SDO.TextField = "Desc1";
            viewModel.SDO.OnSelectEventName = "OnSelectProfessional";

            viewModel.EventBudget.ControllerName = "ComboBox";
            viewModel.EventBudget.ActionName = "GetEventBudgets";
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