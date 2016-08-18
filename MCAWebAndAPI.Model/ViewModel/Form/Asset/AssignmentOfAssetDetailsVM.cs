using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel.DataAnnotations;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssignmentOfAssetDetailsVM : Item
    {
        private ComboBoxVM _assetsubasset;

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

        public string OfficeName { get; set; }

        public string Floor { get; set; }

        public string Room { get; set; }

        public string Remarks { get; set; }

        public string Status { get; set; }
    }
}
