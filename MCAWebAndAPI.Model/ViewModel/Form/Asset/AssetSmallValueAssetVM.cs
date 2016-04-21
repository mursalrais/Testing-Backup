using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetSmallValueAssetVM
    {
        private AssetSmallValueAssetHeaderVM _header;
        private IEnumerable<AssetSmallValueAssetItemVM> _items;

        public AssetSmallValueAssetHeaderVM Header
        {
            get
            {
                if (_header == null)
                {
                    _header = new AssetSmallValueAssetHeaderVM();
                }
                return _header;
            }

            set
            {
                _header = value;
            }
        }

        public IEnumerable<AssetSmallValueAssetItemVM> Items
        {
            get
            {
                if (_items == null)
                    _items = new List<AssetSmallValueAssetItemVM>();
                return _items;
            }
            set
            {
                _items = value;
            }
        }
    }
}
