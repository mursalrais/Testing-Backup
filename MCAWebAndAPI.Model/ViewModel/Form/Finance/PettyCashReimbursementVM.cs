using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using MCAWebAndAPI.Model.ViewModel.Control;
using static MCAWebAndAPI.Model.ViewModel.Form.Finance.Shared;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    /// <summary>
    ///     Wireframe FIN12: Petty Cash Reimbursement
    ///         Petty Cash Reimbursement is a transaction for the reimbursement of petty cash only when
    ///         user has not asked for any petty cash advance.
    ///
    ///         Through this feature, finance will create the reimbursement of petty cash which results in 
    ///         user needs to receive the reimbursement. 
    /// </summary>

    [MetadataType(typeof(PettyCashReimbursementVMMetadata))]
    public  class PettyCashReimbursementVM : PettyCashTransactionItem
    {
        public PettyCashReimbursementVM()
        {
            this.TransactionType = Shared.PettyCashTranscationType_PettyCashReimbursement;
        }
        public Operations Operation { get; set; }

        public string DocNo { get; set; }

        [Required]
        [UIHint("ComboBox")]
        public PaidToComboboxVM  PaidTo { get; set; } = new PaidToComboboxVM();

        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM Professional { get; set; } = new AjaxComboBoxVM();

        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM Vendor { get; set; } = new AjaxComboBoxVM();

        public string Driver { get; set; }

        [Required]
        [DisplayName("Currency")]
        [UIHint("ComboBox")]
        public CurrencyComboBoxVM Currency { get; set; } = new CurrencyComboBoxVM();

        [Required]
        [DisplayName("Reason of Payment")]
        public string Reason { get; set; }

        public string Fund { get; } = Shared.Fund;

        [Required]
        [DisplayName("WBS")]
        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM WBS { get; set; } = new AjaxComboBoxVM();

        public string WBSDescription { get; set; }

        [Required]
        [DisplayName("GL")]
        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM GL { get; set; } = new AjaxComboBoxVM();

        public string GLDescription { get; set; }

        [DisplayName("Amount Reimbursed")]
        public decimal AmountReimbursed { get; set; } = 0;

        [UIHint("TextArea")]
        public string Remarks { get; set; }

        [UIHint("MultiFileUploader")]
        public IEnumerable<HttpPostedFileBase> Documents { get; set; } = new List<HttpPostedFileBase>();

        public string DocumentUrl { get; set; }
    }

    internal class PettyCashReimbursementVMMetadata
    {
        [DisplayName("Reimbursement Date")]
        public DateTime Date { get; set; }

        [DisplayName("Amount liquidated")]
        public decimal Amount { get; set; } = 0;
    }

}
