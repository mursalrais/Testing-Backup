using System.Collections.Generic;
using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Service.Finance;

namespace MCAWebAndAPI.Service.Common
{
    public interface IComboBoxService
    {
        void SetSiteUrl(string siteUrl);

        IEnumerable<ProfessionalMaster> GetProfessionals();

        IEnumerable<AjaxComboBoxVM> GetSubActivities(int activityID);
    }
}
