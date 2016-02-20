using System;
using System.Linq;
using System.Text;
using MCAWebAndAPI.Model.ProjectManagement.Schedule;
using System.Collections;
using System.Collections.Generic;

namespace MCAWebAndAPI.Service.ProjectManagement.Schedule
{
    public class TaskService : ITaskService
    { 
        public bool CreateTask(Task task)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Task> GetAllTask()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Task> GetAllTaskNotCompleted()
        {
            throw new NotImplementedException();
        }
    }
}
