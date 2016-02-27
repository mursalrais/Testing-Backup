using System.Collections.Generic;
using MCAWebAndAPI.Model.ProjectManagement.Schedule;
using System.Linq;
using MCAWebAndAPI.Model.ViewModel.Chart;

namespace MCAWebAndAPI.Service.ProjectManagement.Schedule
{
    public interface ITaskService
    {
        IEnumerable<Task> GetAllTask();

        IEnumerable<Task> GetAllTaskNotCompleted();

        IEnumerable<Task> GetMilestones();

        IEnumerable<ProjectStatusBarChartVM> GenerateProjectStatusBarChart();

        int CalculateTaskColumns();

    }
}
