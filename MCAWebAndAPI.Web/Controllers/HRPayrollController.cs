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
            // MANDATORY: Set Site URL
            _hRPayrollService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            // Get blank ViewModel
            var viewModel = _hRPayrollService.GetPopulatedModel();

            // Return to the name of the view and parse the model
            return View("CreateMonthlyFee", viewModel);
        }

        [HttpPost]
        public ActionResult SubmitMonthlyFee(FormCollection form, MonthlyFeeVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _hRPayrollService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            int? headerID = null;
            try
            {
                headerID = _hRPayrollService.CreateHeader(viewModel);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            try
            {
                viewModel.MonthlyFeeDetails = BindMonthlyFeeDetailDetails(form, viewModel.MonthlyFeeDetails);
                _hRPayrollService.CreateMonthlyFeeDetails(headerID, viewModel.MonthlyFeeDetails);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.MonthlyFee);
        }

        public ActionResult EditMonthlyFee(int ID, string siteUrl = null, string Display = null)
        {
            _hRPayrollService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var viewModel = _hRPayrollService.GetHeader(ID);
            var form = "EditMonthlyFee";
            if (Display == "Yes")
            {
                form = "DisplayMonthlyFee";
            }
            return View(form, viewModel);
        }

        [HttpPost]
        public ActionResult UpdateMonthlyFee(FormCollection form, MonthlyFeeVM viewModel, string site)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _hRPayrollService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            _hRPayrollService.UpdateHeader(viewModel);

            try
            {
                viewModel.MonthlyFeeDetails = BindMonthlyFeeDetailDetails(form, viewModel.MonthlyFeeDetails);
                _hRPayrollService.CreateMonthlyFeeDetails(viewModel.ID, viewModel.MonthlyFeeDetails);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.MonthlyFee);
        }

        IEnumerable<MonthlyFeeDetailVM> BindMonthlyFeeDetailDetails(FormCollection form, IEnumerable<MonthlyFeeDetailVM> monthlyFeeDetails)
        {
            var array = monthlyFeeDetails.ToArray();

            for (int i = 0; i < array.Length; i++)
            {
                array[i].DateOfNewFee = BindHelper.BindDateInGrid("MonthlyFeeDetails",
                    i, "DateOfNewFee", form);
            }
            return array;
        }

        public ActionResult DisplayPayrollWorksheet(string siteUrl)
        {
            SessionManager.Set("SiteUrl", siteUrl);
            _hRPayrollService.SetSiteUrl(siteUrl);

            var viewModelPayroll = _hRPayrollService.GetPayrollWorksheetDetails(DateTime.Today);
            SessionManager.Set("PayrollWorksheetDetailVM", viewModelPayroll);

            var viewModel = new PayrollRunVM();  
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult UpdatePeriodWorksheet(PayrollRunVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _hRPayrollService.SetSiteUrl(siteUrl);

            IEnumerable<PayrollWorksheetDetailVM> viewModelPayroll = new List<PayrollWorksheetDetailVM>();
            try
            {
                viewModelPayroll = _hRPayrollService.GetPayrollWorksheetDetails(viewModel.Period);
            }
            catch (Exception e)
            {
                return Json(new { message = e.Message }, JsonRequestBehavior.AllowGet);
            }

            SessionManager.Set("PayrollWorksheetDetailVM", viewModelPayroll);

            return Json(new { message = "Period has been updated" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GridWorksheet_Read([DataSourceRequest] DataSourceRequest request)
        {
            // Get from existing session variable or create new if doesn't exist
            IEnumerable<PayrollWorksheetDetailVM> items = SessionManager.Get<IEnumerable<PayrollWorksheetDetailVM>>("PayrollWorksheetDetailVM");

            // Convert to Kendo DataSource
            DataSourceResult result = items.ToDataSourceResult(request);

            // Convert to Json 
            var json = Json(result, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }

        [HttpPost]
        public ActionResult GridWorksheet_ExportExcel(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            return File(fileContents, contentType, fileName);
        }
    }
}