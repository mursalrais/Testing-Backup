using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Service.Asset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    public class ASSAssetAcquisitionController : Controller
    {
        IAssetAcquisitionService assetAcquisitionService;

        public ASSAssetAcquisitionController()
        {
            assetAcquisitionService = new AssetAcquisitionService();
        }

        // GET: ASSAssetAcquisition
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            var viewModel = new AssetAcquisitionVM();

            return View(viewModel);
        }
    }
}