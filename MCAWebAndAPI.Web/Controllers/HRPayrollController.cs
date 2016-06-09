using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.HR.Payroll;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Web.Filters;
using MCAWebAndAPI.Web.Resources;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Service.Resources;
using System.Net;
using System;

namespace MCAWebAndAPI.Web.Controllers
{
    public class HRPayrollController : Controller
    {
        IHRPayrollServices _hRPayrollService;

        public HRPayrollController()
        {
            _hRPayrollService = new HRPayrollServices();
        }

        public ActionResult CreateMonthlyFee(string siteUrl = null)
        {
            // Clear Existing Session Variables if any
            SessionManager.RemoveAll();

            // MANDATORY: Set Site URL
            _hRPayrollService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            // Get blank ViewModel
            var viewModel = _hRPayrollService.GetPopulatedModel();

            // Return to the name of the view and parse the model
            return View("CreateMonthlyFee", viewModel);
        }

        [HttpPost]
        public ActionResult SubmitMonthlyFee(MonthlyFeeVM viewModel)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var errorMessages = BindHelper.GetErrorMessages(ModelState.Values);
                return JsonHelper.GenerateJsonErrorResponse(errorMessages);
            }
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _hRPayrollService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            try
            {
                var headerID = _hRPayrollService.CreateHeader(viewModel);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.MonthlyFee);
        }

        public ActionResult EditMonthlyFee(int ID, string siteUrl = null)
        {
            _hRPayrollService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var viewModel = _hRPayrollService.GetHeader(ID);

            return View(viewModel);
        }

        [HttpPost]

        public ActionResult UpdateMonthlyFee(MonthlyFeeVM _data, string site)
        {
            if (!ModelState.IsValid)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var errorMessages = BindHelper.GetErrorMessages(ModelState.Values);
                return JsonHelper.GenerateJsonErrorResponse(errorMessages);
            }

            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _hRPayrollService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            _hRPayrollService.UpdateHeader(_data);

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.MonthlyFee);
        }

        // GET: HRMonthly
        public ActionResult Index()
        {
            return View();
        }
    }
}