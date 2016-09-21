using System;
using System.Collections.Generic;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;

namespace MCAWebAndAPI.Service.Common
{
    public class SubActivityService
    {
        public const string ListNameSubActivity = "Sub Activity";

        public static IEnumerable<SubActivity> GetAll(string siteUrl)
        {
            var items = new List<SubActivity>();

            foreach (var item in SPConnector.GetList(ListNameSubActivity, WBSService.GetCompactProgramSiteUrl(siteUrl)))
            {
                items.Add(ConvertToSubActivityModel(item));
            }

            return items;
        }

        public static SubActivity Get(string siteUrl, int id)
        {
            var result = new SubActivity();

            var item = SPConnector.GetListItem(ListNameSubActivity, id, WBSService.GetCompactProgramSiteUrl(siteUrl));

            result = ConvertToSubActivityModel(item);

            return result;
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
    }
}