using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Finance;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using System.Linq;

namespace MCAWebAndAPI.Web.Controllers
{
    /// <summary>
    /// FIN15: Petty Cash Statement
    /// </summary>

    public class FINPettyCashStatementController : Controller
    {
        IPettyCashStatementService service;
        private const string SITE_URL = SharedController.Session_SiteUrl;

        public FINPettyCashStatementController()
        {
            service = new PettyCashStatementService();
        }

        public ActionResult Index(string siteUrl = null)
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);
            SessionManager.Set(SITE_URL, siteUrl);

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

        public ActionResult Display(string siteUrl = null)
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);
            SessionManager.Set(SITE_URL, siteUrl);

            var vm = GetDefaultPettyCashStatementVM();

            return Display(vm);
        }

        [HttpPost]
        public ActionResult Display(PettyCashStatementVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>(SITE_URL) ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);
            SessionManager.Set(SharedController.Session_SiteUrl, siteUrl);

            var dateTo = viewModel.DateTo;
            var dateFrom = viewModel.DateFrom;

            IEnumerable<PettyCashTransactionItem> dataSource = service.GetPettyCashStatements(dateFrom, dateTo);

            return View(dataSource);
        }
    }
}