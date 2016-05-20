using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using MCAWebAndAPI.Service.Asset;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Web.Filters;
using MCAWebAndAPI.Web.Resources;
using MCAWebAndAPI.Web.Helpers;

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
            SessionManager.RemoveAll(); 

            // MANDATORY: Set Site URL
            _assetTransactionService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultBOSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultBOSiteUrl);

            // Get blank ViewModel
            var viewModel = _assetTransactionService.GetPopulatedModel();

            // Modify ViewModel based on spesific case, e.g., Asset Transfer is one of conditions in Asset transaction
            viewModel.Header.TransactionType = "Asset Transfer";

            PopulateInGridComboBox();

            // Return to the name of the view and parse the model
            return View("Create", viewModel);
        }

        private void PopulateInGridComboBox()
        {
            IAssetMasterService _assetMasterService = new AssetMasterService();
            var assetMasters = _assetMasterService.GetAssetMasters();
            ViewData["InGridComboBox_Asset"] = assetMasters.Select(e => new InGridComboBoxVM
            {
                CategoryID = e.ID ?? 0, 
                CategoryName = e.AssetDesc
            });

            //TODO: If Edit Mode, please map to existing data
            ViewData["DefaultValue_Asset"] = assetMasters.FirstOrDefault(e => true);

            var locationMasters = _assetMasterService.GetAssetLocations();
            ViewData["InGridComboBox_Location"] = locationMasters.Select(e => new InGridComboBoxVM
            {
                CategoryID = e.ID ?? 0,
                CategoryName = e.Name
            });
            ViewData["DefaultValue_Location"] = assetMasters.FirstOrDefault(e => true);
        }

        [HttpPost]
        [JsonHandleError]
        public JsonResult Create(AssetTransactionVM viewModel)
        {
            _assetTransactionService.SetSiteUrl(System.Web.HttpContext.Current.Session["SiteUrl"] as string);

            // Get Header ID after inster to SharePoint
            var headerID = _assetTransactionService.CreateHeader(viewModel.Header);

            // Get Items from session variable
            var items = SessionManager.Get<List<AssetTransactionItemVM>>("AssetTransactionItemVM");

            // Insert items to SharePoint
            _assetTransactionService.CreateItems(headerID, items);

            // Clear session variables
            System.Web.HttpContext.Current.Session.Clear();

            // Check whether error is found
            if (!ModelState.IsValid)
            {
                return new JsonResult()
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new { result = "Error" }
                };
            }

            //add to database

            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new { result = "Success" }
            };
        }

        public JsonResult Grid_Read([DataSourceRequest] DataSourceRequest request)
        {
            // Get from existing session variable or create new if doesn't exist
            IEnumerable<AssetTransactionItemVM> viewModel =
                SessionManager.Get<List<AssetTransactionItemVM>>("AssetTransactionItemVM")
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
            var sessionVariables =
                SessionManager.Get<List<AssetTransactionItemVM>>("AssetTransactionItemVM")
                ?? new List<AssetTransactionItemVM>();

            foreach (var item in viewModel)
            {
                // Store in session variable
                sessionVariables.Add(item);
            }
            // Kendo adds new Item on top, so we have to reverse the list
            sessionVariables.Reverse();

            // Overwrite existing session variable
            SessionManager.Set("AssetTransactionItemVM",sessionVariables);

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
            var sessionVariables =
                SessionManager.Get<List<AssetTransactionItemVM>>("AssetTransactionItemVM")
                ?? new List<AssetTransactionItemVM>();

            foreach (var item in viewModel)
            {
                var obj = sessionVariables.FirstOrDefault(e => e.ID == item.ID);
                obj = item;
            }

            // Overwrite existing session variable
            SessionManager.Set("AssetTransactionItemVM", sessionVariables);

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
            var sessionVariables =
                SessionManager.Get<List<AssetTransactionItemVM>>("AssetTransactionItemVM")
                ?? new List<AssetTransactionItemVM>();

            foreach (var item in viewModel)
            {
                var obj = sessionVariables.FirstOrDefault(e => e.ID == item.ID);
                sessionVariables.Remove(obj);
            }

            SessionManager.Set("AssetTransactionItemVM", sessionVariables);
            return Json(viewModel.ToDataSourceResult(request, ModelState));
        }
    }

}
