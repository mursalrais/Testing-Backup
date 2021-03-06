﻿using System;
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

        [UIHint("Date")]
        public DateTime? EndDateFee { get; set; } = new DateTime(2099,12,31);  

        /// <summary>
        /// monthlyfee
        /// </summary>
        [UIHint("Int32")]
        public int MonthlyFee { get; set; }

        /// <summary>
        /// annualfee
        /// </summary>
        [UIHint("Int32")]
        public int AnnualFee { get; set; }

        /// <summary>
        /// currency
        /// </summary>
        [UIHint("InGridComboBox")]
        public InGridComboBoxVM Currency { get; set; } = new InGridComboBoxVM();

        public static IEnumerable<InGridComboBoxVM> GetCurrencyOptions()
        {
            var index = 0;
            var options = new string[]
            {
                "IDR",
                "USD"
            };

            return options.Select(e =>
              new InGridComboBoxVM
              {
                  Value = ++index,
                  Text = e
              });
        }

        public static InGridComboBoxVM GetCurrencyDefaultValue(InGridComboBoxVM model = null)
        {
            var options = GetCurrencyOptions();
            if (model == null || (model.Value == null && model.Text == null) || string.IsNullOrEmpty(model.Text))
                return options.FirstOrDefault();

            return options.FirstOrDefault(e => e.Value == null ?
                e.Value == model.Value : e.Text == model.Text);
        }
    }
}
