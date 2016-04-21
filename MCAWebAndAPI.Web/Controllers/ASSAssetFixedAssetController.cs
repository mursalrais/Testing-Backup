using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Service.Asset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    public class ASSAssetFixedAssetController : Controller
    {
        IAssetFixedAssetService assetFixedAssetService;

        public ASSAssetFixedAssetController()
        {
            assetFixedAssetService = new AssetFixedAssetService();
        }

        // GET: ASSAssetFixedAsset
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            var viewModel = new AssetFixedAssetVM();

            return View(viewModel);
        }
    }
}