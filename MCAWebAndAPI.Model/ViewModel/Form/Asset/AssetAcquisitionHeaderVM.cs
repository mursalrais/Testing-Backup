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
        private ComboBoxVM _accmemo;

        public IEnumerable<AssetAcquisitionItemVM> Details { get; set; } = new List<AssetAcquisitionItemVM>();

        public string  TransactionType { get; set; }

        [DisplayName("Aceptance Memo No")]
        [UIHint("ComboBox")]
        public ComboBoxVM AccpMemo
        {
            get
            {
                if (_accmemo == null)
                {
                    _accmemo = new ComboBoxVM()
                    {
                        Choices = new string[]
                        {
                            "1",
                            "2",
                            "3"
                        }
                    };
                }
                return _accmemo;
            }

            set
            {
                _accmemo = value;
            }
        }

        public string Vendor { get; set; }
        public string PoNo { get; set; }

        [UIHint("Date")]
        public DateTime PurchaseDate { get; set; }

        public string  PurchaseDescription { get; set; }


    }
}



