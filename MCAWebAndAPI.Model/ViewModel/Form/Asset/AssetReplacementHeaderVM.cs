using System.Collections.Generic;
using System.Web;
using System;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetReplacementHeaderVM
    {
        public string CancelURL { get; set; }
        public int Id { get; set; }

        public IEnumerable<AssetReplacementItemVM> Details { get; set; } = new List<AssetReplacementItemVM>();

        public string TransactionType { get; set; }

        private ComboBoxVM _oldTransactionId;
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
                        ,OnSelectEventName = "onIdChange"
                    };
                }
                return _oldTransactionId;
            }

            set
            {
                _oldTransactionId = value;
            }
        }

        public string AccMemoNo { get; set; }
        public string Vendor { get; set; }
        public string Pono { get; set; }
        public string purchaseDescription { get; set; }

        [UIHint("Date")]
        public DateTime? PurchaseDate { get; set; }
        public string purchasedatetext { get; set; }
    }
}
