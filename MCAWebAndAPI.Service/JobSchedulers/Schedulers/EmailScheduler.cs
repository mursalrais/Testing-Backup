using MCAWebAndAPI.Service.JobSchedulers.Jobs;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.JobSchedulers.Schedulers
{
    public class EmailScheduler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetEmails"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="siteUrl"></param>
        public static void SendNow_PerHour_UntilFiveTimes(IEnumerable<string> targetEmails, string subject, string body, string siteUrl = null)
        {
            // construct a scheduler factory
            ISchedulerFactory schedFact = new StdSchedulerFactory();

            // get a scheduler
            IScheduler sched = schedFact.GetScheduler();
            sched.Start();

            var _targetEmails = string.Empty;
            foreach(var email in targetEmails)
            {
                _targetEmails += email + ';';
            }

            IJobDetail job = JobBuilder.Create<EmailJob>()
                .WithIdentity("email-job", "alert-jobs")
                .UsingJobData("target-emails", _targetEmails)
                .UsingJobData("subject", subject)
                .UsingJobData("body", body)
                .UsingJobData("site-url", siteUrl)
                .Build();

            // Trigger the job to run now, and then every 1 hour
            ITrigger trigger = TriggerBuilder.Create()
              .WithIdentity("start-now-per-hour-until-five-times", "repetitive-triggers")
              .StartNow()
              .WithSimpleSchedule(x => x
                  .WithIntervalInHours(1)
                  .WithRepeatCount(5))
              .Build();

            sched.ScheduleJob(job, trigger);
        }
    }
}
