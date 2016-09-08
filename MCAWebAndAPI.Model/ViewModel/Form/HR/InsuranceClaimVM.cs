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
       // public IEnumerable<ClaimPaymentDetailVM> ClaimPaymentDetails { get; set; } = new List<ClaimPaymentDetailVM>();
        public IEnumerable<ClaimComponentDetailVM> ClaimComponentDetails { get; set; } = new List<ClaimComponentDetailVM>();

        [UIHint("Int32")]
        [Required(ErrorMessage = "Professional ID Field Is Required")]
        [DisplayName("Professional ID")]
        public int? ProfessionalID { get; set; }

        public string ProfessionalTextName { get; set; }

        /// <summary>
        /// professional
        /// </summary>
        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM ProfessionalName { get; set; } = new AjaxComboBoxVM
        {
            ActionName = "GetProfessionalsActive",
            ControllerName = "HRDataMaster",
            ValueField = "ID",
            TextField = "Name",
             OnSelectEventName = "OnSelectProfessionalName"
        };
       
       
       [UIHint("Date")]
        [Required(ErrorMessage = "Claim Date Field Is Required")]
        public DateTime? ClaimDate { get; set; }

        
        public string Position { get; set; }

        public int? DependentID { get; set; }


        // [UIHint("AjaxComboBox")]
        //public AjaxComboBoxVM DependantName { get; set; } = new AjaxComboBoxVM
        // {
        //     ActionName = "GetDependantsForInsurance",
        //     ControllerName = "HRDataMaster",
        //     ValueField = "ID",
        //     TextField = "Name",
        //     OnSelectEventName = "OnSelectDependantName"
        // };


        [UIHint("AjaxCascadeComboBox")]
        public AjaxCascadeComboBoxVM DependantName { get; set; } = new AjaxCascadeComboBoxVM
        {
            ActionName = "GetDependantsForInsurance",
            ControllerName = "HRDataMaster",
            ValueField = "ID",
            TextField = "Name",
            Cascade = "ProfessionalName_Value",
            Filter = "filterEventProfessionalID",
            OnSelectEventName = "OnSelectDependantName"
        };

        [UIHint("ComboBox")]
        public ComboBoxVM Type { get; set; } = new ComboBoxVM
        {
            Choices = new string[]
          {
                            "Professional",
                            "Dependent"
          },
            OnSelectEventName = "OnSelectType"
        };


        //[UIHint("DropDown")]
        //public DropDownVM Type { get; set; } = new DropDownVM
        //{
        //    Choices = new[]
        //      {
        //        new DropDownVM() {Text="Professional" ,Value = "Professional"},
        //         new DropDownVM() {Text="Dependent" ,Value = "Dependent"}
        //    },

        //    OnSelectEventName = "OnSelectType"
        //};


        public string IndividualInsuranceNumber { get; set; }

        [DisplayName("Insurance Account Number")]
        [Required(ErrorMessage = "Insurance Account Numbe Field Is Required")]
        public string OrganizationInsuranceID { get; set; }

        [Required(ErrorMessage = "Claim Status Field Is Required")]
        public string ClaimStatus { get; set; }

        public string UserPermission { get; set; }

        public string VisibleTo { get; set; }


        [UIHint("ComboBox")]
        [DisplayName("Claim Status")]
        public ComboBoxVM ClaimStatusHR { get; set; } = new ComboBoxVM
        {
            Choices = new string[]
           {
                String.Empty,
                            "Need HR to Validate",
                            "Validated by HR",
                            "Submitted to AXA",
                            "Rejected",
                            "Paid"
           },
            OnSelectEventName = "OnSelectClaim"
        };


        //[UIHint("DropDown")]
        //[DisplayName("Claim Status")]
        //[Required(ErrorMessage = "Claim Status Field Is Required")]
        //public DropDownVM ClaimStatusHR { get; set; } = new DropDownVM
        //{
        //    Choices = new[]
        //    {
        //         new DropDownVM() {Text=String.Empty,Value = String.Empty},
        //         new DropDownVM() {Text="Need HR to Validate" ,Value = "Need HR to Validate"},
        //         new DropDownVM() {Text="Validated by HR" ,Value = "Validated by HR"},
        //         new DropDownVM() {Text="Submitted to AXA" ,Value = "Submitted to AXA"},
        //         new DropDownVM() {Text="Rejected" ,Value = "Rejected"},
        //         new DropDownVM() {Text="Paid" ,Value = "Paid"}
        //    },

        //    OnSelectEventName = "OnSelectClaim"


        //};


        //[UIHint("Number")]
        public decimal TotalAmount { get; set; }

        [DisplayName("Total Amount")]
        public decimal TotalAmountTemp { get; set; }

        public int Year { get; set; }

        public string URL { get; set; }
    }
}
