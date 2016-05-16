using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Service.Asset;
using MCAWebAndAPI.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    public class ASSAssetMasterController : Controller
    {
        IAssetMasterService _assetMasterService;

        public ASSAssetMasterController()
        {
            _assetMasterService = new AssetMasterService();
        }

        // GET: ASSAssetMaster
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create(string site)
        {
            var viewModel = _assetMasterService.GetAssetMaster();
            return View(viewModel);
        }

        public ActionResult Edit(int ID, string site)
        {
            var viewModel = _assetMasterService.GetAssetMaster(ID);
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Submit(AssetMasterVM _data, string site)
        {
            //return View(new AssetMasterVM());
            _assetMasterService.CreateAssetMaster(_data);
            return new JavaScriptResult
            {
                Script = string.Format("window.parent.location.href = '{0}'", "https://eceos2.sharepoint.com/sites/mca-dev/dev/Lists/AssetMaster/AllItems.aspx")
            };
        }

        public ActionResult Update(AssetMasterVM _data, string site)
        {
            //return View(new AssetMasterVM());
            _assetMasterService.UpdateAssetMaster(_data);
            return new JavaScriptResult
            {
                Script = string.Format("window.parent.location.href = '{0}'", "https://eceos2.sharepoint.com/sites/mca-dev/dev/Lists/AssetMaster/AllItems.aspx")
            };
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditingPopup_Create([DataSourceRequest] DataSourceRequest request, AssetMasterVM _assetMasterVM)
        {
            if (_assetMasterVM != null && ModelState.IsValid)
            {
                //_assetMasterService.CreateAssetMasterItem_dummy(_assetMasterVM);
                //productService.Create(_assetMasterItem);
            }

            return Json(new[] { _assetMasterVM }.ToDataSourceResult(request, ModelState));
        }
    }
}