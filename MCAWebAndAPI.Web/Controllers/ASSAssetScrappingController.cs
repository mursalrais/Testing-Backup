using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Service.Asset;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

namespace MCAWebAndAPI.Web.Controllers
{
    public class ASSAssetScrappingController : Controller
    {
        IAssetScrappingService assetScrappingService;

        public ASSAssetScrappingController()
        {
            assetScrappingService = new AssetScrappingService();
        }

        // GET: ASSAssetScrapping
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            var viewModel = assetScrappingService.GetssetScrappingItems_Dummy();

            return View(viewModel);
        }

        public ActionResult Edit()
        {
            var viewModel = new AssetScrappingVM();

            return View(viewModel);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditingPopup_Create([DataSourceRequest] DataSourceRequest request, AssetScrappingItemVM _AssetScrappingItemVM)
        {
            if (_AssetScrappingItemVM != null && ModelState.IsValid)
            {
                assetScrappingService.CreateAssetScrapping_Dummy(_AssetScrappingItemVM);
            }

            return Json(new[] { _AssetScrappingItemVM }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditingPopup_Update()
        {
            var viewModel = new AssetScrappingVM();

            return View(viewModel);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditingPopup_Destroy()
        {
            var viewModel = new AssetScrappingVM();

            return View(viewModel);
        }
    }
}