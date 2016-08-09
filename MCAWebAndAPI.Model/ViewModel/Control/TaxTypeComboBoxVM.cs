using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Control
{
    public class TaxTypeComboBoxVM : ComboBoxVM
    {
        public TaxTypeComboBoxVM() : base()
        {
            this.Choices = new string[]
            {
                "Income",
                "VAT",
                "Others"
            };
            this.Value = "Income";
        }
    }
}
