using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Service.Asset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    public class ASSAssetTransferController : Controller
    {
        IAssetTransferService assetTransferService;

        public ASSAssetTransferController()
        {
            assetTransferService = new AssetTransferService();
        }

        // GET: ASSAssetTransfer
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            var viewModel = new AssetTransferVM();

            return View(viewModel);
        }
    }
}