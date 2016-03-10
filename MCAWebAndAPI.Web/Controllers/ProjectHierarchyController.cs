using MCAWebAndAPI.Model.ProjectManagement.Common;
using MCAWebAndAPI.Service.ProjectManagement.Common;
using System.Collections.Generic;
using System.Web.Http;

namespace MCAWebAndAPI.Web.Controllers
{
    public class ProjectHierarchyController : ApiController
    {
        IProjectHierarchyService projectHierarchyService;

        public ProjectHierarchyController()
        {
            projectHierarchyService = new ProjectHierarchyService();
        }

        public IEnumerable<Activity> GetActivities()
        {
            return projectHierarchyService.GetAllActivities();
        }

        public IEnumerable<SubActivity> GetSubActivities()
        {
            return projectHierarchyService.GetAllSubActivities();
        }
    }
}
