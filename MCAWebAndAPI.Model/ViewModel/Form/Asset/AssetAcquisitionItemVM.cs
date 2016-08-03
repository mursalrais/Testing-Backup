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
        private ComboBoxVM _assetSubAsset;
        [UIHint("ComboBox")]
        public ComboBoxVM AssetSubAsset
        {
            get
            {
                if (_assetSubAsset == null)
                {
                    _assetSubAsset = new ComboBoxVM()
                    {
                        Choices = new string[]
                        {
                            ""
                        }
                    };
                }
                return _assetSubAsset;
            }

            set
            {
                _assetSubAsset = value;
            }
        }
    }
}
