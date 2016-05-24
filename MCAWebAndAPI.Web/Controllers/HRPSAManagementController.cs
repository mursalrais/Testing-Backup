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
    [Filters.HandleError]
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
            SessionManager.RemoveAll();

            // MANDATORY: Set Site URL
            psaManagementService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            // Get blank ViewModel
            var viewModel = psaManagementService.GetPSAManagement(null);

            return View(viewModel);
        }

        public ActionResult DisplayPSAManagement(string siteUrl = null, int? ID = null)
        {
            // Clear Existing Session Variables if any
            SessionManager.RemoveAll();

            // MANDATORY: Set Site URL
            psaManagementService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

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

        [HttpPost]
        public ActionResult CreateApplicationData(FormCollection form, ApplicationDataVM viewModel)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { errorMessage = BindHelper.GetErrorMessages(ModelState.Values) },
                    JsonRequestBehavior.AllowGet);
            }

            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _service.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            int? headerID = null;
            try
            {
                headerID = _service.CreateApplicationData(viewModel);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return Json(new { errorMessage = e.Message },
                   JsonRequestBehavior.AllowGet);
            }

            try
            {
                viewModel.EducationDetails = BindEducationDetails(form, viewModel.EducationDetails);
                _service.CreateEducationDetails(headerID, viewModel.EducationDetails);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return Json(new { errorMessage = e.Message },
                   JsonRequestBehavior.AllowGet);
            }

            try
            {
                viewModel.TrainingDetails = BindTrainingDetails(form, viewModel.TrainingDetails);
                _service.CreateTrainingDetails(headerID, viewModel.TrainingDetails);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return Json(new { errorMessage = e.Message },
                  JsonRequestBehavior.AllowGet);
            }

            try
            {
                viewModel.WorkingExperienceDetails = BindWorkingExperienceDetails(form, viewModel.WorkingExperienceDetails);
                _service.CreateWorkingExperienceDetails(headerID, viewModel.WorkingExperienceDetails);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return Json(new { errorMessage = e.Message },
                 JsonRequestBehavior.AllowGet);
            }

            try
            {
                _service.CreateProfessionalDocuments(headerID, viewModel.Documents);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return Json(new { errorMessage = e.Message },
                JsonRequestBehavior.AllowGet);
            }

            // Use this if only not embedded in SharePoint page
            return Json(new
            {
                successMessage = string.Format(MessageResource.SuccessCreateApplicationData, viewModel.FirstMiddleName)
            },
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
