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
    public class TaxExemptionDataVM
    {
        private ComboBoxVM _typeOfTax;
        private ComboBoxVM _typeOfWithHoldingTax;
        private DateTime _taxPeriod;

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

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MMM-yyyy}")]
        [DisplayName("Tax Period")]
        public DateTime TaxPeriod
        {
            get
            {
                if (_taxPeriod == null)
                    _taxPeriod = new DateTime();
                return _taxPeriod;
            }
            set
            {
                _taxPeriod = value;
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

        [UIHint("TextArea")]
        public string Remarks
        {
            get;
            set;
        }

        public string DocumentUrl { get; set; }

        [UIHint("MultiFileUploader")]
        [DisplayName("Attachment")]
        public IEnumerable<HttpPostedFileBase> Documents { get; set; } = new List<HttpPostedFileBase>();
    }
}
