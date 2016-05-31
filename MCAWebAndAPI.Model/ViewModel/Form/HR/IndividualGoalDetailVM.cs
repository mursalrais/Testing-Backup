using Kendo.Mvc.UI;
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
    public class IndividualGoalDetailVM : Item
    {
        /// <summary>
        /// individualgoalcategory
        /// </summary>
        [UIHint("InGridComboBox")]
        public InGridComboBoxVM Category { get; set; }

        public static IEnumerable<InGridComboBoxVM> GetCategoryOptions()
        {
            var index = 0;
            var options = new string[] {
                "Program Management"};

            return options.Select(e =>
                new InGridComboBoxVM
                {
                    Value = ++index,
                    Text = e
                });
        }

        public static InGridComboBoxVM GetCategoryDefaultValue(InGridComboBoxVM model = null)
        {
            var options = GetCategoryOptions();
            if (model == null || model.Value == null || string.IsNullOrEmpty(model.Text))
                return options.FirstOrDefault();

            return options.FirstOrDefault(e =>
                e.Value == model.Value || e.Text == model.Text);
        }

        /// <summary>
        /// individualgoalplan
        /// </summary>
        [UIHint("TextArea")]
        public string IndividualGoalAndPlan { get; set; }

        /// <summary>
        /// individualgoalweight
        /// </summary>
        public int Weight { get; set; }

        /// <summary>
        /// individualgoalremarks
        /// </summary>
        [UIHint("TextArea")]
        public string Remarks { get; set; }
    }
}
