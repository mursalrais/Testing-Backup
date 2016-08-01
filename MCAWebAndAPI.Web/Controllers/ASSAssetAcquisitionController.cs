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
            var viewModel = _assetAcquisitionService.GetAssetAcquisitionItems_Dummy();

            return View(viewModel);
        }

        public ActionResult Edit()
        {
            var viewModel = new AssetAcquisitionVM();

            return View(viewModel);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditingPopup_Create([DataSourceRequest] DataSourceRequest request, AssetAcquisitionItemVM _AssetAcqItemVM)
        {
            if (_AssetAcqItemVM != null && ModelState.IsValid)
            {
                _assetAcquisitionService.CreateAssetAcquisition_Dummy(_AssetAcqItemVM);
            }

            return Json(new[] { _AssetAcqItemVM }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditingPopup_Update()
        {
            var viewModel = new AssetAcquisitionVM();

            return View(viewModel);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditingPopup_Destroy()
        {
            var viewModel = new AssetAcquisitionVM();

            return View(viewModel);
        }
    }
}