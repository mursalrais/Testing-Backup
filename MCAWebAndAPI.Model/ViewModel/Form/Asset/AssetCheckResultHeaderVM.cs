using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Control;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetCheckResultHeaderVM
    {
        private DateTime _date;

        public string CountedBy1 { get; set; }

        public string CountedBy2 { get; set; }

        public DateTime Date
        {
            get
            {
                if (_date == null)
                {
                    _date = new DateTime();
                }
                return _date;
            }

            set
            {
                _date = value;
            }
        }

        public int FormId { get; set; }

    }
}
