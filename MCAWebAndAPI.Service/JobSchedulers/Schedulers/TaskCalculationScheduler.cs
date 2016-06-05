﻿using MCAWebAndAPI.Service.JobSchedulers.Jobs;
using NLog;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.JobSchedulers.Schedulers
{
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
                .UsingJobData("site-url", siteUrl) // passing variable
                .Build();

            // Trigger the job to run now, and then every 1 hour
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