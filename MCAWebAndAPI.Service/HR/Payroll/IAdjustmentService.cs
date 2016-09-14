using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Model.ViewModel.Form.HR;
using System.Collections.Generic;
using System.Web;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.HR.Payroll
{
    public interface IAdjustmentService
    {
        void SetSiteUrl(string siteUrl = null);

        AdjustmentDataVM GetAjusmentData(string period);

        AdjustmentDataVM GetPeriod(int? id);

        void CreateAdjustmentData(string period, AdjustmentDataVM CompensatoryList);

        bool CheckRequest(AdjustmentDataVM header);        
    }
}
