using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using NLog;

namespace MCAWebAndAPI.Service.Asset
{
    public class AssetFixedAssetService : IAssetFixedAssetService
    {
        string _siteUrl = null;
        static Logger logger = LogManager.GetCurrentClassLogger();

        public void SetSiteUrl(string siteUrl)
        {
            _siteUrl = siteUrl;
        }

        public AssetFixedAssetVM GetAssetFixedAssets()
        {
            throw new NotImplementedException();
        }

        public bool CreateAssetFixedAsset(AssetFixedAssetVM assetFixedAsset)
        {
            throw new NotImplementedException();
        }

        public bool UpdateAssetFixedAsset(AssetFixedAssetVM assetFixedAsset)
        {
            throw new NotImplementedException();
        }

        IEnumerable<AssetFixedAssetVM> IAssetFixedAssetService.GetAssetFixedAssets()
        {
            throw new NotImplementedException();
        }
    }
}
