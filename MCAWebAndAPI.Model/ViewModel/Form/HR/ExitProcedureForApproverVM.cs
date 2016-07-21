using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Linq;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class ExitProcedureForApproverVM : Item
    {
        [DisplayName("Approver Email")]
        public string ApproverMail { get; set; }

        [DisplayName("Approver Level")]
        public int ApproverLevel { get; set; }

        [DisplayName("Exit Procedure ID")]
        public int ExitProcedureID { get; set; }

        [DisplayName("Exit Procedure Checklist ID")]
        public int ExitProcedureChecklistID { get; set; }

        [DisplayName("Requestor Email")]
        public string RequestorMail { get; set; }

        [DisplayName("Item Exit Procedure")]
        public string ItemExitProcedure { get; set; }

        /// <summary>
        /// Professional's Exit Reason
        /// </summary>
        [UIHint("ComboBox")]
        [DisplayName("Checklist Status")]
        public ComboBoxVM ChecklistItemApproval { get; set; } = new ComboBoxVM
        {
            Choices = new string[]
            {
                "Pending Approval",
                "Approved"
            },
            Value = "Pending Approval"
        };

        [DisplayName("Exit Checklist ID")]
        public int ExitCheckListID { get; set; }
    }
}