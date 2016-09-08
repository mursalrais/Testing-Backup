using MCAWebAndAPI.Service.ProjectManagement.Common;
using MCAWebAndAPI.Web.Helpers;
using System.Web.Mvc;

using System.Linq;
using System.Collections.Generic;

using MCAWebAndAPI.Web.Resources;
using MCAWebAndAPI.Model.ProjectManagement.Common;

namespace MCAWebAndAPI.Web.Controllers
{
    public class ProjectHierarchyController : Controller
    {
        IProjectHierarchyService _service;

        public ProjectHierarchyController()
        {
            _service = new ProjectHierarchyService();
        }
        
        public ActionResult ProjectStatusByProject()
        {
            return View();
        }

        public ActionResult ProjectStatusByActivity()
        {
            return View();
        }

        public ActionResult AllWBS()
        {
            return View();
        }

        public ActionResult GetProjectHealthStatusChartByActivity(string siteUrl = null)
        {
            _service.SetSiteUrl(siteUrl);
            var data = _service.GenerateProjectHealthStatusChartByActivity();
            return this.Jsonp(data);
        }

        public ActionResult GetProjectHealthStatusChartByProject(string siteUrl = null)
        {
            _service.SetSiteUrl(siteUrl);
            var data = _service.GenerateProjectHealthStatusChartByProject();
            return this.Jsonp(data);
        }

        public ActionResult GetActivities(string siteUrl = null)
        {
            _service.SetSiteUrl(siteUrl);
            var data = _service.GetAllActivities();
            return this.Jsonp(data);
        }

        public ActionResult GetProjects(string siteUrl = null)
        {
            _service.SetSiteUrl(siteUrl);
            var data = _service.GetAllProjects();
            return this.Jsonp(data);
        }

        public ActionResult GetWBSMappings(string siteUrl = null)
        {
            _service.SetSiteUrl(siteUrl);
            var data = _service.GetAllWBSMappings();
            return this.Jsonp(data);
        }

        public ActionResult UpdateWBSMapping(string siteUrl = null)
        {
            _service.SetSiteUrl(siteUrl);
            var data = _service.UpdateWBSMapping();
            return this.Jsonp(data);
        }

        public JsonResult GetWBS()
        {
            _service.SetSiteUrl(ConfigResource.DefaultProgramSiteUrl);
            var data = GetWBSMappingFromExistingSession();

            return Json(data.Select(e =>
                new
                {
                    e.WBSID, 
                    e.WBSDescription, 
                    e.Activity, 
                    e.SubActivity, 
                    e.Project
                }
            ), JsonRequestBehavior.AllowGet);
        }


        private IEnumerable<WBSMapping> GetWBSMappingFromExistingSession()
        {
            //Get existing session variable
            var sessionVariable = System.Web.HttpContext.Current.Session["WBSMapping"] as IEnumerable<WBSMapping>;
            var wbsMapping = sessionVariable ?? _service.GetWBSMappingsInProgram();

            if (sessionVariable == null) // If no session variable is found
                System.Web.HttpContext.Current.Session["WBSMapping"] = wbsMapping;
            return wbsMapping;
        }
    }
}