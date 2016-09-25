using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.Common;
using System.ComponentModel.DataAnnotations;
using MCAWebAndAPI.Model.ViewModel.Control;

namespace MCAWebAndAPI.Model.ViewModel.Form.Travel
{
    public class TRAAuthAdvReq_PerDiemInCountryVM : Item
    {
        private const double BREAKFAST_DEDUCTION_PERCENTAGE = 10;
        private const double LUNCH_DEDUCTION_PERCENTAGE = 30;
        private const double DINNER_DEDUCTION_PERCENTAGE = 30;

        [Required]
        [DataType(DataType.Date)]
        [UIHint("Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime Date { get; set; }

        [UIHint("InGridAjaxComboBox")]
        [Required]
        public AjaxComboBoxVM ProvinceOfDeparture { get; set; } = new AjaxComboBoxVM();

        [UIHint("InGridAjaxComboBox")]
        [Required]
        public AjaxComboBoxVM PlaceOfDeparture { get; set; } = new AjaxComboBoxVM();

        [UIHint("InGridAjaxComboBox")]
        [Required]
        public AjaxComboBoxVM ProvinceOfArrival { get; set; } = new AjaxComboBoxVM();

        [UIHint("InGridAjaxComboBox")]
        [Required]
        public AjaxComboBoxVM PlaceOfArrival { get; set; } = new AjaxComboBoxVM();

        [UIHint("InGridAjaxComboBox")]
        [Required]
        public AjaxComboBoxVM ProvinceOfRatePerDay { get; set; } = new AjaxComboBoxVM();

        //todo: onchange ProvinceOfRatePerDay
        //todo: values from allowablerate (?)
        public Decimal Rate { get; set; }

        public bool DeductBreakfast { get; set; }

        //todo: onchange Rate & DeductBreakfast
        public Decimal BreakfastDeductionAmount { get; set; }

        public double BreakfastDeductionPercentage
        {
            get
            {
                return BREAKFAST_DEDUCTION_PERCENTAGE;
            }
        }

        public bool DeductLunch { get; set; }

        //todo: onchange Rate & DeductBreakfast
        public Decimal LunchDeductionAmount { get; set; }

        public double LunchDeductionPercentage
        {
            get
            {
                return LUNCH_DEDUCTION_PERCENTAGE;
            }
        }

        public bool DeductDinner { get; set; }

        //todo: onchange Rate & DeductBreakfast
        public Decimal DinnerDeductionAmount { get; set; }

        public double DinnerDeductionPercentage
        {
            get
            {
                return DINNER_DEDUCTION_PERCENTAGE;
            }
        }

        public Decimal Amount
        {
            get
            {
                return Rate - BreakfastDeductionAmount - LunchDeductionAmount - DinnerDeductionAmount;
            }
        }

        public string Remarks { get; set; }
    }
}
