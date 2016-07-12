using System;
using MCAWebAndAPI.Model.Common;
using System.Collections.Generic;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class PSAManagementVM : Item
    {
        /// <summary>
        /// WFPSANum
        /// </summary>
        [DisplayName("PSA Number")]
        public string PSANumber { get; set; }

        /// <summary>
        /// isrenewal
        /// </summary>
        [UIHint("ComboBox")]
        [DisplayName("Renewal?")]
        public ComboBoxVM IsRenewal { get; set; } = new ComboBoxVM
        {
            Choices = new string[] 
            {
                "Yes",
                "No"
            },
            Value = "Yes",
            OnSelectEventName = "isrenewalChanged"
        };

        /// <summary>
        /// renewalnumber for display value
        /// </summary>
        [DisplayName("Renewal#")]
        [Required]
        public int RenewalNumber { get; set;}

        /// <summary>
        /// renewalnumber for edit value
        /// </summary>
        [DisplayName("Renewal#")]
        [Required]
        public int PSARenewalNumber { get; set; }

        public int PSAId { get; set; }

        /// <summary>
        /// to keep the next renewalnumber
        /// </summary>
        public int HidRenewalNumber { get; set; }

        /// <summary>
        /// ProjectOrUnit
        /// </summary>
        [UIHint("ComboBox")]
        [DisplayName("Div/Project/Unit")]
        public ComboBoxVM ProjectOrUnit { get; set; } = new ComboBoxVM
        {
            Choices = new string[]
            {
                "Executive Director",
                "Executive Officer",
                "Legal Unit",
                "Monitoring & Evaluation Unit",
                "Communications & Outreach Unit",
                "Risk & Audit Unit",
                "Program Div.",
                "Procurement Modernization Project",
                "Community-Based Health & Nutrition Project",
                "Green Prosperity Project",
                "Cross-Cutting Sector",
                "Economic Analysis Unit",
                "Social & Gender Assessment Unit",
                "Environment & Social Performance Unit",
                "Operations Support Div.",
                "Finance Unit",
                "Procurement Unit",
                "Information Technology Unit",
                "Human Resources Unit",
                "Office Support Unit",
                "Fiscal Agent (FA)",
                "Procurement Agent (PA)"

            },
            Value = "Ops-P"
        };

        /// <summary>
        /// joindate
        /// </summary>
        [UIHint("Date")]
        [DisplayName("Join Date")]
        [Required]
        public DateTime? JoinDate { get; set; } = DateTime.Now;

        /// <summary>
        /// joindate in string format
        /// </summary>
        public string StrJoinDate { get; set; }

        /// <summary>
        /// dateofnewpsa
        /// </summary>
        [UIHint("Date")]
        [DisplayName("Date of New PSA")]
        [Required]
        public DateTime? DateOfNewPSA { get; set; } = DateTime.Now;

        /// <summary>
        /// position
        /// </summary>
        [UIHint("AjaxComboBox")]
        [DisplayName("Position Title")]
        [Required]
        public AjaxComboBoxVM Position { get; set; } = new AjaxComboBoxVM
        {
            ActionName = "GetPositions",
            ControllerName = "HRDataMaster",
            ValueField = "ID",
            TextField = "Desc",
            OnSelectEventName = "OnSelectPosition"
        };
        
        /// <summary>
        /// professional
        /// </summary>
        [UIHint("AjaxComboBox")]
        [DisplayName("Professional Name")]
        [Required]
        public AjaxComboBoxVM Professional { get; set; } = new AjaxComboBoxVM
        {
            ActionName = "GetProfessionals",
            ValueField = "ID",
            ControllerName = "HRDataMaster",
            TextField = "Desc",
            OnSelectEventName = "OnSelectAssetHolderFrom"
        };

        /// <summary>
        /// tenure
        /// </summary>
        [UIHint("Int32")]
        [DisplayName("Tenure")]
        [Required]
        public int Tenure { get; set; }

        [DisplayName("Tenure")]
        public string TenureString { get; set; }

        /// <summary>
        /// psaexpirydate
        /// </summary>
        [UIHint("Date")]
        [DisplayName("PSA Expiry Date")]
        public DateTime? PSAExpiryDate { get; set; } = DateTime.Now;

        public DateTime? HiddenExpiryDate { get; set; } = DateTime.Now;

        public string ExpiryDateBefore { get; set; }

        public DateTime? ExpireDateBefore { get; set; } = DateTime.Now;

        public string NextExpiryDate { get; set; }

       public DateTime? DateOfNewPSABefore { get; set; } = DateTime.Now;

       public string DateNewPSABefore { get; set; }

        [UIHint("MultiFileUploader")]
        [Required]
        public IEnumerable<HttpPostedFileBase> Documents { get; set; } = new List<HttpPostedFileBase>();

        /// <summary>
        /// Url to access document
        /// </summary>
        public string DocumentUrl { get; set; }

        /// <summary>
        /// Type of document
        /// </summary>
        public string DocumentType { get; set;}

        public string KeyPosition { get; set; }

        public string KeyPositionValue { get; set; }

        /// <summary>
        /// Created Date
        /// </summary>
        [UIHint("Date")]
        public DateTime? Created { get; set; } = DateTime.Now;

        /// <summary>
        /// PSA Status
        /// </summary>
        [UIHint("ComboBox")]
        public ComboBoxVM PSAStatus { get; set; } = new ComboBoxVM
        {
            Choices = new string[]
            {
                "Active",
                "Non Active"
            },
            Value = "Active"
        };

        /// <summary>
        /// Initiate Performance Plan
        /// </summary>
        [UIHint("Initiate Performance Plan")]
        [UIHint("ComboBox")]
        public ComboBoxVM PerformancePlan { get; set; } = new ComboBoxVM
        {
            Choices = new string[]
            {
                "No",
                "Yes"
            },
            Value = "No"
        };

    }
}
