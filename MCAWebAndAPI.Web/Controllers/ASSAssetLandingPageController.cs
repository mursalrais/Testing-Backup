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
    public class ASSAssetLandingPageController : Controller
    {
        AssetLandingPageService assetLandingPageService;

        public ASSAssetLandingPageController()
        {
            assetLandingPageService = new AssetLandingPageService();
        }

        // GET: ASSAssetLandingPage
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create(string siteUrl, int? ID)
        {
            assetLandingPageService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            var viewModelDetail = assetLandingPageService.GetPopulatedModel();

            return View(viewModelDetail);
        }
    }
}