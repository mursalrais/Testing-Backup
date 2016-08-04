using MCAWebAndAPI.Model.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class PayrollRunVM : Item
    {
        [UIHint("Month")]
        public DateTime Period { get; set; } = DateTime.Now;
    }
}
