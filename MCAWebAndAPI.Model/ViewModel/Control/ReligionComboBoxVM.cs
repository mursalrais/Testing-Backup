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
                 "Protestantism",
                 "Catholicism",
                 "Hinduism",
                 "Buddhism",
                 "Atheism"
             };
            DefaultValue = "Islam";
        }
    }
}
