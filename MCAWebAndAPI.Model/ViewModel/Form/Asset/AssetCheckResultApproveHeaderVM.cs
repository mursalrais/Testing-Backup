using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetCheckResultApproveHeaderVM
    {
        private DateTime _createDate, _countDate;

        public DateTime CountDate
        {
            get
            {
                if (_countDate == null)
                {
                    _countDate = new DateTime();
                }
                return _countDate;
            }

            set
            {
                _countDate = value;
            }
        }

        public string CountedBy1 { get; set; }

        public string CountedBy2 { get; set; }

        public DateTime CreateDate
        {
            get
            {
                if (_createDate == null)
                {
                    _createDate = new DateTime();
                }
                return _createDate;
            }

            set
            {
                _createDate = value;
            }
        }

        public int FormId { get; set; }
    }
}
