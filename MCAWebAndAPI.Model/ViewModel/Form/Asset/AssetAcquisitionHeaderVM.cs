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
        public int Id { get; set; }
        private DateTime _purchaseDate;
        private ComboBoxVM _acceptanceMemoNo;

        public IEnumerable<AssetAcquisitionItemVM> AssetAcquisitionItems { get; set; } = new List<AssetAcquisitionItemVM>();
        public string TransactionType { get; set; }
        public string PurchaseDescription { get; set; }
        public string Vendor { get; set; }
        public string PoNo { get; set; }

        [UIHint("Date")]
        public DateTime PurchaseDate
        {
            get
            {
                if (_purchaseDate == null)
                    _purchaseDate = new DateTime();
                return _purchaseDate;
            }

            set
            {
                _purchaseDate = value;
            }
        }

        [UIHint("ComboBox")]
        public ComboBoxVM AcceptanceMemoNo
        {
            get
            {
                if (_acceptanceMemoNo == null)
                    _acceptanceMemoNo = new ComboBoxVM()
                    {
                        Choices = new string[]
                        {
                            
                        }
                        ,OnSelectEventName = "onAcceptanceMemoChange"
                    };
                return _acceptanceMemoNo;
            }
            set
            {
                _acceptanceMemoNo = value;
            }
        }

    }
}
