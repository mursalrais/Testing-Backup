using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class ClaimComponentDetailVM : Item
    {
        /// <summary>
        /// insuranceclaimid
        /// </summary>
        public int InsuranceClaimID { get; set; }

        /// <summary>
        /// claimcomponenttype
        /// </summary>
        [UIHint("InGridComboBox")]
        public InGridComboBoxVM Type { get; set; }
        public static IEnumerable<InGridComboBoxVM> GetTypeOptions()
        {
            var index = 0;
            var options = new string[]
            {
                "Medical Examination",
                "Laboratorium",
                "Prescription",
                "Others",
            };

            return options.Select(e =>
              new InGridComboBoxVM
              {
                  Value = ++index,
                  Text = e
              });
        }

        /// <summary>
        /// claimcomponentcurrency
        /// </summary>
        [UIHint("InGridComboBox")]
        public InGridComboBoxVM Currency { get; set; }
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
        /// <summary>
        /// claimcomponentamount
        /// </summary>
        [UIHint("Number")]
        [Required(ErrorMessage = "Amount Field Is Required")]
        public double Amount { get; set; }

        /// <summary>
        /// claimcomponentreceiptdate
        /// </summary>
        [UIHint("Date")]
        [Required(ErrorMessage = "Receipt Date Field Is Required")]
        public DateTime? ReceiptDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// claimcomponentremarks
        /// </summary>
        [UIHint("TextArea")]
        public string Remarks { get; set; }

       

        public static InGridComboBoxVM GetCurrencyDefaultValue(InGridComboBoxVM model = null)
        {
            var options = GetCurrencyOptions();
          
            if (model == null || string.IsNullOrEmpty(model.Text))
                return options.FirstOrDefault();

            return options.FirstOrDefault(e =>
                e.Value == model.Value || e.Text == model.Text);
        }

        public static InGridComboBoxVM GetTypeDefaultValue(InGridComboBoxVM model = null)
        {
            var options = GetTypeOptions();

            if (model == null ||  string.IsNullOrEmpty(model.Text))
                return options.FirstOrDefault();

            return options.FirstOrDefault(e =>
                e.Value == model.Value || e.Text == model.Text);
        }
    }
}
