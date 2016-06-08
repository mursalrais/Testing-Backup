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
        [UIHint("Integer")]
        [DisplayName("Renewal#")]
        [Required]
        public int RenewalNumber { get; set;}

        /// <summary>
        /// renewalnumber for edit value
        /// </summary>
        [DisplayName("Renewal#")]
        [Required]
        public int PSARenewalNumber { get; set; }

        /// <summary>
        /// ProjectOrUnit
        /// </summary>
        [UIHint("ComboBox")]
        [DisplayName("Div/Project/Unit")]
        public ComboBoxVM ProjectOrUnit { get; set; } = new ComboBoxVM
        {
            Choices = new string[]
            {
                "Ops-P",
                "ME",
                "PM",
                "GP",
                "Ops -IT",
                "Ops",
                "CC -E",
                "Ops -F",
                "CC",
                "EO",
                "COM",
                "CC -SGA",
                "RI",
                "HN -NST",
                "No"
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
        [DisplayName("Tenure")]
        [Required]
        public int Tenure { get; set; }

        /// <summary>
        /// psaexpirydate
        /// </summary>
        [UIHint("Date")]
        [DisplayName("PSA Expiry Date")]
        public DateTime? PSAExpiryDate { get; set; } = DateTime.Now;

        public string ExpiryDateBefore { get; set; }

        /*
        [UIHint("Date")]
        public DateTime? ExpiryDateBefore { get; set; } = DateTime.Now;
        */

        [UIHint("MultiFileUploader")]
        [Required]
        public IEnumerable<HttpPostedFileBase> Documents { get; set; } = new List<HttpPostedFileBase>();

        public string DocumentUrl { get; set; }

        /// <summary>
        /// DocumentType
        /// </summary>
        /*
        [UIHint("ComboBox")]
        [DisplayName("Document Type")]
        public ComboBoxVM DocumentType { get; set; } = new ComboBoxVM
        {
            Choices = new string[]
            {
                "PSA Document",
                "MCC No Objection Letter"
            },
            Value = "PSA Document"
        };
        */

        public string DocumentType { get; set;}

        //public string KeyPosition { get; set; }

        //public string KeyPositionValue { get; set; }

        /// <summary>
        /// psaexpirydate
        /// </summary>
        [UIHint("Date")]
        public DateTime? Created { get; set; } = DateTime.Now;
    }
}
