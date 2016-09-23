using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    public class SCAReimbursementItemVM : Item
    {
        /// <summary>
        /// Wireframe FIN08: SCA Reimbursement
        /// </summary>

        [UIHint("Date")]
        [Required]
        [DataType(DataType.Date)]
        [DisplayName("Receipt Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ReceiptDate { get; set; } = DateTime.Now;

        [DisplayName("Receipt No")]
        public string ReceiptNo { get; set; }

        public string Payee { get; set; }

        [DisplayName("Description of Expenses")]
        [UIHint("TextArea")]
        public string DescriptionOfExpense { get; set; }

        [UIHint("InGridAjaxComboBox")]
        [Required]
        public AjaxComboBoxVM WBS { get; set; } = new AjaxComboBoxVM();

        [UIHint("InGridAjaxComboBox")]
        [Required]
        public AjaxComboBoxVM GL { get; set; } = new AjaxComboBoxVM();

        [DisplayName("Amount (per item)")]
        public decimal Amount { get; set; }

        [UIHint("MultiFileUploader")]
        [DisplayName("Attachment")]
        public IEnumerable<HttpPostedFileBase> Documents { get; set; } = new List<HttpPostedFileBase>();

        public string DocumentUrl { get; set; }
    }
}
