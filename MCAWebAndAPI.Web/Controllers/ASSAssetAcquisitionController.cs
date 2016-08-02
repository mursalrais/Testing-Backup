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

        public ActionResult Create()
        {
            var viewModel = _assetAcquisitionService.GetPopulatedModel();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Submit(AssetAcquisitionHeaderVM _data, string site)
        {
            //return View(new AssetMasterVM());
            _assetAcquisitionService.CreateHeader(_data);
            return new JavaScriptResult
            {
                Script = string.Format("window.parent.location.href = '{0}'", "https://eceos2.sharepoint.com/sites/mca-dev/bo/Lists/AssetAcquisition/AllItems.aspx")
            };
        }

        public JsonResult GetAssetSubSAssetGrid()
        {
            _assetAcquisitionService.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);

            var positions = GetFromPositionsExistingSession();

            return Json(positions.Select(e =>
                new {
                    Value = Convert.ToString(e.ID),
                    Text = e.AssetNoAssetDesc + " - " + e.AssetDesc
                }),
                JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<AssetMasterVM> GetFromPositionsExistingSession()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["AssetSubAsset"] as IEnumerable<AssetMasterVM>;
            var positions = sessionVariable ?? _assetAcquisitionService.GetAssetSubAsset();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["AssetSubAsset"] = positions;
            return positions;
        }
    }
}