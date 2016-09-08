using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class MedicalCheckUpVM : Item
    {
       
        [UIHint("Int32")]
        [Required(ErrorMessage = "Professional ID Field Is Required")]
        [DisplayName("Professional ID")]
        public int? ProfessionalID { get; set; }

        public string ProfessionalTextName { get; set; }

        /// <summary>
        /// professional
        /// </summary>
        [UIHint("AjaxComboBox")]
        [Required(ErrorMessage = "Professional Name Field Is Required")]
        public AjaxComboBoxVM ProfessionalName { get; set; } = new AjaxComboBoxVM
        {
            ActionName = "GetProfessionals",
            ControllerName = "HRDataMaster",
            ValueField = "ID",
            TextField = "Name",
            OnSelectEventName = "OnSelectProfessionalName"
        };


        [UIHint("Date")]
        [Required(ErrorMessage = "Claim Date Field Is Required")]
        public DateTime? ClaimDate { get; set; } = DateTime.UtcNow;

        [DisplayName("Project/Unit")]
        [Required(ErrorMessage = "Project/Unit Field Is Required")]
        public string Unit { get; set; }

        [Required(ErrorMessage = "Position Field Is Required")]
        public string Position { get; set; }


        public string Description { get; set; } = "Medical Check Up";

        [DisplayName("Unit")]
        public string UnitQty { get; set; } = "1";

        public string WBS { get; set; } = "5511011 - PA1 - Benefits";

        public string GL { get; set; } = "6102000";

        [UIHint("Number")]
        [DisplayName("Claim Amount (IDR)")]
        [Required(ErrorMessage = "Claim Amount (IDR) Field Is Required")]
        public double Amount { get; set; }

        [UIHint("TextArea")]
        public string Remarks { get; set; }

        public string ClaimStatus { get; set; }

        public string UserPermission { get; set; }

        public string VisibleTo { get; set; }


        public string Year { get; set; }

        public string URL { get; set; }

        public string OfficeEmail { get; set; }
    }
}
