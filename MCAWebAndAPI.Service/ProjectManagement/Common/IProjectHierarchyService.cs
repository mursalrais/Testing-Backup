using MCAWebAndAPI.Model.ProjectManagement.Common;
using MCAWebAndAPI.Model.ViewModel.Chart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.ProjectManagement.Common
{
    public interface IProjectHierarchyService
    {
        void SetSiteUrl(string siteUrl);

        IEnumerable<SubActivity> GetAllSubActivities();

        IEnumerable<Activity> GetAllActivities();

        IEnumerable<StackedBarChartVM> GenerateProjectHealthStatusChartByActivity();





    }
}
