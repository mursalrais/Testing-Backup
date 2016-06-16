using System;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MCAWebAndAPI.Model.Common;
using System.Collections.Generic;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class MonthlyFeeVM : Item
    {
        public IEnumerable<MonthlyFeeDetailVM> MonthlyFeeDetails { get; set; } = new List<MonthlyFeeDetailVM>();

        /// <summary>
        /// ProfessionalId
        /// </summary>
        [UIHint("Integer")]
        [Required(ErrorMessage = "Professional ID Field Is Required")]
        [DisplayName("Professional ID")]
        public int? ProfessionalID { get; set; }

        /// <summary>
        /// ProjectOrUnit
        /// </summary>
        public string ProjectUnit { get; set; }

        /// <summary>
        /// position
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// maritalstatus
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// professional
        /// </summary>
        [DisplayName("Professional Name")]
        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM ProfessionalNameEdit { get; set; } = new AjaxComboBoxVM
        {
            ActionName = "GetProfessionalMonthlyFeesEdit",
            ControllerName = "HRDataMaster",
            ValueField = "ID",
            TextField = "Name",
            OnSelectEventName = "OnSelectProfessionalName"
        };

        /// <summary>
        /// professional
        /// </summary>
        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM ProfessionalName { get; set; } = new AjaxComboBoxVM
        {
            ActionName = "GetProfessionalMonthlyFees",
            ControllerName = "HRDataMaster",
            ValueField = "ID",
            TextField = "Name",
            OnSelectEventName = "OnSelectProfessionalName"
        };

        /// <summary>
        /// joindate
        /// </summary>
        [Required(ErrorMessage = "Professional Doesn't Have PSA")]
        public string JoinDate { get; set; }

        /// <summary>
        /// dateofnewpsa
        /// </summary>
        public string DateOfNewPsa { get; set; }

        /// <summary>
        /// psaexpirydate
        /// </summary>
        public string EndOfContract { get; set; }
    }
}