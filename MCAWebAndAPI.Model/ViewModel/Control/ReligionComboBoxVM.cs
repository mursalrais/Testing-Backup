using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Control
{
    public class ReligionComboBoxVM : ComboBoxVM
    {
        public ReligionComboBoxVM() : base()
        {
            Choices = new string[]
            {
                "Islam",
                "Christian",
                "Catholic",
                "Hindu",
                "Buddha",
                "Others"
            };
            Value = "Others";
        }
    }
}
