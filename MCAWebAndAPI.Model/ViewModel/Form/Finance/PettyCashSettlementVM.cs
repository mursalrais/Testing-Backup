using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using static MCAWebAndAPI.Model.ViewModel.Form.Finance.Shared;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    /// <summary>
    /// Wireframe FIN11: Petty Cash Settlement
    /// 
    ///     Petty Cash Settlement is a transaction for settlement-reimbursement of petty cash where 
    ///     user has already asked for petty cash advance previously. 
    ///     
    ///     Through this feature, user will create the settlement-reimbursement of 
    ///     petty cash which results whether user needs to return the excess petty cash advance or 
    ///     receive the reimbursement in the case where the actual expense for 
    ///     petty cash exceeds the petty cash advance given. 
    ///     
    ///     It is created and maintained by finance. 																									
    ///
    /// </summary>

    [MetadataType(typeof(PettyCashSettlementMetadata))]
    public class PettyCashSettlementVM : PettyCashTransactionItem
    {
        public PettyCashSettlementVM()
        {
            this.TransactionType = Shared.PettyCashTranscationType_PettyCashSettlementr;
        }
        [Required]
        [DisplayName("Settlement Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Date { get; set; } = DateTime.Today;

        [Required]
        [DisplayName("Petty Cash Voucher No.")]
        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM PettyCashVoucher { get; set; }

        [Required]
        [DisplayName("Advance Received Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public string AdvanceReceivedDate { get; }

        public string Status { get; }

        [DisplayName("Paid to")]
        public string PaidTo { get; }

        public string Currency { get; }

        [DisplayName("Amount paid")]
        public string AmountPaid { get; }

        [DisplayName("Amount paid in words")]
        public string AmountPaidInWords { get; }

        [DisplayName("Reason of payment")]
        public string ReasonOfPayment { get; }

        public string Fund { get; } = Shared.Fund;

        public string WBS { get; }

        public string GL { get; }

        [Required]
        [DisplayName("Amount Liquidated")]
        [UIHint("Currency")]
        public decimal AmountLiquidated { get; set; } = 0;

        [Required]
        public string Remarks { get; set; }

        [DisplayName("Attachment")]
        [UIHint("MultiFileUploader")]
        public IEnumerable<HttpPostedFileBase> Documents { get; set; } = new List<HttpPostedFileBase>();

        public string DocumentUrl { get; set; }

        public Operations Operation { get; set; }
    }

    internal class PettyCashSettlementMetadata
    {
        [DisplayName("Advance Receive Date")]
        public DateTime Date { get; set; }

        [DisplayName("Amount Reimbursed/Returned")]
        public decimal Amount { get; set; }
    }
}