using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Service.Asset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    public class ASSAssetCheckFormController : Controller
    {
        IAssetCheckFormService assetCheckFormService;

        public ASSAssetCheckFormController()
        {
            assetCheckFormService = new AssetCheckFormService();
        }

        // GET: ASSAssetCheckForm
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            var viewModel = new AssetCheckFormVM();

            return View(viewModel);
        }
    }
}