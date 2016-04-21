using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Service.Asset;

namespace MCAWebAndAPI.Web.Controllers
{
    public class ASSAssetCheckResultController : Controller
    {
        // GET: AssetCheckResult
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            var viewModel = new AssetCheckResultVM();
            return View();
        }

        public ActionResult Search()
        {
            var viewModel = new AssetCheckResultVM();
            return View();
        }
    }
}