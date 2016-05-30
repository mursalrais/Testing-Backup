using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class DependentDetailVM : Item
    {
        /// <summary>
        /// Title
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// relationship
        /// </summary>
        [UIHint("InGridComboBox")]
        public InGridComboBoxVM Relationship { get; set; } = new InGridComboBoxVM();

        public static IEnumerable<InGridComboBoxVM> GetRelationshipOptions()
        {
            var index = 0;
            var options = new string[]
            {
                "Spouse",
                "Children"
            };

            return options.Select(e =>
               new InGridComboBoxVM
               {
                   Value = ++index,
                   Text = e
               });
        }

        public static InGridComboBoxVM GetRelationshipDefaultValue(InGridComboBoxVM model = null)
        {
            var options = GetRelationshipOptions();

            if (model == null || model.Value == null || string.IsNullOrEmpty(model.Text))
                return options.FirstOrDefault();

            return options.FirstOrDefault(e =>
                e.Value == model.Value || e.Text == model.Text);
        }

        /// <summary>
        /// placeofbirth
        /// </summary>
        public string PlaceOfBirth { get; set; }

        /// <summary>
        /// dateofbirth
        /// </summary>
        [UIHint("Date")]
        public DateTime? DateOfBirth { get; set; } = DateTime.Now.AddYears(-10);

        /// <summary>
        /// insurancenr
        /// </summary>
        public string InsuranceNumber { get; set; }

        /// <summary>
        /// remarks
        /// </summary>
        [UIHint("TextArea")]
        public string Remark { get; set; }
    }
}