using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class PSAManagementVM
    {
        public int? ID { get; set; }

        [DisplayName("PSA Number")]
        public string psaNumber { get; set; }

        private ComboBoxVM isrenewal = new ComboBoxVM { Choices = new string[] { "Yes", "No" } };

        [DisplayName("Renewal#")]
        public int renewalnumber { get; set; }

        private ComboBoxVM ProjectOrUnit = new ComboBoxVM { Choices = new string[] { "GP", "HN", "PM" } };

        DateTime? joinDate = DateTime.Now;

        [UIHint("Date")]
        [DisplayName("Join Date")]
        public DateTime? joindate
        {
            get
            {
                return joinDate;
            }

            set
            {
                joinDate = value;
            }
        }

        DateTime? dateofNewPSA = DateTime.Now;

        [UIHint("Date")]
        [DisplayName("Date of New PSA")]
        public DateTime? dateofnewpsa
        {
            get
            {
                return dateofNewPSA;
            }

            set
            {
                dateofNewPSA = value;
            }
        }

        AjaxComboBoxVM position = new AjaxComboBoxVM
        {
            ActionName = "GetPositions",
            ControllerName = "HRDataMaster",
            ValueField = "ID",
            TextField = "Desc",
            OnSelectEventName = "OnSelectPosition"
        };

        [UIHint("AjaxComboBox")]
        [DisplayName("Position Title")]
        public AjaxComboBoxVM Position
        {
            get
            {
                return position;
            }

            set
            {
                position = value;
            }
        }

        AjaxComboBoxVM professional = new AjaxComboBoxVM
        {
            ActionName = "GetProfessionals",
            ValueField = "ID",
            ControllerName = "HRDataMaster",
            TextField = "Desc",
            OnSelectEventName = "OnSelectAssetHolderFrom"
        };

        [UIHint("AjaxComboBox")]
        [DisplayName("Professional Name")]
        public AjaxComboBoxVM Professional
        {
            get
            {
                return professional;
            }

            set
            {
                professional = value;
            }
        }

        [DisplayName("Tenure")]
        public int tenure { get; set; }

        DateTime? pSAExpiryDate = DateTime.Now;

        [UIHint("Date")]
        [DisplayName("PSA Expiry Date")]
        public DateTime? psaexpirydate
        {
            get
            {
                return pSAExpiryDate;
            }

            set
            {
                pSAExpiryDate = value;
            }
        }

        [UIHint("ComboBox")]
        [DisplayName("Renewal?")]
        public ComboBoxVM IsRenewal
        {
            get
            {
                return isrenewal;
            }

            set
            {
                isrenewal = value;
            }
        }

        [UIHint("ComboBox")]
        [DisplayName("Div/Project/Unit")]
        public ComboBoxVM ProjectOrUnit1
        {
            get
            {
                return ProjectOrUnit;
            }

            set
            {
                ProjectOrUnit = value;
            }
        }
    }
}
