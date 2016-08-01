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
    public class AssetAcquisitionItemVM : Item
    {
        public int AssetAcquisitionID { get; set; }

        public string PoLineItem { get; set; }

        [UIHint("InGridComboBox")]
        public InGridComboBoxVM AssetSubAssetID { get; set; } = new InGridComboBoxVM();
        
        public string AssetSubAssetString { get; set; }

        public AjaxComboBoxVM WBSID { get; set; } = new AjaxComboBoxVM
        {
            ActionName = "GetWBSForDetails",
            ControllerName = "ASSAssetAcquisition",
            ValueField = "ID",
            //TextField = String.Format("{0}-{1}","ID","Title"),
            TextField = "Title",
            OnSelectEventName = "OnWBSChange"
        };

        public string WBSString { get; set; }

        public int? CostIDR { get; set; }
        public int? CostUSD { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }

    }
}
