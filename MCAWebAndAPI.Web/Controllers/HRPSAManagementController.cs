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

            return View("Create", viewModel);
        }

        public ActionResult Edit(int ID, string site)
        {
            var viewModel = psaManagementService.GetPSAManagement(ID);
            return View(viewModel);
        }

        [HttpPost]
       
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

            // Clear session variables
            System.Web.HttpContext.Current.Session.Clear();

            // Return JSON
            return Json(new { success = true, urlToRedirect = "google.com" },
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult Update(PSAManagementVM psaManagement, string site)
        {
            //return View(new AssetMasterVM());
            psaManagementService.UpdatePSAManagement(psaManagement);
            return new JavaScriptResult
            {
                Script = string.Format("window.parent.location.href = '{0}'", "https://eceos2.sharepoint.com/sites/mca-dev/hr/_layouts/15/start.aspx#/Lists/PSA/AllItems.aspx")
            };
        }

        

        public JsonResult GetPsa(string id)
        {
            psaManagementService.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);
            var professionals = GetFromExistingSession();
            return Json(professionals.OrderByDescending(e => e.PSAID).Where(e => e.ID == id).Select(
                    e =>
                    new
                    {
                        e.PSAID,
                        e.ID,
                        e.JoinDate,
                        e.DateOfNewPSA,
                        e.PsaExpiryDate,
                        e.ProjectOrUnit,
                        e.Position
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
