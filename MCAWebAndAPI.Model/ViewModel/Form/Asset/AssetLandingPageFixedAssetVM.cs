using MCAWebAndAPI.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetLandingPageFixedAssetVM : Item
    {
        public IEnumerable<string> list_fxa { get; set; }
        public string A { get; set; }
        public int? B { get; set; }
        public string C { get; set; }
        public string D { get; set; }

        public int? totalAsset_PCFF { get; set; }
        public string valueIDR_PCFF { get; set; }
        public string valueUSD_PCFF { get; set; }

        public int? totalAsset_PCOE { get; set; }
        public string valueIDR_PCOE { get; set; }
        public string valueUSD_PCOE { get; set; }

        public int? totalAsset_GPOE { get; set; }
        public string valueIDR_GPOE { get; set; }
        public string valueUSD_GPOE { get; set; }
    }
}
