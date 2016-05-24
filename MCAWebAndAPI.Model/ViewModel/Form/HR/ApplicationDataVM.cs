using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class ApplicationDataVM : Item
    {
       
        public IEnumerable<EducationDetailVM> EducationDetails { get; set; } = new List<EducationDetailVM>();
        public IEnumerable<TrainingDetailVM> TrainingDetails { get; set; } = new List<TrainingDetailVM>();
        public IEnumerable<WorkingExperienceDetailVM> WorkingExperienceDetails { get; set; } = new List<WorkingExperienceDetailVM>();

        [UIHint("MultiFileUploader")]
        public IEnumerable<HttpPostedFileBase> Documents { get; set; } = new List<HttpPostedFileBase>();

        public string DocumentUrl { get; set; }

        [UIHint("TextArea")]
        public string SpecializationField { get; set; }

        [DisplayName("Years of Relevant Work")]
        public int YearRelevanWork { get; set; }

        [DisplayName("Months of Relevant Work")]
        public int MonthRelevantWork { get; set; }

        [DisplayName("First & Middle Name")]
        [Required]
        public string FirstMiddleName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [DisplayName("Place of Birth")]
        public string PlaceOfBirth { get; set; }

        [UIHint("ComboBox")]
        public GenderComboBoxVM Gender { get; set; } = new GenderComboBoxVM();

        [UIHint("Date")]
        [Required]
        [DisplayName("Date of Birth")]
        public DateTime? DateOfBirth { get; set; } = DateTime.Now.AddYears(-28);

        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM Nationality { get; set; } = new AjaxComboBoxVM
        {
            ControllerName = "Location",
            ActionName = "GetCountries",
            ValueField = "ID",
            TextField = "Title"
        };

        [UIHint("ComboBox")]
        public MaritalStatusComboBoxVM MaritalStatus { get; set; } = new MaritalStatusComboBoxVM();

        [UIHint("ComboBox")]
        public ComboBoxVM BloodType { get; set; } = new ComboBoxVM
        {
            Choices = new string[]
            {
                "A",
                "B",
                "O",
                "AB"
            },
            DefaultValue = "A"
        };

        [UIHint("ComboBox")]
        public ReligionComboBoxVM Religion { get; set; } = new ReligionComboBoxVM();

        [UIHint("ComboBox")]
        [DisplayName("ID Card Type")]
        [Required]
        public ComboBoxVM IDCardType { get; set; } = new ComboBoxVM
        {
            Choices = new string[]
            {
                "KTP",
                "KITAS",
                "Passport",
                "SIM"
            },
            DefaultValue = "KTP", 
            OnSelectEventName = "onSelectIDCardType"
        };

        [Required]
        [DisplayName("ID Card Number")]
        public string IDCardNumber { get; set; }

        [UIHint("Date")]
        [DisplayName("ID Card Expiry")]
        public DateTime? IDCardExpiry { get; set; } = DateTime.Now;

        [UIHint("TextArea")]
        [DataType(DataType.MultilineText)]
        [Required]
        public string PermanentAddress { get; set; }

        [UIHint("TextArea")]
        [DataType(DataType.MultilineText)]
        public string CurrentAddress { get; set; }


        [UIHint("PhoneNumber")]
        public string Telephone { get; set; }

        [UIHint("PhoneNumber")]
        public string CurrentTelephone { get; set; }

        [UIHint("PhoneNumber")]
        [DisplayName("Mobile Number [1]")]
        [Required]
        public string MobileNumberOne { get; set; }

        [UIHint("PhoneNumber")]
        [DisplayName("Mobile Number [2]")]
        public string MobileNumberTwo { get; set; }

        [UIHint("EmailAddress")]
        [DisplayName("Email Address [1]")]
        [Required]
        public string EmailAddresOne { get; set; }

        [UIHint("EmailAddress")]
        [DisplayName("Email Address [2]")]
        public string EmailAddresTwo { get; set; }
    }
}
