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
        public IEnumerable<AssetLandingPageFixedAssetVM> DetailFixedAsset { get; set; } = new List<AssetLandingPageFixedAssetVM>();
        public IEnumerable<AssetLandingPageSmallValueAssetVM> DetailSmallValueAsset { get; set; } = new List<AssetLandingPageSmallValueAssetVM>();

        public int? totalAsset_PCFF { get; set; }
        public int? valueIDR_PCFF { get; set; }
        public int? valueUSD_PCFF { get; set; }

        public int? totalAsset_PCOE { get; set; }
        public int? valueIDR_PCOE { get; set; }
        public int? valueUSD_PCOE { get; set; }

        public int? totalAsset_GOPE { get; set; }
        public int? valueIDR_GOPE { get; set; }
        public int? valueUSD_GOPE { get; set; }
    }
}
