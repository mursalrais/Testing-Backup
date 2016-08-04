using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Finance;

namespace MCAWebAndAPI.Service.Finance
{
    public class TaxReimbursementService : ITaxReimbursementService
    {
        string _siteUrl = null;

        public TaxReimbursementVM GetTaxReimbursement(int? id = default(int?))
        {
            return new TaxReimbursementVM();
        }

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }


    }
}
