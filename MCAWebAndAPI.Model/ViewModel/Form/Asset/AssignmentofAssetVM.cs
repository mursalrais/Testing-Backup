using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssignmentofAssetVM
    {
        private AssignmentofAssetHeaderVM _header;
        private IEnumerable<AssignmentofAssetItemVM> _items;

        public AssignmentofAssetHeaderVM Header
        {
            get
            {
                if (_header == null)
                {
                    _header = new AssignmentofAssetHeaderVM();
                }
                return _header;
            }

            set
            {
                _header = value;
            }
        }

        public IEnumerable<AssignmentofAssetItemVM> Items
        {
            get
            {
                if (_items == null)
                {
                    _items = new List<AssignmentofAssetItemVM>();
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
