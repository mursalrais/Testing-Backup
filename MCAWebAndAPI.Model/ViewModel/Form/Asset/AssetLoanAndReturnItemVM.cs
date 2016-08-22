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
    public class AssetLoanAndReturnItemVM : Item
    {
        private ComboBoxVM _assetSubAsset;

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

        [DisplayName("Est Return Date")]
        [UIHint("Date")]
        public DateTime? EstReturnDate { get; set; } = DateTime.UtcNow;

        [DisplayName("Return Date")]
        [UIHint("Date")]
        public DateTime? ReturnDate { get; set; } = DateTime.UtcNow;

        public string Status { get; set; }

    }
}
