using MCAWebAndAPI.Model.ProjectManagement.Common;
using MCAWebAndAPI.Model.ViewModel.Chart;
using MCAWebAndAPI.Model.ViewModel.Control;
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

        IEnumerable<Project> GetAllProjects();

        IEnumerable<Activity> GetAllActivities();

        IEnumerable<StackedBarChartVM> GenerateProjectHealthStatusChartByActivity();

        IEnumerable<StackedBarChartVM> GenerateProjectHealthStatusChartByProject();
        IEnumerable<WBSMapping> GetAllWBSMappings();

        IEnumerable<WBSMapping> GetWBSMappingsInProgram();

        IEnumerable<InGridComboBoxVM> GetProjectComboBox();

        bool UpdateWBSMapping();
    }
}
