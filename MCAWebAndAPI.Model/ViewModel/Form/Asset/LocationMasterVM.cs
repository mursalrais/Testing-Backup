using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class LocationMasterVM
    {
        private LocationMasterHeaderVM _header;

        public LocationMasterHeaderVM Header
        {
            get
            {
                if (_header == null)
                {
                    _header = new LocationMasterHeaderVM();
                }
                return _header;
            }

            set
            {
                _header = value;
            }
        }
    }
}
