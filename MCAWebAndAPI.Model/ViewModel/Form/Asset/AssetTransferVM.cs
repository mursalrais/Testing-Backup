using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetTransferVM : Item
    {
        public string CancelURL { get; set; }

        public string AssetIDs { get; set; }
        public string nameOnlyFrom { get; set; }
        public string positionFrom { get; set; }
        public string nameOnlyTo { get; set; }
        public string positionTo { get; set; }

        public IEnumerable<AssetTransferDetailVM> Details { get; set; } = new List<AssetTransferDetailVM>();

        private ComboBoxVM _assetHolder;
        private ComboBoxVM _completeStatus;
        private ComboBoxVM _assetHolderTo;

        public int? ID { get; set; }

        public string TransactionType { get; set; }

        [Required]
        [UIHint("Date")]
        public DateTime? Date { get; set; }

        [Required]
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
                            "1",
                            "2",
                            "3"
                        }
                        ,
                        OnSelectEventName = "onAssetChange"
                    };
                }
                return _assetHolder;
            }

            set
            {
                _assetHolder = value;
            }
        }

        public string ProjectUnit { get; set; }

        public string ContactNo { get; set; }

        [UIHint("ComboBox")]
        public ComboBoxVM CompletionStatus
        {
            get
            {
                if (_completeStatus == null)
                {
                    _completeStatus = new ComboBoxVM()
                    {
                        Choices = new string[]
                        {
                            "In Progress",
                            "Complete"
                        }
                    };
                }
                return _completeStatus;
            }

            set
            {
                _completeStatus = value;
            }
        }

        [Required]
        [UIHint("ComboBox")]
        public ComboBoxVM AssetHolderTo
        {
            get
            {
                if (_assetHolderTo == null)
                {
                    _assetHolderTo = new ComboBoxVM()
                    {
                        Choices = new string[]
                        {
                            "1",
                            "2",
                            "3"
                        }
                        ,
                        OnSelectEventName = "onAssetToChange"
                    };
                }
                return _assetHolderTo;
            }

            set
            {
                _assetHolderTo = value;
            }
        }

        public string ProjectUnitTo { get; set; }

        public string ContactNoTo { get; set; }
    }
}
