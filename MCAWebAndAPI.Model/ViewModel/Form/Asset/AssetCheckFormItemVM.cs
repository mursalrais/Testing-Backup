using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetCheckFormItemVM : Item
    {
        [DisplayName("Asset ID")]
        public int AssetID { get; set; }

        [DisplayName("Item")]
        public int item { get; set; }

        [DisplayName("Asset-Sub Asset")]
        public string assetSubAsset { get; set; }

        [DisplayName("Serial No")]
        public string serialNo { get; set; }

        [DisplayName("Provice")]
        public string province { get; set; }

        [DisplayName("Locaion")]
        public string location { get; set; }

        [DisplayName("Status")]
        public string status { get; set; }
        
        [DisplayName("Existense")]
        public string existense { get; set; }

        [DisplayName("Condition")]
        public string condition { get; set; }

        [DisplayName("Specification")]
        public string specification { get; set; }

        [DisplayName("System Qty")]
        public int systemQty { get; set; }

        [DisplayName("Physical Qty")]
        public int physicalQty{ get; set; }
    }
}
