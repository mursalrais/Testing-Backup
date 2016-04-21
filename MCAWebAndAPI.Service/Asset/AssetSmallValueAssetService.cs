using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using NLog;

namespace MCAWebAndAPI.Service.Asset
{
    public class AssetSmallValueAssetService : IAssetSmallValueAssetService
    {
        string _siteUrl = null;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public AssetSmallValueAssetVM GetAssetSmallValueAssets()
        {
            throw new NotImplementedException();
        }

        public bool CreateAssetSmallValueAsset(AssetSmallValueAssetVM assetSmallValueAsset)
        {
            throw new NotImplementedException();
        }

        public bool UpdateAssetSmallValueAsset(AssetSmallValueAssetVM assetSmallValueAsset)
        {
            throw new NotImplementedException();
        }

        IEnumerable<AssetSmallValueAssetVM> IAssetSmallValueAssetService.GetAssetSmallValueAssets()
        {
            throw new NotImplementedException();
        }
    }
}
