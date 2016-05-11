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
    public class ASSAssetCheckResultApproveController : Controller
    {

        IAssetCheckResultApproveService assetCheckResultApproveService;

        public ASSAssetCheckResultApproveController()
        {
            assetCheckResultApproveService = new AssetCheckResultApproveService();
        }
        // GET: ASSAssetCheckResultApprove
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            var viewModel = assetCheckResultApproveService.GetAssetCheckResultApproveItems_Dummy();
            return View(viewModel);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditingPopup_Create([DataSourceRequest] DataSourceRequest request, AssetCheckResultApproveItemVM _AssetCheckResultApproveItemVM)
        {
            if (_AssetCheckResultApproveItemVM != null && ModelState.IsValid)
            {
                assetCheckResultApproveService.CreateAssetCheckResultApprove_Dummy(_AssetCheckResultApproveItemVM);
            }

            return Json(new[] { _AssetCheckResultApproveItemVM }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditingPopup_Update()
        {
            var viewModel = new AssetCheckResultApproveVM();

            return View(viewModel);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditingPopup_Destroy()
        {
            var viewModel = new AssetCheckResultApproveVM();

            return View(viewModel);
        }
    }
}