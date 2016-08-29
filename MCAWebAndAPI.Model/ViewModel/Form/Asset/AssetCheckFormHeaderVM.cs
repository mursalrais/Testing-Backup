using System.Collections.Generic;
using System.Web;
using System;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using MCAWebAndAPI.Model.Common;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetCheckFormHeaderVM : Item
    {
        public IEnumerable<AssetCheckFormItemVM> Details { get; set; } = new List<AssetCheckFormItemVM>();
        private ComboBoxVM _office, _floor, _room;

        [DisplayName("Create Dates")]
        [UIHint("Date")]
        public DateTime? CreateDate { get; set; }


        [UIHint("ComboBox")]
        public ComboBoxVM Office
        {
            get
            {
                if (_office == null)
                    _office = new ComboBoxVM()
                    {
                        Choices = new string[]
                    {
                        ""
                    },
                        OnSelectEventName = "onSelectedLocation"
                    };
                return _office;
            }
            set
            {
                _office = value;
            }
        }

        [UIHint("ComboBox")]
        public ComboBoxVM Floor
        {
            get
            {
                if (_floor == null)
                    _floor = new ComboBoxVM()
                    {
                        Choices = new string[]
                    {
                        ""
                    },
                        OnSelectEventName = "onSelectedLocation"
                    };
                return _floor;
            }
            set
            {
                _floor = value;
            }
        }

        [UIHint("ComboBox")]
        public ComboBoxVM Room
        {
            get
            {
                if (_room == null)
                    _room = new ComboBoxVM()
                    {
                        Choices = new string[]
                    {
                        ""
                    },
                        OnSelectEventName = "onSelectedLocation"
                    };
                return _room;
            }
            set
            {
                _room = value;
            }
        }
    }
}
