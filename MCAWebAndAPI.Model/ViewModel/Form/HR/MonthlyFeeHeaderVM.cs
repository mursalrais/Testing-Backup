using System;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class MonthlyFeeHeaderVM
    {
        private int _iD;

        [DisplayName("Professional ID")]
        public int? ProfessionalID { get; set; }

        public string ProjectUnit { get; set; }

        public string Position { get; set; }

        public string Status { get; set; }

        AjaxComboBoxVM _professionalName = new AjaxComboBoxVM
        {
            ActionName = "GetProfessionalMonthlyFees",
            ControllerName = "HRDataMaster",
            ValueField = "ID",
            TextField = "Name",
            OnSelectEventName = "OnSelectProfessionalName"
        };

        public string JoinDate { get; set; }

        public string DateOfNewPsa { get; set; }

        public string EndOfContract { get; set; }

        [UIHint("Date")]
        public DateTime? DateOfNewFee
        {
            get
            {
                return _dateOfNewFee;
            }

            set
            {
                _dateOfNewFee = value;
            }
        }

        DateTime? _dateOfNewFee = DateTime.UtcNow;

        public int MonthlyFee { get; set; }

        public int AnnualFee { get; set; }

        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM ProfessionalName
        {
            get
            {
                return _professionalName;
            }

            set
            {
                _professionalName = value;
            }
        }

        [UIHint("ComboBox")]
        public ComboBoxVM Currency
        {
            get
            {
                return _currency;
            }

            set
            {
                _currency = value;
            }
        }

        public int ID
        {
            get
            {
                return _iD;
            }

            set
            {
                _iD = value;
            }
        }

        private ComboBoxVM _currency = new ComboBoxVM()
        {
            Choices = new string[]
            {
                "USD",
                "IDR"
            }
        };
    }
}
