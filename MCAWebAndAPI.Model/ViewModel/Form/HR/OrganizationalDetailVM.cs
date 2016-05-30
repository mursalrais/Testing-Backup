using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class OrganizationalDetailVM : Item
    {
        /// <summary>
        /// projectunit
        /// </summary>
        [UIHint("InGridComboBox")]
        public InGridComboBoxVM Project { get; set; } = new InGridComboBoxVM();

        public static IEnumerable<InGridComboBoxVM> GetProjectOptions()
        {
            var index = 0;
            var options = new string[] {
                "LGL",
                "Ops - HR",
                "CC - ESP",
                "HN",
                "Ops - P",
                "ME",
                "PM",
                "GP",
                "Ops - IT",
                "Ops",
                "CC - E",
                "Ops - F",
                "CC",
                "EO",
                "COM",
                "CC - SGA",
                "RI",
                "HN - NST" };

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
            if (model == null || model.Value == null || string.IsNullOrEmpty(model.Text))
                return options.FirstOrDefault();

            return options.FirstOrDefault(e =>
                e.Value == model.Value || e.Text == model.Text);
        }

        public static InGridComboBoxVM GetProfessionalStatusDefaultValue(InGridComboBoxVM model = null)
        {
            var options = GetProfessionalStatusOptions();

            if (model == null || model.Value == null || string.IsNullOrEmpty(model.Text))
                return options.FirstOrDefault();

            return options.FirstOrDefault(e => 
                e.Value == model.Value || e.Text == model.Text);
        }

        /// <summary>
        /// Active
        /// Inactive
        /// Resigned
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<InGridComboBoxVM> GetProfessionalStatusOptions()
        {
            var index = 0;
            var options = new string[]
            {
                "Active",
                "Inactive",
                "Resigned"
            };

            return options.Select(e =>
              new InGridComboBoxVM
              {
                  Value = ++index,
                  Text = e
              });
        }


        /// <summary>
        /// Status
        /// </summary>
        [UIHint("InGridComboBox")]
        public InGridComboBoxVM ProfessionalStatus { get; set; } = new InGridComboBoxVM();


        /// <summary>
        /// Position
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// Level
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// psanr
        /// </summary>
        public string PSANumber { get; set; }

        /// <summary>
        /// startdate
        /// </summary>
        [UIHint("Date")]
        public DateTime? StartDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// lastworkingday
        /// </summary>
        [UIHint("Date")]
        public DateTime? LastWorkingDay { get; set; } = DateTime.UtcNow;

        
    }
}