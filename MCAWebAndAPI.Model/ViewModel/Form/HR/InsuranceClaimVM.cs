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
        public IEnumerable<ClaimPaymentDetailVM> ClaimPaymentDetail { get; set; } = new List<ClaimPaymentDetailVM>();
        public IEnumerable<ClaimComponentDetailVM> ClaimComponentDetail { get; set; } = new List<ClaimComponentDetailVM>();

        /// <summary>
        /// professionalid
        /// </summary>
        public int ProfessionalID { get; set; }

        /// <summary>
        /// dependentid
        /// </summary>
        public int DependentID { get; set; }

        public string ProfessionalName { get; set; }

        /// <summary>
        /// type
        /// </summary>
        [UIHint("ComboBox")]
        public ComboBoxVM Type { get; set; } = new ComboBoxVM
        {
            Choices = new string[]
            {
                ""
            }
        };

        public int? OrganizationInsuranceID { get; set; }

        [UIHint("AjaxComboBox")]
        public AjaxComboBoxVM DependantName { get; set; } = new AjaxComboBoxVM
        {

        };

        public int IndividualInsuranceNumber { get; set; }

        [UIHint("Date")]
        public DateTime? SubmissonDate { get; set; }

        /// <summary>
        /// claimstatus
        /// </summary>
        [UIHint("ComboBox")]
        public ComboBoxVM ClaimStatus { get; set; } = new ComboBoxVM
        {
            Choices = new string[]
            {
                ""
            }
        };
    }
}
