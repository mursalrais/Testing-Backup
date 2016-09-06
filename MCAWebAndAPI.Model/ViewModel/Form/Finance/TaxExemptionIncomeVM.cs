using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    public class TaxExemptionIncomeVM : TaxExemptionBaseVM
    {
        /// <summary>
        /// FIN17: Tax Exemption
        /// </summary>

        private ComboBoxVM typeOfWithHoldingTax;

        [Required]
        [DisplayName("Type Of Withholding Tax")]
        [UIHint("ComboBox")]
        public ComboBoxVM TypeOfWithHoldingTax
        {
            get
            {
                if (typeOfWithHoldingTax == null)
                {
                    typeOfWithHoldingTax = new ComboBoxVM();
                }
                return typeOfWithHoldingTax;
            }
            set
            {
                typeOfWithHoldingTax = value;
            }
        }

        [DisplayName("Total Income Recepients")]
        [UIHint("Int")]
        public int? TotalIncomeRecepients { get; set; }

        [Required(ErrorMessage = "asdadasdsa")]
        [DisplayName("Gross Income (IDR)")]
        [UIHint("Number")]
        public decimal? GrossIncome { get; set; }

        [Required]
        [DisplayName("Total Income Tax Borne by Government(IDR)")]
        [UIHint("Number")]
        public decimal? TotalIncomeTaxBorneByGovernment { get; set; }
    }
}