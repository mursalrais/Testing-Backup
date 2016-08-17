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
    public class FINSCAVoucherController : Controller
    {
        ISCAVoucherService _scaService;
        IEventBudgetService _eventBudgetService;
        private const string _siteUrl = "SiteUrl";
        private const string _eventBudgetDetailSess = "EventBudgetDetail";
        private const string _scaVoucherIDSess = "SESS_SCAVoucherID";
        private const string _eventBudgetIDSess = "SESS_EventBudgetID";
        private const string _subTitle = "Special Cash Advance Voucher";
        private const string _printPageURL = "~/Views/FINSCAVoucher/Print.cshtml";

        public FINSCAVoucherController()
        {
            _scaService = new SCAVoucherService();
            _eventBudgetService = new EventBudgetService();
        }

        public ActionResult Create(string siteUrl = null, string userAccess = null)
        {
            _scaService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set(_siteUrl, siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Remove(_eventBudgetDetailSess);

            SCAVoucherVM model = new SCAVoucherVM();

            return View(model);
        }

        public ActionResult Edit(string siteUrl = null, int? ID = null, string userAccess = null)
        {
            _scaService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set(_siteUrl, siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            SCAVoucherVM model = new SCAVoucherVM();
            if (ID != null)
            {
                model = _scaService.GetSCAVoucherVMData(ID);
                model.SDOName = string.Format("{0} - {1}", model.SDOName, model.Position);
                model.Action = SCAVoucherVM.ActionType.edit.ToString();
                model.Currency = "IDR";
                SessionManager.Set(_scaVoucherIDSess, ID);
                SessionManager.Set(_eventBudgetIDSess, model.EventBudgetID);
            }
            
            return View(model);
        }

        public ActionResult Approve(string siteUrl = null, int? ID = null, string userAccess = null)
        {
            _scaService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set(_siteUrl, siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            SCAVoucherVM model = new SCAVoucherVM();
            if (ID != null)
            {
                model = _scaService.GetSCAVoucherVMData(ID);
                model.SDOName = string.Format("{0} - {1}", model.SDOName, model.Position);
                model.Action = SCAVoucherVM.ActionType.approve.ToString();
                model.Currency = "IDR";
                SessionManager.Set(_scaVoucherIDSess, ID);
            }

            return View(model);
        }

        public ActionResult Display(int? ID = null, string userAccess = null)
        {
            SCAVoucherVM model = new SCAVoucherVM();
            var siteUrl = SessionManager.Get<string>(_siteUrl) ?? ConfigResource.DefaultBOSiteUrl;
            _scaService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            model = _scaService.GetSCAVoucherVMData(ID);

            ViewBag.SubTitle = _subTitle;
            return View(model);
        }

        public ActionResult GetEventBudget(string ID)
        {
            List<SCAVoucherVM> r = new List<SCAVoucherVM>();
            List<SCAVoucherItemsVM> d = new List<SCAVoucherItemsVM>();
            decimal? total = 0;
            string totalInWord = string.Empty;

            _eventBudgetService.SetSiteUrl(ConfigResource.DefaultBOSiteUrl);
            _scaService.SetSiteUrl(ConfigResource.DefaultBOSiteUrl);

            var result = _scaService.GetEventBudget(Convert.ToInt32(ID));
            var detail = _scaService.GetEventBudgetItems(Convert.ToInt32(ID)).ToList();

            foreach (var item in detail)
            {
                total += item.Amount;
            }

            totalInWord = FormatUtil.ConvertToEnglishWords(Convert.ToInt32(total));
            r.Add(result);

            return Json(r.Select(m =>
                new {
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
            ViewBag.SubTitle = _subTitle;

            var siteUrl = SessionManager.Get<string>(_siteUrl);
            _scaService.SetSiteUrl(siteUrl);

            var viewModel = new SCAVoucherVM();
            viewModel = _scaService.GetSCAVoucherVMData(ID);
            viewModel.SCAVoucherItems = _scaService.GetSCAVoucherItems(Convert.ToInt32(ID)).ToList();

            ViewData.Model = viewModel;
            var view = ViewEngines.Engines.FindView(ControllerContext, _printPageURL, null);
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
        public async Task<ActionResult> CreateSCAVoucher(FormCollection form, SCAVoucherVM viewModel, string userAccess = null)
        {
            var siteUrl = SessionManager.Get<string>(_siteUrl) ?? ConfigResource.DefaultBOSiteUrl;
            _scaService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            int? ID = null;
            ID = _scaService.CreateSCAVoucher(viewModel);
            viewModel.SCAVoucherItems = SessionManager.Get<List<SCAVoucherItemsVM>>(_eventBudgetDetailSess);

            Task createSCAVoucherItemTask = _scaService.CreateSCAVoucherItem(ID, viewModel.SCAVoucherItems);
            Task createSCAVoucherDocumentTask = _scaService.CreateSCAVoucherDocumentAsync(ID, viewModel.Documents);
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

            return RedirectToAction("PrintSCAVoucher",
                "FINSCAVoucher",
                new
                {
                    ID = ID
                });
        }

        [HttpPost]
        public async Task<ActionResult> EditSCAVoucher(FormCollection form, SCAVoucherVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>(_siteUrl) ?? ConfigResource.DefaultBOSiteUrl;
            _scaService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            try
            {
                if (viewModel.Action == SCAVoucherVM.ActionType.approve.ToString())
                {
                    _scaService.UpdateStatusSCAVoucher(viewModel);
                }
                else
                {
                    if (_scaService.UpdateSCAVoucher(viewModel))
                    {
                        viewModel.SCAVoucherItems = SessionManager.Get<List<SCAVoucherItemsVM>>(_eventBudgetDetailSess);

                        Task createSCAVoucherItemTask = _scaService.UpdateSCAVoucherItem(viewModel.ID, viewModel.SCAVoucherItems);
                        Task createSCAVoucherDocumentTask = _scaService.CreateSCAVoucherDocumentAsync(viewModel.ID, viewModel.Documents);
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

            return RedirectToAction("PrintSCAVoucher",
                "FINSCAVoucher",
                new
                {
                    ID = viewModel.ID
                });
        }

        public JsonResult GetEventBudgetItem([DataSourceRequest] DataSourceRequest request, int? eventBudgetId)
        {
            var siteUrl = SessionManager.Get<string>(_siteUrl);
            _scaService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            List<SCAVoucherItemsVM> details = new List<SCAVoucherItemsVM>();
            details = _scaService.GetEventBudgetItems(eventBudgetId.Value).ToList();
            SessionManager.Set(_eventBudgetDetailSess, details);
            DataSourceResult result = details.ToDataSourceResult(request);
            var json = Json(result, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;

            return json;
        }

        public JsonResult GetDisplayEventBudgetItem([DataSourceRequest] DataSourceRequest request, int? eventBudgetId)
        {
            var siteUrl = SessionManager.Get<string>(_siteUrl);
            var sess_scaVoucherID = SessionManager.Get<int>(_scaVoucherIDSess);
            var sess_eventBudgetId = SessionManager.Get<int>(_eventBudgetIDSess);

            _scaService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            List<SCAVoucherItemsVM> details = new List<SCAVoucherItemsVM>();
            if (eventBudgetId > 0 && eventBudgetId != sess_eventBudgetId)
            {
                details = _scaService.GetEventBudgetItems(Convert.ToInt32(eventBudgetId)).ToList();
            }
            else
            {
                details = _scaService.GetSCAVoucherItems(sess_scaVoucherID).ToList();
            }
            SessionManager.Set(_eventBudgetDetailSess, details);
            DataSourceResult result = details.ToDataSourceResult(request);
            var json = Json(result, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;

            return json;
        }

    }
}