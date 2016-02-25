using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ProjectManagement.Common;
using MCAWebAndAPI.Service.SPUtil;
using Microsoft.SharePoint.Client;

namespace MCAWebAndAPI.Service.ProjectManagement.Common
{
    public class ProjectHierarchyService : IProjectHierarchyService
    {
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
                    ActivityFK = Convert.ToString((item["Activity"] as FieldLookupValue).LookupValue)
                });
            }
            return result;
        }
    }
}
