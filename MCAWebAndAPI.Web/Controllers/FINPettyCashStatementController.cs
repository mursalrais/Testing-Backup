using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Finance;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;

namespace MCAWebAndAPI.Web.Controllers
{
    /// <summary>
    /// FIN15: Petty Cash Statement
    /// </summary>

    public class FINPettyCashStatementController : Controller
    {
        IPettyCashStatementService service;

        public FINPettyCashStatementController()
        {
            service = new PettyCashStatementService();
        }

        public ActionResult Index(string siteUrl = null)
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);
            SessionManager.Set("SiteUrl", siteUrl);

            return View();
        }

        public ActionResult Display(string siteUrl = null)
        {
            siteUrl = siteUrl ?? ConfigResource.DefaultBOSiteUrl;
            service.SetSiteUrl(siteUrl);
            SessionManager.Set(SharedFinanceController.Session_SiteUrl, siteUrl);

            var dateTo = DateTime.Today;
            var dateFrom = dateTo.AddDays(-14);     //going back 2 weeks

            IEnumerable<PettyCashTransactionItem> dataSource = service.GetPettyCashStatements(dateFrom, dateTo);

            return View(dataSource);
        }
    }
}