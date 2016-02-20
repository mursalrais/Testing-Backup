using System.Collections.Generic;
using MCAWebAndAPI.Model.ProjectManagement.Schedule;

namespace MCAWebAndAPI.Service.ProjectManagement.Schedule
{
    public interface ITaskService
    {
        bool CreateTask(Task task);

        IEnumerable<Task> GetAllTask();

        Task Get(string title);

        IEnumerable<Task> GetAllTaskNotCompleted();

    }
}
