using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetTransferVM
    {
        private AssetTransferHeaderVM _header;
        private IEnumerable<AssetTransferItemVM> _items;

        public AssetTransferHeaderVM Header
        {
            get
            {
                if (_header == null)
                {
                    _header = new AssetTransferHeaderVM();
                }
                return _header;
            }

            set
            {
                _header = value;
            }
        }

        public IEnumerable<AssetTransferItemVM> Items
        {
            get
            {
                if (_items == null)
                    _items = new List<AssetTransferItemVM>();
                return _items;
            }
            set
            {
                _items = value;
            }
        }
    }
}
