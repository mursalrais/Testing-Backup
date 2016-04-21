using System.Collections.Generic;
using System.Web;
using System;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetTransferHeaderVM
    {
        private DateTime _date;
        private ComboBoxVM _assetHolderFrom,_assetHolderTo;

        public string TransactionType { get; set; }

             
        public DateTime Date
        {
            get
            {
                if (_date == null)
                    _date = new DateTime();
                return _date;
            }

            set
            {
                _date = value;
            }
        }

        [DisplayName("Asset Holder (from)")]
        [UIHint("ComboBox")]
        public ComboBoxVM AssetHolderForm
        {
            get
            {
                if (_assetHolderFrom == null)
                    _assetHolderFrom = new ComboBoxVM()
                    {
                        Choices = new string[]
                        {
                            "Asset Folder No 1",
                            "Asset Folder No 2",
                            "Asset Folder No 3"
                        }
                    };
                return _assetHolderFrom;
            }
            set
            {
                _assetHolderFrom = value;
            }
        }

        [DisplayName("Asset Holder (to)")]
        [UIHint("ComboBox")]
        public ComboBoxVM AssetHolderTo
        {
            get
            {
                if (_assetHolderTo == null)
                    _assetHolderTo = new ComboBoxVM()
                    {
                        Choices = new string[]
                        {
                            "Asset Folder No 1",
                            "Asset Folder No 2",
                            "Asset Folder No 3"
                        }
                    };
                return _assetHolderTo;
            }
            set
            {
                _assetHolderTo = value;
            }
        }

    }
}
