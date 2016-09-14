using MCAWebAndAPI.Model.Common;
using MCAWebAndAPI.Model.ViewModel.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetDisposalDetailVM : Item
    {
        public int? Header_ID { get; set; }

    

        public string Status { get; set; }

        [UIHint("InGridAjaxComboBox")]
        public AjaxComboBoxVM AssetSubAsset { get; set; } = new AjaxComboBoxVM();

        [UIHint("InGridAjaxComboBox")]
        public AjaxComboBoxVM ProvinceFrom { get; set; } = new AjaxComboBoxVM();

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

       



        public string Remarks { get; set; }

     

       
    }
}
