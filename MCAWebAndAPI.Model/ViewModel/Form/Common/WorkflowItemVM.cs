﻿using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MCAWebAndAPI.Model.ViewModel.Form.Common
{
    public class WorkflowItemVM : Item
    {
        public string ListName { get; set; }

        public string Level { get; set; }

        public bool IsDefault { get; set; }

        public bool IsSequential { get; set; }

        public string RequestorUnit { get; set; }

        public string RequestorPosition { get; set; }

        [UIHint("InGridComboBox")]
        public InGridComboBoxVM ApproverUnit { get; set; } = new InGridComboBoxVM();

        [UIHint("InGridAjaxCascadeComboBox")]
        public AjaxCascadeComboBoxVM ApproverPosition { get; set; } = new AjaxCascadeComboBoxVM();

        [UIHint("InGridAjaxCascadeComboBox")]
        public AjaxCascadeComboBoxVM ApproverUserName { get; set; } = new AjaxCascadeComboBoxVM();

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
            if (model == null || model.Value == null || string.IsNullOrEmpty(model.Text))
                return options.FirstOrDefault();

            return options.FirstOrDefault(e =>
                e.Value == model.Value || e.Text == model.Text);
        }
    }
}
