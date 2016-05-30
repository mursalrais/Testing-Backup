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
        
        [DisplayName("PSA Number")]
        public string psaNumber { get; set; }

        [UIHint("ComboBox")]
        [DisplayName("Renewal?")]
        public ComboBoxVM isrenewal { get; set; } = new ComboBoxVM { Choices = new string[] { "Yes", "No" } };

        [DisplayName("Renewal#")]
        public int renewalnumber { get; set; }

        [UIHint("ComboBox")]
        [DisplayName("Div/Project/Unit")]
        [Required]
        public ComboBoxVM ProjectOrUnit { get; set; } = new ComboBoxVM { Choices = new string[] { "GP", "HN", "PM" } };

        [UIHint("Date")]
        [DisplayName("Join Date")]
        [Required]
        public DateTime? joinDate { get; set; } = DateTime.Now;

        [UIHint("Date")]
        [DisplayName("Date of New PSA")]
        [Required]
        public DateTime? dateofNewPSA { get; set; } = DateTime.Now;

        
        [UIHint("AjaxComboBox")]
        [DisplayName("Position Title")]
        [Required]
        public AjaxComboBoxVM position { get; set; } = new AjaxComboBoxVM
        {
            ActionName = "GetPositions",
            ControllerName = "HRDataMaster",
            ValueField = "ID",
            TextField = "Desc",
            OnSelectEventName = "OnSelectPosition"
        };

        [UIHint("AjaxComboBox")]
        [DisplayName("Professional Name")]
        [Required]
        public AjaxComboBoxVM professional { get; set; } = new AjaxComboBoxVM
        {
            ActionName = "GetProfessionals",
            ValueField = "ID",
            ControllerName = "HRDataMaster",
            TextField = "Desc",
            OnSelectEventName = "OnSelectAssetHolderFrom"
        };

        [DisplayName("Tenure")]
        [Required]
        public int tenure { get; set; }


        [UIHint("Date")]
        [DisplayName("PSA Expiry Date")]
        public DateTime? pSAExpiryDate { get; set; } = DateTime.Now;

        //public DateTime test { get; set; } = DateTime.Today

        [UIHint("MultiFileUploader")]
        [Required]
        public IEnumerable<HttpPostedFileBase> Documents { get; set; } = new List<HttpPostedFileBase>();

        public string DocumentUrl { get; set; }
    }
}
