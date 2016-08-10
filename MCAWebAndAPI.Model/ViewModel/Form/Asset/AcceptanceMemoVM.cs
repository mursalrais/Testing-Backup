using MCAWebAndAPI.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AcceptanceMemoVM : Item
    {
        public int ID { get; set; }

        public string AcceptanceMemo { get; set; }

        public int? VendorID { get; set; }

        public string VendorName { get; set; }

        public string PoNo { get; set; }
    }
}
