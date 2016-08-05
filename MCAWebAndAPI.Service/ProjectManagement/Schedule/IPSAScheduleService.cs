using System.Collections.Generic;
using MCAWebAndAPI.Model.ProjectManagement.Schedule;
using System.Linq;
using MCAWebAndAPI.Model.ViewModel.Chart;
using MCAWebAndAPI.Model.ViewModel.Gantt;

namespace MCAWebAndAPI.Service.ProjectManagement.Schedule
{
    public interface IPSAScheduleService
    {
        void SetSiteUrl(string siteUrl);

        bool CheckTwoMonthsBeforeExpireDate();

        void changePSAstatus();

    }
}
