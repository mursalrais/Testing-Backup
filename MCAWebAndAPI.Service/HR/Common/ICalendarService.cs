using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.HR.Common
{
    public interface ICalendarService
    {
        void SetSiteUrl(string siteUrl);

        CalendarEventVM GetPopulatedModel(int? id = null);

        void CreateHeader(CalendarEventVM calendarEvent);

        IEnumerable<EventCalendar> GetEventCalendars(IEnumerable<DateTime> dateRange);

        IEnumerable<EventCalendar> GetHolidays(IEnumerable<DateTime> dateRange);

        IEnumerable<EventCalendar> GetPublicHolidays(IEnumerable<DateTime> dateRange);
    }
}
