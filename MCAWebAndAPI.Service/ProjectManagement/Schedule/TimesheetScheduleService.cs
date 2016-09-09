using System;
using System.Linq;
using MCAWebAndAPI.Model.ProjectManagement.Schedule;
using MCAWebAndAPI.Service.Utils;
using System.Collections.Generic;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Chart;
using MCAWebAndAPI.Service.HR.Common;
using Microsoft.SharePoint.Client;
using NLog;

namespace MCAWebAndAPI.Service.ProjectManagement.Schedule
{
  public  class TimesheetScheduleService : ITimesheetScheduleService
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        private IDataMasterService _dataService;

        const string SP_PROFESSIONAL_LIST_NAME = "Professional Master";

        string _siteUrl = null;

        public TimesheetScheduleService()
        {
            _dataService = new DataMasterService();

        }

        public void SetSiteUrl(string siteUrl)
      {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
            _dataService.SetSiteUrl(_siteUrl);
        }

     


        public void SendEmailEveryMonth()
        {
            if (DateTime.Now.ToLocalTime().ToString("dd") != "19") return;
            var professionalData = _dataService.GetProfessionals();

            List<string> lstEmail = (from item in professionalData
                                     where !string.IsNullOrEmpty(item.OfficeEmail)
                                     select item.OfficeEmail).ToList();

            var strSubject = "Reminder to Create Timesheet";
            var strBody = "";

            var startPeriod = DateTime.Now.ToLocalTime().GetFirstPayrollDay();
            var endPeriod = DateTime.Now.ToLocalTime().GetLastPayrollDay();
            strBody += "Dear Professional,<br/><br/>";
            strBody += @"This email serves as reminder that your timesheet for period " + startPeriod.ToString("MMMM dd yyyy") + " to " +
                       endPeriod.ToString("MMMM dd yyyy") + " is due today.<br/>";
            strBody += @"An action on your part is now required to complete and submit the timesheet to your supervisor soon.<br/>";
            strBody += @"Please be informed that your payroll will not be processed until your timesheet is approved.";
            strBody += "<br/><br/>Thank you for your attention.";


            EmailUtil.SendMultiple(lstEmail, strSubject, strBody);
        }
    }
}
