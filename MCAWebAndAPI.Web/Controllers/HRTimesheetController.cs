using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.HR.Timesheet;
using MCAWebAndAPI.Web.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Service.Resources;

namespace MCAWebAndAPI.Web.Controllers
{
    public class HRTimesheetController : Controller
    {
        ITimesheetService _timesheetService;
        public HRTimesheetController()
        {
            _timesheetService = new TimesheetService();
        }

        public ActionResult EditTimesheet(string siteUrl, string userlogin)
        {
            SessionManager.Set("SiteUrl", siteUrl);
            _timesheetService.SetSiteUrl(siteUrl);
            var viewModel = _timesheetService.GetTimesheet(userlogin, DateTime.Now.GetFirstPayrollDay());

            SessionManager.Set("TimesheetDetails", viewModel.TimesheetDetails);
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult UpdatePeriod(TimesheetVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _timesheetService.SetSiteUrl(siteUrl);

            try
            {
                viewModel = _timesheetService.GetTimesheet(viewModel.UserLogin, ((DateTime)(viewModel.Period)));
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { message = e.Message }, JsonRequestBehavior.AllowGet);
            }

            SessionManager.Set("TimesheetDetails", viewModel.TimesheetDetails);

            return Json(new { message = "Period is updated" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddTimesheet(TimesheetVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _timesheetService.SetSiteUrl(siteUrl);

            var strLocation = viewModel.LocationName;
            var intLocationID = viewModel.LocationID;

            var items = SessionManager.Get<IEnumerable<TimesheetDetailVM>>("TimesheetDetails");
           
            try
            {
               items = _timesheetService.AppendWorkingDays(items, Convert.ToDateTime(viewModel.From).ToLocalTime(),
                    Convert.ToDateTime(viewModel.To).ToLocalTime(), viewModel.IsFullDay, strLocation, intLocationID);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { message = e.Message }, JsonRequestBehavior.AllowGet);
            }
           
            SessionManager.Set("TimesheetDetails", items);

            return Json(new
            {
                message = "Timesheet is updated"
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SubmitTimesheet(TimesheetVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _timesheetService.SetSiteUrl(siteUrl);
            int? headerId;
            try
            {
                headerId= _timesheetService.CreateHeader(viewModel);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { message = e.Message }, JsonRequestBehavior.AllowGet);
            }

            var items = SessionManager.Get<IEnumerable<TimesheetDetailVM>>("TimesheetDetails");

            try
            {
                viewModel.TimesheetDetails = items;
                _timesheetService.CreateTimesheetDetails(headerId, viewModel.TimesheetDetails);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e);
            }

            var strPages = viewModel.UserPermission == "HR" ? "/sitePages/hrInsuranceView.aspx" : "/sitePages/ProfessionalClaim.aspx";

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + strPages);

        }

        public JsonResult GridHolidays_Read([DataSourceRequest] DataSourceRequest request)
        {
            // Get from existing session variable or create new if doesn't exist
            var items = SessionManager.Get<IEnumerable<TimesheetDetailVM>>("TimesheetDetails").Where(e => e.Type != null);

            // Convert to Kendo DataSource
            DataSourceResult result = items.ToDataSourceResult(request);

            // Convert to Json
            var json = Json(result, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }

        public JsonResult GridWorkingDays_Read([DataSourceRequest] DataSourceRequest request)
        {
            // Get from existing session variable or create new if doesn't exist
            var items = SessionManager.Get<IEnumerable<TimesheetDetailVM>>("TimesheetDetails").Where(e => e.Type == null);

            // Convert to Kendo DataSource
            DataSourceResult result = items.ToDataSourceResult(request);

            // Convert to Json
            var json = Json(result, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }
        

    }
}