using MCAWebAndAPI.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Control;

namespace MCAWebAndAPI.Model.ViewModel.Form.Shared
{

    public class VendorVM : Item
    {
        public string Vendor { get; set; }

        public string VendorId { get; set; }

        public string Name { get; set; }

        public string Street { get; set; }

        public string PostalCode { get; set; }

        public string City { get; set; }

        public CurrencyComboBoxVM Currency { get; set; } = new CurrencyComboBoxVM();

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string Group { get; set; }

        public string IdName
        {
            get { return string.Format("{0} - {1}", VendorId, Name); }
        }
    }
}