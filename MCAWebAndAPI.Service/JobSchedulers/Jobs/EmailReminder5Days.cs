using MCAWebAndAPI.Service.Utils;
using Microsoft.SharePoint.Client;
using NLog;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MCAWebAndAPI.Service.JobSchedulers.Jobs
{
    public class EmailReminder5Days : IJob
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        public void Execute(IJobExecutionContext context)
        {
            const string SP_MANPOW_LIST_NAME = "Manpower Requisition";
            const string SP_PROMAS_LIST_NAME = "Professional Master";
            const string SP_PERFORMANCE_EVALUATION = "Professional Performance Evaluation";

            DateTime currentDate = DateTime.UtcNow;
            DateTime startDateApproval;
            string EmailTo, approver, requestor;
            string SubjectEmail;
            string EmailContent;
            string caml;
            int totalBusinessDays = 0;
            JobKey key = context.JobDetail.Key;
            JobDataMap dataMap = context.MergedJobDataMap;

            var siteUrl = dataMap.GetString("site-url");

            //Reminder Manpower HR01
            caml = @"<View><Query><Where><Eq><FieldRef Name='manpowerrequeststatus' /><Value Type='Text'>Pending Approval</Value></Eq></Where></Query><ViewFields><FieldRef Name='requestdate' /><FieldRef Name='ID' /><FieldRef Name='currentapprover' /><FieldRef Name='iskeyposition' /></ViewFields><QueryOptions /></View>";
            //caml = @"<View><Query>
            //           <Where>
            //              <Eq>
            //                 <FieldRef Name='manpowerrequeststatus' />
            //                 <Value Type='Text'>Pending Approval</Value>
            //              </Eq>
            //           </Where>
            //        </Query>
            //        <ViewFields>
            //           <FieldRef Name='requestdate' />
            //           <FieldRef Name='ID' />
            //           <FieldRef Name='iskeyposition' />
            //           <FieldRef Name='currentapprover' />
            //           <FieldRef Name='visibleto' />
            //        </ViewFields>
            //        <QueryOptions /></View>";
            foreach (var item in SPConnector.GetList(SP_MANPOW_LIST_NAME, siteUrl, caml))
            {
                startDateApproval = Convert.ToDateTime(item["requestdate"]).ToLocalTime();
                totalBusinessDays = BusinessDays(startDateApproval, currentDate);
                //approver = Convert.ToString(item["currentapprover"]);
                //requestor
                if (totalBusinessDays >= 5)
                {
                    requestor = Convert.ToString(item["visibleto"]);
                    approver = Convert.ToString(item["currentapprover"]);
                    if (Convert.ToBoolean(item["iskeyposition"]))
                    {
                        EmailTo = GetApprover("Executive Director", siteUrl);
                    }
                    else
                    {
                        EmailTo = GetApprover("Deputy Executive Director", siteUrl);
                    }
                    SubjectEmail = "Pending for Approval of Manpower Requisition";
                    EmailContent = String.Format("Dear {0}, <br> This alert is sent to you to notify that there is a pending action to approve the Manpower Requisition which has been requested by {1}. Please complete the approval process immediately. <br> Thank You. ", approver, requestor);
                    //EmailContent = "There are still Manpower Request yet to be processed (item id = " + Convert.ToInt32(item["ID"]).ToString() + ") and waiting for your approval. Please check your Manpower Approval Page";
                    EmailUtil.Send(EmailTo, SubjectEmail, EmailContent);
                }
            }

            //Reminder Professional Master HR06
            caml = @"<View><Query><Where><Eq><FieldRef Name='datavalidationstatus' /><Value Type='Choice'>Need HR to Validate</Value></Eq></Where></Query><ViewFields><FieldRef Name='ID' /><FieldRef Name='Modified' /></ViewFields><QueryOptions /></View>";
            foreach (var item in SPConnector.GetList(SP_PROMAS_LIST_NAME, siteUrl, caml))
            {
                startDateApproval = Convert.ToDateTime(item["Modified"]).ToLocalTime();
                totalBusinessDays = BusinessDays(startDateApproval, currentDate);
                if (totalBusinessDays >= 5)
                {
                    List<string> EmailsHR = GetEmailHR(siteUrl);
                    foreach (var email in EmailsHR)
                    {
                        if (!(string.IsNullOrEmpty(email)))
                        {
                            SubjectEmail = "Still Not Approved After 5 Working Days";
                            EmailContent = "There are still Professional Request that need to be validated by HR (item id = " + Convert.ToInt32(item["ID"]).ToString() + "). Please check your Professional Master Page";
                            EmailUtil.Send(email, SubjectEmail, EmailContent);
                        }
                    }
                }
            }

            //Reminder Performance Evaluation HR20
            caml = @"<View><Query><Where><Eq><FieldRef Name='ppestatus' /><Value Type='Choice'>Initiated</Value></Eq></Where></Query><ViewFields><FieldRef Name='ID' /><FieldRef Name='Created' /><FieldRef Name='professional_x003a_Office_x0020_' /></ViewFields><QueryOptions /></View>";
            foreach (var item in SPConnector.GetList(SP_PERFORMANCE_EVALUATION, siteUrl, caml))
            {
                startDateApproval = Convert.ToDateTime(item["Created"]).ToLocalTime();
                totalBusinessDays = BusinessDays(startDateApproval, currentDate);
                if (totalBusinessDays >= 5)
                {
                    if (((item["professional_x003a_Office_x0020_"] as FieldLookupValue)) != null)
                    {
                        EmailTo = (item["professional_x003a_Office_x0020_"] as FieldLookupValue).LookupValue;
                        if (EmailTo != null)
                        {
                            SubjectEmail = "Still Not Processed After 5 Working Days";
                            EmailContent = "Your Evaluation Plan is yet to be processed. Please create your evaluation in your Performance Evaluation Page";
                            EmailUtil.Send(EmailTo, SubjectEmail, EmailContent);
                        }



                    };


                }
            }


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
                int firstDayOfWeek = firstDay.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)firstDay.DayOfWeek;
                int lastDayOfWeek = lastDay.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)lastDay.DayOfWeek;
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
        public string GetApprover(string Position, string _siteUrl)
        {
            string email = "anugerahseptian@gmail.com";
            string caml = @"<View><Query><Where><Eq><FieldRef Name='Position' /><Value Type='Lookup'>" + Position + "</Value></Eq></Where></Query><ViewFields><FieldRef Name='officeemail' /></ViewFields><QueryOptions /></View>";
            foreach (var item in SPConnector.GetList("Professional Master", _siteUrl, caml))
            {
                email = Convert.ToString(item["officeemail"]);
            }
            return email;
        }

        public List<string> GetEmailHR(string _siteUrl)
        {
            List<string> EmailHR = new List<string>();
            string caml = @"<View><Query><Where><Contains><FieldRef Name='Position' />
            <Value Type='Lookup'>HR</Value></Contains></Where></Query><ViewFields><FieldRef Name='officeemail' />
            <FieldRef Name='Position' /></ViewFields><QueryOptions /></View>";
            foreach (var item in SPConnector.GetList("Professional Master", _siteUrl, caml))
            {
                EmailHR.Add(Convert.ToString(item["officeemail"]));
            }
            return EmailHR;
        }
    }
}
