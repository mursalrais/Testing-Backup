using System.Collections.Generic;
using System.Web;
using System;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetAcquisitionHeaderVM
    {
        public int Id { get; set; }
        private DateTime _purchaseDate;
        private ComboBoxVM _acceptanceMemoNo;        

        public string TransactionType { get; set; }
        public string PurchaseDescription { get; set; }      

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
                            "Acceptance Memo No 1",
                            "Acceptance Memo No 2",
                            "Acceptance Memo No 3"
                        }
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
