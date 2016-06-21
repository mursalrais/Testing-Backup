using System;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MCAWebAndAPI.Model.Common;
using System.Collections.Generic;
using System.Linq;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class MonthlyFeeDetailVM : Item
    {
        /// <summary>
        /// dateofnewfee
        /// </summary>
        [UIHint("Date")]
        public DateTime? DateOfNewFee { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// monthlyfee
        /// </summary>
        [UIHint("Integer")]
        public int MonthlyFee { get; set; }

        /// <summary>
        /// annualfee
        /// </summary>
        [UIHint("Integer")]
        public int AnnualFee { get; set; }

        /// <summary>
        /// currency
        /// </summary>
        [UIHint("InGridAjaxComboBox")]
        public AjaxComboBoxVM Currency { get; set; } = new AjaxComboBoxVM();

        public static IEnumerable<AjaxComboBoxVM> GetCurrencyOptions()
        {
            var index = 0;
            var options = new string[]
            {
                "IDR",
                "USD"
            };

            return options.Select(e =>
              new AjaxComboBoxVM
              {
                  Value = ++index,
                  Text = e
              });
        }

        public static AjaxComboBoxVM GetCurrencyDefaultValue(AjaxComboBoxVM model = null)
        {
            var options = GetCurrencyOptions();
            if (model == null || model.Value == null || string.IsNullOrEmpty(model.Text))
                return options.FirstOrDefault();

            return options.FirstOrDefault(e =>
                e.Value == model.Value || e.Text == model.Text);
        }
    }
}
