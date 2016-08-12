using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssignmentofAssetDetailVM : Item
    {
        //private ComboBoxVM _assetSubAsset, _province, _office, _floor, _room;

        public int Id { get; set; }

        [DisplayName("Asset-Sub Asset")]
        [UIHint("InGridAjaxComboBox")]
        public AjaxComboBoxVM AssetSubAsset { get; set; } = new AjaxComboBoxVM();

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

        [UIHint("InGridAjaxComboBox")]
        public AjaxComboBoxVM Province { get; set; } = new AjaxComboBoxVM();

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

        [UIHint("InGridAjaxComboBox")]
        public AjaxComboBoxVM Office { get; set; } = new AjaxComboBoxVM();

        public static AjaxComboBoxVM GetOfficeDefaultValue(AjaxComboBoxVM model = null)
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

        [UIHint("InGridAjaxComboBox")]
        public AjaxComboBoxVM Floor { get; set; } = new AjaxComboBoxVM();

        public static AjaxComboBoxVM GetFloorDefaultValue(AjaxComboBoxVM model = null)
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

        [UIHint("InGridAjaxComboBox")]
        public AjaxComboBoxVM Room { get; set; } = new AjaxComboBoxVM();

        public static AjaxComboBoxVM GetRoomDefaultValue(AjaxComboBoxVM model = null)
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

        //[DisplayName("Asset-Sub Asset")]
        //[UIHint("ComboBox")]
        //public ComboBoxVM AssetSubAsset
        //{
        //    get
        //    {
        //        if (_assetSubAsset == null)
        //        {
        //            _assetSubAsset = new ComboBoxVM()
        //            {
        //                Choices = new string[]
        //                {
        //                    "FXA-GP-LA-0001- Server Machine",
        //                    "FXA-GP-LA-0001- Server Machine",
        //                    "FXA-GP-LA-0001- Server Machine"
        //                }
        //            };
        //        }
        //        return _assetSubAsset;
        //    }

        //    set
        //    {
        //        _assetSubAsset = value;
        //    }
        //}

        //[UIHint("ComboBox")]
        //public ComboBoxVM Province
        //{
        //    get
        //    {
        //        if (_province == null)
        //        {
        //            _province = new ComboBoxVM()
        //            {
        //                Choices = new string[]
        //                {
        //                    "Jawa Timur",
        //                    "Jawa Barat",
        //                    "Jawa Tengah"
        //                }
        //            };
        //        }
        //        return _province;
        //    }

        //    set
        //    {
        //        _province = value;
        //    }
        //}

        //[UIHint("ComboBox")]
        //public ComboBoxVM Office
        //{
        //    get
        //    {
        //        if (_office == null)
        //        {
        //            _office = new ComboBoxVM()
        //            {
        //                Choices = new string[]
        //                {
        //                    "Gedung MR21",
        //                    "Eceos"
        //                }
        //            };
        //        }
        //        return _office;
        //    }

        //    set
        //    {
        //        _office = value;
        //    }
        //}

        //[UIHint("ComboBox")]
        //public ComboBoxVM Floor
        //{
        //    get
        //    {
        //        if (_floor == null)
        //        {
        //            _floor = new ComboBoxVM()
        //            {
        //                Choices = new string[]
        //                {
        //                    "11",
        //                    "12",
        //                    "13"
        //                }
        //            };
        //        }
        //        return _floor;
        //    }

        //    set
        //    {
        //        _floor = value;
        //    }
        //}

        //[UIHint("ComboBox")]
        //public ComboBoxVM Room
        //{
        //    get
        //    {
        //        if (_room == null)
        //        {
        //            _room = new ComboBoxVM()
        //            {
        //                Choices = new string[]
        //                {
        //                    "A",
        //                    "B",
        //                    "C"
        //                }
        //            };
        //        }
        //        return _room;
        //    }

        //    set
        //    {
        //        _room = value;
        //    }
        //}

        public string Remarks { get; set; }
        public string Status { get; set; }

    }
}
