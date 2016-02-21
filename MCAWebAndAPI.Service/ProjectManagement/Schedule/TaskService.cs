using System;
using System.Linq;
using System.Text;
using MCAWebAndAPI.Model.ProjectManagement.Schedule;
using MCAWebAndAPI.Service.SPUtil;
using System.Collections;
using System.Collections.Generic;
using MCAWebAndAPI.Model.ViewModel.Chart;
using Microsoft.SharePoint.Client;

namespace MCAWebAndAPI.Service.ProjectManagement.Schedule
{
    public class TaskService : ITaskService
    {
        Task ConvertToModel(Microsoft.SharePoint.Client.ListItem item)
        {
            return new Task
            {
                Title = Convert.ToString(item["Title"]),
                AssignedTo = Convert.ToString(item["AssignedTo"]),
                DueDate = Convert.ToDateTime(item["DueDate"]),
                StartDate = Convert.ToDateTime(item["StartDate"]),
                Percentage = Convert.ToInt32(item["PercentComplete"]),
                Activity = Convert.ToString( (item["Activity"] as FieldLookupValue).LookupValue ?? string.Empty),
                SubActivity = Convert.ToString( (item["Sub_x002d_Activity"] as FieldLookupValue).LookupValue ?? string.Empty)
            };
        }

         
        public bool CreateTask(Task task)
        {
            throw new NotImplementedException();
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
            List<Task> jsonList = new List<Task>();  
            
            foreach(var item in SPConnector.GetList("Tasks"))
            {
                jsonList.Add(ConvertToModel(item));
            }

            return jsonList;
        }

        public IEnumerable<Task> GetAllTaskNotCompleted()
        {
            var list = SPConnector.GetList("Tasks",
                "<View><Query><Where><And><Lt><FieldRef Name='PercentComplete' /><Value Type='Number'>100</Value></Lt><Neq><FieldRef Name='Status' /><Value Type='Choice'>Completed</Value></Neq></And></Where><OrderBy><FieldRef Name='StartDate' Ascending='True' /></OrderBy></Query></View>");

            var result = new List<Task>();
            foreach (var item in list)
            {
                result.Add(ConvertToModel(item));
            }

            return result;
        }

        public IEnumerable<Task> GetMilestones()
        {
            var list = SPConnector.GetList("Tasks",
                "<View><Query><Where><Eq><FieldRef Name='IsMilestone' /><Value Type='Boolean'>1</Value></Eq></Where></Query></View>");

            var result = new List<Task>();
            foreach (var item in list)
            {
                result.Add(ConvertToModel(item));
            }

            return result;
        }

        public IEnumerable<ProjectStatusBarChartVM> GenerateProjectStatusBarChart()
        {
            var list = SPConnector.GetList("Tasks");
            var result = new List<ProjectStatusBarChartVM>();

            foreach(var item in list)
            {
                result.Add(new ProjectStatusBarChartVM
                {
                    ProjectName = Convert.ToString(item["Title"]),
                    Value = CalculateProjectStatusWeight(item["StartDate"], item["DueDate"])
                });
            }

            return result;
        }

        private int CalculateProjectStatusWeight(object start, object finish)
        {
            var startDate = Convert.ToDateTime(start);
            var finishDate = Convert.ToDateTime(finish);

            var days = Convert.ToInt32(finishDate.Subtract(startDate).TotalDays);

            return days <= 0 ? 0 : days;
        }
    }
}
