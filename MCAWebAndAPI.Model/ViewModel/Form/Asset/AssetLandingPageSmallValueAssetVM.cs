using MCAWebAndAPI.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Model.ViewModel.Form.Asset
{
    public class AssetLandingPageSmallValueAssetVM : Item
    {
        int? totalAsset_PCFF { get; set; }
        int? valueIDR_PCFF { get; set; }
        int? valueUSD_PCFF { get; set; }

        int? totalAsset_PCOE { get; set; }
        int? valueIDR_PCOE { get; set; }
        int? valueUSD_PCOE { get; set; }

        int? totalAsset_GOPE { get; set; }
        int? valueIDR_GOPE { get; set; }
        int? valueUSD_GOPE { get; set; }
    }
}
