using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Service.Asset;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    public class ASSAssetTransactionController : Controller
    {
        IAssetTransactionService _assetTransactionService;

        public ASSAssetTransactionController()
        {
            _assetTransactionService = new AssetTransactionService();
        }

        // GET: ASSAssetTransaction
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateAssetTransfer(string siteUrl = null)
        {
            _assetTransactionService.SetSiteUrl(siteUrl);
            var viewModel = _assetTransactionService.GetPopulatedModel();
            viewModel.Header.TransactionType = "Asset Transfer";

            return View("Create", viewModel);
        }

        [HttpPost]
        public ActionResult Create(AssetTransactionVM viewModel)
        {
            if(!ModelState.IsValid)
            {
                ModelState.AddModelError("500", "Internal Server Error");
                return View("Create", viewModel);
            }

            var headerID = _assetTransactionService.CreateHeader(viewModel.Header);
            _assetTransactionService.CreateItems(headerID, viewModel.Items);

            return Json(new { Status = "Success", UrlToRedirect = "google.com"}, JsonRequestBehavior.AllowGet);
        }
    }
}