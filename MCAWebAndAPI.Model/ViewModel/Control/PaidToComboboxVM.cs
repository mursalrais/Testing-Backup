using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Control
{
    public class PaidToComboboxVM : ComboBoxVM
    {
        public PaidToComboboxVM() : base()
        {
            Choices = new string[]
            {
                string.Empty,
               "Professional",
               "Vendor",
               "Driver"
            };
            Value = string.Empty;
        }
    }
}
