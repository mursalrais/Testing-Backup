using MCAWebAndAPI.Service.Asset;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    public class ASSReportFixedAssetController : Controller
    {
        IReportFixedAssetService _service;

        public ASSReportFixedAssetController()
        {
            _service = new ReportFixedAssetService();
        }

        public ActionResult Show(string SiteUrl)
        {
            _service.SetSiteUrl(SiteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", SiteUrl ?? ConfigResource.DefaultBOSiteUrl);
            _service.GetReport(SiteUrl);
            return View();
        }
    }
}