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
    public class ClaimPaymentDetailVM : Item
    {
        /// <summary>
        /// insuranceid
        /// </summary>
        public int InsuranceID { get; set; }

        public int No { get; set; }

        /// <summary>
        /// claimdate
        /// </summary>
        [UIHint("Date")]
        public DateTime? Date { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// paymentstatus
        /// </summary>
        [UIHint("InGridComboBox")]
        public InGridComboBoxVM Status { get; set; }

        /// <summary>
        /// paymentcurrency
        /// </summary>
        [UIHint("InGridComboBox")]
        public InGridComboBoxVM Currency { get; set; }

        /// <summary>
        /// paymentamount
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// paymentremarks
        /// </summary>
        public string Remarks { get; set; }
    }
}
