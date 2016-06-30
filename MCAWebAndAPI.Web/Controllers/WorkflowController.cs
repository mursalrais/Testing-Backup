using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Model.ViewModel.Form.Common;
using MCAWebAndAPI.Service.Common;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MCAWebAndAPI.Web.Controllers
{
    public class WorkflowController : Controller
    {
        readonly IWorkflowService _service;
        public WorkflowController()
        {
            _service = new WorkflowService();
        }

        public ActionResult DisplayMyApprovalRequest(string siteUrl = null, string userLogin = null)
        {
            _service.SetSiteUrl(siteUrl);

            return View();
        }

        public JsonResult GetApproverPositions(int approverUnit)
        {
            _service.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);
            var listName = SessionManager.Get<string>("WorkflowRouterListName");
            var requestorPosition = SessionManager.Get<string>("WorkflowRouterRequestorPosition");
            var requestorUnitName = SessionManager.Get<string>("WorkflowRouterRequestorUnit");
            var approverUnitName = WorkflowItemVM.GetUnitOptions().FirstOrDefault(e => e.Value == approverUnit).Text;

            var viewModel = _service.GetPositionsInWorkflow(listName, approverUnitName, 
                requestorUnitName, requestorPosition);
            return Json(viewModel.Select(e => new {
                e.ID,
                e.PositionName
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetApproverNames(int position)
        {
            _service.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);
            var positionName = _service.GetPositionName(position);
            var viewModel = SessionManager.Get<IEnumerable<ProfessionalMaster>>("WorkflowApprovers", "Position" + position)
                ?? _service.GetApproverNames(positionName);
            SessionManager.Set("WorkflowApprovers", "Position"+ position, viewModel);

            return Json(viewModel.Select(e => new
            {
                e.ID, 
                e.Name
            }), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> DisplayWorkflowRouter(string listName, string requestor, bool isPartial = true)
        {
            _service.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);
            var viewModel = await _service.GetWorkflowRouter(listName, requestor);
            SessionManager.Set("WorkflowItems", viewModel.WorkflowItems);
            SessionManager.Set("WorkflowRouterListName", viewModel.ListName);
            SessionManager.Set("WorkflowRouterRequestorUnit", viewModel.RequestorUnit);
            SessionManager.Set("WorkflowRouterRequestorPosition", viewModel.RequestorPosition);

            if (isPartial)
                return PartialView("_WorkflowDetails", viewModel);
            return View("_WorkflowDetails", viewModel);
        }

        
        public async Task<ActionResult> DisplayWorkflowRouterExitProcedure(string listName, string requestor, bool isPartial = true)
        {
            _service.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);
            var viewModel = await _service.GetWorkflowRouter(listName, requestor);
            //var viewModel = await _service.GetWorkflowRouterRequestorPosition(listName, requestorPosition);
            SessionManager.Set("WorkflowItems", viewModel.WorkflowItems);
            SessionManager.Set("WorkflowRouterListName", viewModel.ListName);
            SessionManager.Set("WorkflowRouterRequestorUnit", viewModel.RequestorUnit);
            SessionManager.Set("WorkflowRouterRequestorPosition", viewModel.RequestorPosition);
            
            
            if (isPartial)
                return PartialView("_WorkflowDetails", viewModel);
            return View("_WorkflowDetails", viewModel);
            
        }
        

        [HttpPost]
        public JsonResult Grid_Read([DataSourceRequest] DataSourceRequest request)
        {
            // Get from existing session variable or create new if doesn't exist
            var workflowItems = SessionManager.Get<IEnumerable<WorkflowItemVM>>("WorkflowItems");

            // Convert to Kendo DataSource
            DataSourceResult result = workflowItems.ToDataSourceResult(request);

            // Convert to Json
            var json = Json(result, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }
        
        [HttpPost]
        public ActionResult Grid_Update([DataSourceRequest] DataSourceRequest request,
            [Bind(Prefix = "models")]IEnumerable<WorkflowItemVM> viewModel)
        {
            // Get existing session variable
            var sessionVariables = SessionManager.Get<IEnumerable<WorkflowItemVM>>("WorkflowItems");

            foreach (var item in viewModel)
            {
                var obj = sessionVariables.FirstOrDefault(e => e.ID == item.ID);
                obj = item;
            }

            // Overwrite existing session variable
            SessionManager.Set("WorkflowItems", sessionVariables);

            // Return JSON
            DataSourceResult result = sessionVariables.ToDataSourceResult(request);
            var json = Json(result, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }
    }
}