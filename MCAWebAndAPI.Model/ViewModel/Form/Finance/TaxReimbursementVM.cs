using MCAWebAndAPI.Model.Common;
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
    public class TaxReimbursementVM : Item
    {
        ////public enum TaxType { Income = 1, VAT = 2, Others = 3 }
        public enum CategoryType { Vendor, MCAI }

        private const string TaxTypeIncomeTax = "Income Tax";
        private const string TaxTypeVAT = "VAT";
        private const string TaxTypeOthers = "others";

        ////private Dictionary<TaxType, string> taxChoices = new Dictionary<TaxType, string>();


        private TaxTypeComboBoxVM type;

        ////public TaxReimbursementVM()
        ////{
        ////    this.taxChoices.Add(TaxType.Income, TaxTypeIncomeTax);
        ////    this.taxChoices.Add(TaxType.VAT, TaxTypeVAT);
        ////    this.taxChoices.Add(TaxType.Others, TaxTypeOthers);
        ////}

        [Required]
        [DisplayName("Type of Tax")]
        [UIHint("ComboBox")]
        public TaxTypeComboBoxVM Type
        {
            get
            {
                if (type == null)
                    type = new TaxTypeComboBoxVM();

                return type;
            }

            set
            {
                type = value;
            }
        }

        [Required]
        [DisplayName("Letter No.")]
        public string LetterNo { get; set; }

        [Required]
        [UIHint("Date")]
        public DateTime LetterDate { get; set; }

        [Required]
        [UIHint("ComboBox")]
        public string Category { get; set; }

        [Required]
        //[UIHint("ComboBox")]
        public int ContractorId { get; set; }

        [Required]
        public string ContractorName { get; set; }

        [Required]
        public string Object { get; set; }

        [Required]
        [DisplayName("Tax Peiod")]
        [UIHint("Date")]
        public DateTime Period { get; set; }

        [Required]
        public decimal AmountIDR { get; set; }

        [Required]
        [UIHint("TextArea")]
        [DataType(DataType.MultilineText)]
        public string Remarks { get; set; }

        [Required]
        [UIHint("Date")]
        public DateTime PaymentReceivedDate { get; set; }

        public string DocumentNo { get; set; }

        //TODO: attachment
    }
}
