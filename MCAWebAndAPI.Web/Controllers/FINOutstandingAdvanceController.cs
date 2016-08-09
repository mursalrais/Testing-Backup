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
using MCAWebAndAPI.Service.Finance.RequisitionNote;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;

namespace MCAWebAndAPI.Web.Controllers
{
    [Filters.HandleError]
    public class FINOutstandingAdvanceController : Controller
    {
        private const string SessionSiteUrl = "SiteUrl";

        readonly IOutstandingAdvanceService service;

        public FINOutstandingAdvanceController()
        {
            service = new OutstandingAdvanceService();
        }

        public ActionResult Create(string siteUrl = null)
        {
            service.SetSiteUrl(siteUrl);
            SessionManager.Set(SessionSiteUrl, siteUrl);

            var viewModel = service.Get(null);
            return View(viewModel);
        }
    }
}