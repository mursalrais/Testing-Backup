using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Finance;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using System.IO;
using Elmah;
using MCAWebAndAPI.Service.Converter;

namespace MCAWebAndAPI.Web.Controllers
{
    /// <summary>
    /// FIN15: Petty Cash Statement
    /// </summary>

    public class FINPettyCashStatementController : Controller
    {
        IPettyCashStatementService service;

        private const string FirstPageUrl = "{0}/Lists/Petty%20Cash%20Statement/AllItems.aspx";
        private const string PrintPageUrl = "~/Views/FINPettyCashStatement/Print.cshtml";
        private const string FileName = "PC Statement";
        private const string Session_DateForm = "PCStmt_DtFrom";
        private const string Session_DateTo = "PCStmt_DtTo";

        public FINPettyCashStatementController()
        {
            service = new PettyCashStatementService();
        }

        public ActionResult Index(string siteUrl = null)
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);
            SessionManager.Set(SharedController.Session_SiteUrl, siteUrl);

            var vm = GetDefaultPettyCashStatementVM();

            return View(vm);
        }

        private PettyCashStatementVM GetDefaultPettyCashStatementVM()
        {
            PettyCashStatementVM vm = new PettyCashStatementVM();
            vm.DateTo = DateTime.Now;
            vm.DateFrom = vm.DateTo.AddDays(-14); //going back 2 weeks

            return vm;
        }


        [HttpPost]
        public ActionResult Display(PettyCashStatementVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>(SharedController.Session_SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);
            SessionManager.Set(SharedController.Session_SiteUrl, siteUrl);

            var dateFrom = viewModel.DateFrom;
            var dateTo = viewModel.DateTo;

            // ini katrok banget, bisa diganti dengan cara yang lebih elegant
            SessionManager.Set(Session_DateForm, dateFrom);
            SessionManager.Set(Session_DateTo, dateTo);

            IEnumerable<PettyCashTransactionItem> dataSource = service.GetPettyCashStatements(dateFrom, dateTo);

            ViewBag.ListUrl = string.Format(FirstPageUrl, siteUrl);
            ViewBag.DateFrom = dateFrom;
            ViewBag.DateTo = dateTo;

            return View(dataSource);
        }


        [HttpPost]
        public ActionResult Print(FormCollection form, PettyCashStatementVM viewModel)
        {
            string RelativePath = PrintPageUrl;

            var siteUrl = SessionManager.Get<string>(SharedController.Session_SiteUrl) ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);
            SessionManager.Set(SharedController.Session_SiteUrl, siteUrl);

            // ini katrok banget, bisa diganti dengan cara yang lebih elegant
            DateTime dateFrom = SessionManager.Get<DateTime>(Session_DateForm);
            DateTime dateTo = SessionManager.Get<DateTime>(Session_DateTo);

            IEnumerable<PettyCashTransactionItem> dataSource = service.GetPettyCashStatements(dateFrom, dateTo);

            ViewData.Model = dataSource;
            var view = ViewEngines.Engines.FindView(ControllerContext, RelativePath, null);

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
                    pdfBuf = PDFConverter.Instance.ConvertFromHTML(FileName, content);
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