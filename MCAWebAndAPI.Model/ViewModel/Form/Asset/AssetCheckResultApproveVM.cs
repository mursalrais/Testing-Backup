using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetCheckResultApproveVM
    {
        private AssetCheckResultApproveHeaderVM _header;
        private List<AssetCheckResultApproveItemVM> _items;

        public AssetCheckResultApproveHeaderVM Header
        {
            get
            {
                if (_header == null)
                {
                    _header = new AssetCheckResultApproveHeaderVM();
                }
                return _header;
            }

            set
            {
                _header = value;
            }
        }

        public List<AssetCheckResultApproveItemVM> Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new List<AssetCheckResultApproveItemVM>();
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
