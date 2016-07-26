using Kendo.Mvc.UI;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Linq;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class FeeSlipVM : Item
    {
        public IEnumerable<FeeSlipDetailVM> FeeSlipDetails { get; set; } = new List<FeeSlipDetailVM>();

        [UIHint("Int32")]
        public int Slip { get; set; }

        [UIHint("Int32")]
        public int? ProfessionalID { get; set; }

        public string Name { get; set; }

        [UIHint("Date")]
        public string JoiningDate { get; set; }

        public string Designation { get; set; }

        public string PaymentMode { get; set; }

        [UIHint("Int32")]
        public int TakeHomePay { get; set; }
    }
}
