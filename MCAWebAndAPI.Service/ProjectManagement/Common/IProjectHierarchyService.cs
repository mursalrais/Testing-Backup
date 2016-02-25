using MCAWebAndAPI.Model.ProjectManagement.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.ProjectManagement.Common
{
    public interface IProjectHierarchyService
    {
        IEnumerable<SubActivity> GetAllSubActivities();

        IEnumerable<Activity> GetAllActivities();


    }
}
