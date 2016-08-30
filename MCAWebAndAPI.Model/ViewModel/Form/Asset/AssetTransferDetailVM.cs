using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetTransferDetailVM : Item
    {
        [DisplayName("Asset Sub Asset")]
        public string textasset { get; set; }

        [DisplayName("Description")]
        public string description { get; set; }

        [DisplayName("Quantity")]
        public int quantity { get; set; }

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

        [DisplayName("Province - Location - Floor - Room From")]
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

        public string OfficeName { get; set; }
        public string Floor { get; set; }
        public string Room { get; set; }
        public string Remarks { get; set; }

        [DisplayName("Province - Location - Floor - Room To")]
        [UIHint("InGridAjaxComboBox")]
        public AjaxComboBoxVM ProvinceTo { get; set; } = new AjaxComboBoxVM();

        public static AjaxComboBoxVM GetProvinceToDefaultValue(AjaxComboBoxVM model = null)
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

        public string OfficeNameTo { get; set; }
        public string FloorTo { get; set; }
        public string RoomTo { get; set; }

        public string Status { get; set; }
    }
}
