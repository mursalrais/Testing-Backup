using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Procurement
{
    public class PurchaseOrderItemVM
    {
        public int POItemNumber { get; set; }

        public string ShortText { get; set; }

        public int Qty { get; set; }

        public double UnitPrice { get; set; }

    }
}
