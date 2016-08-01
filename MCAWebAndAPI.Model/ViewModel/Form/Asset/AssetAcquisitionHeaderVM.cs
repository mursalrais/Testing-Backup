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
        private DateTime _purchaseDate;
        private ComboBoxVM _oldTransactionId, _purchaseDesc, _assetSubAsset, _wbs;

        public int Id { get; set; }

        public string TransactionType { get; set; }

        [DisplayName("PO Line Item")]
        public string PoLineItem { get; set; }

        [UIHint("Currency")]
        [DisplayName("Cost (IDR)")]
        public decimal CostIdr { get; set; }

        [UIHint("Currency")]
        [DisplayName("Cost (USD)")]
        public decimal CostUsd { get; set; }

        public DateTime PurchaseDate
        {
            get
            {
                if (_purchaseDate == null)
                {
                    _purchaseDate = new DateTime();
                }
                return _purchaseDate;
            }

            set
            {
                _purchaseDate = value;
            }
        }

        [DisplayName("Old Transaction ID")]
        [UIHint("ComboBox")]
        public ComboBoxVM OldTransactionId
        {
            get
            {
                if (_oldTransactionId == null)
                {
                    _oldTransactionId = new ComboBoxVM()
                    {
                        Choices = new string[]
                        {
                            "1",
                            "2",
                            "3"
                        }
                    };
                }
                return _oldTransactionId;
            }

            set
            {
                _oldTransactionId = value;
            }
        }

        [DisplayName("Purchase Description")]
        [UIHint("ComboBox")]
        public ComboBoxVM PurchaseDesc
        {
            get
            {
                if (_purchaseDesc == null)
                {
                    _purchaseDesc = new ComboBoxVM()
                    {
                        Choices = new string[]
                        {
                            "Cash",
                            "AP",
                            "Warranty"
                        }
                    };
                }
                return _purchaseDesc;
            }

            set
            {
                _purchaseDesc = value;
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
                            "Procurement",
                            "Green",
                            "Health"
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

        [UIHint("ComboBox")]
        public ComboBoxVM Wbs
        {
            get
            {
                if (_wbs == null)
                {
                    _wbs = new ComboBoxVM()
                    {
                        Choices = new string[]
                        {
                            "1",
                            "2",
                            "3"
                        }
                    };
                }
                return _wbs;
            }

            set
            {
                _wbs = value;
            }
        }

    }
}
