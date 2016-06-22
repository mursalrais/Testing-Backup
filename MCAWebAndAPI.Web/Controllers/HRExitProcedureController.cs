﻿using MCAWebAndAPI.Model.ViewModel.Form.HR;
using System.Web.Mvc;
using System;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using MCAWebAndAPI.Service.HR.Recruitment;
using Elmah;
using System.Net;

namespace MCAWebAndAPI.Web.Controllers
{
    public class HRExitProcedureController : Controller
    {
        IExitProcedureService exitProcedureService;

        public HRExitProcedureController()
        {
            exitProcedureService = new ExitProcedureService();
        }

        /// <summary>
        /// HTTP Get to View Create with specific argument
        /// </summary>
        /// <param name="siteUrl"></param>
        /// <returns></returns>
        public ActionResult CreateExitProcedure(string siteUrl = null)
        {
            
            // MANDATORY: Set Site URL
            exitProcedureService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            // Get blank ViewModel
            var viewModel = exitProcedureService.GetExitProcedure(null);

            return View("CreateExitProcedure", viewModel);
        }

        //Submit every data in Exit Procedure to List
        [HttpPost]
        public ActionResult CreateExitProcedure(FormCollection form, ExitProcedureVM viewModel)
        {
            // Check whether error is found
            if (!ModelState.IsValid)
            {
                RedirectToAction("Index", "Error");
            }

            var siteUrl = SessionManager.Get<string>("SiteUrl");
            exitProcedureService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            int? exitProcID = null;

            try
            {
                exitProcID = exitProcedureService.CreateExitProcedure(viewModel);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error");
            }

            /*
            try
            {
                exitProcedureService.CreatePSAManagementDocuments(exitProcID, viewModel.Documents, viewModel);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error");
            }
            */

            return RedirectToAction("Index",
                "Success",
                new { errorMessage = string.Format(MessageResource.SuccessCreateExitProcedureData, exitProcID) });
        }

        public ActionResult DisplayExitProcedure(string siteUrl = null, int? ID = null)
        {



            // MANDATORY: Set Site URL
            exitProcedureService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var viewModel = exitProcedureService.GetExitProcedure(ID);

            return View("EditExitProcedure", viewModel);
        }

        public ActionResult UpdateExitProcedure(ExitProcedureVM exitProcedure, string site)
        {

            var siteUrl = SessionManager.Get<string>("SiteUrl");
            exitProcedureService.SetSiteUrl(siteUrl);

            try
            {
                var headerID = exitProcedureService.UpdateExitProcedure(exitProcedure);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return RedirectToAction("Index",
                "Success",
                new { errorMessage = string.Format(MessageResource.SuccessUpdateExitProcedure, exitProcedure.ID) });

        }
    }
}