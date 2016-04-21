using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Service.Asset;

namespace MCAWebAndAPI.Web.Controllers
{
    public class ASSAssignmentofAssetController : Controller
    {
        // GET: ASSAssignmentofAsset
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            var viewModel = new AssignmentofAssetVM();

            return View(viewModel);
        }

        public ActionResult Edit()
        {
            var viewModel = new AssignmentofAssetVM();

            return View(viewModel);
        }
    }
}