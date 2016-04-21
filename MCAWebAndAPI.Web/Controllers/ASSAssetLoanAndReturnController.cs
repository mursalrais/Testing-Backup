using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Service.Asset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    public class ASSAssetLoanAndReturnController : Controller
    {
        IAssetLoanAndReturnService assetLoanAndReturnService;

        public ASSAssetLoanAndReturnController()
        {
            assetLoanAndReturnService = new AssetLoanAndReturnService();
        }

        // GET: ASSAssetLoanAndReturn
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            var viewModel = new AssetLoanAndReturnVM();

            return View(viewModel);
        }
    }
}