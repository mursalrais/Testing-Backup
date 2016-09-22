using System;
using System.Collections.Generic;
using System.Linq;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Service.ProjectManagement.Common;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;

namespace MCAWebAndAPI.Service.Common
{
    public partial class WBSService : IWBSMasterService
    {
        public const string ListName = "WBS Mapping";

        public const string ListNameActivity = "Activity";
        public const string ListNameSubActivity = "Sub Activity";

        /// <summary>
        /// Gets a WBSMapping by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static WBS Get(string siteUrl, int id)
        {
            var item = SPConnector.GetListItem(ListName, id, GetCurrentSiteUrl(siteUrl));

            return ConvertToVM(item);
        }

        public static IEnumerable<WBS> GetAll(string siteUrl)
        {
            var wbs = new List<WBS>();

            foreach (var item in SPConnector.GetList(ListName, GetCurrentSiteUrl(siteUrl)))
            {
                wbs.Add(ConvertToVM(item));
            }

            return wbs;
        }

        //TODO: remove this temporary function
        public static string GetCurrentSiteUrl(string siteUrl)
        {
            return siteUrl;
            //return GetCompactProgramSiteUrl(siteUrl);
        }

        public static bool UpdateWBSMapping(string siteUrl)
        {
            var wbsList = GetAll(siteUrl);
            var wbsProjectDict = new Dictionary<string, WBS>();

            foreach (var item in wbsList)
            {
                if (!wbsProjectDict.ContainsKey(item.WBSID))
                {
                    wbsProjectDict.Add(item.WBSID, item);
                }
            }

            var wbsProgramDict = new Dictionary<string, ListItem>();
            var anyUpdatedValue = false;

            foreach (var item in SPConnector.GetList(ListName, siteUrl))
            {
                try
                {
                    wbsProgramDict.Add(Convert.ToString(item["WBS_x0020_ID"]), item);
                    var isUpdated = UpdateIfChanged(siteUrl, item, wbsProjectDict);

                    if (isUpdated && !anyUpdatedValue)
                        anyUpdatedValue = true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            var appendIfNew = AppendIfNewWBSExist(siteUrl, wbsProjectDict, wbsProgramDict);
            return anyUpdatedValue || appendIfNew;
        }

        //public static IEnumerable<WBSMapping> GetAllWBSMappings(string siteUrl)
        //{
        //    var siteUrlCP = GetCompactProgramSiteUrl(siteUrl);

        //    var activities = GetActivitiesAcrossProjects(siteUrlCP);
        //    var subActivities = GetSubActivitiesAcrossProjects(siteUrlCP);
        //    var items = GetWBSAcrossProjects(siteUrlCP, subActivities, activities);

        //    return items;
        //}

        public static IEnumerable<Activity> GetAllActivities(string siteUrl)
        {
            var items = new List<Activity>();

            foreach (var item in SPConnector.GetList(ListNameActivity, GetCompactProgramSiteUrl(siteUrl)))
            {
                items.Add(ConvertToActivityModel(item));
            }

            return items;
        }

        public static IEnumerable<SubActivity> GetAllSubActivities(string siteUrl)
        {
            var items = new List<SubActivity>();

            foreach (var item in SPConnector.GetList(ListNameSubActivity, GetCompactProgramSiteUrl(siteUrl)))
            {
                items.Add(ConvertToSubActivityModel(item));
            }

            return items;
        }

        public static IEnumerable<Activity> GetActivitiesAcrossProjects(string siteUrl)
        {
            var items = new List<Activity>();
            items.AddRange(GetProjectActivities(siteUrl, "/gp"));
            items.AddRange(GetProjectActivities(siteUrl, "/hn"));
            items.AddRange(GetProjectActivities(siteUrl, "/pm"));
            return items;
        }

        public static SubActivity ConvertToSubActivityModel(ListItem item)
        {
            var model = new SubActivity();
            model.SubActivityName = Convert.ToString(item["Title"]);
            model.ActivityName = item["Activity"] == null ? string.Empty
                : Convert.ToString((item["Activity"] as FieldLookupValue).LookupValue);
            model.ScheduleStatus = Convert.ToString(item["Schedule_x0020_Status"]);

            return model;
        }

        public static string GenerateScheduleStatusColor(string scheduleStatus)
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

        public static string GetCompactProgramSiteUrl(string siteUrl)
        {
            if (string.IsNullOrEmpty(siteUrl))
            {
                throw new InvalidOperationException("siteUrl parameter cannot be null.");
            }

            return CommonService.GetSiteUrlFromCurrent(siteUrl, CommonService.Sites.CP);
        }

        #region Private Members

        //private static IEnumerable<WBSMapping> GetWBSAcrossProjects(string siteUrl, IEnumerable<SubActivity> subActivities,
        //    IEnumerable<Activity> activities)
        //{
        //    var items = new List<WBSMapping>();
        //    items.AddRange(GetProjectWBS(siteUrl, "/gp", subActivities, activities));
        //    items.AddRange(GetProjectWBS(siteUrl, "/hn", subActivities, activities));
        //    items.AddRange(GetProjectWBS(siteUrl, "/pm", subActivities, activities));
        //    return items;
        //}

        //private static IEnumerable<WBSMapping> GetProjectWBS(string siteUrl, string projectRelativeUrl,
        //    IEnumerable<SubActivity> subActivities,
        //    IEnumerable<Activity> activities)
        //{
        //    var programSiteUrl = siteUrl;
        //    siteUrl = programSiteUrl + projectRelativeUrl;
        //    var wbses = GetAllWBSes(siteUrl, subActivities, activities);
        //    siteUrl = programSiteUrl;

        //    return wbses;
        //}

        //private static IEnumerable<WBSMapping> GetAllWBSes(string siteUrl, IEnumerable<SubActivity> subActivities,
        //    IEnumerable<Activity> activities)
        //{
        //    var items = new List<WBSMapping>();

        //    foreach (var item in SPConnector.GetList(SP_WBSMAPPING_IN_PROJECT_LIST_NAME, siteUrl))
        //    {
        //        items.Add(ConvertToWBSModel(siteUrl, item, subActivities, activities));
        //    }

        //    return items;
        //}

        private static bool AppendIfNewWBSExist(string siteUrl, Dictionary<string, WBS> wbsProjectDict, Dictionary<string, ListItem> wbsProgramDict)
        {
            var itemKeysToAppend = new List<string>();
            foreach (var item in wbsProjectDict)
            {
                if (!wbsProgramDict.ContainsKey(item.Key))
                {
                    itemKeysToAppend.Add(item.Key);
                }
            }

            if (itemKeysToAppend.Count == 0)
                return false;

            foreach (var itemKey in itemKeysToAppend)
            {
                var item = wbsProjectDict[itemKey];

                var columnValues = new Dictionary<string, object>();
                columnValues.Add("WBS_x0020_ID", item.WBSID);
                columnValues.Add("WBS_x0020_Description", item.WBSDescription);
                columnValues.Add("Sub_x0020_Activity", item.SubActivity);
                columnValues.Add("Activity", item.Activity);
                columnValues.Add("Project", item.Project);

                try
                {
                    SPConnector.AddListItem(ListName, columnValues, siteUrl);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            return true;
        }

        private static bool UpdateIfChanged(string siteUrl, ListItem item, Dictionary<string, WBS> wbsDictionary)
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

            if (updatedValues.Count > 0)
            {
                try
                {
                    SPConnector.UpdateListItem(ListName, Convert.ToInt32(item["ID"]),
                        updatedValues, siteUrl);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        //private static WBSMapping ConvertToVM(string siteUrl, ListItem item, IEnumerable<SubActivity> subActivities,
        //    IEnumerable<Activity> activities)
        //{
        //    var model = new WBSMapping();
        //    model.WBSID = Convert.ToString(item["Title"]);
        //    model.WBSDescription = Convert.ToString(item["Description"]);
        //    model.SubActivity = item["SubActivity"] == null ? string.Empty :
        //        Convert.ToString((item["SubActivity"] as FieldLookupValue).LookupValue);

        //    var relatedActivity = RetrieveRelatedActivity(model.SubActivity, subActivities, activities);
        //    if (relatedActivity == null)
        //        return model;

        //    model.Activity = relatedActivity.ActivityName;
        //    model.Project = ProjectHierarchyService.GetProjectNameFromSiteUrl(siteUrl) ?? relatedActivity.ProjectName;
        //    return model;
        //}

        private static Activity RetrieveRelatedActivity(string subActivityName, IEnumerable<SubActivity> subActivities, IEnumerable<Activity> activities)
        {
            if (string.IsNullOrEmpty(subActivityName))
                return null;

            var subActivity = subActivities.FirstOrDefault(e =>
                string.Compare(e.SubActivityName, subActivityName, StringComparison.OrdinalIgnoreCase) == 0);
            return activities.FirstOrDefault(e =>
                string.Compare(e.ActivityName, subActivity.ActivityName, StringComparison.OrdinalIgnoreCase) == 0);
        }

        private static WBS ConvertToVM(ListItem item)
        {
            return new WBS
            {
                ID = Convert.ToInt32(item["ID"]),
                WBSID = Convert.ToString(item["WBS_x0020_ID"]),
                WBSDescription = Convert.ToString(item["WBS_x0020_Description"]),
                Activity = Convert.ToString(item["Activity"]),
                Project = Convert.ToString(item["Project"]),
                SubActivity = Convert.ToString(item["Sub_x0020_Activity"])
            };
        }

        private static IEnumerable<Activity> GetProjectActivities(string siteUrl, string projectRelativeUrl)
        {
            var programSiteUrl = siteUrl;
            siteUrl = programSiteUrl + projectRelativeUrl;
            var activities = GetAllActivities(siteUrl);
            siteUrl = programSiteUrl;

            foreach (var item in activities)
            {
                item.ProjectName = projectRelativeUrl.Contains("hn") ? ProjectHierarchyService.IMS_PROJECT_HN_NAME :
                    projectRelativeUrl.Contains("pm") ? ProjectHierarchyService.IMS_PROJECT_PM_NAME :
                    ProjectHierarchyService.IMS_PROJECT_GP_NAME;
            }

            return activities;
        }

        private static IEnumerable<SubActivity> GetProjectSubActivities(string siteUrl, string projectRelativeUrl)
        {
            var programSiteUrl = siteUrl;
            siteUrl = programSiteUrl + projectRelativeUrl;
            var subActivities = GetAllSubActivities(siteUrl);
            siteUrl = programSiteUrl;

            return subActivities;
        }

        private static IEnumerable<SubActivity> GetSubActivitiesAcrossProjects(string siteUrl)
        {
            var items = new List<SubActivity>();
            items.AddRange(GetProjectSubActivities(siteUrl, "/gp"));
            items.AddRange(GetProjectSubActivities(siteUrl, "/hn"));
            items.AddRange(GetProjectSubActivities(siteUrl, "/pm"));
            return items;
        }

        private static Activity ConvertToActivityModel(ListItem item)
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

        #endregion
    }
}