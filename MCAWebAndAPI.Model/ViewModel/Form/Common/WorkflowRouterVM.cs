using MCAWebAndAPI.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Common
{
    public class WorkflowRouterVM : Item
    {
        public string ListName { get; set; }

        public string RequestorUnit { get; set; }

        public string RequestorPosition { get; set; }

        public IEnumerable<WorkflowItemVM> WorkflowItems { get; set; } = new List<WorkflowItemVM>();


    }
}
