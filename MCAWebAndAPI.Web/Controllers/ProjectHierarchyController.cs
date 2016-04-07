using MCAWebAndAPI.Service.ProjectManagement.Common;
using MCAWebAndAPI.Web.Helpers;
using System.Web.Mvc;

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
    }
}