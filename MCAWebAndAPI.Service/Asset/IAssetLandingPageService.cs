using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.Asset
{
    interface IAssetLandingPageService
    {
        void SetSiteUrl(string siteUrl);

        AssetLandingPageFixedAssetVM GetPopulatedModelFixedAsset(int? ID = null);
        AssetLandingPageSmallValueAssetVM GetPopulatedModelSmallValueAsset(int? ID = null);
    }
}
