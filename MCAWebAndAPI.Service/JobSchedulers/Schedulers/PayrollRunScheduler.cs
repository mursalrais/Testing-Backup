using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz.Collection;
using Quartz.Impl.Matchers;
using Quartz.Spi;
using NLog;
using Quartz.Impl;
using MCAWebAndAPI.Service.JobSchedulers.Jobs;

namespace MCAWebAndAPI.Service.JobSchedulers.Schedulers
{
    public class PayrollRunScheduler
    {
        static Logger logger = LogManager.GetCurrentClassLogger();

        public static void DoNow_Once(string siteUrl, string filePath, int periodDay, int periodMonth, int periodYear)
        {
            // construct a scheduler factory
            ISchedulerFactory scheduleFactory = new StdSchedulerFactory();

            // get a scheduler
            IScheduler scheduler = scheduleFactory.GetScheduler();
            scheduler.Start();
            logger.Debug(string.Format("{0} has been started at {1} in site {2}",
                scheduler.SchedulerName, DateTime.Now.ToLongDateString(), siteUrl));

            IJobDetail job = JobBuilder.Create<PayrollRunJob>()
                .WithIdentity("payroll-run-insite-" + siteUrl)
                .UsingJobData("site-url", siteUrl) // passing variable
                .UsingJobData("file-path", filePath)
                .UsingJobData("period-day", periodDay)
                .UsingJobData("period-month", periodMonth)
                .UsingJobData("period-year", periodYear)
                .Build();

            // Trigger the job to run now, and then every 24 hours
            ITrigger trigger = TriggerBuilder.Create()
              .WithIdentity("start-now-once-insite-" + siteUrl, "repetitive-triggers")
              .StartNow() // start when?
              .Build();

            scheduler.ScheduleJob(job, trigger);
        }
    }
}
