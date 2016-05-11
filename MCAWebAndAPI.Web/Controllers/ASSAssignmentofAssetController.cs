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
    public class ASSAssignmentofAssetController : Controller
    {
        AssignmentofAssetService assignmentofAssetService;

        public ASSAssignmentofAssetController()
        {
            assignmentofAssetService = new AssignmentofAssetService();
        }

        // GET: ASSAssignmentofAsset
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            var viewModel = assignmentofAssetService.GetAssignmentofAssetItems_Dummy();

            return View(viewModel);
        }

        public ActionResult Edit()
        {
            var viewModel = new AssignmentofAssetVM();

            return View(viewModel);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditingPopup_Create([DataSourceRequest] DataSourceRequest request, AssignmentofAssetItemVM _AssignmentofAssetItemVM)
        {
            if (_AssignmentofAssetItemVM != null && ModelState.IsValid)
            {
                assignmentofAssetService.CreateAssignmentofAsset_Dummy(_AssignmentofAssetItemVM);
            }

            return Json(new[] { _AssignmentofAssetItemVM }.ToDataSourceResult(request, ModelState));
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