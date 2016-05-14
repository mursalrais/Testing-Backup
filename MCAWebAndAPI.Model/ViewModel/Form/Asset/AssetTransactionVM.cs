using System.Collections.Generic;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetTransactionVM
    {
        AssetTransactionHeaderVM _header;
        IEnumerable<AssetTransactionItemVM> _items;

        public AssetTransactionHeaderVM Header
        {
            get
            {
                if (_header == null)
                {
                    _header = new AssetTransactionHeaderVM();
                }
                return _header;
            }

            set
            {
                _header = value;
            }
        }

        public IEnumerable<AssetTransactionItemVM> Items
        {
            get
            {
                if (_items == null)
                    _items = new List<AssetTransactionItemVM>();
                return _items;
            }
            set
            {
                _items = value;
            }
        }
    }
}
