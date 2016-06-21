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
using Elmah;
using MCAWebAndAPI.Service.Converter;
using MCAWebAndAPI.Service.HR.Common;
using System.IO;
using System.Web;
using System.Globalization;
using MCAWebAndAPI.Service.JobSchedulers.Schedulers;
using MCAWebAndAPI.Service.HR.Payroll;
using MCAWebAndAPI.Service.Resources;
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
            // Clear Existing Session Variables if any
            SessionManager.RemoveAll();

            // MANDATORY: Set Site URL
            exitProcedureService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            // Get blank ViewModel
            var viewModel = exitProcedureService.GetExitProcedure(null);

            return View("CreateExitProcedure", viewModel);
        }
    }
}