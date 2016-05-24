using MCAWebAndAPI.Model.ViewModel.Form.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.HR.DataMaster;

namespace MCAWebAndAPI.Service.HR.Recruitment
{
    public interface IPSAManagementService
    {
        void SetSiteUrl(string siteUrl);

        IEnumerable<PSAMaster> GetPSAs();

        int CreatePSAManagement(PSAManagementVM psaManagement);

        PSAManagementVM GetPSAManagement(int? ID);
        
        bool UpdatePSAManagement(PSAManagementVM psaManagement);
    }
}
