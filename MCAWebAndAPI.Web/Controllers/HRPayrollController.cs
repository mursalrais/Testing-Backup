using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.HR.Payroll;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MCAWebAndAPI.Web.Resources;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Service.Resources;
using System.Net;
using System;
using System.IO;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.Common;
using Elmah;
using MCAWebAndAPI.Service.JobSchedulers.Schedulers;
using System.Globalization;
using System.Data;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.Text.RegularExpressions;

namespace MCAWebAndAPI.Web.Controllers
{
    /// <summary>
    /// Unfinished tasks:
    /// 1. Get .draft file from Filesystem. Convert to ViewModel. Convert to SessionVariable. Load into Grid.
    /// 2. Split view in GridDraftWorksheet_Read between .draft and .xlsx
    /// 3. Business Logic: Continue to produce pivot / summarized view. 
    /// </summary>
    public class HRPayrollController : Controller
    {
        IPayrollService _hRPayrollService;
        private const string PAYROLL_WORKSHEET_FILENAME = "PayrollWorksheet-Period-{0}-RunOn-{1}.{2}";
        private const string PAYROLL_WORKSHEET_DIRECTORY = "~/App_Data/PayrollWorksheet/";

        public HRPayrollController()
        {
            _hRPayrollService = new PayrollService();
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
                array[i].EndDateFee = BindHelper.BindDateInGrid("MonthlyFeeDetails",
                    i, "EndDateFee", form);
            }
            return array;
        }

        public ActionResult DisplayPayrollWorksheet(string siteUrl, string userLogin)
        {
            SessionManager.Set("SiteUrl", siteUrl);
            _hRPayrollService.SetSiteUrl(siteUrl);

            SessionManager.Set("UserLogin", userLogin);
            
            var viewModel = new PayrollRunVM();  
            return View(viewModel);
        }

        [AsyncTimeout(1000000)]
        public async Task<ActionResult> DisplayPayrollWorksheetSummary(string siteUrl)
        {
            SessionManager.Set("SiteUrl", siteUrl);
            _hRPayrollService.SetSiteUrl(siteUrl);

            var viewModelPayroll = await _hRPayrollService.GetPayrollWorksheetDetailsAsync(DateTime.Today);
            SessionManager.Set("PayrollWorksheetDetailVM", viewModelPayroll);

            var viewModel = new PayrollRunVM();
            return View(viewModel);
        }

        /// <summary>
        /// Triggered after period is updated
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [AsyncTimeout(1000000)]
        [HttpPost]
        public async Task<ActionResult> DisplayInScreenPeriodWorksheet(PayrollRunVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _hRPayrollService.SetSiteUrl(siteUrl);
            await PopulatePayrollWorksheet(viewModel.Period);
            return Json(new { message = "Period has been updated" }, JsonRequestBehavior.AllowGet);
        }

        [AsyncTimeout(1000000)]
        public ActionResult RunInBackgroundPeriodWorksheet(string periodString)
        {
            var period = DateTime.ParseExact(periodString, "dd-MM-yyyy", CultureInfo.CurrentCulture);
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            var fileName = string.Format(PAYROLL_WORKSHEET_FILENAME,
                period.ToLocalTime().ToString("yyyy-MM"),
                DateTime.Today.ToLocalTime().ToString("yyyy-MM-dd"), "draft");
            var userLogin = SessionManager.Get<string>("UserLogin");

            var path = Path.Combine(Server.MapPath(PAYROLL_WORKSHEET_DIRECTORY), fileName);
            PayrollRunScheduler.DoNow_Once(siteUrl, path, userLogin, period.Day, period.Month, period.Year);
            
            return Json(new
            {
                message =
                string.Format(@"Payroll from {0} to {1} has been run in background. 
                    You may download 5-10 minutes later in <a href='/HRPayroll/DisplayPayrollWorksheetDrafts'>
                    Page Display Previous Drafts</a>.",
                period.GetFirstPayrollDay(), period.GetLastPayrollDay())
            }, JsonRequestBehavior.AllowGet);
        }

        private async Task PopulatePayrollWorksheet(DateTime period)
        {
            IEnumerable<PayrollWorksheetDetailVM> viewModelPayroll = new List<PayrollWorksheetDetailVM>();
            viewModelPayroll = await _hRPayrollService.GetPayrollWorksheetDetailsAsync(period);
            
            SessionManager.Set("PayrollWorksheetDetailVM", viewModelPayroll);
            SessionManager.Set("PayrollWorksheetPeriod", period.ToLocalTime().ToString("yyyy-MM"));
        }

        [HttpPost]
        public ActionResult GridWorksheet_Read([DataSourceRequest] DataSourceRequest request)
        {
            // Get from existing session variable or create new if doesn't exist
            IEnumerable<PayrollWorksheetDetailVM> items = SessionManager.Get<IEnumerable<PayrollWorksheetDetailVM>>("PayrollWorksheetDetailVM") ?? 
                new List<PayrollWorksheetDetailVM>();

            // Convert to Kendo DataSource
            DataSourceResult result = items.ToDataSourceResult(request);

            // Convert to Json 
            var json = Json(result, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentType">application/vnd.openxmlformats-officedocument.spreadsheetml.sheet</param>
        /// <param name="base64"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GridWorksheet_ExportExcel(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            fileName = string.Format(PAYROLL_WORKSHEET_FILENAME, 
                SessionManager.Get<string>("PayrollWorksheetPeriod"), 
                DateTime.Today.ToLocalTime().ToString("yyyy-MM-dd"), "xlsx");

            // Store the file on the session variable
            var fileContent = fileContents;
            SessionManager.Set("PayrollWorksheet_ExcelFile", fileContents);

            return File(fileContents, contentType, fileName);
        }

        public ActionResult GridWorksheet_SaveAsDraft()
        {
            var fileContents = SessionManager.Get<byte[]>("PayrollWorksheet_ExcelFile");
            var fileName = string.Format(PAYROLL_WORKSHEET_FILENAME,
                SessionManager.Get<string>("PayrollWorksheetPeriod"),
                DateTime.Today.ToLocalTime().ToString("yyyy-MM-dd"), "xlsx");

            try
            {
                if (fileContents.Length > 0)
                {
                    var path = Path.Combine(Server.MapPath(PAYROLL_WORKSHEET_DIRECTORY), fileName);
                    System.IO.File.WriteAllBytes(path, fileContents);
                }
                else
                {
                    ErrorSignal.FromCurrentContext().Raise(new FileNotFoundException());
                }
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { message = e.Message }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { message = "Payroll Worksheet has been stored as a Draft" },
                 JsonRequestBehavior.AllowGet);
        }

        public ActionResult DisplayPayrollWorksheetDrafts()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GridDraftWorksheet_Read([DataSourceRequest] DataSourceRequest request)
        {
            // List all files in Folder
            var files = Directory.EnumerateFiles(Server.MapPath(PAYROLL_WORKSHEET_DIRECTORY));

            IEnumerable<PayrollWorksheetDraftVM> items = files.Select(e =>
                new PayrollWorksheetDraftVM {
                    FileName = e,
                    Period = GetPeriodFromWorksheetDraftFileName(e),
                    RunOn = GetRunOnFromWorksheetDraftFileName(e),
                    UrlToDownload = string.Format("/File/Download?fileName={0}",ConvertToRelativeUrl(e))
                });

            // Convert to Kendo DataSource
            DataSourceResult result = items.ToDataSourceResult(request);

            // Convert to Json 
            var json = Json(result, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }

        private DateTime GetPeriodFromWorksheetDraftFileName(string worksheetDraftFileName)
        {
            var periodString = worksheetDraftFileName.Split(new string[] { "Period" }, StringSplitOptions.None);
            periodString = periodString[1].Split('-');

            var year = Convert.ToInt32(periodString[1]);
            var month = Convert.ToInt32(periodString[2]);

            return new DateTime(year, month, 1);
        }

        private DateTime GetRunOnFromWorksheetDraftFileName(string worksheetDraftFileName)
        {
            var periodString = worksheetDraftFileName.Split(new string[] { "RunOn" }, StringSplitOptions.None);
            periodString = periodString[1].Split('-');

            var year = Convert.ToInt32(periodString[1]);
            var month = Convert.ToInt32(periodString[2]);
            var day = Convert.ToInt32(periodString[3].Split('.')[0]);

            return new DateTime(year, month, day);
        }

        [HttpGet]
        public ActionResult Upload(string siteUrl = null)
        {

            if (siteUrl == null)
                return RedirectToAction("Index", "Error", new { errorMessage = "Parameter cannot be null" });


            SessionManager.Set("SiteUrl", siteUrl);

            var emptyTable = GenerateEmptyDataTable();
            SessionManager.Set("CSVDataTable", emptyTable);

            var viewModel = new CSVVM();
            viewModel.ListName = "Monthly Fee";
            viewModel.DataTable = emptyTable;

            return View(viewModel);
        }

        public ActionResult SubmitUpload(string listName)
        {
            // Get existing session variable
            var sessionVariables = SessionManager.Get<DataTable>("CSVDataTable") ?? new DataTable();
            var siteUrl = SessionManager.Get<string>("SiteUrl");

            //cek apakah header / item
            int? latestIDHeader = 0;
            int? latestIDDetail = 0;
            List<int> idsHeader = new List<int>();
            List<int> idsDetail = new List<int>();
            var TableHeader = new DataTable();
            var TableDetail = new DataTable();

            var listNameHeader = "Asset Acquisition";
            var listNameHeaderMemo = "Acceptance Memo";
            var listNameDetail = "Asset Acquisition Details";
            int IDProfessional;
            DateTime DateofNewFee;
            DateTime EndDateFee;
            int MonthlyFee;
            int Currency;
            foreach (DataRow d in SessionManager.Get<DataTable>("CSVDataTable").Rows)
            {
                IDProfessional = Convert.ToInt32(d.ItemArray[0]);
                DateofNewFee = Convert.ToDateTime(d.ItemArray[1]);
                EndDateFee = Convert.ToDateTime(d.ItemArray[2]);
            }
            //return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.AssetAcquisition);
            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + UrlResource.AssetAcquisition);
        }

        private DataTable GenerateEmptyDataTable()
        {
            DataTable table = new DataTable();

            var column = new DataColumn("ID", typeof(int));
            table.Columns.Add(column);
            table.PrimaryKey = new DataColumn[] { column };
            table.Columns.Add("Title", typeof(string));

            table.Rows.Add(0, string.Empty);
            return table;
        }

        private string ConvertToRelativeUrl(string absoluteUrl)
        {
            var splitUrl = absoluteUrl.Split(new string[] { "App_Data" }, StringSplitOptions.None);
            return splitUrl[1].Replace(@"\", @"/");
        }
    }
}