using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class ExitProcedureChecklistVM : Item
    {
        /// <summary>
        /// Exit Checklist Item 
        /// </summary>
        [DisplayName("Item")]
        public string Item { get; set; }

        /// <summary>
        /// Approver Position
        /// </summary>
        [UIHint("AjaxComboBox")]
        [DisplayName("Approver Position")]
        [Required]
        public AjaxComboBoxVM ApproverPosition { get; set; } = new AjaxComboBoxVM
        {
            ActionName = "GetPositions",
            ControllerName = "HRDataMaster",
            ValueField = "ID",
            TextField = "Desc",
            OnSelectEventName = "OnSelectPosition"
        };

        /// <summary>
        /// Approver Name
        /// </summary>
        [DisplayName("Approver Name")]
        public string ApproverName { get; set; }

        /// <summary>
        /// Date of Approval
        /// </summary>
        [UIHint("Date")]
        [DisplayName("Date of Approval")]
        public DateTime? DateOfApproval { get; set; } = DateTime.Now;

        /// <summary>
        /// Checklist Item Approval
        /// </summary>
        [UIHint("ComboBox")]
        [DisplayName("Indicator")]
        public ComboBoxVM ProjectOrUnit { get; set; } = new ComboBoxVM
        {
            Choices = new string[]
            {
                "Approved",
                "Pending Approval"
            },
            Value = "Approved"
        };

        /// <summary>
        /// Remarks
        /// </summary>
        [UIHint("TextArea")]
        public string Remarks { get; set; }

    }
}