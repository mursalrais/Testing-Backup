using MCAWebAndAPI.Service.Utils;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MCAWebAndAPI.Service.JobSchedulers.Jobs
{
    public class EmailJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            JobKey key = context.JobDetail.Key;
            JobDataMap dataMap = context.MergedJobDataMap;
            
            IEnumerable<string> targetEmails = dataMap.GetString("target-emails").Split(';')
                .ToList().Where(e => !string.IsNullOrEmpty(e));
            var subject = dataMap.GetString("subject");
            var body = dataMap.GetString("body");
            var siteUrl = dataMap.GetString("site-url");

            SPConnector.SendEmails(targetEmails, body, subject, siteUrl);
        }
    }
}
