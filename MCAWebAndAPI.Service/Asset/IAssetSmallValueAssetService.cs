using MCAWebAndAPI.Model.ViewModel.Form.Asset;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCAWebAndAPI.Service.Asset
{
    public interface IAssetSmallValueAssetService
    {
        void SetSiteUrl(string siteUrl);

        IEnumerable<AssetSmallValueAssetVM> GetAssetSmallValueAssets();

        bool CreateAssetSmallValueAsset(AssetSmallValueAssetVM assetSmallValueAsset);

        bool UpdateAssetSmallValueAsset(AssetSmallValueAssetVM assetSmallValueAsset);
    }
}
