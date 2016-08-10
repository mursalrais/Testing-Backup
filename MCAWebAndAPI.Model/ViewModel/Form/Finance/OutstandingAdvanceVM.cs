using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    public class OutstandingAdvanceVM : Item
    {    
        [Required]
        [DisplayName("Date (Upload)")]
        [UIHint("Date")]
        public DateTime? DateOfUpload { get; set; } = DateTime.Now;

        [Required]
        [DisplayName("Staff")]
        [UIHint("AjaxComboBox")]
        public AjaxCascadeComboBoxVM StaffID { get; set; } = new AjaxCascadeComboBoxVM
        {
            //TODO: ganti dengan yang bener ya...
            ControllerName = "FINEventBudget",
            ActionName = "GetEventBudgetList",
            ValueField = "ID",
            TextField = "Title",
            OnSelectEventName = "onSelectEventBudgetNo"

        };

        /// <summary>
        /// Title
        /// </summary>
        [Required]
        [DisplayName("Reference")]
        public string Reference { get; set; }

        [Required]
        [DisplayName("Due Date")]
        [UIHint("Date")]
        public DateTime? DueDate { get; set; } = DateTime.Now;

        [Required]
        [DisplayName("Currency")]
        [UIHint("ComboBox")]
        public CurrencyComboBoxVM Currency { get; set; } = new CurrencyComboBoxVM();

        [Required]
        [DisplayName("Amount")]
        public decimal Amount { get; set; }
       
        [Required]
        [DisplayName("Project")]
        [UIHint("ComboBox")]
        public ProjectComboBoxVM Project { get; set; } = new ProjectComboBoxVM();

        [Required]
        [DisplayName("Remarks")]
        [UIHint("TextArea")]
        public string Remarks { get; set; }

        [Required]
        [DisplayName("Attachment")]
        [UIHint("MultiFileUploader")]
        public IEnumerable<HttpPostedFileBase> Attachment { get; set; } = new List<HttpPostedFileBase>();

    }
}
