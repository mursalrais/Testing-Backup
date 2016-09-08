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
        public int? TotalIncomeRecepients { get; set; }

        [Required(ErrorMessage = "asdadasdsa")]
        [DisplayName("Gross Income (IDR)")]
        public decimal? GrossIncome { get; set; }

        [Required]
        [DisplayName("Total Income Tax Borne by Government(IDR)")]
        public decimal? TotalIncomeTaxBorneByGovernment { get; set; }
    }
}