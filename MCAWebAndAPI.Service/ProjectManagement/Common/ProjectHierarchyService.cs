using System;
using System.Collections.Generic;
using System.Linq;
using MCAWebAndAPI.Model.ProjectManagement.Common;
using MCAWebAndAPI.Model.ViewModel.Chart;
using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Service.Common;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;

namespace MCAWebAndAPI.Service.ProjectManagement.Common
{
    public class ProjectHierarchyService : IProjectHierarchyService
    {
        string _siteUrl = null;
        static Logger logger = LogManager.GetCurrentClassLogger();


        const string SP_PROJECT_INFORMATION_LIST_NAME = "Project Information";


        public const string IMS_PROJECT_HN_NAME = "Health and Nutrition";
        public const string IMS_PROJECT_PM_NAME = "Procurement Modernization";
        public const string IMS_PROJECT_GP_NAME = "Green Prosperity";

        private Project ConvertToProjectModel(ListItem item)
        {
            var model = new Project();
            model.ProjectName = Convert.ToString(item["Title"]);
            model.Start = Convert.ToDateTime(item["Start"]);
            model.Finish = Convert.ToDateTime(item["Finish"]);
            model.Director = (FieldUserValue)item["Project_x0020_Manager"] == null ? "" :
                    Convert.ToString(((FieldUserValue)item["Project_x0020_Manager"]).LookupValue);
            model.ColorStatus = WBSMasterService.GenerateScheduleStatusColor(Convert.ToString(item["Schedule_x0020_Status"]));
            model.ScheduleStatus = Convert.ToString(item["Schedule_x0020_Status"]);
            model.PercentComplete = Convert.ToDouble(item["_x0025__x0020_Complete"]);

            return model;
        }

        public static string GetProjectNameFromSiteUrl(string siteUrl)
        {
            if (siteUrl.Contains("/gp"))
                return "Green Prosperity";
            if (siteUrl.Contains("/hn"))
                return "Health and Nutrition";
            if (siteUrl.Contains("/pm"))
                return "Procurement Modernization";
            return null;
        }

        public void SetSiteUrl(string siteUrl)
        {
            if (siteUrl != null)
            {
                _siteUrl = siteUrl;
                _siteUrl = _siteUrl.Replace("\"", "");
                _siteUrl = _siteUrl.Replace("\'", "");
            }
        }


        public IEnumerable<Activity> GetAllActivities()
        {
            return WBSMasterService.GetAllActivities(_siteUrl);
        }

        public IEnumerable<SubActivity> GetAllSubActivities()
        {
            return WBSMasterService.GetAllSubActivities(_siteUrl);
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

            foreach (var item in SPConnector.GetList(WBSMasterService.SP_SUB_ACTIVITY_LIST_NAME, _siteUrl))
            {
                items.Add(WBSMasterService.ConvertToSubActivityModel(item));
            }

            return items.Select(e => new StackedBarChartVM()
            {
                CategoryName = e.ActivityName,
                GroupName = GenerateOrderedScheduleStatus(e.ScheduleStatus),
                Value = 1,
                Color = WBSMasterService.GenerateScheduleStatusColor(e.ScheduleStatus)
            });
        }

        public IEnumerable<StackedBarChartVM> GenerateProjectHealthStatusChartByProject()
        {
            var items = WBSMasterService.GetActivitiesAcrossProjects(_siteUrl);

            return items.Select(e => new StackedBarChartVM()
            {
                CategoryName = e.ProjectName,
                GroupName = GenerateOrderedScheduleStatus(e.ScheduleStatus),
                Value = 1,
                Color = WBSMasterService.GenerateScheduleStatusColor(e.ScheduleStatus)
            });
        }


        public IEnumerable<WBSMapping> GetAllWBSMappings()
        {
            return WBSMasterService.GetAll(_siteUrl);

            //var activities = GetActivitiesAcrossProjects();
            //var subActivities = GetSubActivitiesAcrossProjects();
            //var items = GetWBSAcrossProjects(subActivities, activities);

            //return items;
        }

        public bool UpdateWBSMapping()
        {
            return WBSMasterService.UpdateWBSMapping(_siteUrl);

            //var wbsList = GetAllWBSMappings();
            //var wbsProjectDict = new Dictionary<string, WBSMapping>();
            //foreach(var item in wbsList)
            //{
            //    if(!wbsProjectDict.ContainsKey(item.WBSID)){
            //        wbsProjectDict.Add(item.WBSID, item);
            //    }
            //}

            //var wbsProgramDict = new Dictionary<string, ListItem>();
            //var anyUpdatedValue = false;
            //foreach (var item in SPConnector.GetList(SP_WBSMAPPING_IN_PROGRAM_LIST_NAME, _siteUrl))
            //{
            //    try {
            //        wbsProgramDict.Add(Convert.ToString(item["WBS_x0020_ID"]), item);
            //        var isUpdated = UpdateIfChanged(item, wbsProjectDict);

            //        if (isUpdated && !anyUpdatedValue)
            //            anyUpdatedValue = true;
            //    }
            //    catch (Exception)
            //    {
            //        return false;
            //    }
            //}

            //var appendIfNew = AppendIfNewWBSExist(wbsProjectDict, wbsProgramDict);
            //return anyUpdatedValue || appendIfNew;
        }


        public IEnumerable<WBSMapping> GetWBSMappingsInProgram()
        {
            return WBSMasterService.GetAll(_siteUrl);

            //var wbs = new List<WBSMapping>();

            //foreach(var item in SPConnector.GetList(SP_WBSMAPPING_IN_PROGRAM_LIST_NAME, _siteUrl))
            //{
            //    wbs.Add(ConvertToWBSModelInProgram(item));
            //}

            //return wbs;
        }

        public IEnumerable<InGridComboBoxVM> GetProjectComboBox()
        {
            return new List<InGridComboBoxVM>()
            {
                new InGridComboBoxVM
                {
                    Value = 1,
                    Text = "Green Prosperity"
                },
                new InGridComboBoxVM
                {
                    Value = 2,
                    Text = "Health and Nutrition"
                },
                new InGridComboBoxVM
                {
                    Value = 3,
                    Text = "Procurement Modernization"
                }
            };
        }
    }
}