using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.HR;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Web.Filters;
using MCAWebAndAPI.Web.Resources;
using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Service.HR.Recruitment;

namespace MCAWebAndAPI.Web.Controllers
{
    public class HRPSAManagementController : Controller
    {
        IPSAManagementService psaManagementService;

        public HRPSAManagementController()
        {
            psaManagementService = new PSAManagementService();
        }

        /// <summary>
        /// HTTP Get to View Create with specific argument
        /// </summary>
        /// <param name="siteUrl"></param>
        /// <returns></returns>
        public ActionResult CreatePSAManagement(string siteUrl = null)
        {
            // Clear Existing Session Variables if any
            if (System.Web.HttpContext.Current.Session.Keys.Count > 0)
                System.Web.HttpContext.Current.Session.Clear();

            // MANDATORY: Set Site URL
            psaManagementService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            System.Web.HttpContext.Current.Session["SiteUrl"] = siteUrl ?? ConfigResource.DefaultHRSiteUrl;

            // Get blank ViewModel
            var viewModel = psaManagementService.GetPopulatedModel();

            // Modify ViewModel based on spesific case, e.g., Asset Transfer is one of conditions in Asset transaction
            //viewModel.Header.TransactionType = "Asset Transfer";

            //PopulateInGridComboBox();

            // Return to the name of the view and parse the model
            return View("Create", viewModel);
        }

        /*
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
        */

        [HttpPost]
        [JsonHandleError]
        public JsonResult Create(PSAManagementVM viewModel)
        {
            // Check whether error is found
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("500", "Internal Server Error");
                return Json(new { success = false, urlToRedirect = "google.com" },
                JsonRequestBehavior.AllowGet);
            }

            psaManagementService.SetSiteUrl(System.Web.HttpContext.Current.Session["SiteUrl"] as string);

            // Get Header ID after inster to SharePoint
            var psaID = psaManagementService.CreatePSA(viewModel);

            // Get Items from session variable
            //var items = System.Web.HttpContext.Current.Session["AssetTransactionItemVM"] as List<AssetTransactionItemVM>;

            // Insert items to SharePoint
            //_assetTransactionService.CreateItems(headerID, items);

            // Clear session variables
            System.Web.HttpContext.Current.Session.Clear();

            // Return JSON
            return Json(new { success = true, urlToRedirect = "google.com" },
                JsonRequestBehavior.AllowGet);
        }

        /*
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
        */

        /*
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Grid_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<AssetTransactionItemVM> viewModel)
        {
            // CODE OF PRACTICE: Return immidiately if error found
            if (viewModel == null || !ModelState.IsValid)
                return Json(viewModel.ToDataSourceResult(request, ModelState));

            // Get existing session variable if any otherwise create new object
            var sessionVariables = System.Web.HttpContext.Current.Session["AssetTransactionItemVM"] as List<AssetTransactionItemVM>
                ?? new List<AssetTransactionItemVM>();

            foreach (var item in viewModel)
            {
                // Store in session variable
                sessionVariables.Add(item);
            }
            // Kendo adds new Item on top, so we have to reverse the list
            sessionVariables.Reverse();

            // Overwrite existing session variable
            System.Web.HttpContext.Current.Session["AssetTransactionItemVM"] = sessionVariables;

            // Return JSON
            return Json(sessionVariables.ToDataSourceResult(request, ModelState));
        }
        */

        /*
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Grid_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<AssetTransactionItemVM> viewModel)
        {
            // CODE OF PRACTICE: Return immidiately if error found
            if (viewModel == null || !ModelState.IsValid)
                return Json(viewModel.ToDataSourceResult(request, ModelState));

            // Get existing session variable
            var sessionVariables = System.Web.HttpContext.Current.Session["AssetTransactionItemVM"] as List<AssetTransactionItemVM>
                ?? new List<AssetTransactionItemVM>();

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
        */

        /*
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
        */

        public JsonResult GetPsa(string id)
        {
            psaManagementService.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);
            var professionals = GetFromExistingSession();
            return Json(professionals.Where(e => e.ID == id).Select(
                    e =>
                    new
                    {
                        e.ID,
                        e.JoinDate,
                        e.DateOfNewPSA,
                        e.PsaExpiryDate,
                        e.ProjectOrUnit
                    }
                ), JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<PSAMaster> GetFromExistingSession()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["PSA"] as IEnumerable<PSAMaster>;
            var psa = sessionVariable ?? psaManagementService.GetPSAs();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["PSA"] = psa;
            return psa;
        }

    }

}
