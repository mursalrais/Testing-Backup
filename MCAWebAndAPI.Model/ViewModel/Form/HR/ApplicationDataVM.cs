using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class ApplicationDataVM
    {
        IEnumerable<EducationDetailVM> _educationDetails = new List<EducationDetailVM>();
        IEnumerable<TrainingDetailVM> _trainingDetails = new List<TrainingDetailVM>();
        IEnumerable<WorkingExperienceDetailVM> _workingExperienceDetails = new List<WorkingExperienceDetailVM>();

        IEnumerable<HttpPostedFileBase> _documents = new List<HttpPostedFileBase>();

        [UIHint("TextArea")]
        public string SpecializationField { get; set; }


        [DisplayName("Years of Relevant Work")]
        public int YearRelevanWork { get; set; }

        [DisplayName("Months of Relevant Work")]
        public int MonthRelevantWork { get; set; }

        [DisplayName("First & Middle Name")]
        public string FirstMiddleName { get; set; }

        public string LastName { get; set; }

        [DisplayName("Place of Birth")]
        public string PlaceOfBirth { get; set; }

        GenderComboBoxVM _gender = new GenderComboBoxVM();

        public string MyProperty { get; set; }

        [UIHint("Date")]
        [DisplayName("Date of Birth")]
        public DateTime? DateOfBirth
        {
            get
            {
                return _dateOfBirth;
            }
            set
            {
                _dateOfBirth = value;
            }
        }

        DateTime? _dateOfBirth = DateTime.Now.AddYears(-28);
        AjaxComboBoxVM _nationality = new AjaxComboBoxVM
        {
            ControllerName = "Location",
            ActionName = "GetCountries",
            ValueField = "ID",
            TextField = "Title"
        };

        MaritalStatusComboBoxVM _maritalStatus = new MaritalStatusComboBoxVM();
        ComboBoxVM _bloodType = new ComboBoxVM
        {
            Choices = new string[]
            {
                "A",
                "B",
                "C",
                "D"
            },
            DefaultValue = "A"
        };

        ReligionComboBoxVM _religion = new ReligionComboBoxVM();
        ComboBoxVM _IDCardType = new ComboBoxVM
        {
            Choices = new string[]
            {
                "KTP",
                "KITAS",
                "Passport",
                "SIM"
            },
            DefaultValue = "KTP"
        };

        [DisplayName("ID Card Number")]
        public string IDCardNumber { get; set; }

        [UIHint("Date")]
        [DisplayName("ID Card Expiry")]
        public DateTime? IDCardExpiry
        {
            get
            {
                return _IDCardExpiry;
            }
            set
            {
                _IDCardExpiry = value;
            }
        }

        DateTime? _IDCardExpiry = DateTime.Now;

        [UIHint("TextArea")]
        [DataType(DataType.MultilineText)]
        public string PermanentAddress { get; set; }

        [UIHint("TextArea")]
        [DataType(DataType.MultilineText)]
        public string CurrentAddress { get; set; }


        [UIHint("ComboBox")]
        [DisplayName("ID Card Type")]
        public ComboBoxVM IDCardType
        {
            get
            {
                return _IDCardType;
            }
            set
            {
                _IDCardType = value;
            }
        }

        [UIHint("ComboBox")]
        public MaritalStatusComboBoxVM MaritalStatus
        {
            get
            {
                return _maritalStatus;
            }
            set
            {
                _maritalStatus = value;
            }
        }

        [UIHint("ComboBox")]
        public ComboBoxVM BloodType
        {
            get
            {
                return _bloodType;
            }

            set
            {
                _bloodType = value;
            }
        }

        [UIHint("ComboBox")]
        public ReligionComboBoxVM Religion
        {
            get
            {
                return _religion;
            }

            set
            {
                _religion = value;
            }
        }

        [UIHint("ComboBox")]
        public GenderComboBoxVM Gender
        {
            get
            {
                return _gender;
            }

            set
            {
                _gender = value;
            }
        }

        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM Nationality
        {
            get
            {
                return _nationality;
            }

            set
            {
                _nationality = value;
            }
        }

        [UIHint("PhoneNumber")]
        [RegularExpression("([0-9]+)")]
        public string Telephone { get; set; }

        [UIHint("PhoneNumber")]
        [RegularExpression("([0-9]+)")]
        public string CurrentTelephone { get; set; }

        [UIHint("PhoneNumber")]
        [RegularExpression("([0-9]+)")]
        [DisplayName("Mobile Number [1]")]
        public string MobileNumberOne { get; set; }

        [UIHint("PhoneNumber")]
        [RegularExpression("([0-9]+)")]
        [DisplayName("Mobile Number [2]")]
        public string MobileNumberTwo { get; set; }

        [UIHint("EmailAddress")]
        [DisplayName("Email Address [1]")]
        public string EmailAddresOne { get; set; }

        [UIHint("EmailAddress")]
        [DisplayName("Email Address [2]")]
        public string EmailAddresTwo { get; set; }

        public IEnumerable<TrainingDetailVM> TrainingDetails
        {
            get
            {
                return _trainingDetails;
            }

            set
            {
                _trainingDetails = value;
            }
        }

        public IEnumerable<EducationDetailVM> EducationDetails
        {
            get
            {
                return _educationDetails;
            }

            set
            {
                _educationDetails = value;
            }
        }

        [UIHint("MultiFileUploader")]
        public IEnumerable<HttpPostedFileBase> Documents
        {
            get
            {
                return _documents;
            }

            set
            {
                _documents = value;
            }
        }

        public IEnumerable<WorkingExperienceDetailVM> WorkingExperienceDetails
        {
            get
            {
                return _workingExperienceDetails;
            }

            set
            {
                _workingExperienceDetails = value;
            }
        }
    }
}
