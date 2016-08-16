using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Control
{
    public class TaxTypeComboBoxVM : ComboBoxVM
    {
        public const string INCOME = "Income";
        public const string VAT = "VAT";
        public const string OTHERS = "Other";

        public TaxTypeComboBoxVM() : base()
        {

            this.Choices = new string[]
            {
                INCOME,
                VAT,
                OTHERS
            };
            this.Value = INCOME;
        }
    }
}
