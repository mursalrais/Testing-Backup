using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Service.Asset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    public class ASSAssetSmallValueAssetController : Controller
    {
        IAssetSmallValueAssetService assetSmallValueAssetService;

        public ASSAssetSmallValueAssetController()
        {
            assetSmallValueAssetService = new AssetSmallValueAssetService();
        }

        // GET: ASSAssetSmallValueAsset
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            var viewModel = new AssetSmallValueAssetVM();

            return View(viewModel);
        }
    }
}