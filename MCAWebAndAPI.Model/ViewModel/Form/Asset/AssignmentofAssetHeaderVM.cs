using System.Collections.Generic;
using System.Web;
using System;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssignmentofAssetHeaderVM
    {
        private DateTime _date;
        private ComboBoxVM _assetHolder, _assetSubAsset;

        public int Id { get; set; }

        public string TransactionType { get; set; }

        public string Office { get; set; }

        public string Floor { get; set; }

        public string Room { get; set; }

        [UIHint("ComboBox")]
        public ComboBoxVM AssetHolder
        {
            get
            {
                if (_assetHolder == null)
                {
                    _assetHolder = new ComboBoxVM()
                    {
                        Choices = new string[]
{
    "Mca",
    "Eceos",
    "Servio"
}
                    };

                }
                return _assetHolder;
            }

            set
            {
                _assetHolder = value;
            }
        }

        public DateTime Date
        {
            get
            {
                if (_date == null)
                {
                    _date = new DateTime();
                }
                return _date;
            }

            set
            {
                _date = value;
            }
        }

        [DisplayName("Asset-Sub Asset")]
        [UIHint("ComboBox")]
        public ComboBoxVM AssetSubAsset
        {
            get
            {
                if (_assetSubAsset == null)
                {
                    _assetSubAsset = new ComboBoxVM()
                    {
                        Choices = new string[]
                        {
                            "Mca",
                            "Eceos",
                            "Servio"
                        }
                    };
                }
                return _assetSubAsset;
            }

            set
            {
                _assetSubAsset = value;
            }
        }
    }
}
