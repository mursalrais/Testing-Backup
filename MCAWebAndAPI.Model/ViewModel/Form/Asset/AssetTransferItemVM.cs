using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetTransferItemVM
    {
        public int? Header_ID { get; set; }

        public int? ID { get; set; }

        InGridComboBoxVM _asset = new InGridComboBoxVM();
        InGridComboBoxVM _floorFrom = new InGridComboBoxVM();
        InGridComboBoxVM _floorTo = new InGridComboBoxVM();
        InGridComboBoxVM _locationFrom = new InGridComboBoxVM();
        InGridComboBoxVM _locationTo = new InGridComboBoxVM();
        InGridComboBoxVM _provinceFrom = new InGridComboBoxVM();
        InGridComboBoxVM _provinceTo = new InGridComboBoxVM();
        InGridComboBoxVM _roomFrom = new InGridComboBoxVM();
        InGridComboBoxVM _roomTo = new InGridComboBoxVM();

        /*
        [UIHint("CurrencyIDR")]
        [DisplayName("Cost (IDR)")]
        public decimal? CostIDR { get; set; }

        [UIHint("CurrencyUSD")]
        [DisplayName("Cost (USD)")]
        public decimal? CostUSD { get; set; }

        [UIHint("Date")]
        public DateTime? ReturnDate { get; set; }
        */

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

        [UIHint("InGridComboBox_Floor")]
        public InGridComboBoxVM FloorFrom
        {
            get
            {
                return _floorFrom;
            }

            set
            {
                _floorFrom = value;
            }
        }

        [UIHint("InGridComboBox_Floor")]
        public InGridComboBoxVM FloorTo
        {
            get
            {
                return _floorTo;
            }

            set
            {
                _floorTo = value;
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

        [UIHint("InGridComboBox_Province")]
        public InGridComboBoxVM ProvinceFrom
        {
            get
            {
                return _provinceFrom;
            }

            set
            {
                _provinceFrom = value;
            }
        }

        [UIHint("InGridComboBox_Province")]
        public InGridComboBoxVM ProvinceTo
        {
            get
            {
                return _provinceTo;
            }

            set
            {
                _provinceTo = value;
            }
        }

        [UIHint("InGridComboBox_Room")]
        public InGridComboBoxVM RoomFrom
        {
            get
            {
                return _roomFrom;
            }

            set
            {
                _roomFrom = value;
            }
        }

        [UIHint("InGridComboBox_Room")]
        public InGridComboBoxVM RoomTo
        {
            get
            {
                return _roomTo;
            }

            set
            {
                _roomTo = value;
            }
        }
    }
}
