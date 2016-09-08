using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    public class TaxExemptionVM
    {
        private ComboBoxVM _typeOfTax;

        public int? ID { get; set; }

        [DisplayName("Type Of Tax")]
        [UIHint("ComboBox")]
        public ComboBoxVM TypeOfTax
        {
            get
            {
                if (_typeOfTax == null)
                {
                    _typeOfTax = new ComboBoxVM();
                }
                return _typeOfTax;
            }
            set
            {
                _typeOfTax = value;
            }
        }

        [UIHint("TextArea")]
        public string Remarks
        {
            get;
            set;
        }

        public string DocumentUrl
        {
            get;
            set;

        }

        [UIHint("MultiFileUploader")]
        [DisplayName("Attachment")]
        public IEnumerable<HttpPostedFileBase> Documents { get; set; } = new List<HttpPostedFileBase>();

        public TaxExemptionIncomeVM TaxExemptionIncomeVM { get; set; }
        public TaxExemptionVATVM TaxExemptionVATVM { get; set; }
        public TaxExemptionOtherVM TaxExemptionOtherVM { get; set; }
    }
}
