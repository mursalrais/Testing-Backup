﻿using System.Collections.Generic;
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
        public string CancelURL { get; set; }

        private ComboBoxVM _accmemo;

        public IEnumerable<AssetAcquisitionItemVM> Details { get; set; } = new List<AssetAcquisitionItemVM>();

        public string TransactionType { get; set; }

        [Required]
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
                        },
                        OnSelectEventName = "onSelectedAcceptanceMemo"
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
        public string VendorID { get; set; }
        public string PoNo { get; set; }

        [Required]
        [UIHint("Date")]
        public DateTime PurchaseDate { get; set; }
        public string purchasedatetext { get; set; }

        public string PurchaseDescription { get; set; }


    }
}