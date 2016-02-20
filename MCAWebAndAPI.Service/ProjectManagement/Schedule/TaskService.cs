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

        public Task Get(string title)
        {
            var tasks = Connector.SPConnector.GetList("Tasks");
            var task = tasks.First(e => e["Title"].Equals(title));
            return new Task
            {
                Title = task["Title"].ToString()
            };
                
        }

        public IEnumerable<Task> GetAllTask()
        {
            var list = Connector.SPConnector.GetList("Tasks");
            return list.Select(e => new Task
            {
                Title = e["Title"].ToString()
            });
        }

        public IEnumerable<Task> GetAllTaskNotCompleted()
        {
            throw new NotImplementedException();
        }
    }
}
