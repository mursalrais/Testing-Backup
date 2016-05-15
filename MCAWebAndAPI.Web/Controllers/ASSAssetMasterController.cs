using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
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
    public class ASSAssetMasterController : Controller
    {
        IAssetMasterService _assetMasterService;

        public ASSAssetMasterController()
        {
            _assetMasterService = new AssetMasterService();
        }

        public JsonResult GetAssetMasters()
        {
            _assetMasterService.SetSiteUrl(ConfigResource.DefaultBOSiteUrl);
            var result = _assetMasterService.GetAssetMasters();

            //TODO: To map object based on other requirements
            return Json(result.Select(e => (new
            {
                e.Id,
                e.AssetDesc
            })), JsonRequestBehavior.AllowGet);
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
            _assetMasterService.CreateAssetMaster_dummy(_data);

            return this.Jsonp(_data);
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