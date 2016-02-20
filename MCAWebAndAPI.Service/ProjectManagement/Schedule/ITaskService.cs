using System.Collections.Generic;
using MCAWebAndAPI.Model.ProjectManagement.Schedule;
using System.Linq;

namespace MCAWebAndAPI.Service.ProjectManagement.Schedule
{
    public interface ITaskService
    {
        bool CreateTask(Task task);

        IEnumerable<Task> GetAllTask();

        Task Get(string title);

        IEnumerable<Task> GetAllTaskNotCompleted();
        IEnumerable<Task> GetMilestones();
    }
}
