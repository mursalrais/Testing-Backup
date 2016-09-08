using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Service.Utils;

namespace MCAWebAndAPI.Service.ProjectManagement.Schedule
{
    public class WBSMasterSyncService : IWBSMasterSyncService
    {
        string siteUrl = null;

        public void SetSiteUrl(string siteUrl)
        {
            this.siteUrl = FormatUtil.ConvertToCleanSiteUrl(siteUrl);
        }

        public void Sync()
        {
            //what's the meat???
        }
    }
}
