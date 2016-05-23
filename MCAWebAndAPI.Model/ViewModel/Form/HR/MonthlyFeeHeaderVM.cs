using System;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class MonthlyFeeHeaderVM
    {
        private int _iD;

        public int? ProfessionalID { get; set; }

        public string ProjectUnit { get; set; }

        public string Position { get; set; }

        public string Status { get; set; }

        AjaxComboBoxVM _professionalName = new AjaxComboBoxVM
        {
            ActionName = "GetProfessionals",
            ControllerName = "HRDataMaster",
            ValueField = "ID",
            TextField = "Name",
            OnSelectEventName = "OnSelectProfessionalName"
        };

        [UIHint("Date")]
        public DateTime? JoinDate
        {
            get
            {
                return _joinDate;
            }

            set
            {
                _joinDate = value;
            }
        }

        [UIHint("Date")]
        public DateTime? DateOfNewPsa
        {
            get
            {
                return _dateOfNewPsa;
            }

            set
            {
                _dateOfNewPsa = value;
            }
        }

        [UIHint("Date")]
        public DateTime? EndOfContract
        {
            get
            {
                return _endOfContract;
            }

            set
            {
                _endOfContract = value;
            }
        }

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

        DateTime? _joinDate, _dateOfNewPsa, _endOfContract, _dateOfNewFee = DateTime.Now;

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
