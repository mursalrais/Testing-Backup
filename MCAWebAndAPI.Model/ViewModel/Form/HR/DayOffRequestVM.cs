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

        public IEnumerable<DayOffRequestDetailDisplayVM> DayOffRequestDetailsDisplay { get; set; } = new List<DayOffRequestDetailDisplayVM>();

        public IEnumerable<DayOffNextBalanceVM> DayOffNextBalance { get; set; } = new List<DayOffNextBalanceVM>();

        public string Professional { get; set; }

        [DisplayName("Name")]
        public string ProfessionalFullName { get; set; }

        public int PositionID { get; set; }

        public string PositionName { get; set; }

        public int? ProfessionalID { get; set; }

        [DisplayName("Project/Unit")]
        public string ProjectUnit { get; set; }

        public string Requestor { get; set; }

        public string StatusForm { get; set; }

        public string TypeForm { get; set; }

        [UIHint("Date")]
        [DisplayName("Day-Off Request Date")]
        [Required]
        public DateTime? RequestDate { get; set; } = DateTime.UtcNow;

        [UIHint("Date")]
        public DateTime? LastWorkingDate { get; set; } = DateTime.UtcNow;

        [UIHint("Date")]
        public DateTime? PSAExpiryDate { get; set; } = DateTime.UtcNow;

        public string ProfessionalStatus { get; set; }

        [UIHint("ComboBox")]
        public ComboBoxVM DayOffRequestStatus { get; set; } = new ComboBoxVM
        {
            Choices = new string[]
            {
                "Draft",
                "Pending Approval",
                "Approved",
                "Rejected"
            },
            Value = "Draft"
        };
    }
}
