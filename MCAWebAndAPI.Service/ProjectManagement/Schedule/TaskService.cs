using System;
using System.Linq;
using System.Text;
using MCAWebAndAPI.Model.ProjectManagement.Schedule;
using MCAWebAndAPI.Service.SPUtil;
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

        private Task ConverToModel(Microsoft.SharePoint.Client.ListItem item)
        {
            return new Task
            {
                Title = Convert.ToString(item["Title"]),
                AssignedTo = new Model.ProjectManagement.Common.SPUser { Name = Convert.ToString(item["AssignedTo"]) },
                DueDate = Convert.ToDateTime(item["DueDate"]),
                StartDate = Convert.ToDateTime(item["StartDate"]),
                Percentage = Convert.ToInt32(item["PercentComplete"])
            };
        }

        public Task Get(string title)
        {
            var tasks = SPConnector.GetList("Tasks");
            var task = tasks.First(e => title.Equals(Convert.ToString(e["Title"])));
            return new Task
            {
                Title = task["Title"].ToString()
            };
                
        }

        public IEnumerable<Task> GetAllTask()
        {
            var list = SPConnector.GetList("Tasks");
            var result = new List<Task>();
            foreach (Microsoft.SharePoint.Client.ListItem item in list)
            {
                result.Add(ConverToModel(item));
            }

            return result;
        }

        public IEnumerable<Task> GetAllTaskNotCompleted()
        {
            var list = SPConnector.GetList("Tasks");
            var result = new List<Task>();
            foreach (Microsoft.SharePoint.Client.ListItem item in list)
            {
                if(Convert.ToInt32(item["PercentComplete"]) < 100)
                {
                    result.Add(ConverToModel(item));
                } 
            }

            return result;
        }

        public IEnumerable<Task> GetMilestones()
        {
            var list = SPConnector.GetList("Tasks");
            var result = new List<Task>();
            foreach (Microsoft.SharePoint.Client.ListItem item in list)
            {
                if (Convert.ToString(item["IsMilestone"]).Equals("Yes"))
                {
                    result.Add(ConverToModel(item));
                }
            }

            return result;
        }
    }
}
