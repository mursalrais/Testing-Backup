using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Service.Asset;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using System;
using System.Net;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace MCAWebAndAPI.Web.Controllers
{
    public class ASSAssetAcquisitionController : Controller
    {
        IAssetAcquisitionService _assetAcquisitionService;

        public ASSAssetAcquisitionController()
        {
            _assetAcquisitionService = new AssetAcquisitionService();
        }

       public ActionResult CreateHeader(string siteUrl = null)
        {
            //a must
            //_assetAcquisitionService.SetSiteUrl(siteUrl);
            //SessionManager.Set<string>("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            //get blank viewmodel
            var viewmodel = _assetAcquisitionService.getPopulatedModel();
            return View("Create", viewmodel);
        }
    }
}