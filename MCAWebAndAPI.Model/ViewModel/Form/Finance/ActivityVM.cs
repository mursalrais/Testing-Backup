using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.Common;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    public class ActivityVM:Item
    {
        public string Name { get; set; }

        public int ProjectId { get; set; }
        public string ProjectName { get; set; }

    }
}
