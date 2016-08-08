using MCAWebAndAPI.Service.ProjectManagement.Schedule;
using NLog;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.JobSchedulers.Jobs
{
    public class PSAManagementJob : IJob
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        public void Execute(IJobExecutionContext context)
        {
            JobKey key = context.JobDetail.Key;
            JobDataMap dataMap = context.MergedJobDataMap;

            var siteUrl = dataMap.GetString("site-url");

            IPSAScheduleService _psaManagementService = new PSAScheduleService();
            _psaManagementService.SetSiteUrl(siteUrl);
            _psaManagementService.changePSAstatus();
            _psaManagementService.CheckTwoMonthsBeforeExpireDate();
           


            logger.Info("Task Calculation Job at {0} has been successfully performed", siteUrl);
        }

        //public void ExecutePSAExpired(IJobExecutionContext context)
        //{
        //    JobKey key = context.JobDetail.Key;
        //    JobDataMap dataMap = context.MergedJobDataMap;

        //    var siteUrl = dataMap.GetString("site-url");

        //    IPSAScheduleService _psaManagementService = new PSAScheduleService();
        //    _psaManagementService.SetSiteUrl(siteUrl);
        //    _psaManagementService.changePSAstatus();



        //    logger.Info("Task Calculation Job at {0} has been successfully performed", siteUrl);
        //}

    }
}
