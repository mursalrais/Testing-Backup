using MCAWebAndAPI.Service.HR.Payroll;
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
    public class PayrollRunJob : IJob
    {
        Logger logger = LogManager.GetCurrentClassLogger();

        public void Execute(IJobExecutionContext context)
        {
            JobKey key = context.JobDetail.Key;
            JobDataMap dataMap = context.MergedJobDataMap;

            var siteUrl = dataMap.GetString("site-url");
            var filePath = dataMap.GetString("file-path");
            var period = 
                new DateTime(
                    dataMap.GetInt("period-year"),
                    dataMap.GetInt("period-month"),
                    dataMap.GetInt("period-day"));


            IPayrollService _payrollService = new PayrollService();
            _payrollService.SetSiteUrl(siteUrl);
            _payrollService.SavePayrollWorksheetDetailInBackground(period, filePath);

            logger.Info("Task Calculation Job at {0} has been successfully performed", siteUrl);
        }
    }
}
