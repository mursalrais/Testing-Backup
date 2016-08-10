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
    public class AssetAcquisitionItemVM : Item
    {
        public int? ID { get; set; }
        public string POLineItem { get; set; }

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
        public AjaxComboBoxVM WBS { get; set; } = new AjaxComboBoxVM();

        public static AjaxComboBoxVM GetWBSDefaultValue(AjaxComboBoxVM model = null)
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

        public int? CostIDR { get; set; }
        public int? CostUSD { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }

    }
}
