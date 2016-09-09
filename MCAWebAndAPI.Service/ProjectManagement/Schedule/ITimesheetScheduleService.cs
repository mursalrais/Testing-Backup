using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.ProjectManagement.Schedule
{
    public interface ITimesheetScheduleService
    {

        void SetSiteUrl(string siteUrl);

       void SendEmailEveryMonth();

    }
}
