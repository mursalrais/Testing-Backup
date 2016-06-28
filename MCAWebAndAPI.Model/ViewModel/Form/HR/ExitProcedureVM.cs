using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class ExitProcedureVM : Item
    {
        /// <summary>
        /// Professional Full Name
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Date when request to exit
        /// </summary>
        [UIHint("Date")]
        [DisplayName("Request Date")]
        [Required]
        public DateTime? RequestDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Professional Name
        /// </summary>
        [UIHint("AjaxComboBox")]
        [DisplayName("Professional Name")]
        [Required]
        public AjaxComboBoxVM Professional { get; set; } = new AjaxComboBoxVM
        {
            ActionName = "GetProfessionals",
            ValueField = "ID",
            ControllerName = "HRDataMaster",
            TextField = "Desc",
            OnSelectEventName = "OnSelectAssetHolderFrom"
        };

        /// <summary>
        /// Professional's Project Unit
        /// </summary>
        [DisplayName("Project/Unit")]
        [Required]
        public string ProjectUnit { get; set; }

        /// <summary>
        /// Professional's Position
        /// </summary>
        [DisplayName("Position")]
        [Required]
        public string Position { get; set; }

        /// <summary>
        /// Professional's Phone Number
        /// </summary>
        [DisplayName("Phone Number")]
        [Required]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Professional's Mail Address
        /// </summary>
        [UIHint("EmailAddress")]
        [DisplayName("E-mail Address")]
        [Required]
        public string EmailAddress { get; set; }

        /// <summary>
        /// Professional's Current Address
        /// </summary>
        [UIHint("TextArea")]
        [DataType(DataType.MultilineText)]
        [DisplayName("Current Address")]
        [Required]
        public string CurrentAddress { get; set; }

        /// <summary>
        /// Professional's Join Date
        /// </summary>
        [UIHint("Date")]
        [DisplayName("Join Date")]
        [Required]
        public DateTime? JoinDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Professional's Join Date in String Format
        /// </summary>
        public string StringJoinDate { get; set; }

        /// <summary>
        /// Professional's Last Working Date
        /// </summary>
        [UIHint("Date")]
        [DisplayName("Last Working Date")]
        [Required]
        public DateTime? LastWorkingDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Professional's Exit Reason
        /// </summary>
        [UIHint("ComboBox")]
        [Required]
        public ComboBoxVM ExitReason { get; set; } = new ComboBoxVM
        {
            Choices = new string[]
            {
                "Resign",
                "No Continue the Contract"
            },
            Value = "Resign"
        };

        /// <summary>
        /// Reason Description
        /// </summary>
        [UIHint("TextArea")]
        [DataType(DataType.MultilineText)]
        [DisplayName("Reason Description")]
        [Required]
        public string ReasonDesc { get; set; }

        /// <summary>
        /// PSA Number
        /// </summary>
        [DisplayName("PSA No.")]
        public string PSANumber { get; set; }

        /// <summary>
        /// Documents
        /// </summary>
        [UIHint("MultiFileUploader")]
        [Required]
        public IEnumerable<HttpPostedFileBase> Documents { get; set; } = new List<HttpPostedFileBase>();

        public string DocumentUrl { get; set; }

        public string DocumentType { get; set; }

        public IEnumerable<ExitProcedureChecklistVM> ExitProcedureChecklist { get; set; } = new List<ExitProcedureChecklistVM>();
    }
}
