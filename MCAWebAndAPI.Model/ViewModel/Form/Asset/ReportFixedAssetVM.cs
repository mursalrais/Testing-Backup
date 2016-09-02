using MCAWebAndAPI.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class ReportFixedAssetVM : Item
    {
        public string CancelURL { get; set; }

        public int no { get; set; }
        public string projectunit { get; set; }
        public string assettype { get; set; }
        public string assetid { get; set; }
        public string assetdesc { get; set; }
        public string purchasedesc { get; set; }
        public string purchasedate { get; set; }
        public int quantity { get; set; }
        public int costidr { get; set; }
        public int costusd { get; set; }
        public string vendor { get; set; }
        public string specification { get; set; }
        public string pono { get; set; }
        public string serialnumber { get; set; }
        public string warrantyexpires { get; set; }
        public string condition { get; set; }
        public string assetholdername { get; set; }
        public string province { get; set; }
        public string location { get; set; }
    }
}
