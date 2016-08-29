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
        //TODO: beware there are 2 different dates: Settlement Date and Adcance Receive Date
        //  this can create confusion in relation to inheritance to  PettyCashTransactionItem

        public PettyCashSettlementVM()
        {
            this.TransactionType = Shared.PettyCashTranscationType_PettyCashSettlementr;
        }

        [Required]
        [DisplayName("Petty Cash Voucher No.")]
        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM PettyCashVoucher { get; set; }

        [DisplayName("Advance Received Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? AdvanceReceivedDate { get; set; }

        public string Status { get; set; }

        [DisplayName("Paid to")]
        public string PaidTo { get; set; }

        [UIHint("Decimal")]
        [DisplayName("Amount paid")]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public decimal AmountPaid { get; set; } = 0;

        [DisplayName("Amount paid in words")]
        public string AmountPaidInWords { get; set; }

        [DisplayName("Reason of payment")]
        public string ReasonOfPayment { get; set; }

        public string Fund { get; set; } = Shared.Fund;

        public string WBS { get; set; }

        public string GL { get; set; }

        [Required]
        [DisplayName("Amount Liquidated")]
        [UIHint("Decimal")]
        [DisplayFormat(DataFormatString = "{0:#}", ApplyFormatInEditMode = true)]
        public decimal AmountLiquidated { get; set; } = 0;

        
        [UIHint("TextArea")]
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
        [DisplayName("Settlement Date")]
        public DateTime Date { get; set; }

        [UIHint("Decimal")]
        [DisplayName("Amount Reimbursed/Returned")]
        public decimal Amount { get; set; } = 0;

        [ReadOnly(true)]
        public string Currency { get; }
    }
}