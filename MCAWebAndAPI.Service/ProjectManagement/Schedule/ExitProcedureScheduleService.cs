using System;
using System.Linq;
using MCAWebAndAPI.Model.ProjectManagement.Schedule;
using MCAWebAndAPI.Service.Utils;
using System.Collections.Generic;
using MCAWebAndAPI.Model.ViewModel.Chart;
using Microsoft.SharePoint.Client;
using NLog;
using MCAWebAndAPI.Model.ViewModel.Gantt;
namespace MCAWebAndAPI.Service.ProjectManagement.Schedule
{
    public class ExitProcedureScheduleService : IExitProcedureScheduleService
    {
        static Logger logger = LogManager.GetCurrentClassLogger();

        const string SP_EXIT_CHECKLIST_LIST_NAME = "Exit Procedure Checklist";
        const string SP_EXIT_PROCEDURE_LIST_NAME = "Exit Procedure";
        const string SP_PROF_MASTER = "Professional Master";

        string _siteUrl = null;

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

        public bool FiveDaysStillNotApproved()
        {
            DateTime startDateApproval;
            int? approverID;
            string approverName;
            DateTime today = DateTime.Now;
            string strToday = today.ToLocalTime().ToShortDateString();

            var camlPendingApproval = @"<View>  
            <Query> 
               <Where><Eq><FieldRef Name='checklistitemapproval' /><Value Type='Choice'>Pending Approval</Value></Eq></Where> 
            </Query> 
      </View>";

            foreach (var exitChecklist in SPConnector.GetList(SP_EXIT_CHECKLIST_LIST_NAME, _siteUrl, camlPendingApproval))
            {
                startDateApproval = Convert.ToDateTime(exitChecklist["startdateapproval"]).ToLocalTime();
                approverID = FormatUtil.ConvertLookupToID(exitChecklist, "approverusername");

                var professionalData = SPConnector.GetListItem(SP_PROF_MASTER, approverID, _siteUrl);
                approverName = Convert.ToString(professionalData["Title"]);

                DateTime fiveDaysAfterSubmitApproval = startDateApproval.AddDays(5);

                int totalBusinessDays = BusinessDays(startDateApproval, fiveDaysAfterSubmitApproval);

                if(totalBusinessDays == 5)
                {
                    DateTime trulyFiveBusinessDay = fiveDaysAfterSubmitApproval.AddDays(-1);

                    string fiveDaysStillNotApproved = trulyFiveBusinessDay.ToShortDateString();
                    
                    if (fiveDaysStillNotApproved == strToday)
                    {
                        string approvalMail = Convert.ToString(exitChecklist["approvalmail"]);
                        int? exitProcedureID = FormatUtil.ConvertLookupToID(exitChecklist, "exitprocedure").Value;

                        string requestorName = GetRequestorName(exitProcedureID);

                        string subjectMail = "Pending Action for Exit Checklist Approval";
                        //string contentMail = string.Format("There still a request for exit procedure from: {0} for item: {1} needed approval from you", requestorName, Convert.ToString(exitChecklist["Title"]));
                        string contentMail = string.Format("Dear {0}, {1}{2}This alert is sent to you to notify that there is a pending action on Exit Checklist Approval of {3}. Please complete the process immediately.{4}{5}Thank you.", approverName,Environment.NewLine, Environment.NewLine, requestorName, Environment.NewLine, Environment.NewLine);

                        SendMailTwoMonthBeforeExpired(approvalMail, subjectMail, contentMail);
                    }
                }
                if(totalBusinessDays == 3)
                {
                    DateTime addTwoBusinessDays = fiveDaysAfterSubmitApproval.AddDays(2);
                    string strAddTwoBusinessDays = addTwoBusinessDays.ToLocalTime().ToShortDateString();

                    if (strAddTwoBusinessDays == strToday)
                    {
                        string approvalMail = Convert.ToString(exitChecklist["approvalmail"]);
                        int? exitProcedureID = FormatUtil.ConvertLookupToID(exitChecklist, "exitprocedure").Value;

                        string requestorName = GetRequestorName(exitProcedureID);

                        string subjectMail = "Pending Action for Exit Checklist Approval";
                        //string contentMail = string.Format("There still a request for exit procedure from: {0} for item: {1} needed approval from you", requestorName, Convert.ToString(exitChecklist["Title"]));
                        string contentMail = string.Format("Dear {0}, {1}{2}This alert is sent to you to notify that there is a pending action on Exit Checklist Approval of {3}. Please complete the process immediately.{4}{5}Thank you.", approverName, Environment.NewLine, Environment.NewLine, requestorName, Environment.NewLine, Environment.NewLine);

                        SendMailTwoMonthBeforeExpired(approvalMail, subjectMail, contentMail);
                    }
                }
                if (totalBusinessDays == 4)
                {
                    DateTime addOneBusinessDays = fiveDaysAfterSubmitApproval.AddDays(1);
                    string strAddOneBusinessDays = addOneBusinessDays.ToLocalTime().ToShortDateString();

                    if (strAddOneBusinessDays == strToday)
                    {
                        string approvalMail = Convert.ToString(exitChecklist["approvalmail"]);
                        int? exitProcedureID = FormatUtil.ConvertLookupToID(exitChecklist, "exitprocedure").Value;

                        string requestorName = GetRequestorName(exitProcedureID);

                        string subjectMail = "Pending Action for Exit Checklist Approval";
                        //string contentMail = string.Format("There still a request for exit procedure from: {0} for item: {1} needed approval from you", requestorName, Convert.ToString(exitChecklist["Title"]));
                        string contentMail = string.Format("Dear {0}, {1}{2}This alert is sent to you to notify that there is a pending action on Exit Checklist Approval of {3}. Please complete the process immediately.{4}{5}Thank you.", approverName, Environment.NewLine, Environment.NewLine, requestorName, Environment.NewLine, Environment.NewLine);

                        SendMailTwoMonthBeforeExpired(approvalMail, subjectMail, contentMail);
                    }
                }
            }
            
            return true;
        }

        public string GetRequestorName(int? exitProcedureID)
        {
            var exitProcedureData = SPConnector.GetListItem(SP_EXIT_PROCEDURE_LIST_NAME, exitProcedureID, _siteUrl);
            string requestorName = Convert.ToString(exitProcedureData["Title"]);

            return requestorName;
        }

        public void SendMailTwoMonthBeforeExpired(string approvalMail, string subjectMail, string contentMail)
        {
            EmailUtil.Send(approvalMail, subjectMail, contentMail);
        }

        public int BusinessDays(DateTime firstDay, DateTime lastDay, params DateTime[] bankHolidays)
        {
            firstDay = firstDay.Date;
            lastDay = lastDay.Date;
            if (firstDay > lastDay)
                throw new ArgumentException("Incorrect last day " + lastDay);

            TimeSpan span = lastDay - firstDay;
            int businessDays = span.Days + 1;
            int fullWeekCount = businessDays / 7;
            // find out if there are weekends during the time exceedng the full weeks
            if (businessDays > fullWeekCount * 7)
            {
                // we are here to find out if there is a 1-day or 2-days weekend
                // in the time interval remaining after subtracting the complete weeks
                int firstDayOfWeek = firstDay.DayOfWeek == DayOfWeek.Sunday? 7 : (int)firstDay.DayOfWeek;
                int lastDayOfWeek = lastDay.DayOfWeek == DayOfWeek.Sunday? 7 : (int)lastDay.DayOfWeek;
                if (lastDayOfWeek < firstDayOfWeek)
                    lastDayOfWeek += 7;
                if (firstDayOfWeek <= 6)
                {
                    if (lastDayOfWeek >= 7)// Both Saturday and Sunday are in the remaining time interval
                        businessDays -= 2;
                    else if (lastDayOfWeek >= 6)// Only Saturday is in the remaining time interval
                        businessDays -= 1;
                }
                else if (firstDayOfWeek <= 7 && lastDayOfWeek >= 7)// Only Sunday is in the remaining time interval
                    businessDays -= 1;
            }

            // subtract the weekends during the full weeks in the interval
            businessDays -= fullWeekCount + fullWeekCount;

            // subtract the number of bank holidays during the time interval
            foreach (DateTime bankHoliday in bankHolidays)
            {
                DateTime bh = bankHoliday.Date;
                if (firstDay <= bh && bh <= lastDay)
                    --businessDays;
            }

            return businessDays;
        }

    }
}
