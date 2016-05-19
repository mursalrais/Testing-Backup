using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Service.Asset;
using MCAWebAndAPI.Web.Helpers;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    public class ASSAssetAcquisitionController : Controller
    {
        IAssetAcquisitionService _assetAcquisitionService;

        public ASSAssetAcquisitionController()
        {
            _assetAcquisitionService = new AssetAcquisitionService();
        }

        // GET: ASSAssetAcquisition
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            var viewModel = _assetAcquisitionService.GetAssetAcquisition_Dummy();

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Submit(AssetAcquisitionVM _data)
        {
            //return View(new AssetAcquisitionVM());
            var data = _data;

            return this.Jsonp(data);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditingPopup_Create([DataSourceRequest] DataSourceRequest request, AssetAcquisitionItemVM _assetAcquisitionVM)
        {
            if (_assetAcquisitionVM != null && ModelState.IsValid)
            {
                _assetAcquisitionService.CreateAssetAcquisitionItem_dummy(_assetAcquisitionVM);
                //productService.Create(_assetAcquisitionItem);
            }

            return Json(new[] { _assetAcquisitionVM }.ToDataSourceResult(request, ModelState));
        }
    }
}