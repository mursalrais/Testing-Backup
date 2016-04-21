using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetFixedAssetVM
    {
        private AssetFixedAssetHeaderVM _header;
        private IEnumerable<AssetFixedAssetItemVM> _items;

        public AssetFixedAssetHeaderVM Header
        {
            get
            {
                if (_header == null)
                {
                    _header = new AssetFixedAssetHeaderVM();
                }
                return _header;
            }

            set
            {
                _header = value;
            }
        }

        public IEnumerable<AssetFixedAssetItemVM> Items
        {
            get
            {
                if (_items == null)
                    _items = new List<AssetFixedAssetItemVM>();
                return _items;
            }
            set
            {
                _items = value;
            }
        }
    }
}
