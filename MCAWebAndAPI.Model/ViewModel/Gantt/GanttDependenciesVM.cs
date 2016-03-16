using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Gantt
{
    public class GanttDependenciesVM
    {
        public int ID { get; set; }

        public int PredecessorID { get; set; }

        public int SuccessorID { get; set; }

        public int Type { get; set; }
    }
}
