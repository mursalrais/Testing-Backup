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
    public class ASSAssetCheckResultController : Controller
    {
        IAssetCheckResultService assetCheckResultService;

        public ASSAssetCheckResultController()
        {
            assetCheckResultService = new AssetCheckResultService();
        }

        // GET: AssetCheckResult
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            var viewModel = assetCheckResultService.GetAssetCheckResultItems_Dummy();
            return View(viewModel);
        }

        public ActionResult Search()
        {
            var viewModel = new AssetCheckResultVM();
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditingPopup_Create([DataSourceRequest] DataSourceRequest request, AssetCheckResultItemVM _AssetCheckResultItemVM)
        {
            if (_AssetCheckResultItemVM != null && ModelState.IsValid)
            {
                assetCheckResultService.CreateAssetCheckResult_Dummy(_AssetCheckResultItemVM);
            }

            return Json(new[] { _AssetCheckResultItemVM }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditingPopup_Update()
        {
            var viewModel = new AssetCheckResultVM();

            return View(viewModel);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditingPopup_Destroy()
        {
            var viewModel = new AssetCheckResultVM();

            return View(viewModel);
        }
    }
}