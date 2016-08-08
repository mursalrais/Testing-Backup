using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;

namespace MCAWebAndAPI.Service.Common
{
    public interface IComboBoxService
    {
        void SetSiteUrl(string siteUrl);

        IEnumerable<ProfessionalMaster> GetProfessionals();

        IEnumerable<AjaxComboBoxVM> GetEventBudget();

        IEnumerable<AjaxComboBoxVM> GetSubActivity(int activityID);
    }
}
