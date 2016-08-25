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

        TimesheetVM GetTimesheetLoadUpdate(int? id, string userlogin);

        IEnumerable<TimesheetDetailVM> GetTimesheetDetails(string userlogin, DateTime period);

        IEnumerable<TimesheetDetailVM> AppendWorkingDays(IEnumerable<TimesheetDetailVM> currentDays, 
            DateTime from, DateTime to, bool isFullDay, string location = null, int? locationid = null);

        int CreateHeader(TimesheetVM header);

        Task CreateTimesheetDetailsAsync(int? headerId, IEnumerable<TimesheetDetailVM> timesheetDetails);
        void CreateTimesheetDetails(int? headerId, IEnumerable<TimesheetDetailVM> timesheetDetails);

        Task CreateWorkflowTimesheetAsync(int? headerId, TimesheetVM header);
        void CreateWorkflowTimesheet(int? headerId, TimesheetVM header);

        void UpdateApproval(TimesheetVM header);

    }
}
