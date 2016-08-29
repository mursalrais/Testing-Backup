using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Control
{
    public class TransactionStatusComboBoxVM : ComboBoxVM
    {
        public const string NotLocked = "Not Locked";
        public const string Locked = "Locked";

        public TransactionStatusComboBoxVM() : base()
        {
            this.Choices = new string[]
            {
                NotLocked,
                Locked
            };

            this.Value = NotLocked;
        }
    }
}
