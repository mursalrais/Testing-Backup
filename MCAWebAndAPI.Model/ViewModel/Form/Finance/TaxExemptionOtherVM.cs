using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    public class TaxExemptionOtherVM : TaxExemptionBaseVM
    {
        [DisplayName("Gross Income (IDR)")]
        public Decimal GrossIncome
        {
            get;
            set;
        }

        [DisplayName("Total Tax (IDR)")]
        public Decimal TotalTax
        {
            get;
            set;
        }
    }
}
