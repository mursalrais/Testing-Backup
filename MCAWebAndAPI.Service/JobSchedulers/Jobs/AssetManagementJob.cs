using MCAWebAndAPI.Service.Asset;
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
    public class AssetManagementJob : IJob
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        public void Execute(IJobExecutionContext context)
        {
            JobKey key = context.JobDetail.Key;
            JobDataMap dataMap = context.MergedJobDataMap;

            var siteUrl = dataMap.GetString("site-url");

            IAssetLoanAndReturnService assetLoanAndReturnService = new AssetLoanAndReturnService();
           // IPSAScheduleService _psaManagementService = new PSAScheduleService();

            IAssetScheduleService _assetManagementService = new AssetScheduleService();

            _assetManagementService.SetSiteUrl(siteUrl);
            _assetManagementService.CheckTwoMonthsBeforeExpireDate();
         


            logger.Info("Task Calculation Job at {0} has been successfully performed", siteUrl);
        }

    }
}
