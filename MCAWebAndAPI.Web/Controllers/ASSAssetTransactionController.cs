using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Service.Asset;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Web.Filters;
using MCAWebAndAPI.Web.Resources;

namespace MCAWebAndAPI.Web.Controllers
{
    public class ASSAssetTransactionController : Controller
    {
        IAssetTransactionService _assetTransactionService;

        public ASSAssetTransactionController()
        {
            _assetTransactionService = new AssetTransactionService();
        }

        /// <summary>
        /// HTTP Get to View Create with specific argument
        /// </summary>
        /// <param name="siteUrl"></param>
        /// <returns></returns>
        public ActionResult CreateAssetTransfer(string siteUrl = null)
        {
            // MANDATORY: Set Site URL
            _assetTransactionService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            // Get blank ViewModel
            var viewModel = _assetTransactionService.GetPopulatedModel();

            // Modify ViewModel based on spesific case, e.g., Asset Transfer is one of conditions in Asset transaction
            viewModel.Header.TransactionType = "Asset Transfer";
            
            // Return to the name of the view and parse the model
            return View("Create", viewModel);
        }

        [HttpPost]
        [JsonHandleError]
        public JsonResult Create(AssetTransactionVM viewModel)
        {
            // Check whether error is found
            if(!ModelState.IsValid)
            {
                ModelState.AddModelError("500", "Internal Server Error");
                return Json(new { success = false, urlToRedirect = "google.com" },
                JsonRequestBehavior.AllowGet);
            }

            // Get Header ID after inster to SharePoint
            var headerID = _assetTransactionService.CreateHeader(viewModel.Header);

            // Get Items from session variable
            var items = System.Web.HttpContext.Current.Session["AssetTransactionItemVM"] as List<AssetTransactionItemVM>;

            // Insert items to SharePoint
            _assetTransactionService.CreateItems(headerID, items);  
            
            // Return JSON
            return Json(new { success = true , urlToRedirect = "google.com"}, 
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult Grid_Read([DataSourceRequest] DataSourceRequest request)
        {
            // Get from existing session variable or create new if doesn't exist
            IEnumerable<AssetTransactionItemVM> viewModel = System.Web.HttpContext.Current.Session["AssetTransactionItemVM"] as List<AssetTransactionItemVM> 
                ?? new List<AssetTransactionItemVM>();
            

            // Convert to Kendo DataSource
            DataSourceResult result = viewModel.ToDataSourceResult(request);

            // Convert to Json
            var json = Json(result, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Grid_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<AssetTransactionItemVM> viewModel)
        {
            // CODE OF PRACTICE: Return immidiately if error found
            if (viewModel == null || !ModelState.IsValid)
                return Json(viewModel.ToDataSourceResult(request, ModelState));

            // Get existing session variable if any otherwise create new object
            var sessionVariables = System.Web.HttpContext.Current.Session["AssetTransactionItemVM"] as List<AssetTransactionItemVM>
                ?? new List<AssetTransactionItemVM>();

            // Get Header ID from session variable
            var headerData = System.Web.HttpContext.Current.Session["AssetTransactionHeaderVM"] as AssetTransactionHeaderVM;

            foreach (var item in viewModel)
            {
                // Set it as FK in item
                item.Header_ID = headerData.ID;

                // Store in session variable
                sessionVariables.Add(item);
            }
            
            // Overwrite existing session variable
            System.Web.HttpContext.Current.Session["AssetTransactionItemVM"] = sessionVariables;

            // Return JSON
            return Json(sessionVariables.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Grid_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<AssetTransactionItemVM> viewModel)
        {
            // CODE OF PRACTICE: Return immidiately if error found
            if (viewModel == null || !ModelState.IsValid)
                return Json(viewModel.ToDataSourceResult(request, ModelState));

            // Get existing session variable
            var sessionVariables = System.Web.HttpContext.Current.Session["AssetTransactionItemVM"] as List<AssetTransactionItemVM>
                ?? new List<AssetTransactionItemVM>();

            // Get Header ID from session variable
            var headerData = System.Web.HttpContext.Current.Session["AssetTransactionHeaderVM"] as AssetTransactionHeaderVM;

            foreach (var item in viewModel)
            {
                var obj = sessionVariables.FirstOrDefault(e => e.ID == item.ID);
                obj = item;
            }

            // Overwrite existing session variable
            System.Web.HttpContext.Current.Session["AssetTransactionItemVM"] = sessionVariables;

            // Return JSON
            return Json(sessionVariables.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Grid_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<AssetTransactionItemVM> viewModel)
        {
            // Check if parsed viewModel exist. CODE OF PRACTICE: Return immidiately if error found
            if (!viewModel.Any())
                return Json(viewModel.ToDataSourceResult(request, ModelState));

            // Get existing session variable
            var sessionVariables = System.Web.HttpContext.Current.Session["AssetTransactionItemVM"] as List<AssetTransactionItemVM>;

            foreach (var item in viewModel)
            {
                var obj = sessionVariables.FirstOrDefault(e => e.ID == item.ID);
                sessionVariables.Remove(obj);
            }

            System.Web.HttpContext.Current.Session["AssetTransactionItemVM"] = sessionVariables;
            return Json(viewModel.ToDataSourceResult(request, ModelState));
        }
    }

}
