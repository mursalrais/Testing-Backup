﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;

namespace MCAWebAndAPI.Service.Shared
{
    public class ActivityService: IActivityService
    {
        private const string ListName = "Activity";

        private const string FieldName_ID = "ID";
        private const string FieldName_Name = "Title";
        private const string FieldName_Project = "Project";    

        private string siteUrl;

        public static IEnumerable<ActivityVM> GetActivities(string siteUrl)
        {
            var activities = new List<ActivityVM>();

            activities.Add(new ActivityVM() { ID = -1, Title = string.Empty });

            foreach (var item in SPConnector.GetList(ListName, siteUrl, null))
            {
                activities.Add(ConvertToActivityModel(item));
            }

            return activities;
        }

        public static IEnumerable<ActivityVM> GetActivities(string siteUrl, string projectValue)
        {
            var activities = new List<ActivityVM>();

            if (!string.IsNullOrWhiteSpace(projectValue))
            {
                var caml = @"<View><Query><Where><Eq><FieldRef Name='Project' /><Value Type='Choice'>" + projectValue + "</Value></Eq></Where></Query></View>";
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