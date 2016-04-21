using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.Asset
{
    public interface IAssetFixedAssetService
    {
        void SetSiteUrl(string siteUrl);

        IEnumerable<AssetFixedAssetVM> GetAssetFixedAssets();

        bool CreateAssetFixedAsset(AssetFixedAssetVM assetFixedAsset);

        bool UpdateAssetFixedAsset(AssetFixedAssetVM assetFixedAsset);
    }
}
