using MCAWebAndAPI.Service.JobSchedulers.Jobs;
using NLog;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;

namespace MCAWebAndAPI.Service.JobSchedulers.Schedulers
{
    public class EmailScheduler
    {
        static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetEmails"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="siteUrl"></param>
        public static void SendNow_PerHourUntilFiveTimes(IEnumerable<string> targetEmails, 
            string subject, string body, string siteUrl = null)
        {
            // construct a scheduler factory
            ISchedulerFactory scheduleFactory = new StdSchedulerFactory();

            // get a scheduler
            IScheduler scheduler = scheduleFactory.GetScheduler();
            scheduler.Start();
            logger.Debug(string.Format("{0} has been started at {1}", scheduler.SchedulerName, DateTime.Now.ToLongDateString()));

            var _targetEmails = string.Empty;
            foreach(var email in targetEmails)
            {
                _targetEmails += email + ';';
            }

            IJobDetail job = JobBuilder.Create<EmailJob>()
                .WithIdentity("email-job", "alert-jobs") // put job name and job category
                .UsingJobData("target-emails", _targetEmails) // passing variable
                .UsingJobData("subject", subject) // passing variable
                .UsingJobData("body", body) // passing variable
                .UsingJobData("site-url", siteUrl) // passing variable
                .Build();

            // Trigger the job to run now, and then every 1 hour
            ITrigger trigger = TriggerBuilder.Create()
              .WithIdentity("start-now-per-hour-until-five-times", "repetitive-triggers")
              .StartNow() // start when?
              .WithSimpleSchedule(x => x
                  .WithIntervalInHours(1) // interval or how often?
                  .WithRepeatCount(5)) // repeat until how many times?
              .Build();

            scheduler.ScheduleJob(job, trigger);
        }


        public static void SendNow_Once(IEnumerable<string> targetEmails,
            string subject, string body, string siteUrl = null)
        {
            // construct a scheduler factory
            ISchedulerFactory scheduleFactory = new StdSchedulerFactory();

            // get a scheduler
            IScheduler scheduler = scheduleFactory.GetScheduler();
            scheduler.Start();
            logger.Debug(string.Format("{0} has been started at {1}", scheduler.SchedulerName, DateTime.Now.ToLongDateString()));

            var _targetEmails = string.Empty;
            foreach (var email in targetEmails)
            {
                _targetEmails += email + ';';
            }

            IJobDetail job = JobBuilder.Create<EmailJob>()
                .WithIdentity("email-job", "alert-jobs") // put job name and job category
                .UsingJobData("target-emails", _targetEmails) // passing variable
                .UsingJobData("subject", subject) // passing variable
                .UsingJobData("body", body) // passing variable
                .UsingJobData("site-url", siteUrl) // passing variable
                .Build();

            // Trigger the job to run now, and then every 1 hour
            ITrigger trigger = TriggerBuilder.Create()
              .WithIdentity("start-now-once", "onetime-triggers")
              .StartNow() // start when?
              .Build();

            scheduler.ScheduleJob(job, trigger);
        }

      }
}
