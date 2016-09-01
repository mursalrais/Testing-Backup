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

        [UIHint("ComboBox")]
        [DisplayName("Application Status")]
        public ComboBoxVM WorkflowStatusOptions { get; set; } = new ComboBoxVM();

        public static IEnumerable<string> GetWorkflowStatusOptions(string currentStatus = null)
        {
            return new List<string>
            {
                Workflow.GetApplicationStatus(Workflow.ApplicationStatus.NEW),
                Workflow.GetApplicationStatus(Workflow.ApplicationStatus.ONBOARD)
            };
        }

        /// <summary>
        /// applicationstatus
        /// </summary>
        public string ApplicationStatus { get; set; }

        [UIHint("MultiFileUploader")]
        public IEnumerable<HttpPostedFileBase> Documents { get; set; } = new List<HttpPostedFileBase>();

        public string DocumentUrl { get; set; }

        [UIHint("TextArea")]
        public string SpecializationField { get; set; }

        [DisplayName("Years of Relevant Work")]
        public int YearRelevanWork { get; set; }

        [DisplayName("Months of Relevant Work")]
        public int MonthRelevantWork { get; set; }

        /// <summary>
        /// Title
        /// </summary>
        [DisplayName("Full Name")]
        [Required]
        public string FirstMiddleName { get; set; }

        /// <summary>
        /// position
        /// </summary>
        public string Position { get; set; }

        public string PositionName { get; set; }

        public int PositionID { get; set; }

        /// <summary>
        /// manpowerrequisition
        /// </summary>
        public int? ManpowerRequisitionID { get; set; }

        /// <summary>
        /// lastname
        /// </summary>
        [Required]
        public string LastName { get; set; }

        /// <summary>
        /// placeofbirth
        /// </summary>
        [Required]
        [DisplayName("Place of Birth")]
        public string PlaceOfBirth { get; set; }

        /// <summary>
        /// gender
        /// </summary>
        [UIHint("ComboBox")]
        public GenderComboBoxVM Gender { get; set; } = new GenderComboBoxVM();

        /// <summary>
        /// dateofbirth
        /// </summary>
        [UIHint("Date")]
        [Required]
        [DisplayName("Date of Birth")]
        public DateTime? DateOfBirth { get; set; } = DateTime.Now.AddYears(-28);

        /// <summary>
        /// nationality
        /// </summary>
        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM Nationality { get; set; } = new AjaxComboBoxVM
        {
            ControllerName = "Location",
            ActionName = "GetCountries",
            ValueField = "ID",
            TextField = "Title"
        };

        [DisplayName("Nationality")]
        public string NationalityName { get; set; }

        /// <summary>
        /// maritalstatus
        /// </summary>
        [UIHint("ComboBox")]
        public MaritalStatusComboBoxVM MaritalStatus { get; set; } = new MaritalStatusComboBoxVM();

        /// <summary>
        /// bloodtype
        /// </summary>
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
            Value = "A"
        };

        /// <summary>
        /// religion
        /// </summary>
        [UIHint("ComboBox")]
        public ReligionComboBoxVM Religion { get; set; } = new ReligionComboBoxVM();

        /// <summary>
        /// idcardtype
        /// </summary>
        [UIHint("AjaxCascadeComboBox")]
        [DisplayName("ID Card Type")]
        [Required]
        public AjaxCascadeComboBoxVM IDCardType { get; set; } = new AjaxCascadeComboBoxVM
        {
            ActionName = "GetIDCardType", 
            ControllerName = "HRApplication", 
            ValueField = "Value", 
            TextField = "Text", 
            Cascade = "Nationality_Value", 
            Filter = "filterIDCardType",
            OnSelectEventName = "onSelectIDCardType"
        };

        /// <summary>
        /// idcardnumber
        /// </summary>
        [Required]
        [DisplayName("ID Card Number")]
        public string IDCardNumber { get; set; }

        /// <summary>
        /// idcardexpirydate
        /// </summary>
        [UIHint("Date")]
        [DisplayName("ID Card Expiry")]
        public DateTime? IDCardExpiry { get; set; } = DateTime.Now;

        /// <summary>
        /// permanentaddress
        /// </summary>
        [UIHint("TextArea")]
        [DataType(DataType.MultilineText)]
        [Required]
        public string PermanentAddress { get; set; }

        /// <summary>
        /// currentaddress
        /// </summary>
        [UIHint("TextArea")]
        [DataType(DataType.MultilineText)]
        public string CurrentAddress { get; set; }

        /// <summary>
        /// permanentlandlinephone
        /// </summary>
        public string Telephone { get; set; }

        /// <summary>
        /// currentlandlinephone
        /// </summary>
        public string CurrentTelephone { get; set; }

        /// <summary>
        /// mobilephonenr
        /// </summary>
        [DisplayName("Mobile Number [1]")]
        [Required]
        public string MobileNumberOne { get; set; }

        /// <summary>
        /// mobilephonenr2
        /// </summary>
        [DisplayName("Mobile Number [2]")]
        public string MobileNumberTwo { get; set; }

        /// <summary>
        /// personalemail
        /// </summary>
        [UIHint("EmailAddress")]
        [DisplayName("Email Address [1]")]
        [Required]
        public string EmailAddresOne { get; set; }

        /// <summary>
        /// personalemail2
        /// </summary>
        [UIHint("EmailAddress")]
        [DisplayName("Email Address [2]")]
        public string EmailAddresTwo { get; set; }
    }
}
