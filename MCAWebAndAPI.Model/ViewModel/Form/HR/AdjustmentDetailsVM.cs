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
    public class AdjustmentDetailsVM : Item
    {

        public static AjaxComboBoxVM getAjusmentDefaultValue(AjaxComboBoxVM model = null)
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

        /// <summary>
        /// statusaplication
        /// </summary>
        [UIHint("InGridAjaxComboBox")]
        public AjaxComboBoxVM ajusmentType { get; set; } = new AjaxComboBoxVM();

        public static IEnumerable<InGridComboBoxVM> getAjusmentTypeOptions()
        {
            var index = 0;
            var options = new string[] {
                "Stop Award",
                "Retention Payment",
                "Overtime",
                "Adjustment"};

            return options.Select(e =>
                new InGridComboBoxVM
                {
                    Value = ++index,
                    Text = e
                });
        }

        public static AjaxComboBoxVM getCurrencyDefaultValue(AjaxComboBoxVM model = null)
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
        /// <summary>
        /// statusaplication
        /// </summary>
        [UIHint("InGridAjaxComboBox")]
        public AjaxComboBoxVM currency { get; set; } = new AjaxComboBoxVM();

        public static IEnumerable<InGridComboBoxVM> getCurrencyOptions()
        {
            var index = 0;
            var options = new string[] {
                "USD",
                "IDR"};

            return options.Select(e =>
                new InGridComboBoxVM
                {
                    Value = ++index,
                    Text = e
                });
        }

        public static AjaxComboBoxVM getpayTypeDefaultValue(AjaxComboBoxVM model = null)
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

        /// <summary>
        /// statusaplication
        /// </summary>
        [UIHint("InGridAjaxComboBox")]
        public AjaxComboBoxVM payType { get; set; } = new AjaxComboBoxVM();

        public static IEnumerable<InGridComboBoxVM> getPayTypeOptions()
        {
            var index = 0;
            var options = new string[] {
                "Credit",
                "Debit"};

            return options.Select(e =>
                new InGridComboBoxVM
                {
                    Value = ++index,
                    Text = e
                });
        }

        public static AjaxComboBoxVM getprofDefaultValue(AjaxComboBoxVM model = null)
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

        [UIHint("InGridAjaxComboBox")]
        public AjaxComboBoxVM ddlProfessional { get; set; } = new AjaxComboBoxVM();

        /// <head>
        /// CompensatoryID
        /// </head>
        /// 
        [DisplayName("Professional ID")]
        public int? profId { get; set; }


        /// <head>
        /// CompensatoryID
        /// </head>
        /// 
        public int? profName { get; set; }


        /// <head>
        /// CompensatoryDay
        /// </head>
        /// 
        [DisplayName("Project/Unit")]
        public string projUnit { get; set; }

        /// <head>
        /// CompensatoryDay
        /// </head>
        /// 
        [DisplayName("Position")]
        public string position { get; set; }

        /// <head>
        /// CompensatoryInitial
        /// </head>
        /// 
        [DisplayName("Amount")]
        public string cmpPosition { get; set; }

        /// <head>
        /// CompensatoryDay
        /// </head>
        /// 
        [DisplayName("Amount")]
        public string amount { get; set; }

        /// <head>
        /// CompensatoryDay
        /// </head>
        /// 
        [DisplayName("Remarks")]
        public string remark { get; set; }


        /// <summary>
        /// CompensatoryDate
        /// </summary>
        /// 
        [Required]
        [DisplayName("Period")]
        [DataType(DataType.Date)]
        [UIHint("Date")]
        public DateTime? period { get; set; } = DateTime.Now;

    }
}

