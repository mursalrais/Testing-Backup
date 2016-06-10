using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.JobSchedulers.Jobs
{
    public class EmailExternalJob : IJob
    {
        //TODO: To finish external email with outlook account
        public void Execute(IJobExecutionContext context)
        {
            JobKey key = context.JobDetail.Key;
            JobDataMap dataMap = context.MergedJobDataMap;

            IEnumerable<string> targetEmails = dataMap.GetString("target-emails").Split(';')
                .ToList().Where(e => !string.IsNullOrEmpty(e));
            var subject = dataMap.GetString("subject");
            var body = dataMap.GetString("body");
            var siteUrl = dataMap.GetString("site-url");

        }
    }
}
