using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Finance
{
    public class SCAVoucherItemsVM
    {
        public int SCAVoucherID { get; set; }

        public int WBSID { get; set; }

        public string WBSName { get; set; }

        public int GLID { get; set; }

        public string GLName { get; set; }

        public decimal? Amount { get; set; }
    }
}
