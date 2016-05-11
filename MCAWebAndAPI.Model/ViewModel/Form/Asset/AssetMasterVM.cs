using System.Collections.Generic;
using System.Web;
using System;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetMasterVM
    {
        public int Id { get; set; }

        private ComboBoxVM _assetLevel;
        private ComboBoxVM _assetCategory;
        private ComboBoxVM _projectUnit;
        private ComboBoxVM _assetType;
        private ComboBoxVM _assetNoAssetDesc;
        private ComboBoxVM _condition;

        public string AssetDesc { get; set; }
        public string SerialNo { get; set; }
        public string Spesifications { get; set; }
        public string Remarks { get; set; }

        private DateTime _warrantyExpires;


        public DateTime WarrantyExpires
        {
            get
            {
                if (_warrantyExpires == null)
                    _warrantyExpires = new DateTime();
                return _warrantyExpires;
            }

            set
            {
                _warrantyExpires = value;
            }
        }


        [UIHint("ComboBox")]
        public ComboBoxVM AssetLevel
        {
            get
            {
                if (_assetLevel == null)
                    _assetLevel = new ComboBoxVM()
                    {
                        Choices = new string[]
                        {
                            "Main Asset",
                            "Sub Asset"
                        }
                        //FieldValue = "AssetLevel"
                    };
                return _assetLevel;
            }
            set
            {
                _assetLevel = value;
            }
        }

        [UIHint("ComboBox")]
        public ComboBoxVM AssetCategory
        {
            get
            {
                if (_assetCategory == null)
                    _assetCategory = new ComboBoxVM()
                    {
                        Choices = new string[]
                        {
                            "Fixed Asset",
                            "Small Value Asset"
                        }
                    };
                return _assetCategory;
            }
            set
            {
                _assetCategory = value;
            }
        }

        [UIHint("ComboBox")]
        public ComboBoxVM ProjectUnit
        {
            get
            {
                if (_projectUnit == null)
                    _projectUnit = new ComboBoxVM()
                    {
                        Choices = new string[]
                        {
                            "GP",
                            "HN",
                            "PM",
                            "ME",
                            "PC"
                        }
                    };
                return _projectUnit;
            }
            set
            {
                _projectUnit = value;
            }
        }

        [UIHint("ComboBox")]
        public ComboBoxVM AssetType
        {
            get
            {
                if (_assetType == null)
                    _assetType = new ComboBoxVM()
                    {
                        Choices = new string[]
                        {
                            "LA", "BU", "CO", "CE", "CS", "OE", "FF", "VE", "OT"
                        }
                    };
                return _assetType;
            }
            set
            {
                _assetType = value;
            }
        }

        [UIHint("ComboBox")]
        public ComboBoxVM AssetNoAssetDesc
        {
            get
            {
                if (_assetNoAssetDesc == null)
                    _assetNoAssetDesc = new ComboBoxVM()
                    {
                        Choices = new string[]
                        {
                            "LA",
                            "BU",
                            "CO"
                        }
                    };
                return _assetNoAssetDesc;
            }
            set
            {
                _assetType = value;
            }
        }

        [UIHint("ComboBox")]
        public ComboBoxVM Condition
        {
            get
            {
                if (_condition == null)
                    _condition = new ComboBoxVM()
                    {
                        Choices = new string[]
                        {
                            "Normal",
                            "Good",
                            "Broken"
                        }
                    };
                return _condition;
            }
            set
            {
                _condition = value;
            }
        }
    }
}
