﻿using MCAWebAndAPI.Model.Common;
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
        /// <summary>
        /// Exit Checklist Item 
        /// </summary>
        [DisplayName("Item")]
        public string Item { get; set; }

        [DisplayName("Item")]
        public string ItemExitProcedure { get; set; }

        //[UIHint("InGridAjaxCascadeComboBox")]
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

        //[UIHint("InGridAjaxCascadeComboBox")]
        [UIHint("InGridAjaxComboBox")]
        public AjaxComboBoxVM ProfessionalPosition { get; set; } = new AjaxComboBoxVM();

        /// <summary>
        /// Get Position Default Value
        /// </summary>
        public static AjaxComboBoxVM GetProfessionalPositionDefaultValue(AjaxComboBoxVM model = null)
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
        /// Approver Name
        /// </summary>
        [DisplayName("Approver Name")]
        public string ApproverName { get; set; }

        /// <summary>
        /// Checklist Item Approval
        /// </summary>
        [UIHint("InGridComboBox")]
        [DisplayName("Checklist Item")]
        public InGridComboBoxVM CheckListItemApproval { get; set; } = new InGridComboBoxVM();

        public static InGridComboBoxVM GetCheckListItemApprovalDefaultValue(InGridComboBoxVM model = null)
        {
            var options = GetCheckListItemApproval();
            if (model == null || (model.Value == null && model.Text == null) || string.IsNullOrEmpty(model.Text))
                return options.FirstOrDefault();

            return options.FirstOrDefault(e => e.Value == null ?
                e.Value == model.Value : e.Text == model.Text);
        }

        public static IEnumerable<InGridComboBoxVM> GetCheckListItemApproval()
        {
            var index = 0;
            var options = new string[]
            {
                "Pending Approval",
                "Approved"
            };

            return options.Select(e =>
                new InGridComboBoxVM
                {
                    Value = ++index,
                    Text = e
                });
        }

        /// <summary>
        /// Date of Approval
        /// </summary>
        [UIHint("Date")]
        [DisplayName("Date")]
        public DateTime? DateOfApproval { get; set; } = DateTime.Now;

        /// <summary>
        /// Remarks
        /// </summary>
        [UIHint("TextArea")]
        public string Remarks { get; set; }

        public string TransactionType { get; set; }

        public string RequestorUnit { get; set; }

        public string RequestorPosition { get; set; }

        public string ApproverLevel { get; set; }

        public string AppPosition { get; set; }

        public string WorkflowType { get; set; }

        /// <summary>
        /// Position ID
        /// </summary>
        public int PositionID { get; set; }

        public string ListName { get; set; }

        public string Level { get; set; }

        public string IsDefault { get; set; }

        public bool IsSequential { get; set; }

        [UIHint("InGridAjaxComboBox")]
        [DisplayName("Approver Name")]
        public AjaxComboBoxVM ApproverUserName { get; set; } = new AjaxComboBoxVM();

        /// <summary>
        /// Get Position Default Value
        /// </summary>
        public static AjaxComboBoxVM GetApproverUserNameDefaultValue(AjaxComboBoxVM model = null)
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

        [UIHint("InGridComboBox")]
        public InGridComboBoxVM ApproverUnit { get; set; } = new InGridComboBoxVM();


        public static IEnumerable<InGridComboBoxVM> GetUnitOptions()
        {
            var index = 0;
            var options = new string[] {
                "Executive Office",
                "Legal Unit",
                "Monitoring & Evaluation Unit",
                "Communications & Outreach Unit",
                "Risk & Audit Unit",
                "Program Division",
                "Procurement Modernization Project",
                "Community-based Health & Nutrition Project",
                "Green Prosperity Project",
                "Cross-Cutting Sector",
                "Economic Analysis Unit",
                "Environment & Social Performance Unit",
                "Social & Gender Assessment Unit",
                "Operations Support Div",
                "Finance Unit",
                "Procurement Unit",
                "Information Technology Unit",
                "Human Resources Unit",
                "Office Management"};

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

        [UIHint("InGridComboBox")]
        public InGridComboBoxVM ProfessionalUnit { get; set; } = new InGridComboBoxVM();


        public static IEnumerable<InGridComboBoxVM> GetProfessionalUnitOptions()
        {
            var index = 0;
            var options = new string[] {
                "Executive Office",
                "Legal Unit",
                "Monitoring & Evaluation Unit",
                "Communications & Outreach Unit",
                "Risk & Audit Unit",
                "Program Division",
                "Procurement Modernization Project",
                "Community-based Health & Nutrition Project",
                "Green Prosperity Project",
                "Cross-Cutting Sector",
                "Economic Analysis Unit",
                "Environment & Social Performance Unit",
                "Social & Gender Assessment Unit",
                "Operations Support Div",
                "Finance Unit",
                "Procurement Unit",
                "Information Technology Unit",
                "Human Resources Unit",
                "Office Management"};

            return options.Select(e =>
                new InGridComboBoxVM
                {
                    Value = ++index,
                    Text = e
                });
        }

        public static InGridComboBoxVM GetProfessionalUnitDefaultValue(InGridComboBoxVM model = null)
        {
            var options = GetProfessionalUnitOptions();
            if (model == null || (model.Value == null && model.Text == null) || string.IsNullOrEmpty(model.Text))
                return options.FirstOrDefault();

            return options.FirstOrDefault(e => e.Value == null ?
                e.Value == model.Value : e.Text == model.Text);
        }

        [UIHint("InGridComboBox")]
        public InGridComboBoxVM ApproverIndicator { get; set; } = new InGridComboBoxVM();

        public static IEnumerable<InGridComboBoxVM> GetApproverIndicator()
        {
            var index = 0;
            var options = new string[] {
                "No",
                "Yes"
            };

            return options.Select(e =>
                new InGridComboBoxVM
                {
                    Value = ++index,
                    Text = e
                });
        }

        public static InGridComboBoxVM GetApproverIndicatorDefaultValue(InGridComboBoxVM model = null)
        {
            var options = GetApproverIndicator();
            if (model == null || (model.Value == null && model.Text == null) || string.IsNullOrEmpty(model.Text))
                return options.FirstOrDefault();

            return options.FirstOrDefault(e => e.Value == null ?
                e.Value == model.Value : e.Text == model.Text);
        }

        [DisplayName("Indicator")]
        public string ApprovalIndicator { get; set; }

        public string ApprovalMail { get; set; }

        /// <summary>
        /// Checklist Item Approval
        /// </summary>
        [UIHint("InGridComboBox")]
        public InGridComboBoxVM IsDefaultTemp { get; set; } = new InGridComboBoxVM();

        public static InGridComboBoxVM GetIsDefaultTempValue(InGridComboBoxVM model = null)
        {
            var options = GetIsDefaultTemp();
            if (model == null || (model.Value == null && model.Text == null) || string.IsNullOrEmpty(model.Text))
                return options.FirstOrDefault();

            return options.FirstOrDefault(e => e.Value == null ?
                e.Value == model.Value : e.Text == model.Text);
        }

        public static IEnumerable<InGridComboBoxVM> GetIsDefaultTemp()
        {
            var index = 0;
            var options = new string[]
            {
                "Yes",
                "No"
            };

            return options.Select(e =>
                new InGridComboBoxVM
                {
                    Value = ++index,
                    Text = e
                });
        }

        [UIHint("Date")]
        public DateTime? StartDateApproval { get; set; } = DateTime.Now;
    }
}