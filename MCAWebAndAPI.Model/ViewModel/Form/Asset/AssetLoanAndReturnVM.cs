using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetLoanAndReturnVM
    {
        private AssetLoanAndReturnHeaderVM _header;
        private IEnumerable<AssetLoanAndReturnItemVM> _items;

        public AssetLoanAndReturnHeaderVM Header
        {
            get
            {
                if (_header == null)
                {
                    _header = new AssetLoanAndReturnHeaderVM();
                }
                return _header;
            }

            set
            {
                _header = value;
            }
        }

        public IEnumerable<AssetLoanAndReturnItemVM> Items
        {
            get
            {
                if (_items == null)
                    _items = new List<AssetLoanAndReturnItemVM>();
                return _items;
            }
            set
            {
                _items = value;
            }
        }
    }
}
