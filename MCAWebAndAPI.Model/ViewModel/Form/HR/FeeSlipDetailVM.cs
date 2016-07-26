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
    public class FeeSlipDetailVM : Item
    {
        [UIHint("Int32")]
        public int Fee { get; set; }

        [UIHint("Int32")]
        public int Deduction { get; set; }

        [UIHint("Int32")]
        public int TotalIncome { get; set; }

        [UIHint("Int32")]
        public int TotalDeduction { get; set; }
    }
}
