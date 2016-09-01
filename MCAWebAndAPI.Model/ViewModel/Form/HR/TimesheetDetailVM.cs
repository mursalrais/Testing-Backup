using MCAWebAndAPI.Model.Common;
using System;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class TimesheetDetailVM : Item
    {
        public string Type { get; set; }

        public string SubType { get; set; }

        public DateTime? Date { get; set; } = DateTime.Now;

        public double FullHalf { get; set; } = 1d;

        //public string Status { get; set; }

        public string Location { get; set; }

        public int? LocationID { get; set; }

    }
}
