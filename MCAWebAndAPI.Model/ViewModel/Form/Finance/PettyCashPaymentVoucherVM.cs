using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using MCAWebAndAPI.Model.ViewModel.Control;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    [MetadataType(typeof(PettyCashPaymentVoucherVMMetadata))]
    public class PettyCashPaymentVoucherVM : PettyCashTransactionItem
    {
        public PettyCashPaymentVoucherVM()
        {
            this.TransactionType = Shared.PettyCashTranscationType_PettyCashVoucher;
        }
       
        //[DataType(DataType.Date)]
        //[DisplayName("Advance Receive Date")]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        //public DateTime Date { get; set; } = DateTime.Today;

        [UIHint("ComboBox")]
        [Required]
        public ComboBoxVM Status { get; set; } = new ComboBoxVM();

        [Required]
        [UIHint("ComboBox")]
        [DisplayName("Paid To")]
        public PaidToComboboxVM PaidTo { get; set; } = new PaidToComboboxVM();

        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM Professional { get; set; } = new AjaxComboBoxVM();

        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM Vendor { get; set; } = new AjaxComboBoxVM();

        [Required]
        [DisplayName("Amount paid in words")]
        public string AmountPaidInWord { get; set; }

        [Required]
        [DisplayName("Reason of payment")]
        public string ReasonOfPayment { get; set; }

        [Required]
        public string Fund { get; set; } = Shared.Fund;

        [UIHint("AjaxComboBox")]
        [Required]
        public AjaxComboBoxVM WBS { get; set; } = new AjaxComboBoxVM();

        [UIHint("AjaxComboBox")]
        [Required]
        public AjaxComboBoxVM GL { get; set; } = new AjaxComboBoxVM();

        [UIHint("TextArea")]
        public string Remarks { get; set; }

        [UIHint("MultiFileUploader")]
        [DisplayName("Attachment")]
        [Required]
        public IEnumerable<HttpPostedFileBase> Documents { get; set; } = new List<HttpPostedFileBase>();

        public string DocumentUrl { get; set; }

        //public string DocNo { get; set; }

    }

    internal class PettyCashPaymentVoucherVMMetadata
    {
        [DataType(DataType.Date)]
        [DisplayName("Advance Receive Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Date { get; set; } = DateTime.Today;

        [UIHint("Decimal")]
        [Required]
        [DisplayName("Amount paid")]
        [DisplayFormat(DataFormatString = "{0:#}", ApplyFormatInEditMode = true)]
        public decimal Amount { get; set; } = 0;
    }
}
