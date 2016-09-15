using MCAWebAndAPI.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetLandingPageFixedAssetVM
    {

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
