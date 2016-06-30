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
        public string TransactionType { get; set; }

        public string RequestorUnit { get; set; }

        public string RequestorPosition { get; set; }

        public string ApproverLevel { get; set; }

        public string ApproverUnit { get; set; }

        public string AppPosition { get; set; }

        public string IsDefault { get; set; }

        public string WorkflowType { get; set; }
        
        /// <summary>
        /// Exit Checklist Item 
        /// </summary>
        [DisplayName("Item")]
        public string Item { get; set; }

        /// <summary>
        /// Approver Position
        /// </summary>
        [UIHint("InGridAjaxComboBox")]
        public AjaxComboBoxVM ApproverPosition { get; set; } = new AjaxComboBoxVM();

        /// <summary>
        /// Get Position Default Value
        /// </summary>
        public static AjaxComboBoxVM GetPositionDefaultValue(AjaxComboBoxVM model = null)
        {
            if (model == null)
            {
                return new AjaxComboBoxVM();
            }
            else
            {
                return model;
            }
        }

        /// <summary>
        /// Position ID
        /// </summary>
        public int PositionID { get; set; }

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
        public ComboBoxVM CheckListItemApproval { get; set; } = new ComboBoxVM
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