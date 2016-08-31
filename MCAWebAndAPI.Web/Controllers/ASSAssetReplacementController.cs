using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Service.Asset;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using MCAWebAndAPI.Web.Resources;
using MCAWebAndAPI.Web.Helpers;

namespace MCAWebAndAPI.Web.Controllers
{
    public class ASSAssetReplacementController : Controller
    {
        IAssetReplacementService _service;

        public ASSAssetReplacementController()
        {
            _service = new AssetReplacementService();
        }
        // GET: ASSAssetReplacement
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create(string siteUrl)
        {
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            var viewModel = _service.GetPopulatedModel();

            return View(viewModel);
        }

        public ActionResult Edit()
        {
            var viewModel = new AssetReplacementHeaderVM();

            return View(viewModel);
        }

        public ActionResult GetInfoFromAcquisitin(int ID)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            int IDacquisition = ID;
            var acquisition = _service.GetInfoFromAcquisitin(IDacquisition, siteUrl);
            var details = _service.GetInfoFromAcquisitinDetail(IDacquisition, siteUrl);

            //var professionals = GetFromExistingSession();
            return Json(
                new
                {
                    acquisition.ID,
                    acquisition.Vendor,
                    acquisition.VendorID,
                    acquisition.PoNo,
                    acquisition.purchasedatetext,
                    acquisition.PurchaseDescription

                }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetInfoFromAcquisitinDetail(int ID)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            int IDacquisition = ID;
            var details = _service.GetInfoFromAcquisitinDetail(IDacquisition, siteUrl);
            return Json(details.Select(e =>
                new
                {
                    e.AssetSubAsset,
                    e.WBS,
                    e.CostIDR,
                    e.CostUSD,
                    e.Remarks
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAssetSubSAssetGrid()
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            var assetsubasset = GetFromPositionsExistingSession();

            return Json(assetsubasset.Select(e =>
                new
                {
                    Value = Convert.ToString(e.ID),
                    Text = e.AssetNoAssetDesc.Value + " - " + e.AssetDesc
                }),
                JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<AssetMasterVM> GetFromPositionsExistingSession()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["AssetMaster"] as IEnumerable<AssetMasterVM>;
            var positions = sessionVariable ?? _service.GetAssetSubAsset();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["AssetMaster"] = positions;
            return positions;
        }
    }
}