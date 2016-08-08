using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;

namespace MCAWebAndAPI.Service.Finance
{
    public interface ISCAVoucherService
    {
        SCAVoucherVM GetSCAVoucherVMData(int? ID);

        int? CreateSCAVoucher(SCAVoucherVM scaVoucher);

        int GetActivityIDByEventBudgetID(int eventBudgetID);

        void SetSiteUrl(string siteUrl);

        Task CreateSCAVoucherItem(int? scaVoucherID, IEnumerable<SCAVoucherItemsVM> viewModels);
    }
}
