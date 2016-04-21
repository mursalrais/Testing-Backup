using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetCheckFormVM
    {
        private AssetCheckFormHeaderVM _header;
        private IEnumerable<AssetCheckFormItemVM> _items;

        public AssetCheckFormHeaderVM Header
        {
            get
            {
                if (_header == null)
                {
                    _header = new AssetCheckFormHeaderVM();
                }
                return _header;
            }

            set
            {
                _header = value;
            }
        }

        public IEnumerable<AssetCheckFormItemVM> Items
        {
            get
            {
                if (_items == null)
                    _items = new List<AssetCheckFormItemVM>();
                return _items;
            }
            set
            {
                _items = value;
            }
        }
    }
}
