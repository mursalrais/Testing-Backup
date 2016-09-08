using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Common;
using MCAWebAndAPI.Model.ViewModel.Form.HR;

namespace MCAWebAndAPI.Service.HR.Timesheet
{
    public interface ITimesheetService
    {
        void SetSiteUrl(string siteUrl = null);

      //  TimesheetVM GetTimesheet(string userlogin, DateTime period);

        Task<TimesheetVM> GetTimesheetAsync(string userlogin, DateTime period);

        Task<TimesheetVM> GetTimesheetLoadUpdate(int? id, string userlogin);

        IEnumerable<TimesheetDetailVM> GetTimesheetDetails(string userlogin, DateTime period, string strName);

        IEnumerable<TimesheetDetailVM> AppendWorkingDays(int? id,IEnumerable<TimesheetDetailVM> currentDays, 
            DateTime from, DateTime to, bool isFullDay, string location = null, int? locationid = null);

        int CreateHeader(TimesheetVM header);

        void UpdateHeader(TimesheetVM header);

        Task CreateTimesheetDetailsAsync(int? headerId, IEnumerable<TimesheetDetailVM> timesheetDetails);
        void CreateTimesheetDetails(int? headerId, IEnumerable<TimesheetDetailVM> timesheetDetails);

        Task CreateWorkflowTimesheetAsync(int? headerId, IEnumerable<WorkflowItemVM> workflowItems, string strStatus);
        void CreateWorkflowTimesheet(int? headerId, IEnumerable<WorkflowItemVM> workflowItems);

        void UpdateApproval(TimesheetVM header);

        IEnumerable<TimesheetDetailVM> DeleteSelectedWorkingDays(int? headerId,IEnumerable<TimesheetDetailVM> currentDays , 
            DateTime from, DateTime to);

        TimesheetVM GetTimesheetProfessional(string userlogin);

    }
}
