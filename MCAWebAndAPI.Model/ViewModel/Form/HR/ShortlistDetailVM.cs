using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class ShortlistDetailVM : Item
    {
        public string Candidate { get; set; }

        public string CV { get; set; }

        [UIHint("InGridComboBoxVM")]
        public InGridComboBoxVM Status { get; set; }

        public static IEnumerable<InGridComboBoxVM> GetStatusOptions()
        {
            var index = 0;
            var options = new string[] {
                "New",
                "Shortlisted",
                "Declined"};

            return options.Select(e =>
                new InGridComboBoxVM
                {
                    Value = ++index,
                    Text = e
                });
        }

        public static InGridComboBoxVM GetStatusDefaultValue(InGridComboBoxVM model = null)
        {
            var options = GetStatusOptions();
            if (model == null || model.Value == null || string.IsNullOrEmpty(model.Text))
                return options.FirstOrDefault();

            return options.FirstOrDefault(e =>
                e.Value == model.Value || e.Text == model.Text);
        }

        public string Remarks { get; set; }
    }
}
