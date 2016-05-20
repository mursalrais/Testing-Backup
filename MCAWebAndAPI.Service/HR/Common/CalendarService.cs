using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using NLog;
using Microsoft.SharePoint.Client;
using MCAWebAndAPI.Service.HR.Common;
using MCAWebAndAPI.Service.Utils;

namespace MCAWebAndAPI.Service.HR.Common
{
    public class CalendarService : ICalendarService
    {
        string _siteUrl;
        static Logger logger = LogManager.GetCurrentClassLogger();
        const string SP_LIST_NAME = "Calendar Event";

        /*static Logger logger = LogManager.GetCurrentClassLogger();
        const string CALENDAR_SP_LIST_NAME = "Calendar";
        string _siteUrl = null;
        */

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

            /*
            updatedValues.Add("HolderID", new FieldLookupValue { LookupId = Convert.ToInt32(header.AssetHolderFrom.Value) });
            updatedValues.Add("HolderIDTo", new FieldLookupValue { LookupId = Convert.ToInt32(header.AssetHolderTo.Value) });
            */

            try
            {
                SPConnector.AddListItem(SP_LIST_NAME, updatedValues, _siteUrl);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }

            /*
            var enitity = new CalendarEventVM();
            enitity = calendar;
            return true;
            */
        }

        /*public void SetSiteUrl(string siteUrl)
        {
            if (siteUrl != null)
            {
                _siteUrl = siteUrl;
                _siteUrl = _siteUrl.Replace("\"", "");
                _siteUrl = _siteUrl.Replace("\'", "");
            }
        }*/

        /*Calendar ConvertToModel(ListItem item) {
            return new Calendar(){
                Id = Convert.ToInt32(item["ID"]),
                Title = Convert.ToString(item["Title"]),
                StartDate = Convert.ToDateTime(item["EventDate"]),
                EndDate = Convert.ToDateTime(item["EndDate"]),
                IsAllDayEvent = Convert.ToBoolean(item["fAllDayEvent"]),
                IsRecurring = Convert.ToBoolean(item["fRecurrence"])
            };
        }*/

        /*public IEnumerable<Calendar> GetEvents()
        {
            var result = new List<Calendar>();

            foreach (var item in SPConnector.GetList(CALENDAR_SP_LIST_NAME, _siteUrl))
            {
                result.Add(ConvertToModel(item));
            }

            return result;
        }*/


    }
}
