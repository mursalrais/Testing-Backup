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
    public class ExitProcedureChecklistVM : Item
    {
        public string TransactionType { get; set; }

        public string RequestorUnit { get; set; }

        public string RequestorPosition { get; set; }

        public string ApproverLevel { get; set; }

        public string AppPosition { get; set; }
       
        public string WorkflowType { get; set; }
        
        /// <summary>
        /// Exit Checklist Item 
        /// </summary>
        [DisplayName("Item")]
        public string Item { get; set; }

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

        public string ItemExitProcedure { get; set; }

        public string ListName { get; set; }

        public string Level { get; set; }

        public bool IsDefault { get; set; }

        public bool IsSequential { get; set; }

        [UIHint("InGridComboBox")]
        public InGridComboBoxVM ApproverUnit { get; set; } = new InGridComboBoxVM();

        [UIHint("InGridAjaxCascadeComboBox")]
        public AjaxComboBoxVM ApproverPosition { get; set; } = new AjaxComboBoxVM();

        [UIHint("InGridAjaxCascadeComboBox")]
        public AjaxComboBoxVM ApproverUserName { get; set; } = new AjaxComboBoxVM();

        public static IEnumerable<InGridComboBoxVM> GetUnitOptions()
        {
            var index = 0;
            var options = new string[] {
                "Executive Director",
                "Executive Officer",
                "Legal Unit",
                "Monitoring & Evaluation Unit",
                "Communications & Outreach Unit",
                "Risk & Audit Unit",
                "Program Div.",
                "Procurement Modernization Project",
                "Community-Based Health & Nutrition Project",
                "Green Prosperity Project",
                "Cross-Cutting Sector",
                "Economic Analysis Unit",
                "Social & Gender Assessment Unit",
                "Environment & Social Performance Unit",
                "Operations Support Div.",
                "Finance Unit",
                "Procurement Unit",
                "Information Technology Unit",
                "Human Resources Unit",
                "Office Support Unit",
                "Fiscal Agent (FA)",
                "Procurement Agent (PA)"};

            return options.Select(e =>
                new InGridComboBoxVM
                {
                    Value = ++index,
                    Text = e
                });
        }

        public static InGridComboBoxVM GetUnitDefaultValue(InGridComboBoxVM model = null)
        {
            var options = GetUnitOptions();
            if (model == null || (model.Value == null && model.Text == null) || string.IsNullOrEmpty(model.Text))
                return options.FirstOrDefault();

            return options.FirstOrDefault(e => e.Value == null ?
                e.Value == model.Value : e.Text == model.Text);
        }

    }
}