using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;

namespace MCAWebAndAPI.Service.Finance
{
    public interface IPettyCashPaymentVoucherService
    {
        void SetSiteUrl(string siteUrl);
        PettyCashPaymentVoucherVM GetPettyCashPaymentVoucher(int? ID);

    }
}
