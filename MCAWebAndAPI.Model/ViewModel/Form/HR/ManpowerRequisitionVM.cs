using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class ManpowerRequisitionVM : Item
    {
        /// <summary>
        /// requestdate
        /// </summary>
        [UIHint("Date")]
        [Required]
        public DateTime? DateRequested { get; set; } = DateTime.Now;
        public Boolean IsOnBehalfOf { get; set; } = new Boolean();

        /// <summary>
        /// projectunit
        /// </summary>
        [UIHint("ComboBox")]
        [DisplayName("Project Unit")]
        [Required]
        public ComboBoxVM DivisionProjectUnit { get; set; } = new ComboBoxVM()
        {
            Choices = new string[]
            {
                "Communications & Outreach Unit",
                "Community-Based Health & Nutrition Project",
                "Cross-Cutting Sector",
                "Economic Analysis Unit",
                "Environment & Social Performance Unit",
                "Executive Director",
                "Finance Unit",
                "Human Resources Unit",
                "Information Technology Unit",
                "Legal Unit",
                "Monitoring & Evaluation Unit",
                "Office Support Unit",
                "Operations Support Div.",
                "Procurement Modernization Project",
                "Program Div."

            }
        };

        /// <summary>
        /// reportingto
        /// </summary>
        [UIHint("AjaxComboBox")]
        [Required]
        public AjaxComboBoxVM ReportingTo { get; set; } = new AjaxComboBoxVM
        {
            ControllerName = "HRDataMaster",
            ActionName = "GetProfessionals",
            ValueField = "ID",
            TextField = "Desc"
        };

        /// <summary>
        /// numberofperson
        /// </summary>

        [DisplayName("No. Of Person")]
        [UIHint("Int32")]
        [Required]
        public int NoOfPerson { get; set; }

        /// <summary>
        /// Tenure
        /// </summary>
        [UIHint("Int32")]
        [Required]
        [DisplayName("Tenure (Month)")]
        public int Tenure { get; set; }

        /// <summary>
        /// joblocation
        /// </summary>
        [UIHint("AjaxComboBox")]
        [Required]
        public AjaxComboBoxVM JobLocation { get; set; } = new AjaxComboBoxVM
        {
            ControllerName = "Location",
            ActionName = "GetProvinces",
            ValueField = "ID",
            TextField = "Title"
        };

        /// <summary>
        /// Objectives
        /// </summary>
        [UIHint("TextArea")]
        [Required]
        public string PositionObjectives { get; set; }

        public string Username { get; set; }

        public string EmailOnBehalf { get; set; }

        public string ProjectUnitString { get; set; }


        /// <summary>
        /// iskeyposition
        /// </summary>
        public Boolean IsKeyPosition { get; set; } = new Boolean();

        /// <summary>
        /// onbehalfof
        /// </summary>
        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM OnBehalfOf { get; set; } = new AjaxComboBoxVM
        {
            ControllerName = "HRDataMaster",
            ActionName = "GetProfessionals",
            ValueField = "ID",
            TextField = "Desc",
            OnSelectEventName = "OnBehalfOfChange"
        };

        /// <summary>
        /// positionrequested
        /// </summary>
        [UIHint("AjaxCascadeComboBox")]
        [Required]
        public AjaxCascadeComboBoxVM Position { get; set; } = new AjaxCascadeComboBoxVM
        {
            ControllerName = "HRDataMaster",
            ActionName = "GetPositionsManpower",
            ValueField = "ID",
            TextField = "PositionName",
            OnSelectEventName = "onPositionChange",
            Filter = "filterLevel",
            Cascade = "DivisionProjectUnit_Value"

        };

        /// <summary>
        /// secondaryreportingto
        /// </summary>
        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM SecondaryReportingTo { get; set; } = new AjaxComboBoxVM
        {
            ControllerName = "HRDataMaster",
            ActionName = "GetProfessionals",
            ValueField = "ID",
            TextField = "Desc"
        };

        /// <summary>
        /// expectedjoindate
        /// </summary>
        [UIHint("Date")]
        [Required]
        public DateTime? ExpectedJoinDate { get; set; } = DateTime.Now;

        /// <summary>
        /// istravelrequired
        /// </summary>
        public Boolean IsTravellingRequired { get; set; } = new Boolean();

        [UIHint("MultiFileUploader")]
        [DisplayName("Documents (Max. 2MB)")]
        public IEnumerable<HttpPostedFileBase> Documents { get; set; } = new List<HttpPostedFileBase>();


        public string DocumentUrl { get; set; }

        public IEnumerable<WorkingRelationshipDetailVM> WorkingRelationshipDetails { get; set; } = new List<WorkingRelationshipDetailVM>();

        /// <summary>
        /// personnelmgmt
        /// </summary>
        [UIHint("Int32")]
        [Required]
        [DisplayName("Personnel Management (persons)")]
        public int PersonnelManagement { get; set; }

        /// <summary>
        /// budgetmgmt
        /// </summary>
        [UIHint("Int32")]
        [Required]
        [DisplayName("Budget Management (USD)")]
        public int BudgetManagement { get; set; }
        [UIHint("CheckBoxItem")]
        [Required]
        public IEnumerable<CheckBoxItemVM> Workplan { get; set; } = new List<CheckBoxItemVM>();

        /// <summary>
        /// Workplan
        /// </summary>
        [UIHint("CheckBoxItem")]
        public CheckBoxItemVM workplanItem = new CheckBoxItemVM();

        /// <summary>
        /// totalyrsofexperience
        /// </summary>
        [UIHint("TextArea")]
        [Required]
        [DisplayName("Total Years of Experience")]
        public string TotalYrsOfExperience { get; set; }

        /// <summary>
        /// minimumeducation
        /// </summary>
        [UIHint("TextArea")]
        [Required]
        public string MinimumEducation { get; set; }

        /// <summary>
        /// Industry
        /// </summary>
        [UIHint("TextArea")]
        [Required]
        public string Industry { get; set; }

        /// <summary>
        /// minimumyrsofrelatedexperience
        /// </summary>
        [UIHint("TextArea")]
        [DisplayName("Minimum Years of Experience in Related Field")]
        public string MinimumYrsOfExperienceInRelatedField { get; set; }

        /// <summary>
        /// specifictechnicalskill
        /// </summary>
        [DisplayName("Specific Technical Skill/Qualification")]
        [Required]
        [UIHint("TextArea")]
        public string SpecificTechnicalSkillQualification { get; set; }

        /// <summary>
        /// personalattributes
        /// </summary>
        [DisplayName("Personal Attributes & Competencies")]
        [UIHint("TextArea")]
        [Required]
        public string PersonalAttributesCompetencies { get; set; }

        /// <summary>
        /// otherrequirements
        /// </summary>
        [UIHint("TextArea")]
        public string OtherRequirements { get; set; }

        /// <summary>
        /// remarks
        /// </summary>
        [UIHint("TextArea")]
        public string Remarks { get; set; }

        [UIHint("ComboBox")]
        [DisplayName("Status")]
        public ComboBoxVM Status { get; set; } = new ComboBoxVM()
        {
            Choices = new string[]{
                "Pending MCC Approval",
                "Rejected by MCC",
                "Approved by MCC",
                "Active",
                "Filled",
                "Cancelled"
            }
        };


    }
}
