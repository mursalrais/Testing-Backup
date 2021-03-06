﻿using MCAWebAndAPI.Service.JobSchedulers.Jobs;
using NLog;
using Quartz;
using Quartz.Impl;
using System;

namespace MCAWebAndAPI.Service.JobSchedulers.Schedulers
{
    /// <summary>
    /// 
    /// </summary>
    public class TaskCalculationScheduler
    {
        static Logger logger = LogManager.GetCurrentClassLogger();

        public static void DoNow_OnceEveryDay(string siteUrl = null)
        {
            // construct a scheduler factory
            ISchedulerFactory scheduleFactory = new StdSchedulerFactory();

            // get a scheduler
            IScheduler scheduler = scheduleFactory.GetScheduler();
            scheduler.Start();
            logger.Debug(string.Format("{0} has been started at {1} in site {2}",
                scheduler.SchedulerName, DateTime.Now.ToLongDateString(), siteUrl));

            IJobDetail job = JobBuilder.Create<TaskCalculationJob>()
                .WithIdentity("calculate-task-insite-" + siteUrl)
                .UsingJobData("site-url", siteUrl) // passing variable
                .Build();

            // Trigger the job to run now, and then every 24 hours
            ITrigger trigger = TriggerBuilder.Create()
              .WithIdentity("start-now-per-day-insite-" + siteUrl, "repetitive-triggers")
              .StartNow() // start when?
              .WithSimpleSchedule(x => x
                  .WithIntervalInHours(24)) // interval or how often?
              .Build();

            scheduler.ScheduleJob(job, trigger);
        }

        public static void EmailReminderDoNow_OnceEveryDay(string siteUrl)
        {
            // construct a scheduler factory
            ISchedulerFactory scheduleFactory = new StdSchedulerFactory();

            // get a scheduler
            IScheduler scheduler = scheduleFactory.GetScheduler();
            scheduler.Start();
            logger.Debug(string.Format("{0} has been started at {1} in site {2}",
                scheduler.SchedulerName, DateTime.Now.ToLongDateString(), siteUrl));

            IJobDetail job = JobBuilder.Create<EmailReminder5Days>()
                .WithIdentity("calculate-task-insite-" + siteUrl)
                .UsingJobData("site-url", siteUrl) // passing variable
                .Build();

            // Trigger the job to run now, and then every 24 hours
            ITrigger trigger = TriggerBuilder.Create()
              .WithIdentity("start-now-per-day-insite-" + siteUrl, "repetitive-triggers")
              .StartNow() // start when?
              .WithSimpleSchedule(x => x
                  .WithIntervalInHours(24)) // interval or how often?
              .Build();

            scheduler.ScheduleJob(job, trigger);
        }
    }
}



