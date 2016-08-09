using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    public class SCAVoucherItemsVM
    {
        public int ID { get; set; }

        public int WBSID { get; set; }

        public string WBS { get; set; }

        public int GLID { get; set; }

        public string GL { get; set; }

        public decimal? Amount { get; set; }
    }
}
