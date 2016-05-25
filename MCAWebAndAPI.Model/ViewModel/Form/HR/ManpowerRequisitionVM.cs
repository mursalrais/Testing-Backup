using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class ManpowerRequisition : Item
    {
        [UIHint("Date")]
        public DateTime? DateRequested { get; set; } = DateTime.Now.AddYears(-28);
        public Boolean IsOnBehalfOf { get; set; } = new Boolean();

        [UIHint("ComboBox")]
        [DisplayName("Division/Project/Unit")]
        public ComboBoxVM DivisionProjectUnit { get; set; } = new GenderComboBoxVM();

        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM ReportingTo { get; set; } = new AjaxComboBoxVM
        {
            ControllerName = "HRDataMaster",
            ActionName = "GetProfessionals",
            ValueField = "ID",
            TextField = "Title"
        };

        [DisplayName("No. Of Person")]
        public int NoOfPerson { get; set; }

        public int Tenure { get; set; }

        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM JobLocation { get; set; } = new AjaxComboBoxVM
        {
            ControllerName = "Location",
            ActionName = "GetProvince",
            ValueField = "ID",
            TextField = "Title"
        };

        [UIHint("TextArea")]
        public string PosisitionObjectives { get; set; }

        public Boolean IsKeyPosition { get; set; } = new Boolean();

        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM OnBehalfOf { get; set; } = new AjaxComboBoxVM
        {
            ControllerName = "HRDataMaster",
            ActionName = "GetProfessionals",
            ValueField = "ID",
            TextField = "Title"
        };

        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM Poisition { get; set; } = new AjaxComboBoxVM
        {
            ControllerName = "HRDataMaster",
            ActionName = "GetPositions",
            ValueField = "ID",
            TextField = "Title"
        };

        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM SecondaryReportingTo { get; set; } = new AjaxComboBoxVM
        {
            ControllerName = "HRDataMaster",
            ActionName = "GetProfessionals",
            ValueField = "ID",
            TextField = "Title"
        };

        [UIHint("Date")]
        public DateTime? ExpectedJoinDate { get; set; } = null;

        public Boolean IsTravellingRequired { get; set; } = new Boolean();

        [UIHint("MultiFileUploader")]
        public IEnumerable<HttpPostedFileBase> SupportingDocuments { get; set; } = new List<HttpPostedFileBase>();

        public IEnumerable<WorkingRelationshipDetailVM> WorkingRelationshipDetails { get; set; } = new List<WorkingRelationshipDetailVM>(); 

        public int PersonnelManagement { get; set; }

        public int BudgetManagement { get; set; }

        public IEnumerable<CheckBoxItemVM> Workplan { get; set; } = new List<CheckBoxItemVM>();

        [UIHint("TextArea")]
        public string TotalYrsOfExperience { get; set; }

        [UIHint("TextArea")]
        public string MinimumEducation { get; set; }

        [UIHint("TextArea")]
        public string Industry { get; set; }

        [UIHint("TextArea")]
        public string MinimumYrsOfExperienceInRelatedField { get; set; }

        [DisplayName("Specific Technical Skill/Qualification")]
        [UIHint("TextArea")]
        public string SpecificTechnicalSkillQualification { get; set; }

        [DisplayName("Personal Attributes & Competencies")]
        [UIHint("TextArea")]
        public string PersonalAttributesCompetencies { get; set; }

        [UIHint("TextArea")]
        public string OtherRequirements { get; set; }

        [UIHint("TextArea")]
        public string Remarks { get; set; }
        

        
    }
}
