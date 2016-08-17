using System;
using MCAWebAndAPI.Model.Common;
using System.Collections.Generic;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class AdjustmentDayOffBalanceVM : Item
    {
        /// <summary>
        /// professional
        /// </summary>
        [UIHint("AjaxComboBox")]
        [DisplayName("Professional Name")]
        [Required]
        public AjaxComboBoxVM Professional { get; set; } = new AjaxComboBoxVM
        {
            ActionName = "GetProfessionals",
            ValueField = "ID",
            ControllerName = "HRDataMaster",
            TextField = "Desc2",
            OnSelectEventName = "OnProfessional"
        };

        /// <summary>
        /// Professional Full Name
        /// </summary>
        [DisplayName("Professional Name")]
        public string ProfessionalName { get; set; }

        /// <summary>
        /// ProjectOrUnit
        /// </summary>
        [DisplayName("Project/Unit")]
        [Required]
        public string ProjectUnit { get; set; }

        /// <summary>
        /// Position
        /// </summary>
        [DisplayName("Position")]
        [Required]
        public string Position { get; set; }

        /// <summary>
        /// Adjustment Date
        /// </summary>
        [UIHint("Date")]
        [DisplayName("Adjustment Date")]
        [Required]
        public DateTime? AdjustmentDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Day-Off Type
        /// </summary>
        [UIHint("ComboBox")]
        [DisplayName("Day-Off Type")]
        [Required]
        public ComboBoxVM DayOffType { get; set; } = new ComboBoxVM
        {
            Choices = new string[]
            {
                "Annual Day-Off",
                "Special Day-Off",
                "Day-off due to Compensatory time",
                "Paternity"

            },
            Value = "Annual Day-Off",
            OnSelectEventName = "GetLastBalance"
        };

        /// <summary>
        /// Last Balance
        /// </summary>
        [DisplayName("Last Balance")]
        [Required]
        public int LastBalance { get; set; }

        /// <summary>
        /// Adjustment
        /// </summary>
        [DisplayName("Adjustment")]
        [Required]
        public int Adjustment { get; set; }

        /// <summary>
        /// Day-Off Type
        /// </summary>
        [UIHint("ComboBox")]
        [DisplayName("Debit/Credit")]
        [Required]
        public ComboBoxVM DebitCredit { get; set; } = new ComboBoxVM
        {
            Choices = new string[]
            {
                "Debit", 
                "Credit"
            },
            Value = "Debit",
            OnSelectEventName = "OnCalCulateNewBalance"
        };

        /// <summary>
        /// New Balance
        /// </summary>
        [DisplayName("New Balance")]
        [Required]
        public string NewBalance { get; set; }

        /// <summary>
        /// Remarks
        /// </summary>
        [DisplayName("Remarks")]
        [Required]
        public string Remarks { get; set; }
    }
}
