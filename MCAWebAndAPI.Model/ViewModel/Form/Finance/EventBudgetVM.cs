using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    public class EventBudgetVM : Item
    {
        public enum CurrentUserGroupTypes { User, Finance }

        [Required]
        [DisplayName("Event Name")]
        public string EventName { get; set; }

        [Required]
        public string Venue { get; set; }

        [Required]
        [UIHint("Currency")]
        public decimal Rate { get; set; }

        [Required]
        [DisplayName("Attachment")]
        [UIHint("MultiFileUploader")]
        public IEnumerable<HttpPostedFileBase> Documents { get; set; } = new List<HttpPostedFileBase>();

        public string DocumentUrl { get; set; }

        [Required]
        public string Fund { get; set; } = Shared.Fund;

        [Required]
        [UIHint("Total Direct Payment (IDR)")]
        public decimal TotalDirectPayment { get; set; }

        [Required]
        [DisplayName("Total SCA (IDR)")]
        public decimal TotalSCA { get; set; }

        [Required]
        [DisplayName("Total (IDR)")]
        public decimal TotalIDR { get; set; }

        [Required]
        [DisplayName("Total (USD)")]
        public decimal TotalUSD { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [DisplayName("Date (from)")]
        public DateTime DateFrom { get; set; } = DateTime.Now;

        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [DisplayName("Date (to)")]
        public DateTime DateTo { get; set; } = DateTime.Now;

        [Required]
        [UIHint("ComboBox")]
        public ProjectComboBoxVM Project { get; set; } = new ProjectComboBoxVM();

        [Required]
        [UIHint("AjaxCascadeComboBox")]
        public AjaxCascadeComboBoxVM Activity { get; set; } = new AjaxCascadeComboBoxVM();
   
         public IEnumerable<EventBudgetItemVM> ItemDetails { get; set; } = new List<EventBudgetItemVM>();

        public string No { get; set; }

        public decimal TotalDirectPaymentUSD { get; set; }

        public decimal TotalSCAUSD { get; set; }

        public CurrentUserGroupTypes CurrentUserGroup { get; set; }

        [UIHint("ComboBox")]
        public TransactionStatusComboBoxVM TransactionStatus { get; set; } = new TransactionStatusComboBoxVM();

        public int RequisitionNoteId { get; set; }

        public string RequisitionNoteNo { get; set; }

        public string SCAVoucherNo { get; set; }

        public string UserEmail { get; set; }

    }
}
