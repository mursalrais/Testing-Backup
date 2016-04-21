using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Service.Asset;

namespace MCAWebAndAPI.Web.Controllers
{
    public class ASSAssetReplacementController : Controller
    {
        //IAssetReplacementService assetReplacementService;

        public ASSAssetReplacementController()
        {
            //assetReplacementService = new ass;
        }
        // GET: ASSAssetReplacement
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            var viewModel = new AssetReplacementVM();

            return View(viewModel);
        }

        public ActionResult Edit()
        {
            var viewModel = new AssetReplacementVM();

            return View(viewModel);
        }
    }
}