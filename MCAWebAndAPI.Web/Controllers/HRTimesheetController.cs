﻿using Kendo.Mvc.Extensions;
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
using System.Threading.Tasks;
//using

namespace MCAWebAndAPI.Web.Controllers
{
    public class HRTimesheetController : Controller
    {
        ITimesheetService _timesheetService;
        public HRTimesheetController()
        {
            _timesheetService = new TimesheetService();
        }

        public ActionResult CreateTimesheet(string siteUrl, string userlogin)
        {
            SessionManager.Set("SiteUrl", siteUrl);
            _timesheetService.SetSiteUrl(siteUrl);
            var viewModel = _timesheetService.GetTimesheet(userlogin, DateTime.Now.GetFirstPayrollDay());

            SessionManager.Set("TimesheetDetails", viewModel.TimesheetDetails);
            return View(viewModel);
        }

        public ActionResult EditTimesheet(string siteUrl, int? id , string userlogin)
        {
            SessionManager.Set("SiteUrl", siteUrl);
            _timesheetService.SetSiteUrl(siteUrl);
            var viewModel = _timesheetService.GetTimesheetLoadUpdate(id,userlogin);

            if (viewModel.UserPermission== "Not Authorized") return RedirectToAction("NotAuthorized", "HRTimesheet");

            SessionManager.Set("TimesheetDetails", viewModel.TimesheetDetails);
            return View(viewModel);
        }

        public ActionResult NotAuthorized(string siteUrl = null)
        {

            return View("NotAuthorized", null);

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
        public ActionResult AddTimesheet(int? id,TimesheetVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _timesheetService.SetSiteUrl(siteUrl);

            var strLocation = viewModel.LocationName;
            var intLocationID = viewModel.LocationID;

            var items = SessionManager.Get<IEnumerable<TimesheetDetailVM>>("TimesheetDetails");
           
            try
            {
               items = _timesheetService.AppendWorkingDays(id,items, Convert.ToDateTime(viewModel.From).ToLocalTime(),
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
        public ActionResult DeleteAllTimesheet(int? id)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _timesheetService.SetSiteUrl(siteUrl);


            var items = SessionManager.Get<IEnumerable<TimesheetDetailVM>>("TimesheetDetails");
            var listItems = items.ToList();
            try
            {
                if (id == null)
                {
                    listItems.RemoveAll(d => d.Type == "Working Days");
                }
                else
                {
                    (from u in listItems where u.Type == "Working Days" select u).ToList()
                    .ForEach(u => u.EditMode=-1);

                }

            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { message = e.Message }, JsonRequestBehavior.AllowGet);
            }

            SessionManager.Set("TimesheetDetails", listItems);

            return Json(new
            {
                message = "Timesheet is updated"
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteSelectedTimesheet(int? id,TimesheetVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _timesheetService.SetSiteUrl(siteUrl);



            var items = SessionManager.Get<IEnumerable<TimesheetDetailVM>>("TimesheetDetails");

            try
            {
                items = _timesheetService.DeleteSelectedWorkingDays(id,items, Convert.ToDateTime(viewModel.From).ToLocalTime(),
                    Convert.ToDateTime(viewModel.To).ToLocalTime());

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
        public async Task<ActionResult> SubmitTimesheet(TimesheetVM viewModel)
        {
            try
            {
                var siteUrl = SessionManager.Get<string>("SiteUrl");
                _timesheetService.SetSiteUrl(siteUrl);
                int? headerId;
           
                if (string.IsNullOrEmpty(viewModel.UserLogin)) throw new Exception("Userlogin empty");

                headerId = _timesheetService.CreateHeader(viewModel);
             
                var items = SessionManager.Get<IEnumerable<TimesheetDetailVM>>("TimesheetDetails");

               
                viewModel.TimesheetDetails = items;
                Task createTimesheetDetailsTask = _timesheetService.CreateTimesheetDetailsAsync(headerId, viewModel.TimesheetDetails);
                Task createTimesheetWorkflowTask = _timesheetService.CreateWorkflowTimesheetAsync(headerId, viewModel);

                Task allTask = Task.WhenAll(createTimesheetDetailsTask, createTimesheetWorkflowTask);
            
                await allTask;

                var strPages = "";

                if (viewModel.UserPermission == "HR")
                {
                    strPages = "/sitePages/HRTimesheetView.aspx";
                }
                else if (viewModel.UserPermission == "Professional")
                {
                    strPages = "/sitePages/ProfessionalTimesheetView.aspx";
                }
                else if (viewModel.UserPermission == "Approver")
                {
                    strPages = "";

                }

                return JsonHelper.GenerateJsonSuccessResponse(siteUrl + strPages);
            }
            catch (Exception e)
            {

                //Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e.Message);
            }


        }

        [HttpPost]
        public async Task<ActionResult> UpdateTimesheet(TimesheetVM viewModel)
        {
            try
            {
                var siteUrl = SessionManager.Get<string>("SiteUrl");
                _timesheetService.SetSiteUrl(siteUrl);
              
                if (string.IsNullOrEmpty(viewModel.UserLogin)) throw new Exception("Userlogin empty");

                _timesheetService.UpdateHeader(viewModel);

            
                var items = SessionManager.Get<IEnumerable<TimesheetDetailVM>>("TimesheetDetails");


                viewModel.TimesheetDetails = items;
                Task createTimesheetDetailsTask = _timesheetService.CreateTimesheetDetailsAsync(viewModel.ID, viewModel.TimesheetDetails);
                Task createTimesheetWorkflowTask = _timesheetService.CreateWorkflowTimesheetAsync(viewModel.ID, viewModel);

                Task allTask = Task.WhenAll(createTimesheetDetailsTask, createTimesheetWorkflowTask);

                await allTask;

                var strPages = "";

                if (viewModel.UserPermission == "HR")
                {
                    strPages = "/sitePages/HRTimesheetView.aspx";
                }
                else if (viewModel.UserPermission == "Professional")
                {
                    strPages = "/sitePages/ProfessionalTimesheetView.aspx";
                }
                else if (viewModel.UserPermission == "Approver")
                {
                    strPages = "";

                }

                return JsonHelper.GenerateJsonSuccessResponse(siteUrl + strPages);
            }
            catch (Exception e)
            {

                //Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e.Message);
            }


        }

        [HttpPost]
        public ActionResult UpdateApproval(TimesheetVM viewModel)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _timesheetService.SetSiteUrl(siteUrl);

            try
            {
                _timesheetService.UpdateApproval(viewModel);
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { message = e.Message }, JsonRequestBehavior.AllowGet);
            }



            var strPages = "";

            if (viewModel.UserPermission == "HR")
            {
                strPages = "/sitePages/HRTimesheetView.aspx";
            }
            else if (viewModel.UserPermission == "Professional")
            {
                strPages = "/sitePages/ProfessionalTimesheetView.aspx";
            }
            else if (viewModel.UserPermission == "Approver")
            {
                strPages = "";

            }

            return JsonHelper.GenerateJsonSuccessResponse(siteUrl + strPages);
        }

        public JsonResult GridHolidays_Read([DataSourceRequest] DataSourceRequest request)
        {
            // Get from existing session variable or create new if doesn't exist
            if (SessionManager.Get<IEnumerable<TimesheetDetailVM>>("TimesheetDetails") == null) return null;
            var items = SessionManager.Get<IEnumerable<TimesheetDetailVM>>("TimesheetDetails").Where(e => e.Type != null && e.Type != "Working Days");

            // Convert to Kendo DataSource
            DataSourceResult result = items.ToDataSourceResult(request);

            // Convert to Json
            var json = Json(result, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
            // return null;
        }

        public JsonResult GridWorkingDays_Read([DataSourceRequest] DataSourceRequest request)
        {
            // Get from existing session variable or create new if doesn't exist
            if (SessionManager.Get<IEnumerable<TimesheetDetailVM>>("TimesheetDetails") == null) return null;
            var items = SessionManager.Get<IEnumerable<TimesheetDetailVM>>("TimesheetDetails").Where(e => e.Type != null && e.Type == "Working Days" && e.EditMode != -1);

            // Convert to Kendo DataSource
            DataSourceResult result = items.ToDataSourceResult(request);

            // Convert to Json
            var json = Json(result, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
            // return null;
        }

        public JsonResult GridWorkingDays_Read2([DataSourceRequest] DataSourceRequest request)
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