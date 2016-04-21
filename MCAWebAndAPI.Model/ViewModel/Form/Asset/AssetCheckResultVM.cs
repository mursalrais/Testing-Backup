using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetCheckResultVM
    {
        private AssetCheckResultHeaderVM _header;
        private List<AssetCheckResultItemVM> _items;

        public AssetCheckResultHeaderVM Header
        {
            get
            {
                if (_header == null)
                {
                    _header = new AssetCheckResultHeaderVM();
                }
                return _header;
            }

            set
            {
                _header = value;
            }
        }

        public List<AssetCheckResultItemVM> Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new List<AssetCheckResultItemVM>();
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
