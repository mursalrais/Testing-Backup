using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static MCAWebAndAPI.Model.ViewModel.Form.Finance.Shared;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    /// <summary>
    /// Wireframe FIN18: Tax Reimbursement
    /// </summary>
    public class TaxReimbursementVM : Item
    {
        
        ////public enum TaxType { Income = 1, VAT = 2, Others = 3 }
        public enum CategoryType { Vendor, MCAI }


        private const string TaxTypeIncomeTax = "Income Tax";
        private const string TaxTypeVAT = "VAT";
        private const string TaxTypeOthers = "others";

        private const string Category_Vendor = "Vendor";
        private const string Category_MCAI = "MCA Indonesia";

        private TaxTypeComboBoxVM type;

        public Operations Operation { get; set; }

        [Required]
        [DisplayName("Type of Tax")]
        [UIHint("ComboBox")]
        public TaxTypeComboBoxVM Type { get; set; } = new TaxTypeComboBoxVM();

        [Required]
        [DisplayName("Letter No.")]
        public string LetterNo { get; set; }

        [Required]
        [DisplayName("Letter Date")]
        [UIHint("Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime LetterDate { get; set; } = DateTime.Now;

        [Required]
        [UIHint("ComboBox")]
        public ComboBoxVM Category { get; set; } = new ComboBoxVM
        {
            Choices = new string[]
            {
                Category_Vendor,
                Category_MCAI
            }
        };

        [DisplayName("Vendor")]
        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM Vendor { get; set; } = new AjaxComboBoxVM();

        [Required]
        public string Contractor { get; set; } 

        [Required]
        public string Object { get; set; }

        [Required]
        [DisplayName("Tax Period")]
        [UIHint("Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MMM-yyyy}")]
        public DateTime Period { get; set; } = DateTime.Now;

        [Required]
        [DisplayName("Amount (IDR)")]
        public decimal AmountIDR { get; set; }

        [Required]
        [UIHint("TextArea")]
        [DataType(DataType.MultilineText)]
        public string Remarks { get; set; }

        [Required]
        [UIHint("Date")]
        [DisplayName("Payment Received Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime PaymentReceivedDate { get; set; } = DateTime.Now;

        [Required]
        [DisplayName("Document No.")]
        public string DocumentNo { get; set; }

        [UIHint("MultiFileUploader")]
        [DisplayName("Attachment")]
        public IEnumerable<HttpPostedFileBase> Documents { get; set; } = new List<HttpPostedFileBase>();

        public string DocumentUrl { get; set; }
    }
}
