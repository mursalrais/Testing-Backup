using MCAWebAndAPI.Model.ProjectManagement.Schedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.ProjectManagement.Schedule
{
    public interface ICalendarService
    {
        void SetSiteUrl(string siteUrl);

        IEnumerable<Calendar> GetEvents();

    }
}
