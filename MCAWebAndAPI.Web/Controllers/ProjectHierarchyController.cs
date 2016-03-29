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

        // GET: ProjectHierarchy
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetProjectHealthStatusChartByActivity(string siteUrl = null)
        {
            _service.SetSiteUrl(siteUrl);

            var data = _service.GenerateProjectHealthStatusChartByActivity();

            return this.Jsonp(data);
        }
    }
}