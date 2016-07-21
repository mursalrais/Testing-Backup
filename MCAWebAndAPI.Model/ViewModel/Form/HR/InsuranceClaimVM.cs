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
    public class InsuranceClaimVM : Item
    {
        public IEnumerable<ClaimPaymentDetailVM> ClaimPaymentDetails { get; set; } = new List<ClaimPaymentDetailVM>();
        public IEnumerable<ClaimComponentDetailVM> ClaimComponentDetails { get; set; } = new List<ClaimComponentDetailVM>();

        [UIHint("Int32")]
        [Required(ErrorMessage = "Professional ID Field Is Required")]
        [DisplayName("Professional ID")]
        public int? ProfessionalID { get; set; }

        /// <summary>
        /// professional
        /// </summary>
        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM ProfessionalName { get; set; } = new AjaxComboBoxVM
        {
            ActionName = "GetProfessionals",
            ControllerName = "HRDataMaster",
            ValueField = "ID",
            TextField = "Name"
        };
       
       
        public string Name { get; set; }

       [UIHint("Date")]
        [Required(ErrorMessage = "Claim Date Field Is Required")]
        public DateTime? ClaimDate { get; set; }

        public string Position { get; set; }

        public int DependentID { get; set; }

        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM DependantName { get; set; } = new AjaxComboBoxVM
        {
            ActionName = "GetDependants",
            ControllerName = "HRDataMaster",
            ValueField = "ID",
            TextField = "Name",
            OnSelectEventName = "OnSelectDependantName"
        };

      [UIHint("ComboBox")]
        public ComboBoxVM Type { get; set; } = new ComboBoxVM
        {
            Choices = new string[]
            {
                "Professional",
                "Dependent",
            },
            Value = "Professional"
        };
        
        public int? IndividualInsuranceNumber { get; set; }

        public int? OrganizationInsuranceID { get; set; }

        [UIHint("ComboBox")]
        [Required(ErrorMessage = "Claim Status Field Is Required")]
        public ComboBoxVM ClaimStatus { get; set; } = new ComboBoxVM
        {
            Choices = new string[]
            {
                    "Draft", "Need HR to Validate", "Validated by HR", "Submitted to AXA", "Rejected","Paid"
            },
            Value = "Draft"

        };

       

    }
}
