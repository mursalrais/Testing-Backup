using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using MCAWebAndAPI.Model.HR.DataMaster;
using MCAWebAndAPI.Model.ViewModel.Control;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;

namespace MCAWebAndAPI.Service.Finance
{
    public interface ISCAVoucherService
    {
        SCAVoucherVM Get(int? ID);

        IEnumerable<SCAVoucherVM> GetAll();

        IEnumerable<AjaxComboBoxVM> GetAllAjaxComboBoxVM();

        SCAVoucherVM GetEventBudget(int? ID);

        void CreateSCAVoucherItems(string siteUrl, int? scaVoucherID, IEnumerable<SCAVoucherItemsVM> viewModels);

        Task CreateSCAVoucherItemAsync(int? scaVoucherID, IEnumerable<SCAVoucherItemsVM> viewModels);

        Task CreateSCAVoucherAttachmentAsync(int? ID, IEnumerable<HttpPostedFileBase> attachment);

        Task UpdateSCAVoucherItem(int? scaVoucherID, IEnumerable<SCAVoucherItemsVM> viewModels);

        IEnumerable<SCAVoucherItemsVM> GetSCAVoucherItems(int? scaVoucherID);

        IEnumerable<SCAVoucherItemsVM> GetEventBudgetItems(int eventBudgetID);

        int? CreateSCAVoucher(ref SCAVoucherVM scaVoucher, IEnumerable<ProfessionalMaster> professionals = null);

        int GetActivityIDByEventBudgetID(int eventBudgetID);

        bool UpdateSCAVoucher(SCAVoucherVM scaVoucher, IEnumerable<ProfessionalMaster> professionals = null);

        bool UpdateStatusSCAVoucher(SCAVoucherVM scaVoucher);
        
        void DeleteDetail(int id);

        Tuple<int, string> GetIdAndNoByEventBudgetID(int eventBudgetId);
    }
}