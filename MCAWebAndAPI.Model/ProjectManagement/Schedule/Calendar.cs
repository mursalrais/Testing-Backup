using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ProjectManagement.Schedule
{
    public class Calendar
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public DateTime Startdate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsRecurring { get; set; }

        public bool IsAllDayEvent { get; set; }

    }
}
