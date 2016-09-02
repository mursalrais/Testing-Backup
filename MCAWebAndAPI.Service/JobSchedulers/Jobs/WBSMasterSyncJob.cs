using MCAWebAndAPI.Service.ProjectManagement.Schedule.Common;
using NLog;
using Quartz;

namespace MCAWebAndAPI.Service.JobSchedulers.Jobs
{
    public class WBSMasterSyncJob : IJob
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        public void Execute(IJobExecutionContext context)
        {
            JobKey key = context.JobDetail.Key;
            JobDataMap dataMap = context.MergedJobDataMap;

            var siteUrl = dataMap.GetString("site-url");

            WBSMasterService _psaManagementService = new WBSMasterService();
            _psaManagementService.SetCompactProgramSiteUrl(siteUrl);
            _psaManagementService.Sync();

            logger.Info("WBS Master Sync Job was successfully performed on {0}.", siteUrl);
        }
    }
}
