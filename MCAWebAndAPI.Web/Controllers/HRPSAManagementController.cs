﻿using Kendo.Mvc.Extensions;
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
using Elmah;
using MCAWebAndAPI.Service.Converter;
using MCAWebAndAPI.Service.HR.Common;
using System.IO;
using System.Web;
using System.Globalization;

using MCAWebAndAPI.Service.HR.Payroll;
using MCAWebAndAPI.Service.Resources;
using System.Net;

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

            return View("CreatePSAManagement", viewModel);
        }

        
        public ActionResult DisplayPSAManagement(string siteUrl = null, int? ID = null)
        {
            // Clear Existing Session Variables if any
            SessionManager.RemoveAll();

            // MANDATORY: Set Site URL
            psaManagementService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var viewModel = psaManagementService.GetPSAManagement(ID);
            return View("EditPSAManagement", viewModel);
        }

        public ActionResult ViewPSAManagement(string siteUrl = null, int? ID = null)
        {
            // Clear Existing Session Variables if any
            SessionManager.RemoveAll();

            // MANDATORY: Set Site URL
            psaManagementService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var viewModel = psaManagementService.ViewPSAManagementData(ID);
            return View("DisplayPSAManagement", viewModel);
        }

        [HttpPost]
        public ActionResult CreatePSAManagement(FormCollection form, PSAManagementVM viewModel)
        {
            // Check whether error is found
            if (!ModelState.IsValid)
            {
                RedirectToAction("Index", "Error");
            }

            var siteUrl = SessionManager.Get<string>("SiteUrl");
            psaManagementService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            int? psaID = null;
            try
            {
                psaID = psaManagementService.CreatePSAManagement(viewModel);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error");
            }

            try
            {
                psaManagementService.CreatePSAManagementDocuments(psaID, viewModel.Documents);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error");
            }

            return RedirectToAction("Index",
                "Success",
                new { successMessage = string.Format(MessageResource.SuccessCreatePSAManagementData, viewModel.PSANumber) });

        }

        public ActionResult UpdatePSAManagement(PSAManagementVM psaManagement, string site)
        {

            var siteUrl = SessionManager.Get<string>("SiteUrl");
            psaManagementService.SetSiteUrl(siteUrl);

            try
            {
                var headerID = psaManagementService.UpdatePSAManagement(psaManagement);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(
                string.Format("{0}/{1}", siteUrl, UrlResource.PSAManagement),
                string.Format(MessageResource.SuccessUpdatePSAManagementData, psaManagement.PSANumber));
            
            /*
            return RedirectToAction("Index",
                "Success",
                new { successMessage = string.Format(MessageResource.SuccessUpdatePSAManagementData, psaManagement.PSANumber) });
            */
        }

        public JsonResult GetPsa(string id)
        {
            psaManagementService.SetSiteUrl(SessionManager.Get<string>("SiteUrl"));
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
