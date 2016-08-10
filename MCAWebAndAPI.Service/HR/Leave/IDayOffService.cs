using MCAWebAndAPI.Model.ViewModel.Form.HR;
using System.Collections.Generic;
using MCAWebAndAPI.Model.HR.DataMaster;
using System.Web;
using System;

namespace MCAWebAndAPI.Service.HR.Leave
{
    public interface IDayOffService
    {
        void SetSiteUrl(string siteUrl);

        void PopulateBalance(int idPSA,  PSAManagementVM viewModel, string action);

        int GetUnpaidDayOffTotalDays(int professionalID, IEnumerable<DateTime> dateRange);

        bool IsUnpaidDayOff(int professionalID, DateTime date, IEnumerable<DateTime> dateRange);

    }
}
