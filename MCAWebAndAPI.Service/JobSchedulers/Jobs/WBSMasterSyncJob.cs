using MCAWebAndAPI.Service.ProjectManagement.Schedule;
using NLog;
using Quartz;

namespace MCAWebAndAPI.Service.JobSchedulers.Jobs
{
    public class WBSMasterSyncJob
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        public void Execute(IJobExecutionContext context)
        {
            JobKey key = context.JobDetail.Key;
            JobDataMap dataMap = context.MergedJobDataMap;

            var siteUrl = dataMap.GetString("site-url");

            WBSMasterSyncService _psaManagementService = new WBSMasterSyncService();
            _psaManagementService.SetSiteUrl(siteUrl);
            _psaManagementService.Sync();

            logger.Info("WBS Master Sync Job was successfully performed on {0}.", siteUrl);
        }
    }
}
