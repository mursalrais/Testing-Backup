using System;
using System.Collections.Generic;
using System.Globalization;
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
        private ISCAVoucherService service;
        private IEventBudgetService eventBudgetService;

        private const string SiteUrl = "SiteUrl";
        private const string SCAVoucherIDSess = "SESS_SCAVoucherID";
        private const string EventBudgetIDSess = "SESS_EventBudgetID";
        private const string SubTitle = "Special Cash Advance Voucher";
        private const string PrintPageURL = "~/Views/FINSCAVoucher/Print.cshtml";
        private const string SuccessMsgFormatCreated = "SCA Voucher number {0} has been successfully created.";
        private const string SuccessMsgFormatUpdated = "SCA Voucher number {0} has been successfully updated.";
        private const string FirstPageUrl = "{0}/Lists/SCA%20Voucher/AllItems.aspx";
        private const string FirstPageFinanceUrl = "{0}/SitePages/FinSCAVoucher.aspx";

        private const string FooterFinance = "This form was revised and printed by {0}, {1:MM/dd/yyyy}, {2:HH:mm}";
        private const string FooterUser = "This form was printed by {0}, {1:MM/dd/yyyy}, {2:HH:mm}";

        public ActionResult Create(string siteUrl = null, string userEmail = "")
        {
            if (userEmail == string.Empty)
            {
                throw new InvalidOperationException("Invalid parameter: userEmail.");
            }

            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
            SessionManager.Set(SharedController.Session_SiteUrl, siteUrl);

            service = new SCAVoucherService(siteUrl);

            SCAVoucherVM model = new SCAVoucherVM();
            model.UserEmail = userEmail;

            SetAdditionalSettingToVM(ref model);

            ProfessionalMaster user = COMProfessionalController.GetFirstOrDefaultByOfficeEmail(siteUrl, model.UserEmail);
            var cancelUrl = user == null ? FirstPageUrl : (COMProfessionalController.IsPositionFinance(user.Position) ? FirstPageFinanceUrl : FirstPageUrl);

            ViewBag.CancelUrl = string.Format(cancelUrl, siteUrl);

            return View(model);
        }

        public ActionResult Edit(string siteUrl = null, int? ID = null, string userEmail = "")
        {
            if (userEmail == string.Empty)
            {
                throw new InvalidOperationException("Invalid parameter: userEmail.");
            }

            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
            SessionManager.Set(SharedController.Session_SiteUrl, siteUrl);

            service = new SCAVoucherService(siteUrl);
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

            ProfessionalMaster user = COMProfessionalController.GetFirstOrDefaultByOfficeEmail(siteUrl, model.UserEmail);
            var cancelUrl = user == null ? FirstPageUrl : (COMProfessionalController.IsPositionFinance(user.Position) ? FirstPageFinanceUrl : FirstPageUrl);

            ViewBag.CancelUrl = string.Format(cancelUrl, siteUrl);

            SetAdditionalSettingToVM(ref model);
            return View(model);
        }

        public ActionResult Approve(string siteUrl = null, int? ID = null, string userEmail = "")
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
            SessionManager.Set(SiteUrl, siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            service = new SCAVoucherService(siteUrl);

            SCAVoucherVM model = new SCAVoucherVM();
            if (ID != null)
            {
                model = service.Get(ID);
                model.Action = SCAVoucherVM.ActionType.approve.ToString();
                model.UserEmail = userEmail;
                SessionManager.Set(SCAVoucherIDSess, ID);
            }

            ProfessionalMaster user = COMProfessionalController.GetFirstOrDefaultByOfficeEmail(siteUrl, model.UserEmail);
            var cancelUrl = user == null ? FirstPageUrl : (COMProfessionalController.IsPositionFinance(user.Position) ? FirstPageFinanceUrl : FirstPageUrl);

            ViewBag.CancelUrl = string.Format(cancelUrl, siteUrl);

            return View(model);
        }

        public ActionResult Display(string siteUrl = null, int? ID = null, string userEmail = "")
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
            SessionManager.Set(SharedController.Session_SiteUrl, siteUrl);

            service = new SCAVoucherService(siteUrl);
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

            var siteUrl = ConfigResource.DefaultBOSiteUrl;

            eventBudgetService = new EventBudgetService(siteUrl);
            service = new SCAVoucherService(siteUrl);

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
            SessionManager.Set(SharedController.Session_SiteUrl, siteUrl);

            service = new SCAVoucherService(siteUrl);

            var viewModel = new SCAVoucherVM();
            viewModel = service.Get(model.ID);

            ViewData.Model = viewModel;
            var view = ViewEngines.Engines.FindView(ControllerContext, PrintPageURL, null);
            var fileName = viewModel.SCAVoucherNo + "_Application.pdf";
            byte[] pdfBuf = null;
            string content;

            ProfessionalMaster user = COMProfessionalController.GetFirstOrDefaultByOfficeEmail(siteUrl, viewModel.UserEmail);
            var userName = user == null ? viewModel.UserEmail : user.Name;

            var clientTime = Request.Form[nameof(viewModel.ClientDateTime)];
            DateTime dt = !string.IsNullOrWhiteSpace(clientTime) ? (DateTime.ParseExact(clientTime.ToString().Substring(0, 24), "ddd MMM d yyyy HH:mm:ss", CultureInfo.InvariantCulture)) : DateTime.Now;

            var footerMask = user == null ? FooterUser : (COMProfessionalController.IsPositionFinance(user.Position) ? FooterFinance : FooterUser);
            var footer = string.Format(footerMask, userName, dt, dt);

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
            SessionManager.Set(SharedController.Session_SiteUrl, siteUrl);

            service = new SCAVoucherService(siteUrl);

            int? ID = null;
            ID = service.CreateSCAVoucher(ref viewModel, COMProfessionalController.GetAll());

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

            ProfessionalMaster user = COMProfessionalController.GetFirstOrDefaultByOfficeEmail(siteUrl, viewModel.UserEmail);
            var previousUrl = user == null ? FirstPageUrl : (COMProfessionalController.IsPositionFinance(user.Position) ? FirstPageFinanceUrl : FirstPageUrl);

            return RedirectToAction("Index", "Success",
                new
                {
                    successMessage = string.Format(SuccessMsgFormatCreated, viewModel.SCAVoucherNo),
                    previousUrl = string.Format(previousUrl, siteUrl)
                });
        }

        [HttpPost]
        public async Task<ActionResult> Edit(FormCollection form, SCAVoucherVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>(SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;
            SessionManager.Set(SharedController.Session_SiteUrl, siteUrl);

            service = new SCAVoucherService(siteUrl);

            try
            {
                if (viewModel.Action == SCAVoucherVM.ActionType.approve.ToString())
                {
                    service.UpdateStatusSCAVoucher(viewModel);
                }
                else
                {
                    if (service.UpdateSCAVoucher(viewModel, COMProfessionalController.GetAll()))
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

            ProfessionalMaster user = COMProfessionalController.GetFirstOrDefaultByOfficeEmail(siteUrl, viewModel.UserEmail);
            var previousUrl = user == null ? FirstPageUrl : (COMProfessionalController.IsPositionFinance(user.Position) ? FirstPageFinanceUrl : FirstPageUrl);

            return RedirectToAction("Index", "Success",
                new
                {
                    successMessage = string.Format(SuccessMsgFormatUpdated, viewModel.SCAVoucherNo),
                    previousUrl = string.Format(previousUrl, siteUrl)
                });
        }

        public JsonResult GetEventBudgetItem([DataSourceRequest] DataSourceRequest request, int? eventBudgetId)
        {
            var siteUrl = SessionManager.Get<string>(SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;
            SessionManager.Set(SharedController.Session_SiteUrl, siteUrl);

            service = new SCAVoucherService(siteUrl);

            List<SCAVoucherItemsVM> details = new List<SCAVoucherItemsVM>();
            details = service.GetEventBudgetItems(eventBudgetId.Value).ToList();

            DataSourceResult result = details.ToDataSourceResult(request);
            var json = Json(result, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;

            return json;
        }

        public JsonResult GetDisplayEventBudgetItem([DataSourceRequest] DataSourceRequest request, int? eventBudgetId)
        {
            var siteUrl = SessionManager.Get<string>(SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;
            var sess_scaVoucherID = SessionManager.Get<int>(SCAVoucherIDSess);
            var sess_eventBudgetId = SessionManager.Get<int>(EventBudgetIDSess);

            service = new SCAVoucherService(siteUrl);

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
            var siteUrl = SessionManager.Get<string>(SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;
            SessionManager.Set(SharedController.Session_SiteUrl, siteUrl);

            service = new SCAVoucherService(siteUrl);

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
            viewModel.SDO.ControllerName = "COMProfessional";
            viewModel.SDO.ActionName = "GetForCombo";
            viewModel.SDO.ValueField = "ID";
            viewModel.SDO.TextField = "NameAndPos";
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