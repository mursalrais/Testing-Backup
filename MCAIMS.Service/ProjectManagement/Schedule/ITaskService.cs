using System.Collections.Generic;
using MCAIMS.Model.ProjectManagement.Schedule;

namespace MCAIMS.Service.ProjectManagement.Schedule
{
    public interface ITaskService
    {
        bool CreateTask(Task task);

        IEnumerable<Task> GetAllTask();

        IEnumerable<Task> GetAllTaskNotCompleted();

    }
}
