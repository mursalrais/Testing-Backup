using System.Collections.Generic;
using System.Web;
using System;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using MCAWebAndAPI.Model.Common;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetAcquisitionHeaderVM : Item
    {
        public IEnumerable<AssetAcquisitionItemVM> AssetAcquisitionDetails { get; set; }
        public int? Id { get; set; }

        private ComboBoxVM _accepMemoNo;

        public string TransactionType { get; set; }

        public AjaxComboBoxVM AcceptanceMemoNoID { get; set; } = new AjaxComboBoxVM
        {
            ActionName = "GetAcceptanceMemoNos",
            ControllerName = "ASSAssetAcquisition",
            ValueField = "ID",
            //TextField = String.Format("{0}-{1}","ID","Title"),
            TextField = "Title",
            OnSelectEventName = "OnSelectAcceptMemoNo"
        };

        public string AcceptanceMemoNoString { get; set; }

        public string Vendor { get; set; }

        public string PoNo { get; set; }

        [UIHint("Date")]
        public DateTime? PurchaseDate { get; set; }

        public string PurchaseDescription { get; set; }
    }
}
