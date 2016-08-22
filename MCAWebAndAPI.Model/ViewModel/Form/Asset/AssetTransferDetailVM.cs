using System;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MCAWebAndAPI.Model.Common;
using System.Collections.Generic;
using System.Linq;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetTransferDetailVM : Item
    {
        public int? Header_ID { get; set; }

        public int? ID { get; set; }

        [UIHint("InGridAjaxComboBox")]
        public AjaxComboBoxVM AssetSubAsset { get; set; } = new AjaxComboBoxVM();

        
        [UIHint("InGridAjaxComboBox")]
        public AjaxComboBoxVM ProvinceFrom { get; set; } = new AjaxComboBoxVM();

        [UIHint("InGridAjaxComboBox")]
        public AjaxComboBoxVM Floore { get; set; } = new AjaxComboBoxVM();

        [UIHint("InGridAjaxComboBox")]
        public AjaxComboBoxVM LocationFrom { get; set; } = new AjaxComboBoxVM();

        public static AjaxComboBoxVM GetAssetSubAssetDefaultValue(AjaxComboBoxVM model = null)
        {
            if (model == null)
            {
                return new AjaxComboBoxVM();
            }
            else
            {
                return model;
            }
        }

        public static AjaxComboBoxVM GetProvinceDefaultValue(AjaxComboBoxVM model = null)
        {
            if (model == null)
            {
                return new AjaxComboBoxVM();
            }
            else
            {
                return model;
            }
        }

        public static AjaxComboBoxVM GetLocationDefaultValue(AjaxComboBoxVM model = null)
        {
            if (model == null)
            {
                return new AjaxComboBoxVM();
            }
            else
            {
                return model;
            }
        }

        InGridComboBoxVM _floorFrom = new InGridComboBoxVM();
        InGridComboBoxVM _floorTo = new InGridComboBoxVM();
      
        InGridComboBoxVM _locationTo = new InGridComboBoxVM();
        AjaxCascadeComboBoxVM _provinceFrom = new AjaxCascadeComboBoxVM();
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

        //[UIHint("InGridComboBox_Asset")]
        //public InGridComboBoxVM Asset
        //{
        //    get
        //    {
        //        return _asset;
        //    }

        //    set
        //    {
        //        _asset = value;
        //    }
        //}

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

        //[UIHint("InGridComboBox_Floor")]
        //public InGridComboBoxVM FloorTo
        //{
        //    get
        //    {
        //        return _floorTo;
        //    }

        //    set
        //    {
        //        _floorTo = value;
        //    }
        //}

        //[UIHint("InGridComboBox_Location")]
        //public InGridComboBoxVM LocationFrom
        //{
        //    get
        //    {
        //        return _locationFrom;
        //    }

        //    set
        //    {
        //        _locationFrom = value;
        //    }
        //}

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
