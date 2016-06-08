using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class WorkingRelationshipDetailVM : Item
    {
        [UIHint("InGridAjaxComboBox")]
        public AjaxComboBoxVM PositionWorking { get; set; } = new AjaxComboBoxVM();

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
                
        [UIHint("InGridMultiSelect")]
        public InGridMultiSelectVM Frequency { get; set; } = new InGridMultiSelectVM();

        public static IEnumerable<InGridMultiSelectVM> GetFrequencyOptions()
        {
            var options = new string[]
            {
                "Daily", "Regularly", "When Necessary"
            };

            return options.Select(e =>
              new InGridMultiSelectVM
              {
                  Text = e,
                  isSelected = false
              });

        }

        public static IEnumerable<InGridMultiSelectVM> GetFrequencyValues()
        {
            var options = GetFrequencyOptions();
            var values = options.Where(e => e.isSelected).Select(f =>
                new InGridMultiSelectVM
                {
                    Text = f.Text,
                    isSelected = true
                });

            return values;


        }
        
        public static InGridMultiSelectVM GetFrequencyDefaultValue(InGridMultiSelectVM model = null)
        {
            var options = GetFrequencyOptions();
            if (model == null || model.Text == null || string.IsNullOrEmpty(model.Text))
                return new InGridMultiSelectVM();
            
            var tes = new InGridMultiSelectVM();
            tes.Text = model.Text;
            return tes;
        }

        [UIHint("InGridMultiSelect")]
        public InGridMultiSelectVM Relationship { get; set; } = new InGridMultiSelectVM();

        public static IEnumerable<InGridMultiSelectVM> GetRelationshipOptions()
        {
            var options = new string[]
            {
                "Reporting",
                "Coordination",
                "Liason",
                "Supervision"
            };

            return options.Select(e =>
              new InGridMultiSelectVM
              {
                  Text = e,
                  isSelected = false
              });

        }

        public static IEnumerable<InGridMultiSelectVM> GetRelationshipValues()
        {
            var options = GetFrequencyOptions();
            var values = options.Select(f =>
                new InGridMultiSelectVM
                {
                    Text = f.Text,
                    isSelected = true
                });

            return values;


        }
        
        public static InGridMultiSelectVM GetRelationshipDefaultValue(InGridMultiSelectVM model = null)
        {
            var options = GetFrequencyOptions();
            if (model == null || model.Text == null || string.IsNullOrEmpty(model.Text))
                return new InGridMultiSelectVM();

            var tes = new InGridMultiSelectVM();
            tes.Text = model.Text;
            return tes;
        }
        
    }
}