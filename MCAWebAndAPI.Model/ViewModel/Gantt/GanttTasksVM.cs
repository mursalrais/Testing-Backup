using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Gantt
{
    public class GanttTasksVM
    {
        public int ID { get; set; }
        public string Title{ get; set; }

        public int? ParentID { get; set; }

        public int OrderID { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public double PercentComplete { get; set; }

        public bool Summary { get; set; }

        public bool Expanded { get; set; }
        
    }
}
