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

        private Activity ConvertToActivityModel(ListItem item)
        {
            var model = new Activity();
            model.ActivityName = Convert.ToString(item["Title"]);
            model.Start = Convert.ToDateTime(item["Start"]);
            model.Finish = Convert.ToDateTime(item["Finish"]);
            model.Director = (FieldUserValue)item["Project_x0020_Director"] == null ? "" :
                    Convert.ToString(((FieldUserValue)item["Project_x0020_Director"]).LookupValue);
            model.ColorStatus = GenerateScheduleStatusColor(Convert.ToString(item["ScheduleStatus"]));

            return model;
        }

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public IEnumerable<Activity> GetAllActivities()
        {
            var items = new List<Activity>();

            foreach (var item in SPConnector.GetList(SP_ACTIVITY_LIST_NAME))
            {
                items.Add(ConvertToActivityModel(item));
            }

            return items;
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


            var viewModels = new List<StackedBarChartVM>();

            viewModels.AddRange(ConvertToOrderedSubActivitiesVM(items, "Significantly Behind Schedule"));
            viewModels.AddRange(ConvertToOrderedSubActivitiesVM(items, "Behind Schedule"));
            viewModels.AddRange(ConvertToOrderedSubActivitiesVM(items, "On Schedule"));
            viewModels.AddRange(ConvertToOrderedSubActivitiesVM(items, "Future"));

            return viewModels;
        }

        private IEnumerable<StackedBarChartVM> ConvertToOrderedSubActivitiesVM(List<SubActivity> items, string groupName)
        {
            return items.Where(e => string.Compare(e.ScheduleStatus, groupName, StringComparison.OrdinalIgnoreCase) == 0)
                .Select(f => new StackedBarChartVM()
                {
                    CategoryName = f.ActivityName,
                    GroupName = f.ScheduleStatus,
                    Value = 1,
                    Color = GenerateScheduleStatusColor(f.ScheduleStatus)
                });
        }
    }
}
