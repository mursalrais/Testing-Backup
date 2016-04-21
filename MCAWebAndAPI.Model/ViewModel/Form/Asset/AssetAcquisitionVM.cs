using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetAcquisitionVM
    {
        private AssetAcquisitionHeaderVM _header;
        private IEnumerable<AssetAcquisitionItemVM> _items;

        public AssetAcquisitionHeaderVM Header
        {
            get
            {
                if (_header == null)
                {
                    _header = new AssetAcquisitionHeaderVM();
                }
                return _header;
            }

            set
            {
                _header = value;
            }
        }

        public IEnumerable<AssetAcquisitionItemVM> Items
        {
            get
            {
                if (_items == null)
                    _items = new List<AssetAcquisitionItemVM>();
                return _items;
            }
            set
            {
                _items = value;
            }
        }
    }
}
