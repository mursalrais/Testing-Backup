using System;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MCAWebAndAPI.Model.Common;
using System.Collections.Generic;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class DayOffRequestVM : Item
    {
        public IEnumerable<DayOffRequestDetailVM> DayOffRequestDetails { get; set; } = new List<DayOffRequestDetailVM>();
        public IEnumerable<DayOffBalanceVM> DayOffBalanceDetails { get; set; } = new List<DayOffBalanceVM>();

        public string Professional { get; set; }

        public int? ProfessionalID { get; set; }

        public string ProjectUnit { get; set; }

        [UIHint("Date")]
        public DateTime? RequestDate { get; set; } = DateTime.UtcNow;
    }
}
