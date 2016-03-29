using System.Collections.Generic;
using MCAWebAndAPI.Model.ProjectManagement.Schedule;
using System.Linq;
using MCAWebAndAPI.Model.ViewModel.Chart;
using MCAWebAndAPI.Model.ViewModel.Gantt;

namespace MCAWebAndAPI.Service.ProjectManagement.Schedule
{
    public interface ITaskService
    {
        void SetSiteUrl(string siteUrl);

        IEnumerable<Task> GetAllTask();

        IEnumerable<Task> GetAllTaskNotCompleted();

        IEnumerable<Task> GetMilestones();

        IEnumerable<ProjectStatusBarChartVM> GenerateProjectStatusBarChart();

        int CalculateTaskColumns();

        void UpdateTodayValue();

        IEnumerable<ProjectScheduleSCurveVM> GenerateProjectScheduleSCurveChart();

        IEnumerable<GanttTasksVM> GenerateGanttChart();

        IEnumerable<GanttDependenciesVM> GenerateGanttDependecies();


        IEnumerable<StackedBarChartVM> GenerateProjectHealthStatusByActivityChart();

    }
}
