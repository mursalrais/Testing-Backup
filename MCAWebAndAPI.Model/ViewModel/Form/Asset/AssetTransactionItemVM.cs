using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetTransactionItemVM
    {  
        public int? Header_ID { get; set; }

        public int? ID { get; set; }

        InGridComboBoxVM _asset = new InGridComboBoxVM();
        InGridComboBoxVM _locationFrom = new InGridComboBoxVM();
        InGridComboBoxVM _locationTo = new InGridComboBoxVM();
        InGridComboBoxVM _WBS = new InGridComboBoxVM();


        [UIHint("CurrencyIDR")]
        [DisplayName("Cost (IDR)")]
        public decimal? CostIDR { get; set; }

        [UIHint("CurrencyUSD")]
        [DisplayName("Cost (USD)")]
        public decimal? CostUSD { get; set; }

        [UIHint("Date")]
        public DateTime? ReturnDate { get; set; }

        public string Remarks { get; set; }

        [UIHint("InGridComboBox_Asset")]
        public InGridComboBoxVM Asset
        {
            get
            {
                return _asset;
            }

            set
            {
                _asset = value;
            }
        }

        [UIHint("InGridComboBox_Location")]
        public InGridComboBoxVM LocationFrom
        {
            get
            {
                return _locationFrom;
            }

            set
            {
                _locationFrom = value;
            }
        }

        [UIHint("InGridComboBox_Location")]
        public InGridComboBoxVM LocationTo
        {
            get
            {
                return _locationTo;
            }

            set
            {
                _locationTo = value;
            }
        }

        [UIHint("InGridComboBox_WBS")]
        public InGridComboBoxVM WBS
        {
            get
            {
                return _WBS;
            }

            set
            {
                _WBS = value;
            }
        }
    }
}
