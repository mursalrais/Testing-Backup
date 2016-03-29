using System;
using System.Collections.Generic;
using System.Linq;
using MCAWebAndAPI.Model.ProjectManagement.Common;
using MCAWebAndAPI.Model.ViewModel.Chart;
using MCAWebAndAPI.Service.SPUtil;
using Microsoft.SharePoint.Client;

namespace MCAWebAndAPI.Service.ProjectManagement.Common
{
    public class ProjectHierarchyService : IProjectHierarchyService
    {
        string _siteUrl = null;

        const string SP_ACTIVITY_LIST_NAME = "Activity";
        const string SP_SUB_ACTIVITY_LIST_NAME = "Sub Activity";
        const string SP_PROJECT_INFORMATION_LIST_NAME = "Project Information";

        private Project ConvertToProjectModel(ListItem item)
        {
            var model = new Project();
            model.ProjectName = Convert.ToString(item["Title"]);
            model.Start = Convert.ToDateTime(item["Start"]);
            model.Finish = Convert.ToDateTime(item["Finish"]);
            model.Director = (FieldUserValue)item["Project_x0020_Manager"] == null ? "" :
                    Convert.ToString(((FieldUserValue)item["Project_x0020_Manager"]).LookupValue);
            model.ColorStatus = GenerateScheduleStatusColor(Convert.ToString(item["Schedule_x0020_Status"]));
            model.ScheduleStatus = Convert.ToString(item["Schedule_x0020_Status"]);

            return model;
        }

        private SubActivity ConvertToSubActivityModel(ListItem item)
        {
            var model = new SubActivity();
            model.SubActivityName = Convert.ToString(item["Title"]);
            model.ActivityName = item["Activity"] == null ? string.Empty : Convert.ToString((item["Activity"] as FieldLookupValue).LookupValue);
            model.ScheduleStatus = Convert.ToString(item["Schedule_x0020_Status"]);

            return model;
        }

        private Activity ConvertToActivityModel(ListItem item)
        {
            var model = new Activity();
            model.ActivityName = Convert.ToString(item["Title"]);
            model.Start = Convert.ToDateTime(item["Start"]);
            model.Finish = Convert.ToDateTime(item["Finish"]);
            model.Director = (FieldUserValue)item["Project_x0020_Director"] == null ? "" :
                    Convert.ToString(((FieldUserValue)item["Project_x0020_Director"]).LookupValue);
            model.ColorStatus = GenerateScheduleStatusColor(Convert.ToString(item["ScheduleStatus"]));
            model.ScheduleStatus = Convert.ToString(item["ScheduleStatus"]);

            return model;
        }

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public IEnumerable<Activity> GetAllActivities()
        {
            var items = new List<Activity>();

            foreach (var item in SPConnector.GetList(SP_ACTIVITY_LIST_NAME, _siteUrl))
            {
                items.Add(ConvertToActivityModel(item));
            }

            return items;
        }

        public IEnumerable<Project> GetAllProjects()
        {
            var items = new List<Project>();

            foreach (var item in SPConnector.GetList(SP_PROJECT_INFORMATION_LIST_NAME, _siteUrl + "/gp"))
            {
                items.Add(ConvertToProjectModel(item));
            }
            foreach (var item in SPConnector.GetList(SP_PROJECT_INFORMATION_LIST_NAME, _siteUrl + "/hn"))
            {
                items.Add(ConvertToProjectModel(item));
            }
            foreach (var item in SPConnector.GetList(SP_PROJECT_INFORMATION_LIST_NAME, _siteUrl + "/pm"))
            {
                items.Add(ConvertToProjectModel(item));
            }

            return items;
        }

        private string GenerateScheduleStatusColor(string scheduleStatus)
        {
            switch (scheduleStatus)
            {
                case "Significantly Behind Schedule": return "Red";
                case "Behind Schedule": return "Yellow";
                case "On Schedule": return "Green";
                case "Future": return "Blue";
                default: return "Black";
            }
        }


        private string GenerateOrderedScheduleStatus(string scheduleStatus)
        {
            switch (scheduleStatus)
            {
                case "Significantly Behind Schedule": return "1. Significantly Behind Schedule";
                case "Behind Schedule": return "2. Behind Schedule";
                case "On Schedule": return "3. On Schedule";
                case "Future": return "4. Future";
                default: return "Black";
            }
        }

        public IEnumerable<StackedBarChartVM> GenerateProjectHealthStatusChartByActivity()
        {
            var items = new List<SubActivity>();

            foreach(var item in SPConnector.GetList(SP_SUB_ACTIVITY_LIST_NAME, _siteUrl))
            {
                items.Add(ConvertToSubActivityModel(item));
            }


            return items.Select(e => new StackedBarChartVM()
            {
                CategoryName = e.ActivityName,
                GroupName = GenerateOrderedScheduleStatus(e.ScheduleStatus),
                Value = 1,
                Color = GenerateScheduleStatusColor(e.ScheduleStatus)
            });
        }


        public IEnumerable<StackedBarChartVM> GenerateProjectHealthStatusChartByProject()
        {
            var items = new List<Activity>();

            var programSiteUrl = _siteUrl;
            _siteUrl = programSiteUrl + "/gp";
            items.AddRange(GetAllActivities());
            _siteUrl = programSiteUrl + "/hn";
            items.AddRange(GetAllActivities());
            _siteUrl = programSiteUrl + "/pm";
            items.AddRange(GetAllActivities());
            _siteUrl = programSiteUrl;


            return items.Select(e => new StackedBarChartVM()
            {
                CategoryName = e.ActivityName,
                GroupName = GenerateOrderedScheduleStatus(e.ScheduleStatus),
                Value = 1,
                Color = GenerateScheduleStatusColor(e.ScheduleStatus)
            });
        }


    }
}
