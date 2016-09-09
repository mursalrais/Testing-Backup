using MCAWebAndAPI.Model.ViewModel.Form.HR;
using System.Collections.Generic;
using MCAWebAndAPI.Model.HR.DataMaster;
using System.Web;
using System;

namespace MCAWebAndAPI.Service.HR.Leave
{
    public interface IHRDayOffBalanceService
    {
        void SetSiteUrl(string siteUrl);

        void PopulateBalance(int idPSA,  PSAManagementVM viewModel, string action);
    }
}
