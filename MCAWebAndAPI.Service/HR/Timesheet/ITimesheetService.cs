using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.HR;

namespace MCAWebAndAPI.Service.HR.Timesheet
{
    public interface ITimesheetService
    {
        void SetSiteUrl(string siteUrl = null);

        TimesheetVM GetTimesheet(string userlogin, DateTime period);

        IEnumerable<TimesheetDetailVM> GetTimesheetDetails(string userlogin, DateTime period);

        IEnumerable<TimesheetDetailVM> AppendWorkingDays(IEnumerable<TimesheetDetailVM> currentDays, DateTime from, DateTime to, bool isFullDay, string location = null);
        
    }
}
