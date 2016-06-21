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
        [UIHint("Date")]
        [DisplayName("Request Date")]
        [Required]
        public DateTime? RequestDate { get; set; } = DateTime.Now;

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

        [DisplayName("Project/Unit")]
        [Required]
        public string ProjectUnit { get; set; }

        [DisplayName("Position")]
        [Required]
        public string Position { get; set; }

        [DisplayName("Phone Number")]
        [Required]
        public string PhoneNumber { get; set; }

        [UIHint("EmailAddress")]
        [DisplayName("E-mail Address")]
        [Required]
        public string EmailAddress { get; set; }

        [UIHint("TextArea")]
        [DataType(DataType.MultilineText)]
        [DisplayName("Current Address")]
        [Required]
        public string CurrentAddress { get; set; }

        [UIHint("Date")]
        [DisplayName("Join Date")]
        [Required]
        public DateTime? JoinDate { get; set; } = DateTime.Now;

        public string StringJoinDate { get; set; }

        [UIHint("Date")]
        [DisplayName("Last Working Date")]
        [Required]
        public DateTime? LastWorkingDate { get; set; } = DateTime.Now;

        [UIHint("ComboBox")]
        public ComboBoxVM ExitReason { get; set; } = new ComboBoxVM
        {
            Choices = new string[]
            {
                "Resign",
                "No Continue the Contract"
            },
            Value = "Resign"
        };

        [UIHint("TextArea")]
        [DataType(DataType.MultilineText)]
        [DisplayName("Reason Description")]
        [Required]
        public string ReasonDesc { get; set; }
    }
}
