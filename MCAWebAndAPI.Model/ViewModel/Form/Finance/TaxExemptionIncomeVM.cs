using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    public class TaxExemptionIncomeVM : TaxExemptionBaseVM
    {
        private ComboBoxVM _typeOfWithHoldingTax;

        [DisplayName("Type Of Withholding Tax")]
        [UIHint("ComboBox")]
        public ComboBoxVM TypeOfWithHoldingTax
        {
            get
            {
                if (_typeOfWithHoldingTax == null)
                {
                    _typeOfWithHoldingTax = new ComboBoxVM();
                }
                return _typeOfWithHoldingTax;
            }
            set
            {
                _typeOfWithHoldingTax = value;
            }
        }

        [DisplayName("Total Income Recepients")]
        public int TotalIncomeRecepients
        {
            get;
            set;
        }

        [DisplayName("Gross Income (IDR)")]
        public decimal GrossIncome
        {
            get;
            set;
        }


        [DisplayName("Total Income Tax Borne by Government(IDR)")]
        public decimal TotalIncomeTaxBorneByGovernment
        {
            get;
            set;
        }
    }
}
