using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ProjectManagement.Schedule;
using NLog;
using Microsoft.SharePoint.Client;
using MCAWebAndAPI.Service.SPUtil;

namespace MCAWebAndAPI.Service.ProjectManagement.Schedule
{
    public class CalendarService : ICalendarService
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        const string CALENDAR_SP_LIST_NAME = "Calendar";
        string _siteUrl = null;

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        Calendar ConvertToModel(ListItem item) {
            return new Calendar(){
                Id = Convert.ToInt32(item["ID"]),
                Title = Convert.ToString(item["Title"]),
                Startdate = Convert.ToDateTime(item["EventDate"]),
                EndDate = Convert.ToDateTime(item["EndDate"]),
                IsAllDayEvent = Convert.ToBoolean(item["fAllDayEvent"]),
                IsRecurring = Convert.ToBoolean(item["fRecurrence"])
            };
        }

        public IEnumerable<Calendar> GetEvents()
        {
            var result = new List<Calendar>();

            foreach (var item in SPConnector.GetList(CALENDAR_SP_LIST_NAME))
            {
                result.Add(ConvertToModel(item));
            }

            return result;
        }
    }
}
