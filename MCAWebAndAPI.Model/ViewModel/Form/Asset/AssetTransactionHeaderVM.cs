using System;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetTransactionHeaderVM
    {
        public int? ID { get; set; }

        DateTime? _date = DateTime.Now;
        AjaxComboBoxVM _assetHolderFrom = new AjaxComboBoxVM
        {
            ActionName = "GetProfessionals",
            ControllerName = "HRDataMaster",
            ValueField = "ID",
            TextField = "Desc", 
            OnSelectEventName = "OnSelectAssetHolderFrom"
        };
        AjaxComboBoxVM _assetHolderTo = new AjaxComboBoxVM
        {
            ActionName = "GetProfessionals", 
            ControllerName = "HRDataMaster",
            ValueField = "ID", 
            TextField = "Desc",
            OnSelectEventName = "OnSelectAssetHolderTo"
        };

        [DisplayName("Contact No. (From)")]
        public string ContactNoFrom { get; set; }
        [DisplayName("Contact No. (To)")]
        public string ContactNoTo { get; set; }
        [DisplayName("Project/Unit (From)")]
        public string ProjectUnitFrom { get; set; }
        [DisplayName("Project/Unit (To)")]
        public string ProjectUnitTo { get; set; }
        public string TransactionType { get; set; }

        [UIHint("AjaxComboBox")]
        [DisplayName("Asset Holder (From)")]
        public AjaxComboBoxVM AssetHolderFrom
        {   get
            {
                return _assetHolderFrom;
            } set {
                _assetHolderFrom = value;
            }
        }

        [UIHint("AjaxComboBox")]
        [DisplayName("Asset Holder (To)")]
        public AjaxComboBoxVM AssetHolderTo
        {
            get
            {
                return _assetHolderTo;
            }
            set
            {
                _assetHolderTo = value;
            }
        }

        [UIHint("Date")]
        public DateTime? Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
            }
        }
    }
}
