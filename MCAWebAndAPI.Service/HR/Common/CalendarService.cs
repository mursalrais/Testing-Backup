using System;
using System.Collections.Generic;
using System.Linq;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using NLog;
using MCAWebAndAPI.Service.Utils;
using MCAWebAndAPI.Model.Common;
using Microsoft.SharePoint.Client;

namespace MCAWebAndAPI.Service.HR.Common
{
    public class CalendarService : ICalendarService
    {
        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();
        const string SP_LIST_NAME = "Event Calendar";
        
        const string TYPE_DAYOFF = "Day-Off";
        const string TYPE_COMP_LEAVE = "Compensatory Leave";

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

        public CalendarEventVM GetPopulatedModel(int? id = null)
        {
            var model = new CalendarEventVM();
            return model;
        }

        public void CreateHeader(CalendarEventVM calendar)
        {
            var updatedValues = new Dictionary<string, object>();
            updatedValues.Add("CalendarEventDate", calendar.CalendarEventDate);
            updatedValues.Add("Title", calendar.Title);
            updatedValues.Add("EventCategory", calendar.EventCategory.Value);

            try
            {
                SPConnector.AddListItem(SP_LIST_NAME, updatedValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
        }

        public IEnumerable<EventCalendar> GetEventCalendars(IEnumerable<DateTime> dateRange)
        {
            var eventCalendars = new List<EventCalendar>();
            foreach (var item in SPConnector.GetList(SP_LIST_NAME, _siteUrl))
            {
                eventCalendars.Add(ConvertToEventCalendar(item));
            }

            return eventCalendars;
        }

        public IEnumerable<EventCalendar> GetHolidays(IEnumerable<DateTime> dateRange)
        {
            return dateRange.Where(e => e.DayOfWeek == DayOfWeek.Saturday || e.DayOfWeek == DayOfWeek.Sunday)
                .Select(e => new EventCalendar
                {
                    Date = e,
                    EventCategory = EventCalendar.GetType(EventCalendar.Type.HOLIDAY)
                });
        }

        public IEnumerable<EventCalendar> GetPublicHolidays(IEnumerable<DateTime> dateRange)
        {
            var startDateUniversalTimeString = dateRange.ToArray()[0].ToUniversalTime().ToString("o");
            var finishDateUniversalTimeString = dateRange.ToArray()[dateRange.ToArray().Length - 1]
                .ToUniversalTime().ToString("o");
            
            var caml = @"<View>  
            <Query> 
               <Where><And><And><Eq><FieldRef Name='Category' /><Value Type='Choice'>" +  EventCalendar.GetType(EventCalendar.Type.PUBLIC_HOLIDAY) + 
               @"</Value></Eq><Geq><FieldRef Name='EventDate0' /><Value Type='DateTime'>" + startDateUniversalTimeString + 
               @"</Value></Geq></And><Leq><FieldRef Name='EventDate0' /><Value Type='DateTime'>" + finishDateUniversalTimeString +
               @"</Value></Leq></And></Where> 
            </Query> 
            </View>";

            var eventCalendars = new List<EventCalendar>();
            foreach (var item in SPConnector.GetList(SP_LIST_NAME, _siteUrl, caml))
            {
                eventCalendars.Add(ConvertToEventCalendar(item));
            }

            return eventCalendars;
        }

        private EventCalendar ConvertToEventCalendar(ListItem item)
        {
            return new EventCalendar
            {
                ID = Convert.ToInt32(item["ID"]),
                EventCategory = Convert.ToString(item["Category"]),
                Date = Convert.ToDateTime(item["EventDate0"])
            };
        }


        /// <summary>
        /// Return all days subtracted by holidays and public holidays
        /// </summary>
        /// <param name="dateRange"></param>
        /// <returns></returns>
        public int GetTotalWorkingDays(IEnumerable<DateTime> dateRange)
        {
            var numberOfallDays = dateRange.Count();
            var numberOfHolidays = GetHolidays(dateRange).Count();
            var numberOfPublicHolidays = GetPublicHolidays(dateRange).Count();

            return numberOfallDays - (numberOfHolidays + numberOfPublicHolidays);
        }
    }
}
