using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private SubActivity ConvertToSubActivityModel(ListItem item)
        {
            var model = new SubActivity();
            model.SubActivityName = Convert.ToString(item["Title"]);
            model.ActivityName = item["Activity"] == null ? string.Empty : Convert.ToString((item["Activity"] as FieldLookupValue).LookupValue);
            model.ScheduleStatus = Convert.ToString(item["Schedule_x0020_Status"]);

            return model;
        }

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public IEnumerable<Activity> GetAllActivities()
        {
            var list = SPConnector.GetList("Activity");
            var result = new List<Activity>();

            foreach(var item in list)
            {
                result.Add(new Activity
                {
                    ActivityName = Convert.ToString(item["Title"])
                });
            }
            return result;
        }

        public IEnumerable<SubActivity> GetAllSubActivities()
        {
            var list = SPConnector.GetList("SubActivity");
            var result = new List<SubActivity>();

            foreach (var item in list)
            {
                result.Add(new SubActivity
                {
                    SubActivityName = Convert.ToString(item["Title"]),
                    ActivityName = Convert.ToString((item["Activity"] as FieldLookupValue).LookupValue)
                });
            }
            return result;
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

        public IEnumerable<StackedBarChartVM> GenerateProjectHealthStatusChartByActivity()
        {
            var items = new List<SubActivity>();

            foreach(var item in SPConnector.GetList(SP_SUB_ACTIVITY_LIST_NAME))
            {
                items.Add(ConvertToSubActivityModel(item));
            }

            return items.Select(e => new StackedBarChartVM
            {
                CategoryName = e.ActivityName,
                GroupName = e.ScheduleStatus,
                Value = 1,
                Color = GenerateScheduleStatusColor(e.ScheduleStatus)
            });
        }
    }
}
