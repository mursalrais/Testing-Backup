using MCAWebAndAPI.Model.Common;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class PayrollRunVM : Item
    {
        [UIHint("Date")]
        public DateTime From { get; set; } = DateTime.Today;

        [UIHint("Date")]
        public DateTime To { get; set; } = DateTime.Today.AddDays(30);

        [UIHint("Date")]
        [DisplayName("13th Month Date")]
        public DateTime ThirteenMonthDate { get; set; } = DateTime.Today.AddMonths(5);

    }
}
