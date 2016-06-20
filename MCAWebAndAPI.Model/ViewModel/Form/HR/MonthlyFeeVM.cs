using System;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MCAWebAndAPI.Model.Common;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class MonthlyFeeVM : Item
    {
        /// <summary>
        /// ProfessionalId
        /// </summary>
        [UIHint("Int32")]
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

        /// <summary>
        /// DateOfNewFee
        /// </summary>
        [Required(ErrorMessage = "Date Of New Fee Field Is Required")]
        [UIHint("Date")]
        public DateTime? DateOfNewFee { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// MonthlyFee
        /// </summary>
        [Range(1, Int32.MaxValue, ErrorMessage = "Monthly Fee Field Can't Be Zero or Negative")]
        public int MonthlyFee { get; set; }

        /// <summary>
        /// AnnualFee
        /// </summary>
        [UIHint("Int32")]
        public int AnnualFee { get; set; }

        /// <summary>
        /// MonthlyFeeCurrency
        /// </summary>
        [UIHint("ComboBox")]
        public ComboBoxVM Currency { get; set; } = new ComboBoxVM
        {
            Choices = new string[]
            {
                "USD",
                "IDR"
            }
        };
    }
}