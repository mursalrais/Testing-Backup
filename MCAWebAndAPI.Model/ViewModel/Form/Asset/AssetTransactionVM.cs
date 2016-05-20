using System.Collections.Generic;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetTransactionVM
    {

        AssetTransactionHeaderVM header = new AssetTransactionHeaderVM();
        IEnumerable<AssetTransactionItemVM> _items;

        public AssetTransactionHeaderVM Header
        {
            get
            {
                return header;
            }

            set
            {
                header = value;
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
