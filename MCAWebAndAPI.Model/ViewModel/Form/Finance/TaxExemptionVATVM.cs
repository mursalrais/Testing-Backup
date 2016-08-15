using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    public class TaxExemptionVATVM : TaxExemptionBaseVM
    {
        [DisplayName("Total Tax Based (IDR)")]
        public Decimal TotalTaxBased
        {
            get;
            set;
        }

        [DisplayName("Total VAT not collected (IDR)")]
        public Decimal TotalVATNotCollected
        {
            get;
            set;
        }
    }
}
