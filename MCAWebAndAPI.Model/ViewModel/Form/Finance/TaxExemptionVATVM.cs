using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    public class TaxExemptionVATVM : TaxExemptionBaseVM
    {
        /// <summary>
        /// FIN17: Tax Exemption
        /// </summary>

        [Required]
        [DisplayName("Total Tax Based (IDR)")]
        public decimal? TotalTaxBased { get; set; }

        [Required]
        [DisplayName("Total VAT not collected (IDR)")]
        public decimal? TotalVATNotCollected { get; set; }
    }
}