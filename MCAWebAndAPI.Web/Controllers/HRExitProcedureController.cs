using MCAWebAndAPI.Model.ViewModel.Form.HR;
using System.Web.Mvc;
using System;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using MCAWebAndAPI.Service.HR.Recruitment;
using Elmah;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using MCAWebAndAPI.Model.HR.DataMaster;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.ViewModel.Form.Common;
using MCAWebAndAPI.Service.Common;
using System.Collections.Generic;
using System.Data;


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
        public ActionResult CreateExitProcedure(string siteUrl = null, string requestor = null)
        {  
            // MANDATORY: Set Site URL
            exitProcedureService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            //ViewBag.ListName = "Exit%20Procedure";

            ViewBag.ListName = "Exit Procedure";

            var listName = ViewBag.ListName;

            ViewBag.RequestorUserLogin = requestor;
            
            // Get blank ViewModel
            var viewModel = exitProcedureService.GetExitProcedure(null, siteUrl, requestor, listName);

            return View("CreateExitProcedure", viewModel);
        }

        //public async Task<ActionResult> DisplayWorkflowRouterExitProcedure(string listName, string requestor, bool isPartial = true)
        //{
        //    exitProcedureService.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);
        //    var viewModel = await exitProcedureService.GetWorkflowRouterExitProcedure(listName, requestor);
        //    //var viewModel = await _service.GetWorkflowRouterRequestorPosition(listName, requestorPosition);
        //    SessionManager.Set("ExitProcedureChecklist", viewModel.ExitProcedureChecklist);
        //    SessionManager.Set("ExitProcedureListName", viewModel.ListName);
        //    SessionManager.Set("ExitProcedureRequestorUnit", viewModel.RequestorUnit);
        //    SessionManager.Set("ExitProcedureRequestorPosition", viewModel.RequestorPosition);

        //    if (isPartial)
        //        return PartialView("_ExitProcedureChecklist", viewModel);
        //    return View("_ExitProcedureChecklist", viewModel);

        //}

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

            try
            {
                exitProcedureService.CreateExitProcedureDocuments(exitProcID, viewModel.Documents, viewModel);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index", "Error");
            }

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


            if(viewModel.ID != null)
            {
                return View("EditExitProcedure", viewModel);
            }
            else
            {
                return RedirectToAction("Index",
                "Error",
                new { errorMessage = string.Format(MessageResource.ErrorEditExitProcedure) });
            }
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

        public ActionResult ViewExitProcedure(string siteUrl = null, int? ID = null)
        {
            // Clear Existing Session Variables if any

            // MANDATORY: Set Site URL
            exitProcedureService.SetSiteUrl(siteUrl ?? ConfigResource.DefaultHRSiteUrl);
            SessionManager.Set("SiteUrl", siteUrl ?? ConfigResource.DefaultHRSiteUrl);

            var viewModel = exitProcedureService.ViewExitProcedure(ID);
            return View("DisplayExitProcedure", viewModel);
        }

        

        public JsonResult GetApproverPositions(int approverUnit)
        {
            exitProcedureService.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);
            var listName = SessionManager.Get<string>("ExitProcedureListName");
            var requestorPosition = SessionManager.Get<string>("ExitProcedureRequestorPosition");
            var requestorUnitName = SessionManager.Get<string>("ExitProcedureRequestorUnit");
            var approverUnitName = ExitProcedureChecklistVM.GetUnitOptions().FirstOrDefault(e => e.Value == approverUnit).Text;

            var viewModel = exitProcedureService.GetPositionsInWorkflow(listName, approverUnitName,
                requestorUnitName, requestorPosition);
            return Json(viewModel.Select(e => new {
                e.ID,
                e.PositionName
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetApproverNames(int position)
        {
            exitProcedureService.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);
            var positionName = exitProcedureService.GetPositionName(position);
            var viewModel = SessionManager.Get<IEnumerable<ProfessionalMaster>>("WorkflowApprovers", "Position" + position)
                ?? exitProcedureService.GetApproverNames(positionName);
            SessionManager.Set("WorkflowApprovers", "Position" + position, viewModel);

            return Json(viewModel.Select(e => new
            {
                e.ID,
                e.Name
            }), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Grid_Read([DataSourceRequest] DataSourceRequest request)
        {
            // Get from existing session variable or create new if doesn't exist
            var exitProcedureChecklist = SessionManager.Get<IEnumerable<ExitProcedureChecklistVM>>("ExitProcedureChecklist");

            // Convert to Kendo DataSource
            DataSourceResult result = exitProcedureChecklist.ToDataSourceResult(request);

            // Convert to Json
            var json = Json(result, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }

        [HttpPost]
        public ActionResult Grid_Update([DataSourceRequest] DataSourceRequest request,
            [Bind(Prefix = "models")]IEnumerable<ExitProcedureChecklistVM> viewModel)
        {
            // Get existing session variable
            var sessionVariables = SessionManager.Get<IEnumerable<ExitProcedureChecklistVM>>("ExitProcedureChecklist");
            
            /*
            foreach (var item in viewModel)
            {
                var obj = sessionVariables.FirstOrDefault(e => e.ID != item.ID);
                obj = item;
            }
            */

            //DataSourceResult result = sessionVariables.ToDataSourceResult(request);

            // Overwrite existing session variable
            SessionManager.Set("ExitProcedureChecklist", sessionVariables);

            // Return JSON
            DataSourceResult result = sessionVariables.ToDataSourceResult(request);
            var json = Json(result, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }

        [HttpPost]
        public JsonResult Grid_Create([DataSourceRequest] DataSourceRequest request,
            [Bind(Prefix = "models")]IEnumerable<ExitProcedureChecklistVM> viewModel)
        {
            var sessionVariables = SessionManager.Get<IEnumerable<ExitProcedureChecklistVM>>("ExitProcedureChecklist");

            SessionManager.Set("ExitProcedureChecklist", sessionVariables);

            DataSourceResult result = sessionVariables.ToDataSourceResult(request);

            var json = Json(result, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }


        //[HttpPost]
        //public ActionResult Grid_Destroy([DataSourceRequest] DataSourceRequest request,
        //    [Bind(Prefix = "models")] ExitProcedureChecklistVM viewModel)
        //{
        //    if (viewModel != null)
        //    {
        //        exitProcedureService.DestroyExitProcedureChecklist(viewModel);
        //    }

        //    return Json(new[] { viewModel }.ToDataSourceResult(request, ModelState));
        //}

    }
}