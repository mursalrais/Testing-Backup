using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.ProjectManagement.Schedule
{
    public interface IAssetScheduleService
    {
        void SetSiteUrl(string siteUrl);

        bool CheckTwoMonthsBeforeExpireDate();
    }
}
