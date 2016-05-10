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

        public ActionResult Create()
        {
            var viewModel = _assetMasterService.GetAssetMaster_Dummy();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Submit(AssetMasterVM _data)
        {
            //return View(new AssetMasterVM());
            var data = _assetMasterService.GetAssetMaster_Dummy();

            return this.Jsonp(data);
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