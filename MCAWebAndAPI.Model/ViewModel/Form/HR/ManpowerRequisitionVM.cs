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
        public DateTime? DateRequested { get; set; } = DateTime.Now.AddYears(-28);
        public Boolean IsOnBehalfOf { get; set; } = new Boolean();

        /// <summary>
        /// projectunit
        /// </summary>
        [UIHint("ComboBox")]
        [DisplayName("Division/Project/Unit")]
        public ComboBoxVM DivisionProjectUnit { get; set; } = new ComboBoxVM();

        /// <summary>
        /// reportingto
        /// </summary>
        [UIHint("AjaxComboBox")]
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
        public int NoOfPerson { get; set; }

        /// <summary>
        /// Tenure
        /// </summary>
        public int Tenure { get; set; }

        /// <summary>
        /// joblocation
        /// </summary>
        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM JobLocation { get; set; } = new AjaxComboBoxVM
        {
            ControllerName = "Location",
            ActionName = "GetProvince",
            ValueField = "ID",
            TextField = "Title"
        };

        /// <summary>
        /// Objectives
        /// </summary>
        [UIHint("TextArea")]
        public string PositionObjectives { get; set; }

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
            TextField = "Desc"
        };

        /// <summary>
        /// positionrequested
        /// </summary>
        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM Position { get; set; } = new AjaxComboBoxVM
        {
            ControllerName = "HRDataMaster",
            ActionName = "GetPositions",
            ValueField = "ID",
            TextField = "Title"
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
        public DateTime? ExpectedJoinDate { get; set; } = null;

        /// <summary>
        /// istravelrequired
        /// </summary>
        public Boolean IsTravellingRequired { get; set; } = new Boolean();

        [UIHint("MultiFileUploader")]
        public IEnumerable<HttpPostedFileBase> Documents { get; set; } = new List<HttpPostedFileBase>();
        public string DocumentUrl { get; set; }

        public IEnumerable<WorkingRelationshipDetailVM> WorkingRelationshipDetails { get; set; } = new List<WorkingRelationshipDetailVM>();

        /// <summary>
        /// personnelmgmt
        /// </summary>
        public int PersonnelManagement { get; set; }

        /// <summary>
        /// budgetmgmt
        /// </summary>
        public int BudgetManagement { get; set; }
        [UIHint("CheckBoxItem")]
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
        public string TotalYrsOfExperience { get; set; }

        /// <summary>
        /// minimumeducation
        /// </summary>
        [UIHint("TextArea")]
        public string MinimumEducation { get; set; }

        /// <summary>
        /// Industry
        /// </summary>
        [UIHint("TextArea")]
        public string Industry { get; set; }

        /// <summary>
        /// minimumyrsofrelatedexperience
        /// </summary>
        [UIHint("TextArea")]
        public string MinimumYrsOfExperienceInRelatedField { get; set; }

        /// <summary>
        /// specifictechnicalskill
        /// </summary>
        [DisplayName("Specific Technical Skill/Qualification")]
        [UIHint("TextArea")]
        public string SpecificTechnicalSkillQualification { get; set; }

        /// <summary>
        /// personalattributes
        /// </summary>
        [DisplayName("Personal Attributes & Competencies")]
        [UIHint("TextArea")]
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
        

        
    }
}
