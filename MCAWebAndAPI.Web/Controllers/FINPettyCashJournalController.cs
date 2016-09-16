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
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using static MCAWebAndAPI.Model.ViewModel.Form.Finance.Shared;

namespace MCAWebAndAPI.Web.Controllers
{
    public class FINPettyCashJournalController : Controller
    {
        /// <summary>
        /// Wireframe FIN13: Petty Cash Journal
        /// </summary>
        /// 
        private const string SessionSiteUrl = "SiteUrl";
        private const string SuccessMsgFormatCreated = "Petty Cash Transactions from {0} to {1} has been successfully created.";
        private const string FirstPageUrl = "{0}/Lists/Petty%20Cash%20Journal1/AllItems.aspx";
        private const string PRINT_PAGE_URL = "~/Views/FINPettyCashJournal/Print.cshtml";

        readonly IPettyCashJournalService service;

        public FINPettyCashJournalController()
        {
            service = new PettyCashJournalService();
        }

        // GET: FINPettyCashJournal
        public ActionResult Item(string siteUrl = null, string op = null, int? id = null)
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);
            SessionManager.Set(SessionSiteUrl, siteUrl);

            var viewModel = service.Get(GetOperation(op), id);

            ViewBag.CancelUrl = string.Format(FirstPageUrl, siteUrl);

            return View(viewModel);
        }
        
        public JsonResult GetPettyCashTransaction([DataSourceRequest] DataSourceRequest request, string dateFrom, string dateTo, bool itemEdited)
        {
            var siteUrl = SessionManager.Get<string>(SharedController.Session_SiteUrl);
            service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            var details = new List<PettyCashJournalItemVM>();

            if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
            {
                //this mess is just to ensure date format yyyy-MM-dd
                //TODO: find a better way
                var from = Convert.ToDateTime(DateTime.Parse(dateFrom, System.Globalization.CultureInfo.InvariantCulture));
                var to = Convert.ToDateTime(DateTime.Parse(dateTo, System.Globalization.CultureInfo.InvariantCulture));

                if (itemEdited)
                    details = service.GetPettyCashTransactions(from, to).ToList();
            }

            DataSourceResult result = details.ToDataSourceResult(request);
           
            // Convert to Json
            var json = Json(result, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;

            return json;
        }

        [HttpPost]
        public async Task<ActionResult> Save(FormCollection form, PettyCashJournalVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>(SessionSiteUrl) ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);

            try
            {
                int? id = service.Create(viewModel);
                Task createPettyCashJournalItem = service.CreateItems(id, viewModel);
                Task allTasks = Task.WhenAll(createPettyCashJournalItem);

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
                    successMessage = string.Format(SuccessMsgFormatCreated, viewModel.DateFrom, viewModel.DateTo),
                    previousUrl = string.Format(FirstPageUrl, siteUrl)
                });
        }

        public ActionResult Print(FormCollection form, PettyCashJournalVM param)
        {
            string RelativePath = PRINT_PAGE_URL;
            string domain = new SharedFinanceController().GetImageLogoPrint(Request.IsSecureConnection, Request.Url.Authority);
            var siteUrl = SessionManager.Get<string>(SharedController.Session_SiteUrl);
            service.SetSiteUrl(siteUrl);
            
            var viewModel = service.Get(param.Operation, param.ID);

            ViewData.Model = viewModel;
            var view = ViewEngines.Engines.FindView(ControllerContext, RelativePath, null);
            var fileName = viewModel.Title + "_Application.pdf";
            byte[] pdfBuf = null;
            string content;

            string footer = string.Empty;

            //TODO: Resolve user name
            string userName = "xxxx";

            //if (viewModel.Modified > viewModel.Created)
            //{
            //    DateTime dt = DateTime.Now;
            //    footer = string.Format("This form was printed by {0}, {1:MM/dd/yyyy}, {2:HH:mm}", userName, dt, dt);
            //}

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
    }
}