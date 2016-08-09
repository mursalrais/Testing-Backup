using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;

namespace MCAWebAndAPI.Service.Finance
{
    public interface ISCAVoucherService
    {
        SCAVoucherVM GetSCAVoucherVMData(int? ID);

        SCAVoucherVM GetEventBudget(int? ID);

        Task CreateSCAVoucherItem(int? scaVoucherID, IEnumerable<SCAVoucherItemsVM> viewModels);

        Task CreateSCAVoucherDocumentAsync(int? ID, IEnumerable<HttpPostedFileBase> attachment);

        IEnumerable<SCAVoucherItemsVM> GetSCAVoucherItems(int scaVoucherID);

        IEnumerable<SCAVoucherItemsVM> GetEventBudgetItems(int eventBudgetID);

        int? CreateSCAVoucher(SCAVoucherVM scaVoucher);

        int GetActivityIDByEventBudgetID(int eventBudgetID);

        void SetSiteUrl(string siteUrl);
    }
}
