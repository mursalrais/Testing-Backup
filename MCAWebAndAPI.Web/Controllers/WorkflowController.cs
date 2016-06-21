using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.ViewModel.Form.Common;
using MCAWebAndAPI.Service.Common;
using MCAWebAndAPI.Web.Helpers;
using MCAWebAndAPI.Web.Resources;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

        public JsonResult GetApproverPositions(string listName, string requestorPosition,
            string requestorUnit)
        {
            _service.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);

            var viewModel = _service.GetPositionsInWorkflow(listName, requestorPosition, requestorUnit);
            return Json(viewModel.Select(e => new {
                e.ID,
                e.PositionName
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetApproverNames(string position)
        {
            _service.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);

            var viewModel = _service.GetApproverNames(position);
            return Json(viewModel.Select(e => new
            {
                e.ID, 
                e.Name, 
                e.UserLogin
            }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult DisplayWorkflowRouter(string listName, string requestor)
        {
            _service.SetSiteUrl(ConfigResource.DefaultHRSiteUrl);
            var viewModel = _service.GetWorkflowRouter(listName, requestor);
            SessionManager.Set("WorkflowItems", viewModel.WorkflowItems);
            return PartialView("_WorkflowDetails", viewModel);
        }

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


        [AcceptVerbs(HttpVerbs.Post)]
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