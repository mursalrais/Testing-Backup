using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Control
{
    public class CurrencyComboBoxVM : ComboBoxVM
    {
        public CurrencyComboBoxVM() : base()
        {
            this.Choices = new string[]
            {
                "USD",
                "IDR"
            };

            this.Value = "IDR";
        }
    }
}
