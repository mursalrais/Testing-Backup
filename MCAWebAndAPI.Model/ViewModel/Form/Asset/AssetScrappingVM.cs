using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetScrappingVM
    {
        private AssetScrappingHeaderVM _header;
        private List<AssetScrappingItemVM> _items;

        public AssetScrappingHeaderVM Header
        {
            get
            {
                if (_header == null)
                {
                    _header = new AssetScrappingHeaderVM();
                }
                return _header;
            }

            set
            {
                _header = value;
            }
        }

        public List<AssetScrappingItemVM> Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new List<AssetScrappingItemVM>();
                }
                return _items;
            }

            set
            {
                _items = value;
            }
        }
    }
}
