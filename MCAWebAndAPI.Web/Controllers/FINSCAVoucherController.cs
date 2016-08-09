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
        private const string PRINT_PAGE_URL = "~/Views/FINSCAVoucher/DisplaySCAVoucher.cshtml";

        public FINSCAVoucherController()
        {
            _scaService = new SCAVoucherService();
            _eventBudgetService = new EventBudgetService();
        }

        public ActionResult CreateSCAVoucher(string siteUrl = null)
        {
            _scaService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Remove("EventBudgetDetail");

            SCAVoucherVM model = new SCAVoucherVM();

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> CreateSCAVoucher(FormCollection form, SCAVoucherVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl") ?? ConfigResource.DefaultBOSiteUrl;
            _scaService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            int? ID = null;
            ID = _scaService.CreateSCAVoucher(viewModel);
            viewModel.SCAVoucherItems = SessionManager.Get<List<SCAVoucherItemsVM>>("EventBudgetDetail");

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

        public ActionResult DisplaySCAVoucher(int? ID = null)
        {
            SCAVoucherVM model = new SCAVoucherVM();
            var siteUrl = SessionManager.Get<string>("SiteUrl") ?? ConfigResource.DefaultBOSiteUrl;
            _scaService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            model = _scaService.GetSCAVoucherVMData(ID);

            ViewBag.SubTitle = "Special Cash Advance Voucher";
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

            SessionManager.Set("EventBudgetDetail", detail);
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

        public JsonResult GetEventBudgetItem([DataSourceRequest] DataSourceRequest request, int? eventBudgetId)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _scaService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            List<SCAVoucherItemsVM> details = new List<SCAVoucherItemsVM>();
            var sessEventBudgetDetail = SessionManager.Get<List<SCAVoucherItemsVM>>("EventBudgetDetail");

            if (sessEventBudgetDetail != null)
            {
                details = sessEventBudgetDetail;
            }
            else if (eventBudgetId.HasValue && eventBudgetId.Value > 0)
            {
                details = _scaService.GetEventBudgetItems(eventBudgetId.Value).ToList();
            }

            DataSourceResult result = details.ToDataSourceResult(request);
            var json = Json(result, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;

            return json;
        }

        public JsonResult GetDisplayEventBudgetItem([DataSourceRequest] DataSourceRequest request, int? scaVoucherID)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _scaService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            List<SCAVoucherItemsVM> details = new List<SCAVoucherItemsVM>();

            if (scaVoucherID.HasValue && scaVoucherID.Value > 0)
            {
                details = _scaService.GetSCAVoucherItems(scaVoucherID.Value).ToList();
            }

            DataSourceResult result = details.ToDataSourceResult(request);
            var json = Json(result, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;

            return json;
        }
        
        public ActionResult PrintSCAVoucher(int? ID)
        {
            ViewBag.SubTitle = "Special Cash Advance Voucher";
            string RelativePath = PRINT_PAGE_URL;

            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _scaService.SetSiteUrl(siteUrl);

            var viewModel = new SCAVoucherVM();
            viewModel = _scaService.GetSCAVoucherVMData(ID);
            viewModel.SCAVoucherItems = _scaService.GetSCAVoucherItems(Convert.ToInt32(ID)).ToList();

            ViewData.Model = viewModel;
            var view = ViewEngines.Engines.FindView(ControllerContext, RelativePath, null);
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
    }
}