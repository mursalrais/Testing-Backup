using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetReplacementVM
    {
        private AssetReplacementHeaderVM _header;
        private IEnumerable<AssetReplacementItemVM> _items;

        public AssetReplacementHeaderVM Header
        {
            get
            {
                if (_header == null)
                {
                    _header = new AssetReplacementHeaderVM();
                }
                return _header;
            }

            set
            {
                _header = value;
            }
        }

        public IEnumerable<AssetReplacementItemVM> Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new List<AssetReplacementItemVM>();
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
