using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using MCAWebAndAPI.Service.HR.Timesheet;
using MCAWebAndAPI.Web.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Service.Resources;
using System.Threading.Tasks;
using Elmah;
using MCAWebAndAPI.Model.ViewModel.Form.Common;
using MCAWebAndAPI.Service.JobSchedulers.Schedulers;

//using

namespace MCAWebAndAPI.Web.Controllers
{
    public class HRTimesheetController : Controller
    {
        ITimesheetService _timesheetService;
        //const string SP_TRANSACTION_WORKFLOW_LIST_NAME = "Timesheet Workflow";
        //const string SP_TRANSACTION_WORKFLOW_LOOKUP_COLUMN_NAME = "timesheet";
        public HRTimesheetController()
        {
            _timesheetService = new TimesheetService();
        }

        public async Task<ActionResult> CreateTimesheet(string siteUrl, string userlogin)
        {
            SessionManager.Set("SiteUrl", siteUrl);
            _timesheetService.SetSiteUrl(siteUrl);

          
            var viewModel = await _timesheetService.GetTimesheetAsync(userlogin, DateTime.Now.ToLocalTime());

            SessionManager.Set("TimesheetDetails", viewModel.TimesheetDetails);
            SessionManager.Set("WorkflowItems", viewModel.WorkflowItems);
            return View(viewModel);
        }

        public async Task<ActionResult> EditTimesheet(string siteUrl, int? id , string userlogin)
        {
            SessionManager.Set("SiteUrl", siteUrl);
            _timesheetService.SetSiteUrl(siteUrl);
            var viewModel = await _timesheetService.GetTimesheetLoadUpdate(id,userlogin);

            if (viewModel.UserPermission== "Not Authorized") return RedirectToAction("NotAuthorized", "HRTimesheet");

            SessionManager.Set("TimesheetDetails", viewModel.TimesheetDetails);
            SessionManager.Set("WorkflowItems", viewModel.WorkflowItems);
            return View(viewModel);
        }

        public ActionResult NotAuthorized(string siteUrl = null)
        {

            return View("NotAuthorized", null);

        }

        [HttpPost]
        public async Task<ActionResult> UpdatePeriod(TimesheetVM viewModel)
        {
            if (viewModel.ProfesionalUserLogin == null) return null;
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _timesheetService.SetSiteUrl(siteUrl);

            try
            {
                viewModel = await _timesheetService.GetTimesheetAsync(viewModel.ProfesionalUserLogin, ((DateTime)(viewModel.Period)));
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new { message = e.Message }, JsonRequestBehavior.AllowGet);
            }

            SessionManager.Set("TimesheetDetails", viewModel.TimesheetDetails);
            SessionManager.Set("WorkflowItems", viewModel.WorkflowItems);
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
        public async Task<ActionResult> SubmitTimesheet(FormCollection form, TimesheetVM viewModel)
        {
            try
            {
                if (viewModel.ProfesionalUserLogin == null) return null;
                var siteUrl = SessionManager.Get<string>("SiteUrl");
                _timesheetService.SetSiteUrl(siteUrl);
                int? headerId;


                if (!viewModel.WorkflowItems.Any())
                {
                    Response.TrySkipIisCustomErrors = true;
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return JsonHelper.GenerateJsonErrorResponse("Please check Workflow Mapping Master");
                }

                var items = SessionManager.Get<IEnumerable<TimesheetDetailVM>>("TimesheetDetails");

                if (items== null || !items.Any())
                {
                    Response.TrySkipIisCustomErrors = true;
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return JsonHelper.GenerateJsonErrorResponse("Please add working days");
                }

                var query = items.Where(e => e.Type != null && e.Type == "Working Days" && e.EditMode != -1);
                var result = query.Sum(x => x.FullHalf);
                if (result <= 0.5)
                {
                    Response.TrySkipIisCustomErrors = true;
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return JsonHelper.GenerateJsonErrorResponse("Working days should be more than 0.5 day");
                }
                headerId = _timesheetService.CreateHeader(viewModel);

                viewModel.TimesheetDetails = items;
                Task createTimesheetDetailsTask = _timesheetService.CreateTimesheetDetailsAsync(headerId, viewModel.TimesheetDetails);


                Task createTimesheetWorkflowTask = _timesheetService.CreateWorkflowTimesheetAsync(headerId, viewModel.WorkflowItems, viewModel.TimesheetStatus);

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

                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e.Message);
            }


        }

        //IEnumerable<WorkflowItemVM> BindWorkflowDetails(FormCollection form,
        //  IEnumerable<WorkflowItemVM> workflowItem)
        //{
        //    var array = workflowItem.ToArray();

        //    for (int i = 0; i < array.Length; i++)
        //    {
        //        array[i].Level = BindHelper.BindStringInGrid("WorkflowItems",
        //            i, "Level", form);
        //    }
        //    return array;
        //}

        [HttpPost]
        public async Task<ActionResult> UpdateTimesheet(FormCollection form, TimesheetVM viewModel)
        {
            try
            {
                if (viewModel.ProfesionalUserLogin == null) return null;
                var siteUrl = SessionManager.Get<string>("SiteUrl");
                _timesheetService.SetSiteUrl(siteUrl);



                var items = SessionManager.Get<IEnumerable<TimesheetDetailVM>>("TimesheetDetails");

                if (items == null || !items.Any())
                {
                    Response.TrySkipIisCustomErrors = true;
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return JsonHelper.GenerateJsonErrorResponse("Please add working days");
                }

                var query = items.Where(e => e.Type != null && e.Type == "Working Days" && e.EditMode != -1);
                var result = query.Sum(x => x.FullHalf);
                if (result <= 0.5)
                {
                    Response.TrySkipIisCustomErrors = true;
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return JsonHelper.GenerateJsonErrorResponse("Working days should be more than 0.5 day");
                }

                _timesheetService.UpdateHeader(viewModel);
                viewModel.TimesheetDetails = items;


                Task createTimesheetDetailsTask = _timesheetService.CreateTimesheetDetailsAsync(viewModel.ID, viewModel.TimesheetDetails);


                Task createTimesheetWorkflowTask = _timesheetService.CreateWorkflowTimesheetAsync(viewModel.ID, viewModel.WorkflowItems, viewModel.TimesheetStatus);

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

                Response.TrySkipIisCustomErrors = true;
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return JsonHelper.GenerateJsonErrorResponse(e.Message);
            }


        }

        [HttpPost]
        public ActionResult UpdateApproval(FormCollection form, TimesheetVM viewModel)
        {
             if (viewModel.ProfesionalUserLogin == null) return null;
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

        public JsonResult GridWorkflow_Read([DataSourceRequest] DataSourceRequest request)
        {
            // Get from existing session variable or create new if doesn't exist
            if (SessionManager.Get<IEnumerable<WorkflowItemVM>>("WorkflowItems") == null) return null;
            var items = SessionManager.Get<IEnumerable<WorkflowItemVM>>("WorkflowItems");

            // Convert to Kendo DataSource
            DataSourceResult result = items.ToDataSourceResult(request);

            // Convert to Json
            var json = Json(result, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
            // return null;
        }
        public JsonResult GridDayOff_Read([DataSourceRequest] DataSourceRequest request)
        {
            // Get from existing session variable or create new if doesn't exist
            if (SessionManager.Get<IEnumerable<TimesheetDetailVM>>("TimesheetDetails") == null) return null;
            var items = SessionManager.Get<IEnumerable<TimesheetDetailVM>>("TimesheetDetails").Where(e => e.Type != null
            && e.Type == "Day-Off");

            // Convert to Kendo DataSource
            DataSourceResult result = items.ToDataSourceResult(request);

            // Convert to Json
            var json = Json(result, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
            // return null;
        }

        public JsonResult GridDayCompen_Read([DataSourceRequest] DataSourceRequest request)
        {
            // Get from existing session variable or create new if doesn't exist
            if (SessionManager.Get<IEnumerable<TimesheetDetailVM>>("TimesheetDetails") == null) return null;
            var items = SessionManager.Get<IEnumerable<TimesheetDetailVM>>("TimesheetDetails").Where(e => e.Type != null &&
            e.Type == "Compensatory Time");

            // Convert to Kendo DataSource
            DataSourceResult result = items.ToDataSourceResult(request);

            // Convert to Json
            var json = Json(result, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
            // return null;
        }

        public JsonResult GridHolidays_Read([DataSourceRequest] DataSourceRequest request)
        {
            // Get from existing session variable or create new if doesn't exist
            if (SessionManager.Get<IEnumerable<TimesheetDetailVM>>("TimesheetDetails") == null) return null;
            var items = SessionManager.Get<IEnumerable<TimesheetDetailVM>>("TimesheetDetails").Where(e => e.Type != null 
            && (e.Type == "Public Holiday" || e.Type == "Holiday"));

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

        public ActionResult ReadProfessional([DataSourceRequest] DataSourceRequest request, string useremail = null)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _timesheetService.SetSiteUrl(siteUrl);
          //  DataTable data = _service.getViewProfessionalClaim(useremail);
         //   return Json(data.ToDataSourceResult(request));
            return null;
        }

        public JsonResult DeleteTimesheetId(int id)
        {
            var siteUrl = SessionManager.Get<string>("SiteUrl");
            _timesheetService.SetSiteUrl(siteUrl);
           // _timesheetService.DeleteClaim(id);


            return null;
        }

        public ActionResult ViewProfessional(string siteUrl = null, string useremail = null)
        {
            SessionManager.Set("SiteUrl", siteUrl);
            _timesheetService.SetSiteUrl(siteUrl);
            var viewModel = _timesheetService.GetTimesheetProfessional(useremail);

            return View(viewModel);

        }

        public ActionResult TimesheetSchedulerEmail(string siteUrl = null)
        {

            try
            {
              

                TimesheetManagementScheduler.DoNowPSAExpired_OnceEveryDay(siteUrl);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error");
            }
            return RedirectToAction("Index", "Success");
        }

    }
}