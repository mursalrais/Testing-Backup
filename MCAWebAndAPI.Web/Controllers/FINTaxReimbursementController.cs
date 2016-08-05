using MCAWebAndAPI.Service.Finance;
using MCAWebAndAPI.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    public class FINTaxReimbursementController : Controller
    {
        readonly ITaxReimbursementService _service;

        public FINTaxReimbursementController()
        {
            _service = new TaxReimbursementService();
        }

        public ActionResult CreateTaxReimbursement(string siteUrl = null, int? ID = null)
        {
            // MANDATORY: Set Site URL
            _service.SetSiteUrl(siteUrl);
            SessionManager.Set("SiteUrl", siteUrl);

            var viewModel = _service.GetTaxReimbursement(null);

            return View(viewModel);

        }

        // GET: FINTaxReimbursement
        public ActionResult Index()
        {
            return View();
        }
    }
}