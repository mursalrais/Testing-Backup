using System;
using System.Collections.Generic;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;

namespace MCAWebAndAPI.Service.Common
{
    public class ActivityService : IActivityService
    {
        private const string ListName = "Activity";

        private const string FieldName_ID = "ID";
        private const string FieldName_Name = "Title";
        private const string FieldName_Project = "Project";

        private string siteUrl;

        public static IEnumerable<ActivityVM> GetAll(string siteUrl)
        {
            var activities = new List<ActivityVM>();

            activities.Add(new ActivityVM() { ID = -1, Title = string.Empty });

            foreach (var item in SPConnector.GetList(ListName, siteUrl, null))
            {
                activities.Add(ConvertToActivityModel(item));
            }

            return activities;
        }

        public static ActivityVM Get(string siteUrl, int id)
        {
            var result = new ActivityVM();

            var item = SPConnector.GetListItem(ListName, id, siteUrl);
            result = ConvertToActivityModel(item);

            return result;
        }

        public static IEnumerable<ActivityVM> GetAllByProject(string siteUrl, string project)
        {
            var activities = new List<ActivityVM>();

            if (!string.IsNullOrWhiteSpace(project))
            {
                var caml = @"<View><Query><Where><Eq><FieldRef Name='Project' /><Value Type='Choice'>" + project + "</Value></Eq></Where></Query></View>";
                foreach (var item in SPConnector.GetList(ListName, siteUrl, caml))
                {
                    activities.Add(ConvertToActivityModel(item));
                }
            }
            return activities;
        }

        private static ActivityVM ConvertToActivityModel(ListItem item)
        {
            ActivityVM toReturn = new ActivityVM();
            toReturn.ID = Convert.ToInt32(item[FieldName_ID]);
            toReturn.Title = Convert.ToString(item[FieldName_Name]);
            toReturn.Project.Text = Convert.ToString(item[FieldName_Project]);

            //return new ActivityVM
            //{
            //    Title = Convert.ToString(item[FieldName_Name]),
            //    Project.Text = Convert.ToString(item[FieldName_Project]),
            //};

            return toReturn;
        }

        public void SetSiteUrl(string siteUrl)
        {
            this.siteUrl = siteUrl;
        }
    }
}