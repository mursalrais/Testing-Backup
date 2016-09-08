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
    public class AssetCheckResultItemVM : Item
    {
        public int? ID;

        public int Item { get; set; }

        [DisplayName("Asset-Sub Asset")]
        public string AssetSubAsset { get; set; }

        [DisplayName("Serial No")]
        public string SerialNo { get; set; }

        [DisplayName("Province")]
        public string Province { get; set; }
        
        [DisplayName("Location Name")]
        public string LocationName { get; set; }

        public string Status { get; set; }

        public string Existense { get; set; }

        public string Condition { get; set; }

        public string Specification { get; set; }

        [DisplayName("System Qty")]
        public int SystemQty { get; set; }

        [DisplayName("Physical Qty")]
        public int PhysicalQty { get; set; }

        [DisplayName("Different Qty")]
        public int DifferentQty { get; set; }

        public string Dispose { get; set; }

        public string Remarks { get; set; }
        
        [DisplayName("Asset ID")]
        public int AssetID { get; set; }
    }
}
