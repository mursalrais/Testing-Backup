using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Control
{
    public class MaritalStatusComboBoxVM : ComboBoxVM
    {
        public MaritalStatusComboBoxVM() : base()
        {
            Choices = new string[] {
                "Married",
                "Single",
                "Divorced",
                "Widow",
                "Widower"
            };
            Value = "Single";
        }
    }
}
