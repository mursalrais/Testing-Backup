using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

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

    public class PettyCashSettlementVM : Item
    {
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Date { get; set; } = DateTime.Today;

        [Required]
        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM PettyCasVoucher { get; set; }

        public string AdvaceReceivedDate { get; }

        public string Status { get; }

        public string PaidTo { get; }

        public string Currency { get; }

        public string AmountPaid { get; }

        public string AmountPaidInWords { get; }

        public string ReasonOfPayment { get; }

        public string Fund { get; } = Shared.Fund;

        public string WBS { get; }

        public string GL { get; }

        [Required]
        public decimal AmountLiquidated { get; set; }

        [Required]
        public decimal AmountReimbursedOrReturned { get; set; }

        [Required]
        public string Remarks { get; set; }

        [DisplayName("Attachment")]
        [UIHint("MultiFileUploader")]
        public IEnumerable<HttpPostedFileBase> Documents { get; set; } = new List<HttpPostedFileBase>();

        public string DocumentUrl { get; set; }
    }
}