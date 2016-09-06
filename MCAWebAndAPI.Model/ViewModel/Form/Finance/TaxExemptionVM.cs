using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using MCAWebAndAPI.Model.ViewModel.Control;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    public class TaxExemptionVM
    {
        /// <summary>
        /// FIN17: Tax Exemption
        /// </summary>

        private ComboBoxVM typeOfTax;

        public int? ID { get; set; }

        [DisplayName("Type Of Tax")]
        [UIHint("ComboBox")]
        public ComboBoxVM TypeOfTax
        {
            get
            {
                if (typeOfTax == null)
                {
                    typeOfTax = new ComboBoxVM();
                }
                return typeOfTax;
            }
            set
            {
                typeOfTax = value;
            }
        }

        [UIHint("TextArea")]
        public string Remarks { get; set; }

        public string DocumentUrl { get; set; }

        [UIHint("MultiFileUploader")]
        [DisplayName("Attachment")]
        public IEnumerable<HttpPostedFileBase> Documents { get; set; } = new List<HttpPostedFileBase>();

        public TaxExemptionIncomeVM TaxExemptionIncomeVM { get; set; }
        public TaxExemptionVATVM TaxExemptionVATVM { get; set; }
        public TaxExemptionOtherVM TaxExemptionOtherVM { get; set; }
    }
}
