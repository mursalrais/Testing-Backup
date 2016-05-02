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
    public class AssetScrappingHeaderVM
    {
        private DateTime _date;

        public int Id { get; set; }

        public string TransactionType { get; set; }

        [DisplayName("Asset-Sub Asset")]
        public string AssetSubAsset { get; set; }

        public string Remarks { get; set; }

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
    }
}
