using MCAWebAndAPI.Model.ViewModel.Form.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.HR
{
    public interface IPSAManagementService
    {
        void SetSiteUrl(string siteUrl);

        PSAManagementVM GetPopulatedModel(int? id = null);

        int CreatePSA(PSAManagementVM psaManagement);

        PSAManagementVM GetPSAManagement(int ID);

        bool UpdatePSAManagement(PSAManagementVM psaManagement);
    }
}
