using MCAWebAndAPI.Model.ViewModel.Form.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.HR.Payroll
{
    public interface IFeeSlipService
    {
        void SetSiteUrl(string siteUrl);

        FeeSlipVM GetPopulatedModel();

        //FeeSlipVM GetHeader(int? ID);

        //int CreateHeader(FeeSlipVM header);

        //bool UpdateHeader(FeeSlipVM header);

        //void CreateFeeSlipDetails(int? headerID, IEnumerable<FeeSlipDetailVM> feeSlipDetails);
    }
}
