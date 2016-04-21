using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Procurement
{
    public class PurchaseOrderHeaderVM
    {
        public string PONumber { get; set; }

        public string VendorName { get; set; }

        public int Year { get; set; }
    }
}
