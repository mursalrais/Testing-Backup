using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    public class TaxExemptionOtherVM : TaxExemptionBaseVM
    {
        /// <summary>
        /// FIN17: Tax Exemption
        /// </summary>

        [Required]
        [DisplayName("Gross Income (IDR)")]
        public decimal? GrossIncome { get; set; }

        [Required]
        [DisplayName("Total Tax (IDR)")]
        public decimal? TotalTax { get; set; }
    }
}