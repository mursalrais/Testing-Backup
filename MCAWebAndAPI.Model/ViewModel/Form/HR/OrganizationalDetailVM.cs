using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class OrganizationalDetailVM : Item
    {
        /// <summary>
        /// projectunit
        /// </summary>
        [DisplayName("Project/Unit")]
        [UIHint("InGridComboBox")]
        public InGridComboBoxVM Project { get; set; } = new InGridComboBoxVM();

        public static IEnumerable<InGridComboBoxVM> GetProjectOptions()
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

        public static InGridComboBoxVM GetProjectDefaultValue(InGridComboBoxVM model = null)
        {
            var options = GetProjectOptions();
            if (model == null || string.IsNullOrEmpty(model.Text))
                return options.FirstOrDefault();

            return options.FirstOrDefault(e =>
                e.Value == model.Value || e.Text == model.Text);
        }

        //public static InGridComboBoxVM GetPSAStatusDefaultValue(InGridComboBoxVM model = null)
        //{
        //    var options = GetPSAStatusOptions();

        //    if (model == null || model.Value == null || string.IsNullOrEmpty(model.Text))
        //        return options.FirstOrDefault();

        //    return options.FirstOrDefault(e => 
        //        e.Value == model.Value || e.Text == model.Text);
        //}

        ///// <summary>
        ///// Active
        ///// Inactive
        ///// Resigned
        ///// </summary>
        ///// <returns></returns>
        //public static IEnumerable<InGridComboBoxVM> GetPSAStatusOptions()
        //{
        //    var index = 0;
        //    var options = new string[]
        //    {
        //        "Active",
        //        "Inactive",
        //        "Resigned"
        //    };

        //    return options.Select(e =>
        //      new InGridComboBoxVM
        //      {
        //          Value = ++index,
        //          Text = e
        //      });
        //}

        /// <summary>
        /// Status
        /// </summary>
        [DisplayName("PSA Status")]
        public string PSAStatus { get; set; }

        /// <summary>
        /// Position
        /// </summary>
        [UIHint("InGridAjaxComboBox")]
        public AjaxComboBoxVM Position { get; set; } = new AjaxComboBoxVM();

        /// <summary>
        /// Level
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// psanr
        /// </summary>
        [UIHint("InGridAjaxComboBox")]
        public AjaxComboBoxVM PSANumber { get; set; } = new AjaxComboBoxVM();

        /// <summary>
        /// startdate
        /// </summary>
        [UIHint("Date")]
        [DisplayName("PSA Start Date")]
        public DateTime? StartDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// lastworkingday
        /// </summary>
        [UIHint("Date")]
        [DisplayName("PSA Expiry Date")]
        public DateTime? LastWorkingDay { get; set; } = DateTime.UtcNow;
    }
}