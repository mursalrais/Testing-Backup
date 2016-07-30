using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class InsuranceClaimAXAVM : Item
    {
     // public IEnumerable<ClaimComponentDetailAXAVM> ClaimComponentAXADetails { get; set; } = new List<ClaimComponentDetailAXAVM>();

        public DataTable dtDetails { get; set; } = new DataTable();

        [Required(ErrorMessage = "Sender Field Is Required")]
        [UIHint("string")]
        public string Sender { get; set; }

        [Required(ErrorMessage = "Recepient Field Is Required")]
        public string Recepient { get; set; }

        [Required(ErrorMessage = "Batch No Field Is Required")]
        [UIHint("string")]
        public string BatchNo { get; set; }

        [Required(ErrorMessage = "Submission Date Field Is Required")]
        [UIHint("Date")]
        public DateTime? SubmissionDate { get; set; } = DateTime.Now;



    }
}
