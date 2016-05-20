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
    }
}
