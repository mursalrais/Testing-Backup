using System;
using System.Collections.Generic;
using System.Linq;
using MCAWebAndAPI.Model.ProjectManagement.Common;
using MCAWebAndAPI.Model.ViewModel.Chart;
using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;

namespace MCAWebAndAPI.Service.ProjectManagement.Common
{
    public class ProjectHierarchyService : IProjectHierarchyService
    {
        string _siteUrl = null;
        static Logger logger = LogManager.GetCurrentClassLogger();

        const string SP_ACTIVITY_LIST_NAME = "Activity";
        const string SP_SUB_ACTIVITY_LIST_NAME = "Sub Activity";
        const string SP_PROJECT_INFORMATION_LIST_NAME = "Project Information";
        const string SP_WBSMAPPING_IN_PROJECT_LIST_NAME = "WBSMapping";
        const string SP_WBSMAPPING_IN_PROGRAM_LIST_NAME = "WBS Mapping";


        const string IMS_PROJECT_HN_NAME = "Health and Nutrition";
        const string IMS_PROJECT_PM_NAME = "Procurement Modernization";
        const string IMS_PROJECT_GP_NAME = "Green Prosperity";

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
            model.PercentComplete = Convert.ToDouble(item["_x0025__x0020_Complete"]);

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
            model.ColorStatus = GenerateScheduleStatusColor(Convert.ToString(item["Schedule_x0020_Status"]));
            model.ScheduleStatus = Convert.ToString(item["Schedule_x0020_Status"]);
            model.NoofSubActivity = Convert.ToInt32(item["NoofSubActivity"]);
            model.PercentComplete = Convert.ToDouble(item["PercentComplete"]);
            model.ProjectName = Convert.ToString(item["ProjectName"]);

            return model;
        }

        private WBSMapping ConvertToWBSModel(ListItem item, IEnumerable<SubActivity> subActivities, 
            IEnumerable<Activity> activities)
        {
            var model = new WBSMapping();
            model.WBSID = Convert.ToString(item["Title"]);
            model.WBSDescription = Convert.ToString(item["Description"]);
            model.SubActivity = item["SubActivity"] == null ? string.Empty :
                Convert.ToString((item["SubActivity"] as FieldLookupValue).LookupValue);

            var relatedActivity = RetrieveRelatedActivity(model.SubActivity, subActivities, activities);
            if (relatedActivity == null)
                return model;

            model.Activity = relatedActivity.ActivityName;
            model.Project = getProjectNameFromSiteUrl() ?? relatedActivity.ProjectName;
            return model;
        }

        WBSMapping ConvertToWBSModelInProgram(ListItem item)
        {
            return new WBSMapping
            {
                WBSID = Convert.ToString(item["WBS_x0020_ID"]), 
                WBSDescription = Convert.ToString(item["WBS_x0020_Description"]), 
                Activity = Convert.ToString(item["Activity"]), 
                Project = Convert.ToString(item["Project"]), 
                SubActivity = Convert.ToString(item["Sub_x0020_Activity"])
            };
        }

        private string getProjectNameFromSiteUrl()
        {
            if (_siteUrl.Contains("/gp"))
                return "Green Prosperity";
            if(_siteUrl.Contains("/hn"))
                return "Health and Nutrition";
            if (_siteUrl.Contains("/pm"))
                return "Procurement Modernization";
            return null;
        }

        private Activity RetrieveRelatedActivity(string subActivityName, IEnumerable<SubActivity> subActivities, IEnumerable<Activity> activities)
        {
            if (string.IsNullOrEmpty(subActivityName))
                return null;

            var subActivity = subActivities.FirstOrDefault(e => 
                string.Compare(e.SubActivityName, subActivityName, StringComparison.OrdinalIgnoreCase) == 0);
            return activities.FirstOrDefault(e => 
                string.Compare(e.ActivityName, subActivity.ActivityName, StringComparison.OrdinalIgnoreCase) == 0);
        }

        public void SetSiteUrl(string siteUrl)
        {
            if(siteUrl != null)
            {
                _siteUrl = siteUrl;
                _siteUrl = _siteUrl.Replace("\"", "");
                _siteUrl = _siteUrl.Replace("\'", "");
            }
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

        public IEnumerable<SubActivity> GetAllSubActivities()
        {
            var items = new List<SubActivity>();

            foreach (var item in SPConnector.GetList(SP_SUB_ACTIVITY_LIST_NAME, _siteUrl))
            {
                items.Add(ConvertToSubActivityModel(item));
            }

            return items;
        }

        private IEnumerable<WBSMapping> GetAllWBSes(IEnumerable<SubActivity> subActivities,
            IEnumerable<Activity> activities)
        {
            var items = new List<WBSMapping>();

            foreach (var item in SPConnector.GetList(SP_WBSMAPPING_IN_PROJECT_LIST_NAME, _siteUrl))
            {
                items.Add(ConvertToWBSModel(item, subActivities, activities));
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

        private IEnumerable<Activity> GetProjectActivities(string projectRelativeUrl)
        {
            var programSiteUrl = _siteUrl;
            _siteUrl = programSiteUrl + projectRelativeUrl;
            var activities = GetAllActivities();
            _siteUrl = programSiteUrl;

            foreach(var item in activities)
            {
                item.ProjectName = projectRelativeUrl.Contains("hn") ? IMS_PROJECT_HN_NAME :
                    projectRelativeUrl.Contains("pm") ? IMS_PROJECT_PM_NAME :
                    IMS_PROJECT_GP_NAME;
            }

            return activities;
        }

        private IEnumerable<SubActivity> GetProjectSubActivities(string projectRelativeUrl)
        {
            var programSiteUrl = _siteUrl;
            _siteUrl = programSiteUrl + projectRelativeUrl;
            var subActivities = GetAllSubActivities();
            _siteUrl = programSiteUrl;

            return subActivities;
        }

        private IEnumerable<WBSMapping> GetProjectWBS(string projectRelativeUrl, IEnumerable<SubActivity> subActivities,
            IEnumerable<Activity> activities)
        {
            var programSiteUrl = _siteUrl;
            _siteUrl = programSiteUrl + projectRelativeUrl;
            var wbses = GetAllWBSes(subActivities, activities);
            _siteUrl = programSiteUrl;

            return wbses;
        }

        public IEnumerable<StackedBarChartVM> GenerateProjectHealthStatusChartByProject()
        {
            var items = GetActivitiesAcrossProjects();

            return items.Select(e => new StackedBarChartVM()
            {
                CategoryName = e.ProjectName,
                GroupName = GenerateOrderedScheduleStatus(e.ScheduleStatus),
                Value = 1,
                Color = GenerateScheduleStatusColor(e.ScheduleStatus)
            });
        }

        private IEnumerable<Activity> GetActivitiesAcrossProjects()
        {
            var items = new List<Activity>();
            items.AddRange(GetProjectActivities("/gp"));
            items.AddRange(GetProjectActivities("/hn"));
            items.AddRange(GetProjectActivities("/pm"));
            return items;
        }

        private IEnumerable<SubActivity> GetSubActivitiesAcrossProjects()
        {
            var items = new List<SubActivity>();
            items.AddRange(GetProjectSubActivities("/gp"));
            items.AddRange(GetProjectSubActivities("/hn"));
            items.AddRange(GetProjectSubActivities("/pm"));
            return items;
        }

        private IEnumerable<WBSMapping> GetWBSAcrossProjects(IEnumerable<SubActivity> subActivities,
            IEnumerable<Activity> activities)
        {
            var items = new List<WBSMapping>();
            items.AddRange(GetProjectWBS("/gp", subActivities, activities));
            items.AddRange(GetProjectWBS("/hn", subActivities, activities));
            items.AddRange(GetProjectWBS("/pm", subActivities, activities));
            return items;
        }

        public IEnumerable<WBSMapping> GetAllWBSMappings()
        {
            var activities = GetActivitiesAcrossProjects();
            var subActivities = GetSubActivitiesAcrossProjects();
            var items = GetWBSAcrossProjects(subActivities, activities);

            return items;
        }

        public bool UpdateWBSMapping()
        {
            var wbsList = GetAllWBSMappings();
            var wbsProjectDict = new Dictionary<string, WBSMapping>();
            foreach(var item in wbsList)
            {
                if(!wbsProjectDict.ContainsKey(item.WBSID)){
                    wbsProjectDict.Add(item.WBSID, item);
                }
            }

            var wbsProgramDict = new Dictionary<string, ListItem>();
            var anyUpdatedValue = false;
            foreach (var item in SPConnector.GetList(SP_WBSMAPPING_IN_PROGRAM_LIST_NAME, _siteUrl))
            {
                try {
                    wbsProgramDict.Add(Convert.ToString(item["WBS_x0020_ID"]), item);
                    var isUpdated = UpdateIfChanged(item, wbsProjectDict);

                    if (isUpdated && !anyUpdatedValue)
                        anyUpdatedValue = true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            var appendIfNew = AppendIfNewWBSExist(wbsProjectDict, wbsProgramDict);
            return anyUpdatedValue || appendIfNew;
        }

        private bool AppendIfNewWBSExist(Dictionary<string, WBSMapping> wbsProjectDict, Dictionary<string, ListItem> wbsProgramDict)
        {
            var itemKeysToAppend = new List<string>();
            foreach(var item in wbsProjectDict)
            {
                if (!wbsProgramDict.ContainsKey(item.Key))
                {
                    itemKeysToAppend.Add(item.Key);
                }
            }

            if (itemKeysToAppend.Count == 0)
                return false;

            
            foreach(var itemKey in itemKeysToAppend)
            {
                var item = wbsProjectDict[itemKey];

                var columnValues = new Dictionary<string, object>();
                columnValues.Add("WBS_x0020_ID", item.WBSID);
                columnValues.Add("WBS_x0020_Description", item.WBSDescription);
                columnValues.Add("Sub_x0020_Activity", item.SubActivity);
                columnValues.Add("Activity", item.Activity);
                columnValues.Add("Project", item.Project);

                try {
                    SPConnector.AddListItem(SP_WBSMAPPING_IN_PROGRAM_LIST_NAME, columnValues, _siteUrl);
                }catch(Exception e)
                {
                    logger.Error(e.Message);
                }
            }

            return true;
        }

        

        private bool UpdateIfChanged(ListItem item, Dictionary<string, WBSMapping> wbsDictionary)
        {
            var key = Convert.ToString(item["WBS_x0020_ID"]);
            if (!wbsDictionary.ContainsKey(key))
                return false;

            var updatedValues = new Dictionary<string, object>();

            if (string.Compare(Convert.ToString(item["WBS_x0020_Description"]), 
                wbsDictionary[key].WBSDescription, StringComparison.OrdinalIgnoreCase) != 0)
                updatedValues.Add("WBS_x0020_Description", wbsDictionary[key].WBSDescription);

            if (string.Compare(Convert.ToString(item["Sub_x0020_Activity"]),
               wbsDictionary[key].SubActivity, StringComparison.OrdinalIgnoreCase) != 0)
                updatedValues.Add("Sub_x0020_Activity", wbsDictionary[key].SubActivity);

            if (string.Compare(Convert.ToString(item["Activity"]),
               wbsDictionary[key].Activity, StringComparison.OrdinalIgnoreCase) != 0)
                updatedValues.Add("Activity", wbsDictionary[key].Activity);

            if (string.Compare(Convert.ToString(item["Project"]),
               wbsDictionary[key].Project, StringComparison.OrdinalIgnoreCase) != 0)
                updatedValues.Add("Project", wbsDictionary[key].Project);

            if(updatedValues.Count > 0)
            {
                try
                {
                    SPConnector.UpdateListItem(SP_WBSMAPPING_IN_PROGRAM_LIST_NAME, Convert.ToInt32(item["ID"]),
                        updatedValues, _siteUrl);
                    return true;
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public IEnumerable<WBSMapping> GetWBSMappingsInProgram()
        {
            var wbs = new List<WBSMapping>();

            foreach(var item in SPConnector.GetList(SP_WBSMAPPING_IN_PROGRAM_LIST_NAME, _siteUrl))
            {
                wbs.Add(ConvertToWBSModelInProgram(item));
            }

            return wbs;
        }

        public IEnumerable<InGridComboBoxVM> GetProjectComboBox()
        {
            return new List<InGridComboBoxVM>()
            {
                new InGridComboBoxVM
                {
                    CategoryID = 1,
                    CategoryName = "Green Prosperity"
                },
                new InGridComboBoxVM
                {
                    CategoryID = 2,
                    CategoryName = "Health and Nutrition"
                },
                new InGridComboBoxVM
                {
                    CategoryID = 3,
                    CategoryName = "Procurement Modernization"
                }
            };
        }
    }
}
