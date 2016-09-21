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
                model = new AjaxComboBoxVM { Text = "", Value = 0 };
                return model;
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
        [DisplayName("Adjustment Type")]
        public AjaxComboBoxVM ajusmentType { get; set; } = new AjaxComboBoxVM();

        public static IEnumerable<InGridComboBoxVM> getAjusmentTypeOptions()
        {
            var index = 0;
            var options = new string[] {
                "Spot Award",
                "Retention Payment",
                "Overtime",
                "Adjustment",
                "Deduction"};

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
                model = new AjaxComboBoxVM { Text = "", Value = 0 };
                return model;
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
        [DisplayName("Currency")]
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
                model = new AjaxComboBoxVM { Text = "", Value = 0 };
                return model;
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
        [DisplayName("Debit/Credit")]
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
                model = new AjaxComboBoxVM { Text = "", Value = 0 };
                return model;
            }
            else
            {
                return model;
            }
        }

        [UIHint("InGridAjaxComboBox")]
        [DisplayName("Name")]
        public AjaxComboBoxVM ddlProfessional { get; set; } = new AjaxComboBoxVM
          {
            ActionName = "GetProfessionalsActive",
            ValueField = "ID",
            ControllerName = "HRDataMaster",
            TextField = "Project_Unit"
        };

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
        [Required]
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
        public DateTime? period { get; set; }

    }
}

