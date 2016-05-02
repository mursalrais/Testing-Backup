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
    public class ASSAssetReplacementController : Controller
    {
        IAssetReplacementService assetReplacementService;

        public ASSAssetReplacementController()
        {
            assetReplacementService = new AssetReplacementService();
        }
        // GET: ASSAssetReplacement
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            var viewModel = assetReplacementService.GetAssetReplacementItems_Dummy();

            return View(viewModel);
        }

        public ActionResult Edit()
        {
            var viewModel = new AssetReplacementVM();

            return View(viewModel);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditingPopup_Create([DataSourceRequest] DataSourceRequest request, AssetReplacementItemVM _AssetReplacementItemVM)
        {
            if (_AssetReplacementItemVM != null && ModelState.IsValid)
            {
                assetReplacementService.CreateAssetReplacement_Dummy(_AssetReplacementItemVM);
            }

            return Json(new[] { _AssetReplacementItemVM }.ToDataSourceResult(request, ModelState));
        }
            
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditingPopup_Update()
        {
            var viewModel = new AssetReplacementVM();

            return View(viewModel);
        }

        [AcceptVerbs(HttpVerbs.Post)]   
        public ActionResult EditingPopup_Destroy()
        {
            var viewModel = new AssetReplacementVM();

            return View(viewModel);
        }
    }
}