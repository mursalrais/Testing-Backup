using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    public class ActivityVM : Item
    {
        public string Name { get; set; }

        public AjaxComboBoxVM Project { get; set; } = new AjaxComboBoxVM();
    }
}
