using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetTransactionItemVM
    {  
        public int? Header_ID { get; set; }

        public int? ID { get; set; }

        [UIHint("InGridComboBox")]
        [DisplayName("Location (from)")]
        public InGridComboBoxVM LocationFrom
        {
            get
            {
                if (_locationFrom == null)
                    _locationFrom = new InGridComboBoxVM();
                return _locationFrom;
            }

            set
            {
                _locationFrom = value;
            }
        }

        [UIHint("InGridComboBox")]
        [DisplayName("Location (to)")]
        public InGridComboBoxVM LocationTo
        {
            get
            {
                if (_locationTo == null)
                    _locationTo = new InGridComboBoxVM();
                return _locationTo;
            }

            set
            {
                _locationTo = value;
            }
        }

        InGridComboBoxVM _locationFrom;
        InGridComboBoxVM _locationTo;
        AjaxComboBoxVM _asset;
        InGridComboBoxVM _WBS;

        [UIHint("CurrencyIDR")]
        [DisplayName("Cost (IDR)")]
        public decimal? CostIDR { get; set; }

        [UIHint("CurrencyUSD")]
        [DisplayName("Cost (USD)")]
        public decimal? CostUSD { get; set; }

        [UIHint("Date")]
        public DateTime? ReturnDate { get; set; }

        public string Remarks { get; set; }

        [UIHint("AjaxComboBoxVM")]
        [DisplayName("Asset")]
        public AjaxComboBoxVM Asset
        {
            get
            {
                if (_asset == null)
                    return new AjaxComboBoxVM
                    {
                        ActionName = "GetAssetMasters",
                        ControllerName = "ASSAssetMaster",
                        TextField = "Text",
                        ValueField = "ID"
                    };
                return _asset;
            }
            set
            {
                _asset = value;
            }
        }

        [UIHint("InGridComboBox")]
        [DisplayName("WBS")]
        public InGridComboBoxVM WBS
        {
            get
            {
                if (_WBS == null)
                    return new InGridComboBoxVM();
                return _WBS;
            }

            set
            {
                _WBS = value;
            }
        }
    }
}
