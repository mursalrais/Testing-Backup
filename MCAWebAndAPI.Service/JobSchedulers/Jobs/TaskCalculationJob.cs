using MCAWebAndAPI.Service.ProjectManagement.Schedule;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.JobSchedulers.Jobs
{
    public class TaskCalculationJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            JobKey key = context.JobDetail.Key;
            JobDataMap dataMap = context.MergedJobDataMap;

            var siteUrl = dataMap.GetString("site-url");


            ITaskService _taskService = new TaskService();
            _taskService.SetSiteUrl(siteUrl);
            _taskService.CalculateTaskColumns();
        }
    }
}
