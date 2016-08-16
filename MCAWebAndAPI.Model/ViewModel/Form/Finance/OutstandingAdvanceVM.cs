using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    /// <summary>
    /// Wireframe FIN09: Outstanding Advance
    /// </summary>

    public class OutstandingAdvanceVM : Item
    {    
        [Required]
        [DisplayName("Date (Upload)")]
        [UIHint("Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DateOfUpload { get; set; } = DateTime.Now;

        [Required]
        [DisplayName("Staff")]
        [UIHint("AjaxComboBox")]
        public AjaxCascadeComboBoxVM Staff { get; set; } = new AjaxCascadeComboBoxVM
        {
            ControllerName = "Vendor",
            ActionName = "GetVendor",
            ValueField = "Value",
            TextField = "Text"
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
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime DueDate { get; set; } = DateTime.Now;

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

        [UIHint("MultiFileUploader")]
        [DisplayName("Attachment")]
        public IEnumerable<HttpPostedFileBase> Documents { get; set; } = new List<HttpPostedFileBase>();

        public string DocumentUrl { get; set; }

    }
}
