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
    public class ExitProcedureManagementJob : IJob
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        public void Execute(IJobExecutionContext context)
        {
            JobKey key = context.JobDetail.Key;
            JobDataMap dataMap = context.MergedJobDataMap;

            var siteUrl = dataMap.GetString("site-url");

            IExitProcedureScheduleService _exitProcedureManagementService = new ExitProcedureScheduleService();
            _exitProcedureManagementService.FiveDaysStillNotApproved();

            logger.Info("Task Calculation Job at {0} has been successfully performed", siteUrl);
        }

    }
}
