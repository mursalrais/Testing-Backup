using MCAWebAndAPI.Model.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.HR
{
    public class TimesheetVM : Item
    {

        [UIHint("Month")]
        public DateTime? Period { get; set; } = DateTime.Today;

        public string UserLogin { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }

        [UIHint("Date")]
        public DateTime? From { get; set; } = DateTime.Today;

        [UIHint("Date")]
        public DateTime? To { get; set; } = DateTime.Today.AddDays(1);

        [DisplayName("Is Full Day?")]
        [UIHint("Boolean")]
        public bool IsFullDay { get; set; }

        public string ProjectUnit { get; set; }

        public IEnumerable<TimesheetDetailVM> TimesheetDetails { get; set; }

    }
}
