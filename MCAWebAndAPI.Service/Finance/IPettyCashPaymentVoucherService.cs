using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;

namespace MCAWebAndAPI.Service.Finance
{
    /// <summary>
    /// Wireframe FIN10: Petty Cash Voucher
    ///     a.k.a.: Petty Cash Payment Voucher
    ///     a.k.a.: Petty Cash Advance Voucher
    /// </summary>
    
    public interface IPettyCashPaymentVoucherService
    {
        void SetSiteUrl(string siteUrl);
        PettyCashPaymentVoucherVM GetPettyCashPaymentVoucher(int? ID);

        int Create(PettyCashPaymentVoucherVM viewModel);
        
        bool Update(PettyCashPaymentVoucherVM viewModel);

        void CreatePettyCashAttachments(int? headerID, IEnumerable<HttpPostedFileBase> documents);
        void EditPettyCashAttachments(int? headerID, IEnumerable<HttpPostedFileBase> documents);
    }
}
