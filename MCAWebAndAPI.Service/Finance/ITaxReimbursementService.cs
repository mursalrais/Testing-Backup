using MCAWebAndAPI.Model.ViewModel.Form.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.Finance
{
    public interface ITaxReimbursementService
    {
        void SetSiteUrl(string siteUrl);

        TaxReimbursementVM GetTaxReimbursement(int? id = null);
    }
}
