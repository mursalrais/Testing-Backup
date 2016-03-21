using System.Collections.Generic;
using MCAWebAndAPI.Model.ProjectManagement.Schedule;
using MCAWebAndAPI.Model.ViewModel.Chart;

namespace MCAWebAndAPI.Service.ProjectManagement.Schedule
{
    public interface IRIAService
    {
        void SetSiteUrl(string siteUrl);

        IEnumerable<Risk> GetAllRisks();
        IEnumerable<Issue> GetAllIssues();
        IEnumerable<Action> GetAllAction();

        IEnumerable<StackedBarChartVM> GetOverallRIAChart();
        IEnumerable<StackedBarChartVM> GetRIAResourceChart(string riaType);
        IEnumerable<DonutsChartVM> GetRIAStatusChart(string riaType);
        IEnumerable<DonutsChartVM> GetRIAPriorityChart(string riaType);
    }
}
