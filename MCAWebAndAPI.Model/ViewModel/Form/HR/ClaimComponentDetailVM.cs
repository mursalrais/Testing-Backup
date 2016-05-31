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
    public class ClaimComponentDetailVM : Item
    {
        /// <summary>
        /// insuranceclaimid
        /// </summary>
        public int InsuranceClaimID { get; set; }

        /// <summary>
        /// claimcomponenttype
        /// </summary>
        [UIHint("InGridComboBox")]
        public InGridComboBoxVM Type { get; set; }


        /// <summary>
        /// claimcomponentcurrency
        /// </summary>
        [UIHint("InGridComboBox")]
        public InGridComboBoxVM Currency { get; set; }

        /// <summary>
        /// claimcomponentamount
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// claimcomponentreceiptdate
        /// </summary>
        [UIHint("Date")]
        public DateTime? ReceiptDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// claimcomponentremarks
        /// </summary>
        public string Remarks { get; set; }
    }
}
