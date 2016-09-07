using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;

namespace MCAWebAndAPI.Service.Asset
{
    public class AssetLandingPageService : IAssetLandingPageService
    {
        string _siteUrl;
        const string SP_FIXEDASSET = "Asset Fixed Asset";
        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public AssetLandingPageFixedAssetVM GetPopulatedModelFixedAsset(int? ID = default(int?))
        {
            
            var model = new AssetLandingPageFixedAssetVM();            

            return model;
        }

        public AssetLandingPageSmallValueAssetVM GetPopulatedModelSmallValueAsset(int? ID = default(int?))
        {
            throw new NotImplementedException();
        }
    }
}
